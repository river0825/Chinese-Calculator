using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator.Models
{
    public class PlayRecord
    {
        public int PlayRecordId { get; set; }
        public int OkCount { get; set; }
        public int FailCount { get; set; }
        public int SkipCount { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
