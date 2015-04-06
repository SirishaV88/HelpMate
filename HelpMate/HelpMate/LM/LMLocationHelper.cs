using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpMate
{
    class LMLocationHelper
    {
        List<LMLocation> frequentLocations;

        public List<LMLocation> GetMostFrequentLocations()
        {
            frequentLocations = new DatabaseMgr().retrieveMostFrequentLMLocations();
            return frequentLocations;
        }

        public bool existsAny()
        {
            DatabaseMgr dbMgr = new DatabaseMgr();
            return dbMgr.existsAnyLMLocations();
        }

        public void LMAddHelper()
        {
            DatabaseMgr dbMgr = new DatabaseMgr();

            dbMgr.AddLMLocation(new LMLocation(46.83016, -96.829341, new DateTime(2014, 12, 9, 17, 0, 0), 100000, "Home" ));
            dbMgr.AddLMLocation(new LMLocation(46.8936083, -96.8035308, new DateTime(2014, 12, 9, 17, 0, 0), 20000, "NDSU"));
            dbMgr.AddLMLocation(new LMLocation(46.836152, -96.879533, new DateTime(2014, 12, 9, 17, 0, 0), 5000,"Gym"));
        }

        public void LMLocationDelete(LMLocation loc)
        {
            DatabaseMgr dbMgr = new DatabaseMgr();
            dbMgr.DeleteLMLocation(loc);
        }
    }
}
