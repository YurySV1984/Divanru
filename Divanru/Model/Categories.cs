using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Divanru
{
    /// <summary>
    /// Категории мебели
    /// </summary>
    internal class Categories : IEnumerable<ListElement>
    {
        private const string siteurl = "https://www.divan.ru/ekaterinburg/";
        private const string categoryUrl = "https://www.divan.ru/ekaterinburg/category/";
        private const string regexString = @"""name"":""[^{]*""url"":""\\u002Fekaterinburg\\u002Fcategory\\u002F[\w-]*";
        private const string catString = "ekaterinburg\\u002Fcategory\\";

        public event EventHandler<NotificationEventArgs> OnError;
        public event EventHandler<CatParsingEventArgs> OnParsing;
        public event EventHandler<AllCategoriesParsingArgs> OnAllCategoriesParsing;

        private List<ListElement> _categories = new List<ListElement>();      //лист с категориями

        public IEnumerator<ListElement> GetEnumerator() => ((IEnumerable<ListElement>)_categories).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_categories).GetEnumerator();

        /// <summary>
        /// Индексатор.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ListElement this [int index] { get { return _categories [index]; } }

        /// <summary>
        /// Возвращает коллекцию из названий категорий для отображения в листбоксе
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<string> GetTitleList()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            _categories.ForEach(c => list.Add(c.Title));
            return list;
        }

        /// <summary>
        /// Количество категорий в коллекции
        /// </summary>
        public int Count { get { return _categories.Count; } }

        /// <summary>
        /// Ищет категории товаров на сайте
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task FindCategoriesAsync(string url = siteurl)
        {
            _categories.Clear();
            var httpclient = new HttpClient();

            try
            {
                var html = await httpclient.GetStringAsync(url);
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(html);
                var regex = new Regex(regexString);

                var html1 = htmlDocument.DocumentNode.Descendants().Where(n => n.Name == "script")
                    .Where(n => n.InnerHtml.Contains(catString)).FirstOrDefault()?.InnerText;
                MatchCollection matches = regex.Matches(html1);
                if (matches.Count > 0)
                {
                    var startindex = 0;
                    var endindex = 0;

                    foreach (Match match in matches)
                    {
                        var element = new ListElement();
                        startindex = match.Value.IndexOf("\"name\":\"") + 8;
                        endindex = match.Value.IndexOf("\"", startindex);
                        element.Title = match?.Value.Substring(startindex, endindex - startindex);
                        startindex = match.Value.IndexOf("\"url\":\"") + 45;
                        endindex = match.Value.Length;
                        element.Link = match?.Value.Substring(startindex, endindex - startindex);

                        if (!element.Link.Contains("skidki") && !element.Link.Contains("rasprodaz") && !element.Link.Contains("extra-sale") && !element.Link.Contains("promo-bud") && (_categories.FindIndex(catsEl => catsEl.Link == element.Link) == -1))
                            _categories.Add(element);
                    }
                    _categories = _categories.OrderBy(element => element.Title).ToList();
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, new NotificationEventArgs(e.Message));
            }
        }



        /// <summary>
        /// Парсит одну категорию товаров по адресу url и возвращает коллекцию мебели.
        /// </summary>
        /// <param name="url">URL-адрес категории.</param>
        /// <returns></returns> 
        public async Task<List<ListElement>> ParseProductsOneCatAsync(string url, CancellationToken cancellationToken)
        {
            var productsList = new List<ListElement>();
            try
            {
                var httpclient = new HttpClient();
                var html = await httpclient.GetStringAsync(url);
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(html);

                CheckProduct(ref productsList, in htmlDocument);

                if (cancellationToken.IsCancellationRequested) 
                { 
                    OnError?.Invoke(this, new NotificationEventArgs("Parsing selected category has been canceled")); 
                    return productsList;
                }

                var lastPage = 1;
                var lastpageNodes = htmlDocument.DocumentNode.Descendants("a")
                    .Where(node => node.GetAttributeValue("class", "").Equals("ImmXq R9QDJ hi0qF PaginationLink")).ToList();
                foreach (var lastPageNode in lastpageNodes)
                {
                    if (int.TryParse(lastPageNode.InnerHtml, out int page))
                        lastPage = page > lastPage ? page : lastPage;
                }

                OnParsing?.Invoke(this, new CatParsingEventArgs(lastPage, 1));
                for (int i = 2; i <= lastPage; i++)
                {
                    httpclient = new HttpClient();
                    html = await httpclient.GetStringAsync(url + "/page-" + i);
                    htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    CheckProduct(ref productsList, in htmlDocument);
                    OnParsing?.Invoke(this, new CatParsingEventArgs(lastPage, i));
                    if (cancellationToken.IsCancellationRequested)
                    {
                        OnError?.Invoke(this, new NotificationEventArgs("Parsing selected category has been canceled"));
                        return productsList;
                    }
                }               
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, new NotificationEventArgs(e.Message));
            }
            return productsList;

        }

        /// <summary>
        /// Проверяет корректность ссылки на продукт в меню категории.
        /// </summary>
        /// <param name="products"></param>
        /// <param name="htmlDocument"></param>
        private void CheckProduct(ref List<ListElement> products, in HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            var hrefs =
                htmlDocument.DocumentNode.Descendants()
                .Where(node => node.GetAttributeValue("href", "").Contains("ekaterinburg/product") && !node.GetAttributeValue("class", "").Equals("ImmXq WSf92")).ToList();
            foreach (var div in hrefs)
            {
                var listElement = new ListElement
                {
                    Link = div.GetAttributeValue("href", "").Substring(22),
                    Title = div.InnerText
                };
                if (!listElement.Title.Equals("Купить") && !listElement.Title.Equals("") && !products.Contains(listElement))
                    products.Add(listElement);
            }
        }

        /// <summary>
        /// Парсит все категории мебели
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ParseAllCategories(CancellationToken cancellationToken)
        {
            Products products = new Products();
            
            products.Clear();
            int ProgressBarValue = 0;
            int ProgressBarMax = _categories.Count;
            foreach (var cat in _categories)
            {
                products.AddRange(await ParseProductsOneCatAsync(categoryUrl + cat.Link, cancellationToken));
                ProgressBarValue++;
                OnAllCategoriesParsing?.Invoke(this, new AllCategoriesParsingArgs(ProgressBarMax, ProgressBarValue,products));
                if (cancellationToken.IsCancellationRequested)
                {
                    OrderProducts();
                    OnError?.Invoke(this, new NotificationEventArgs("All categories parsing has been canceled"));
                    return;
                }
            }
            OrderProducts();

            void OrderProducts()
            {
                products.OrderByTitle();
                for (int j = 0; j < products.Count - 1; j++)
                {
                    if (products[j + 1].Title == products[j].Title)
                        products.RemoveAt(j + 1);
                }
                OnAllCategoriesParsing?.Invoke(this, new AllCategoriesParsingArgs(ProgressBarMax, ProgressBarValue, products));
            }
        }

        
    }
}
