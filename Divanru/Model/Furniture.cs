using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divanru
{
    /// <summary>
    /// Класс для поиска мебели в БД
    /// </summary>
    public class SFurniture
    {
        /// <summary>
        /// Название мебели.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Первичный ключ в БД
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Возвращает коллекцию из названий мебели из массива.
        /// </summary>
        /// <param name="sFurTable">Массив SFurniture.</param>
        /// <returns>Коллекция названий мебели.</returns>
        public static ObservableCollection<string> GetModels(SFurniture[] sFurTable)
        {
            ObservableCollection<string> res = new ObservableCollection<string>();
            foreach (var sFurniture in sFurTable)
            {
                res.Add(sFurniture.Model);
            }
            return res;
        }
    }
        
    /// <summary>
    /// Единица мебели на сайте
    /// </summary>
    public class Furniture : SFurniture
    {
        /// <summary>
        /// Категория мебели.
        /// </summary>
        public string [] Categories { get; set; }

        /// <summary>
        /// Описание мебели.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Цена мебели.
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Старая цена мебели.
        /// </summary>
        public string OldPrice { get; set; }

        /// <summary>
        /// Ссылка текущей мебели на сайт.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Ссылка на изображение мебели на сайте.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Размеры мебели, до трех размеров.
        /// </summary>
        public string[] Size { get; set; }

        /// <summary>
        /// Характеристики мебели, до 14 характеристик.
        /// </summary>
        public string[] Characteristics { get; set; }

        /// <summary>
        /// Изображение мебели.
        /// </summary>
        public  byte[] Image { get; set; }
    }

}
