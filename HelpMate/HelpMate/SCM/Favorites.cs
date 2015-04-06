using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpMate
{
    class   Favorites
    {
        List<PhoneCall> favoritesData;

        public List<PhoneCall> GetAllFavorites()
        {
            List<PhoneCall> allFavorites = new DatabaseMgr().retrieveFavorites();
            favoritesData = allFavorites;

            return favoritesData;
        }

        public void AddFavorite(PhoneCall phoneCall)
        {
            new DatabaseMgr().AddFavorite(phoneCall);
            favoritesData.Add(phoneCall);
        }

        public static void Update(PhoneCall phoneCall)
        {
            phoneCall.CallDateTimeStr = DateTime.Today.ToString("d");
            new DatabaseMgr().UpdateFavorite(phoneCall);            
        }

        public void DeleteFavorite(PhoneCall phoneCall)
        {
            DatabaseMgr dbMgr = new DatabaseMgr();
            dbMgr.DeleteFavorite(phoneCall);
            favoritesData = dbMgr.retrieveFavorites();
        }

        public bool exists(PhoneCall phoneCall)
        {
            DatabaseMgr dbMgr = new DatabaseMgr();
            return dbMgr.exists(phoneCall);            
        }

        public bool existsAny()
        {
            DatabaseMgr dbMgr = new DatabaseMgr();
            return dbMgr.existsAnyFavorites();
        }

        public void FavoriteHelper()
        {
            DatabaseMgr dbMgr = new DatabaseMgr();

            dbMgr.AddFavorite(new PhoneCall("Ravi Eda", "701-367-7826", new DateTime(2014, 11, 16, 17, 0, 0)));
            dbMgr.AddFavorite(new PhoneCall("Jagdish Saripella", "404-775-7160", new DateTime(2014, 11, 18, 10, 0, 0)));
            dbMgr.AddFavorite(new PhoneCall("Sohan Das", "716-598-8112", new DateTime(2014, 11, 30, 13, 30, 0)));
        }
    }
}
