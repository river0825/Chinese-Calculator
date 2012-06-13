using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator
{
    public class CalcItem
    {
        public int Number{get; set;}
        public int Sign { get; set; }
        public override string ToString()
        {
            return ((Sign > 0) ? "+" : "-") + string.Format("{0:000}", Number);
        }
        public int Value { get { return Number * Sign; } }
    }
}
