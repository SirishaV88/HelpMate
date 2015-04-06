using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpMate
{
    class CallHistory
    {
        List<PhoneCall> allCallLog;
        List<string> allPhoneNumList;
        List<PhoneCall> mostFrequentNCalls;        
        const int TopNFrequent = 3;

        public List<PhoneCall> GetMostFrequentCallLog()
        {
            KeyValuePair<List<string>, List<PhoneCall>> callLogs;
            callLogs = new DatabaseMgr().retrieveCallLog();
            allPhoneNumList = callLogs.Key;
            allCallLog = callLogs.Value;

            return mineCallLog();
        }

        private List<PhoneCall> mineCallLog()
        {
            Dictionary<string, int> allCalls = new Dictionary<string, int>();

            foreach (PhoneCall result in allCallLog)
            {
                if (!allCalls.ContainsKey(result.PhoneNumber))
                {
                    allCalls.Add(result.PhoneNumber, 1);
                }
                else
                {
                    int val = allCalls[result.PhoneNumber];
                    allCalls[result.PhoneNumber] = val + 1;
                }
            }

            // Reverse sort.
	        // ... Can be looped over in the same way as above.
	        var items = from pair in allCalls
		        orderby pair.Value descending
		        select pair;

            int count = 0;
            mostFrequentNCalls = new List<PhoneCall>();
            foreach (KeyValuePair<string, int> pair in items)
            {
                if (count < TopNFrequent)
                {
                    int index = allPhoneNumList.IndexOf(pair.Key);
                    PhoneCall tmp = allCallLog[index];
                    mostFrequentNCalls.Add(tmp);
                }
                else
                {
                    break;
                }
                count++;

                Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }

            return mostFrequentNCalls;
        }

        public bool existsAny()
        {
            DatabaseMgr dbMgr = new DatabaseMgr();
            return dbMgr.existsAnyCallLog();
        }

        public List<PhoneCall> getCallLog()
        {
            List<PhoneCall> phoneCallLog = new List<PhoneCall>();
            phoneCallLog.Add(new PhoneCall("Sirisha Veeraganta", "716-704-8252", new DateTime(2014,12,4,18,0,0)));
            phoneCallLog.Add(new PhoneCall("Sirisha Veeraganta", "716-704-8252", new DateTime(2014,12,4, 13, 0, 0)));
            phoneCallLog.Add(new PhoneCall("Sirisha Veeraganta", "716-704-8252", new DateTime(2014, 12, 4, 9, 0, 0)));

            phoneCallLog.Add(new PhoneCall("Sanket Vasa", "701-936-9908", new DateTime(2014, 12, 2, 12, 30, 0)));            
            phoneCallLog.Add(new PhoneCall("Sanket Vasa", "701-936-9908", new DateTime(2014, 11, 27, 18, 30, 0)));

            phoneCallLog.Add(new PhoneCall("Ravi Eda", "701-367-7826", new DateTime(2014, 11, 16, 10, 0, 0)));
            
            return phoneCallLog;
        }

        public List<PhoneCall> getCallLog1()
        {
            List<PhoneCall> phoneCallLog = new List<PhoneCall>();
            phoneCallLog.Add(new PhoneCall("David Wayne", "701-521-8745", new DateTime(2014, 12, 9, 17, 0, 0)));
            phoneCallLog.Add(new PhoneCall("David Wayne", "701-521-8745", new DateTime(2014, 12, 9, 12, 30, 0)));
            phoneCallLog.Add(new PhoneCall("David Wayne", "701-521-8745", new DateTime(2014, 12, 8, 18, 30, 0)));
            phoneCallLog.Add(new PhoneCall("Sam Berg", "701-500-2341", new DateTime(2014, 12, 8, 10, 0, 0)));
            phoneCallLog.Add(new PhoneCall("Sam Berg", "701-500-2341", new DateTime(2014, 12, 6, 21, 0, 0)));
            phoneCallLog.Add(new PhoneCall("Jason Green", "701-281-6542", new DateTime(2014, 12, 5, 13, 30, 0)));

            return phoneCallLog;
        }

        public void CallLogHelper(List<PhoneCall> phoneCallLog)
        {
            DatabaseMgr dbMgr = new DatabaseMgr();

            foreach (PhoneCall call in phoneCallLog)
            {
                dbMgr.AddCallLog(call);                
            }
        }
    }
}
