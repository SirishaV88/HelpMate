using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Sqlite;
using SQLite;

namespace HelpMate
{
    [Table("Location")]
    class LocationDB
    {
        [NotNull]
        public double Latitude { get; set; }
        [NotNull]
        public double Longitude { get; set; }
    }
}
