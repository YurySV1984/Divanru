using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divanru
{
    public class ErrEventArgs
    {
        public ErrEventArgs(string v)
        {
            ErrorText = v;
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
}
