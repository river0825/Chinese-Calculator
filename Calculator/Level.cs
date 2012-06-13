using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator
{
    public class Level
    {
        public int LevelId { get; set; }
        public string Caption { get; set; }
        public int NumberCount { get; set; }
        public int MaxResult { get; set; }
        public Func<int, CalcItem> NextNumber{get;set;}
        //(int currResult, Func<int, CalcItem> t){
        //    return t(currResult);
        //}
    }

}
