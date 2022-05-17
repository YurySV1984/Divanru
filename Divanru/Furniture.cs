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
        public string Model { get; set; }
        public uint Id { get; set; }

        public static ObservableCollection<string> GetModels(SFurniture[] sFurTable)
        {
            ObservableCollection<string> res = new ObservableCollection<string>();
            for (int i = 0; i < sFurTable.Length; i++)
                res.Add(sFurTable[i].Model);
            return res;
        }
    }
    
    
    
    class Furniture : SFurniture
    {
        public string [] Categories { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string OldPrice { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
        public string[] Size { get; set; }
        public string[] Characteristics { get; set; }
        public  byte[] Image { get; set; }
    }

}
