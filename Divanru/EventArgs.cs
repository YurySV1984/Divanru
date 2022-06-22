using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divanru
{
    interface IProgressBarEventArgs
    {
        int MaxVal { get; }
        int Val { get; }
    }
    public class EventArgs
    {
        public EventArgs(string Text)
        {
            this.Text = Text;
        }
        public string Text { get; }
    }

    public class CatParsingEventArgs : IProgressBarEventArgs
    {
        public CatParsingEventArgs(int MaxVal, int Val)
        {
            this.MaxVal = MaxVal;
            this.Val = Val;
        }
        public int MaxVal { get; }
        public int Val { get; }
    }

    public class AllCategoriesParsingArgs : IProgressBarEventArgs
    {
        public AllCategoriesParsingArgs(int MaxVal, int Val, Products products)
        {
            this.MaxVal=MaxVal;
            this.Val=Val;
            this.Products = products;           
        }
        public int MaxVal { get; }
        public int Val { get; }
        public Products Products { get; }
    }

    public class CopyCatToDBArgs : IProgressBarEventArgs
    {
        public CopyCatToDBArgs(int MaxVal, int Val, Furniture furniture)
        {
            this.MaxVal = MaxVal;
            this.Val = Val;
            this.Furniture = furniture;
        }
        public int MaxVal { get; }
        public int Val { get; }
        public Furniture Furniture { get; }

    }
}
