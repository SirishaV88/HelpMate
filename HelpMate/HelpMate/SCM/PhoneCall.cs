using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpMate
{
    public class PhoneCall
    {
        public string FirstName
        {
            get;
            set;
        }
        public string LastName
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Address
        {
            get;
            set;
        }
        public string PhoneNumber
        {
            get;
            set;
        }

        public DateTime CallDateTime
        {
            get;
            set;
        }

        public string CallDateTimeStr
        {
            get;
            set;
        }

        public PhoneCall()
        {

        }

        public PhoneCall(string firstname, string lastname, string phone, DateTime callDateTime)
        {
            this.FirstName = firstname;
            this.LastName = lastname;
            this.CallDateTime = callDateTime;
            this.PhoneNumber = phone;
            this.Name = firstname + ", " + lastname;
            this.CallDateTimeStr = callDateTime.ToString("d");
        }

        public PhoneCall(string name, string phone, DateTime callDateTime)
        {
            this.CallDateTime = callDateTime;
            this.PhoneNumber = phone;
            this.Name = name;
            this.CallDateTimeStr = callDateTime.ToString("d");
        }
    }
}
