using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divanru
{
    class SFurniture
    {
        public string model { get; set; }
        public uint id { get; set; }

        public static ObservableCollection<string> GetModels(SFurniture[] sFurTable)
        {
            ObservableCollection<string> res = new ObservableCollection<string>();
            for (int i = 0; i < sFurTable.Length; i++)
                res.Add(sFurTable[i].model);
            return res;
        }
    }
    
    
    
    class Furniture : SFurniture
    {
        public static string [] Categories { get; set; }
        
        public static string Model { get; set; }
        public static string Description { get; set; }
        public static string Price { get; set; }
        public static string OldPrice { get; set; }
        public static string Link { get; set; }
        public static string ImageUrl { get; set; }
        public static string[] Size { get; set; }
        public static string[] Characteristics { get; set; }
        public static byte[] Image { get; set; }


        public string[] categories { get; set; }
        public string description { get; set; }
        public string price { get; set; }
        public string oldPrice { get; set; }
        public string link { get; set; }
        public string imageUrl { get; set; }
        public string[] size { get; set; }
        public string[] characteristics { get; set; }
        public byte[] image { get; set; }
    }

}
