using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Divanru
{
    internal class Categories
    {
        const string siteurl = "https://www.divan.ru/ekaterinburg/";
        const string regexstring = @"""name"":""[^{]*""url"":""\\u002Fekaterinburg\\u002Fcategory\\u002F[\w-]*";
        const string catstring = "ekaterinburg\\u002Fcategory\\";

        public event EventHandler<ErrEventArgs> OnError;
        public event EventHandler<CatParsingEventArgs> OnParsing;

        private List<ListElement> _categories = new List<ListElement>();      //лист с категориями

        /// <summary>
        /// indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ListElement this [int index]
        { 
            get { return _categories [index]; }
            //set { _categories[index] = value; }
        }

        /// <summary>
        /// возвращает коллекцию из названия для листбокса
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<string> GetList()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            _categories.ForEach(c => list.Add(c.title));
            return list;
        }

        public int Count { get { return _categories.Count; } }

        /// <summary>
        /// Ищет категории товаров на сайте
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task FindCategories(string url = siteurl)
        {
            _categories.Clear();
            var httpclient = new HttpClient();

            try
            {
                var html = await httpclient.GetStringAsync(url);
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(html);
                var regex = new Regex(regexstring);

                var html1 = htmlDocument.DocumentNode.Descendants().Where(n => n.Name == "script")
                    .Where(n => n.InnerHtml.Contains(catstring)).FirstOrDefault()?.InnerText;
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
                        element.title = match?.Value.Substring(startindex, endindex - startindex);
                        startindex = match.Value.IndexOf("\"url\":\"") + 45;
                        endindex = match.Value.Length;
                        element.link = match?.Value.Substring(startindex, endindex - startindex);

                        if (!element.link.Contains("skidki") && !element.link.Contains("rasprodaz") && !element.link.Contains("extra-sale") && !element.link.Contains("promo-bud") && (_categories.FindIndex(catsEl => catsEl.link == element.link) == -1))
                            _categories.Add(element);
                    }
                    _categories = _categories.OrderBy(element => element.title).ToList();
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, new ErrEventArgs(e.Message));
            }
        }



        /// <summary>
        /// Парсить одну категорию товаров по адресу url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns> 
        public async Task<List<ListElement>> ParseProductsOneCat(string url)
        {
            List<ListElement> productsList = new List<ListElement>();
            try
            {
                var httpclient = new HttpClient();
                var html = await httpclient.GetStringAsync(url);
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(html);

                CheckProduct(ref productsList, in htmlDocument);

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
                }               
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, new ErrEventArgs(e.Message));
            }
            return productsList;

        }

        /// <summary>
        /// Проверяет корректность ссылки на продукт в меню категории
        /// </summary>
        /// <param name="products"></param>
        /// <param name="htmlDocument"></param>
        private void CheckProduct(ref List<ListElement> products, in HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            var divs =
                htmlDocument.DocumentNode.Descendants()
                .Where(node => (node.GetAttributeValue("href", "").Contains("ekaterinburg/product") && !node.GetAttributeValue("class", "").Equals("ImmXq WSf92"))).ToList();
            foreach (var div in divs)
            {
                var listElement = new ListElement();
                listElement.link = div.GetAttributeValue("href", "").Substring(22);
                listElement.title = div.InnerText;
                if (!listElement.title.Equals("Купить") && !listElement.title.Equals("") && !products.Contains(listElement))
                    products.Add(listElement);
            }
        }

        
    }
}
