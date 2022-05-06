using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Divanru
{
    internal class Products: IEnumerable<ListElement>
    {
        private ObservableCollection<ListElement> _products = new ObservableCollection<ListElement>();
        public event EventHandler<EventArgs> OnError;

        public IEnumerator<ListElement> GetEnumerator()
        {
            return ((IEnumerable<ListElement>)_products).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_products).GetEnumerator();
        }

        public ListElement this[int index]
        {
            get { return _products[index]; }
            set { _products[index] = value; }
        }
        public int Count { get { return _products.Count; } }

        internal void Clear()
        {
            _products.Clear();
        }

        internal void AddRange(List<ListElement> productsint)
        {
            _products.AddRange(productsint);
        }

        internal void OrderByTitle()
        {
            _products = new ObservableCollection<ListElement>(_products.OrderBy(p => p.Title).Distinct());
        }

        internal void RemoveAt(int v)
        {
            _products.RemoveAt(v);
        }

        public ObservableCollection<string> GetList()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            foreach (var product in _products)
                list.Add(product.Title);            
            return list;
        }

        /// <summary>
        /// Загружает с сайта выбранный продукт
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task GetOneProduct(string url, Furniture furniture)
        {
            try
            {
                var httpclient = new HttpClient();
                var html = await httpclient.GetStringAsync(url);
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(html);

                var divs = htmlDocument.DocumentNode.Descendants("a")
                    .Where(node => node.GetAttributeValue("class", "").Equals("ImmXq q20FV DACcg z4mg0 BreadcrumbLink"))?.ToArray();
                furniture.Categories = new string[divs.Length - 1];
                for (int i = 1; i < divs.Length; i++)
                    furniture.Categories[i - 1] = divs[i].InnerText;
                furniture.Model = null;
                furniture.Model = htmlDocument.DocumentNode.DescendantsAndSelf("h1")
                    .Where(node => node.GetAttributeValue("class", "").Equals("wAww4 z4mg0")).FirstOrDefault()?.InnerText;
                furniture.Description = (htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("OJ95x"))?.FirstOrDefault()?.Descendants("p").FirstOrDefault()?.InnerText)?.Replace("&laquo;", "\"")?.Replace("&raquo;", "\"")?.Replace("&nbsp;", " ")?.Replace("&ndash;", "-")?.Replace("&mdash;", "-");
                furniture.Price = null;
                furniture.Price = htmlDocument.DocumentNode.DescendantsAndSelf("span")
                    .Where(node => node.GetAttributeValue("class", "").Contains("Zq2dF F9ye5 cqsan KgyFz")).FirstOrDefault()?.InnerText;
                furniture.OldPrice = null;
                furniture.OldPrice = htmlDocument.DocumentNode.DescendantsAndSelf("span")
                    .Where(node => node.GetAttributeValue("class", "").Contains("Zq2dF h1mna F9ye5 wfxlK")).FirstOrDefault()?.InnerText;
                furniture.Link = url;
                furniture.Size = null;
                var size = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("Pl7um")).FirstOrDefault()?.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("Pv6jk lgaxY")).ToArray()
                    ?? htmlDocument.DocumentNode.DescendantsAndSelf("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("X6lT5")).FirstOrDefault()?.DescendantsAndSelf("div").ToArray();

                if (size != null)
                {
                    furniture.Size = new string[size.Length];
                    for (int i = 0; i < furniture.Size.Length; i++)
                        furniture.Size[i] = size[i].InnerText.Replace("Размеры (Ш х Д х В): ", "").Replace("Размеры (Д х Ш): ", "");
                }

                var characteristics = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("O1hUI lgaxY R2259")).ToArray();
                furniture.Characteristics = null;
                furniture.Characteristics = new string[characteristics.Length];
                for (int i = 0; i < characteristics.Length; i++)
                {
                    characteristics[i].Descendants("div").FirstOrDefault()?.Remove();
                    furniture.Characteristics[i] = characteristics[i].InnerText;
                }
                var imageUrlString = htmlDocument.DocumentNode.Descendants()
                                 .Where(n => n.Name == "script").Where(n => n.InnerHtml.Contains("\"product\":{\"id\":")).FirstOrDefault()?.InnerText;
                var imageUrlStart = imageUrlString.IndexOf("\"product\":{\"id\":") + 11;
                imageUrlStart = imageUrlString.IndexOf("{\"src\":\"", imageUrlStart) + 8;
                var imageUrlEnd = imageUrlString.IndexOf("orientation", imageUrlStart) - 3;
                furniture.ImageUrl = null;
                furniture.ImageUrl = imageUrlString.Substring(imageUrlStart, imageUrlEnd - imageUrlStart).Trim().Replace("\\u002F", "/");

                
                httpclient = new HttpClient();
                HttpResponseMessage response = await httpclient.GetAsync(furniture.ImageUrl);
                Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
                var br = new BinaryReader(streamToReadFrom);
                furniture.Image = null;
                furniture.Image = br.ReadBytes((int)streamToReadFrom.Length);
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, new EventArgs(e.Message));
            }

        }

        
    }
}
