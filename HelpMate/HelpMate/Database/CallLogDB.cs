using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace HelpMate
{
    [Table("CallLog")]
    class CallLogDB
    {
        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public string PhoneNumber { get; set; }

        [NotNull]
        public DateTime CallDateTime { get; set; }
        
    }
}
