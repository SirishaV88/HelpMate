using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Sqlite;
using SQLite;

namespace HelpMate
{
    [Table("LMLocation")]
    class LMLocationDB
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public double Latitude { get; set; }
        [NotNull]
        public double Longitude { get; set; }
        [NotNull]
        public DateTime LMDateTime { get; set; }
        [NotNull]
        public int Duration { get; set; }

        public string Name { get; set; }
    
    }
}
