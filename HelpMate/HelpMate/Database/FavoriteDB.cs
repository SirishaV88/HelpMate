using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace HelpMate
{
    [Table("Favorite")]
    class FavoriteDB
    {
        [NotNull]
        public string Name { get; set; }

        [PrimaryKey, NotNull]
        public string PhoneNumber { get; set; }
        
        public string CallDateTimeStr { get; set; }       
        
    }
}
