using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divanru
{
    public class EventArgs
    {
        public EventArgs(string ErrorText)
        {
            this.ErrorText = ErrorText;
        }
        public string ErrorText { get; }
    }

    public class CatParsingEventArgs
    {
        public CatParsingEventArgs(int MaxVal, int Val)
        {
            this.MaxVal = MaxVal;
            this.Val = Val;
        }
        public int MaxVal;
        public int Val;
    }

    public class AllCategoriesParsingArgs
    {
        public AllCategoriesParsingArgs(int MaxVal, int Val, Products products)
        {
            this.MaxVal=MaxVal;
            this.Val=Val;
            this.products = products;           
        }
        public int MaxVal;
        public int Val;
        public Products products;
    }

    public class CopyCatToDBArgs
    {
        public CopyCatToDBArgs(int MaxVal, int Val, Furniture furniture)
        {
            this.MaxVal = MaxVal;
            this.Val = Val;
            this.furniture = furniture;
        }
        public int MaxVal;
        public int Val;
        public Furniture furniture;

    }
}
