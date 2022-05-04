using System;
using System.Collections.Generic;
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

        private static List<ListElement> _cats = new List<ListElement>();      //лист с категориями

        public ListElement this [int index]
        { 
            get { return _cats [index]; }
            set { _cats[index] = value; }
        }

        public List<string> GetList()
        {
            List<string> list = new List<string>(_cats.Count);
            //foreach (var item in _cats)
            for (int i = 0; i < _cats.Count; i++)
                list.Add(_cats[i].title);
                
            return list;
        }

        public static int Count { get { return _cats.Count; } }

        /// <summary>
        /// Парсит категории товаров на сайте
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task ParseCats(string url = siteurl)
        {
            _cats.Clear();
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
                        var el = new ListElement();
                        startindex = match.Value.IndexOf("\"name\":\"") + 8;
                        endindex = match.Value.IndexOf("\"", startindex);
                        el.title = match?.Value.Substring(startindex, endindex - startindex);
                        startindex = match.Value.IndexOf("\"url\":\"") + 7;
                        endindex = match.Value.Length;
                        el.link = match?.Value.Substring(startindex, endindex - startindex).Replace("\\u002F", "/");

                        if (!el.link.Contains("skidki") && !el.link.Contains("rasprodaz") && !el.link.Contains("extra-sale") && !el.link.Contains("promo-bud") && (_cats.FindIndex(catsEl => catsEl.link == el.link) == -1))
                            _cats.Add(el);
                    }
                    _cats = _cats.OrderBy(el => el.title).ToList();
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
        public async Task ParseProductsOneCat(string url)
        {
            try
            {
                List<ListElement> productsint = new List<ListElement>();
                var httpclient = new HttpClient();
                var html = await httpclient.GetStringAsync("https://www.divan.ru" + url);
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(html);

                CheckProduct(ref productsint, htmlDocument);


                var lastp = 0;
                var lastpages = htmlDocument.DocumentNode.Descendants("a")
                    .Where(node => node.GetAttributeValue("class", "").Equals("ImmXq R9QDJ hi0qF PaginationLink")).ToList();
                foreach (var lastpage in lastpages)
                {
                    if (int.TryParse(lastpage.InnerHtml, out int page))
                        lastp = page > lastp ? page : lastp;
                }
                for (int i = 2; i <= lastp; i++)
                {
                    httpclient = new HttpClient();
                    html = await httpclient.GetStringAsync("https://www.divan.ru" + url + "/page-" + i);
                    htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    CheckProduct(ref productsint, htmlDocument);

                }
                //for (int i = 0; i < products.Count; i++) Products.Add(products[i]);
                Products.AddRange(productsint);
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, new ErrEventArgs(e.Message));
            }

        }

        /// <summary>
        /// Проверяет корректность ссылки на продукт в меню категории
        /// </summary>
        /// <param name="products"></param>
        /// <param name="htmlDocument"></param>
        private static void CheckProduct(ref List<ListElement> products, HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            var divs =
                htmlDocument.DocumentNode.Descendants()
                .Where(node => (node.GetAttributeValue("href", "").Contains("ekaterinburg/product") && !node.GetAttributeValue("class", "").Equals("ImmXq WSf92"))).ToList();
            foreach (var div in divs)
            {
                var listElement = new ListElement();
                listElement.link = div.GetAttributeValue("href", "").ToString();
                listElement.title = div.InnerText;
                if (!listElement.title.Equals("Купить") && !listElement.title.Equals("") && !products.Contains(listElement))
                    products.Add(listElement);
            }
        }
    }
}
