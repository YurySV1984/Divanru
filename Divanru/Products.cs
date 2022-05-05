using System;
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
    internal class Products
    {
        private static ObservableCollection<ListElement> _products = new ObservableCollection<ListElement>();
        const string iFile = "image.jpg";
        public event EventHandler<ErrEventArgs> OnError;
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

        internal static void AddRange(List<ListElement> productsint)
        {
            _products.AddRange(productsint);
        }

        //internal static List<ListElement> OrderBy()
        //{
        //    return (List<ListElement>)_products.OrderBy(p => p.title);
        //}

        internal void OrdBy()
        {
            //return (List<ListElement>)_products.OrderBy(p => p.title);
            _products = new ObservableCollection<ListElement>(_products.OrderBy(p => p.title).ToList());
        }

        internal void RemoveAt(int v)
        {
            _products.RemoveAt(v);
        }
        public ObservableCollection<string> GetList()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            foreach (var item in _products)
                list.Add(item.title);
            return list;
        }

        /// <summary>
        /// Загружает с сайта выбранный продукт
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task GetOneProduct(string url)
        {
            try
            {
                var httpclient = new HttpClient();
                var html = await httpclient.GetStringAsync("https://www.divan.ru" + url);
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(html);

                var divs = htmlDocument.DocumentNode.Descendants("a")
                    .Where(node => node.GetAttributeValue("class", "").Equals("ImmXq q20FV DACcg z4mg0 BreadcrumbLink"))?.ToArray();
                Furniture.Categories = new string[divs.Length - 1];
                for (int i = 1; i < divs.Length; i++)
                    Furniture.Categories[i - 1] = divs[i].InnerText;
                Furniture.Model = null;
                Furniture.Model = htmlDocument.DocumentNode.DescendantsAndSelf("h1")
                    .Where(node => node.GetAttributeValue("class", "").Equals("wAww4 z4mg0")).FirstOrDefault()?.InnerText;
                Furniture.Description = (htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("OJ95x"))?.FirstOrDefault()?.Descendants("p").FirstOrDefault()?.InnerText)?.Replace("&laquo;", "\"")?.Replace("&raquo;", "\"")?.Replace("&nbsp;", " ")?.Replace("&ndash;", "-");
                Furniture.Price = null;
                Furniture.Price = htmlDocument.DocumentNode.DescendantsAndSelf("span")
                    .Where(node => node.GetAttributeValue("class", "").Contains("Zq2dF F9ye5 cqsan KgyFz")).FirstOrDefault()?.InnerText;
                Furniture.OldPrice = null;
                Furniture.OldPrice = htmlDocument.DocumentNode.DescendantsAndSelf("span")
                    .Where(node => node.GetAttributeValue("class", "").Contains("Zq2dF h1mna F9ye5 wfxlK")).FirstOrDefault()?.InnerText;
                Furniture.Link = null;
                Furniture.Link = "https://www.divan.ru" + url;
                Furniture.Size = null;
                var size = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("Pl7um")).FirstOrDefault()?.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("Pv6jk lgaxY")).ToArray()
                    ?? htmlDocument.DocumentNode.DescendantsAndSelf("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("X6lT5")).FirstOrDefault()?.DescendantsAndSelf("div").ToArray();

                if (size != null)
                {
                    Furniture.Size = new string[size.Length];
                    for (int i = 0; i < Furniture.Size.Length; i++)
                        Furniture.Size[i] = size[i].InnerText.Replace("Размеры (Ш х Д х В): ", "").Replace("Размеры (Д х Ш): ", "");
                }

                var characteristics = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("O1hUI lgaxY R2259")).ToArray();
                Furniture.Characteristics = null;
                Furniture.Characteristics = new string[characteristics.Length];
                for (int i = 0; i < characteristics.Length; i++)
                {
                    characteristics[i].Descendants("div").FirstOrDefault()?.Remove();
                    Furniture.Characteristics[i] = characteristics[i].InnerText;
                }
                var imageUrlString = htmlDocument.DocumentNode.Descendants()
                                 .Where(n => n.Name == "script").Where(n => n.InnerHtml.Contains("\"product\":{\"id\":")).FirstOrDefault()?.InnerText;
                var imageUrlStart = imageUrlString.IndexOf("\"product\":{\"id\":") + 11;
                imageUrlStart = imageUrlString.IndexOf("{\"src\":\"", imageUrlStart) + 8;
                var imageUrlEnd = imageUrlString.IndexOf("orientation", imageUrlStart) - 3;
                Furniture.ImageUrl = null;
                Furniture.ImageUrl = imageUrlString.Substring(imageUrlStart, imageUrlEnd - imageUrlStart).Trim().Replace("\\u002F", "/");

                
                httpclient = new HttpClient();
                HttpResponseMessage response = await httpclient.GetAsync(Furniture.ImageUrl);
                Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
                    
            

                //var client = new WebClient();
                //var uri = new Uri(Furniture.ImageUrl);
                //client.DownloadFile(uri, iFile);
                //System.Threading.Thread.Sleep(30);
                //var fInfo = new FileInfo(iFile);
                //long numBytes = fInfo.Length;
                //var fStream = new FileStream(iFile, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(streamToReadFrom);
                Furniture.Image = null;
                //Furniture.Image = br.ReadBytes((int)numBytes);
                Furniture.Image = br.ReadBytes((int)streamToReadFrom.Length);
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, new ErrEventArgs(e.Message));
            }

        }
    }
}
