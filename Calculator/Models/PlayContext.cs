using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Calculator.Models
{
    public class PlayContext : DbContext
    {
        public DbSet<PlayRecord> PlayRecords { get; set; }
    }
}
