using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Windows.Storage;
using System.IO;

namespace HelpMate
{
    class DatabaseMgr
    {
        //const string dbName = "HelpMate.db";

        // <summary>
        /// The database path.
        /// </summary>
        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "HelpMate.sqlite"));

        private async Task<bool> checkDBAsync(string _dbName)
        {
            bool dbExists = true;
            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(_dbName);
            }
            catch (Exception)
            {
                dbExists = false;
            }

            return dbExists;
        }
       

        public void init()
        {            
            SQLiteConnection conn = new SQLite.SQLiteConnection(DB_PATH);
            conn.CreateTable<LocationDB>();
            conn.CreateTable<CallLogDB>();
            conn.CreateTable<FavoriteDB>();
            conn.CreateTable<LMLocationDB>();
            //conn.CreateTable<LMDoNotTrackLocationDB>();
        }


        #region FavoriteDB methods
        public void AddFavorite(PhoneCall phoneCall)
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);
            FavoriteDB newfavorite = new FavoriteDB()
            {
                Name = phoneCall.Name,
                PhoneNumber = phoneCall.PhoneNumber,
                CallDateTimeStr = phoneCall.CallDateTimeStr
            };

            conn.Insert(newfavorite);

            //this.retrieveFavorites();
            
        }

        public void UpdateFavorite(PhoneCall phoneCall)
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);
            FavoriteDB newfavorite = new FavoriteDB()
            {
                Name = phoneCall.Name,
                PhoneNumber = phoneCall.PhoneNumber,
                CallDateTimeStr = phoneCall.CallDateTimeStr
            };

            conn.Update(newfavorite);            

        }

        public void DeleteFavorite(PhoneCall phoneCall)
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);
            FavoriteDB newfavorite = new FavoriteDB()
            {
                Name = phoneCall.Name,
                PhoneNumber = phoneCall.PhoneNumber
            };

            conn.Delete<FavoriteDB>(newfavorite.PhoneNumber);
        }

        public bool exists(PhoneCall phoneCall)
        {
            bool exists = false;
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);

            string query = "SELECT * FROM Favorite WHERE FAVORITE.PHONENUMBER = " + phoneCall.PhoneNumber;             
            exists = conn.Find<FavoriteDB>(phoneCall.PhoneNumber) != null;

            return exists;
        }

        public bool existsAnyFavorites()
        {
            bool exists = false;
            List<PhoneCall> list = retrieveFavorites();
            if (list.Count == 0)
            {
                exists = false;
            }
            else
            {
                exists = true;
            }

            return exists;
        }

        public List<PhoneCall> retrieveFavorites()
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);
            List<FavoriteDB> retrievedTasks = conn.Table<FavoriteDB>().ToList<FavoriteDB>();
            List<PhoneCall> phoneCallList = new List<PhoneCall>();            

            foreach (FavoriteDB record in retrievedTasks)
            {
                PhoneCall phoneCall = new PhoneCall()
                {
                    Name = record.Name,                    
                    PhoneNumber = record.PhoneNumber,                    
                    CallDateTimeStr = record.CallDateTimeStr
                };

                phoneCallList.Add(phoneCall);
            }

            return phoneCallList;
        }


        #endregion


        #region CallLogDB methods
        public void AddCallLog(PhoneCall phoneCall)
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);
            CallLogDB newCallLog = new CallLogDB()
            {
                Name = phoneCall.Name,
                PhoneNumber = phoneCall.PhoneNumber,
                CallDateTime = phoneCall.CallDateTime
            };

            conn.Insert(newCallLog);            
        }

        //private async void OnExecuteJoin()
        //{
        //    SQLiteAsyncConnection conn = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "people.db"), true);
            
        //}

        public bool existsAnyCallLog()
        {
            bool exists = false;
            List<string> list = retrieveCallLog().Key;
            if (list.Count == 0)
            {
                exists = false;
            }
            else
            {
                exists = true;
            }

            return exists;
        }

        public KeyValuePair<List<string>, List<PhoneCall>> retrieveCallLog()
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);
            //List<CallLogDB> retrievedTasks = conn.Table<CallLogDB>().ToList<CallLogDB>();

            string query = "SELECT * FROM CallLog WHERE NOT EXISTS (SELECT * FROM FAVORITE WHERE FAVORITE.PHONENUMBER = CALLLOG.PHONENUMBER)";
            List<CallLogDB> newCallLog = conn.Query<CallLogDB>(query);

            List<string> phoneNumberList = new List<string>();
            List<PhoneCall> phoneCallObjList = new List<PhoneCall>();

            foreach (CallLogDB record in newCallLog)
            {
                PhoneCall phoneCall = new PhoneCall()
                {
                    Name = record.Name,
                    CallDateTime = record.CallDateTime,
                    CallDateTimeStr = record.CallDateTime.ToString("d"),
                    PhoneNumber = record.PhoneNumber
                };

                phoneNumberList.Add(record.PhoneNumber);
                phoneCallObjList.Add(phoneCall);
            }

            return new KeyValuePair<List<string>,List<PhoneCall>>(phoneNumberList, phoneCallObjList);
        }

        #endregion

        #region LocationDB methods
        public void AddLocation(double lat, double lon)
        {
            LocationDB newLocation = new LocationDB()
            {
                Latitude = lat,
                Longitude = lon
            };

            SQLiteConnection conn = new SQLiteConnection(DB_PATH);

            // Delete any saved locations before inserting new one.
            conn.DeleteAll<LocationDB>();
            conn.Insert(newLocation);
        }

        public bool existsAnyLMLocations()
        {
            bool exists = false;
            List<LMLocation> list = retrieveMostFrequentLMLocations();
            if (list.Count == 0)
            {
                exists = false;
            }
            else
            {
                exists = true;
            }

            return exists;
        }

        public LocationDB FindLocation()
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);
            LocationDB newLoc = null;

            List<LocationDB> retrievedTasks = conn.Table<LocationDB>().ToList<LocationDB>();

            foreach (LocationDB record in retrievedTasks)
            {
                newLoc = record;
            }
            
            return newLoc;
        }

        public void DeleteLocation()
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);

            // Delete all saved locations
            conn.DeleteAll<LocationDB>();
        }
        
        #endregion


        #region LMLocation methods
        public void AddLMLocation(LMLocation location)
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);
            LMLocationDB newLocation = new LMLocationDB()
            {   
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                LMDateTime = location.LMDateTime,
                Duration = location.Duration,
                Name = location.Name
            };            

            // Delete any saved locations before inserting new one.            
            conn.Insert(newLocation);
        }

        public void DeleteLMLocation(LMLocation location)
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);

            LMLocationDB newLoc = new LMLocationDB()
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                LMDateTime = location.LMDateTime,
                Duration = location.Duration,
                Name = location.Name,
                Id = location.Id
            };

            // Delete any saved locations before inserting new one.            
            conn.Delete<LMLocationDB>(newLoc.Id);
        }

        public List<LMLocation> retrieveMostFrequentLMLocations()
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);
            int frequency = 0;

            string query = "SELECT * FROM LMLOCATION ORDER BY LMLOCATION.DURATION DESC";
            List<LMLocationDB> locations = conn.Query<LMLocationDB>(query);
            List<LMLocation> LMLocations = new List<LMLocation>();

            foreach (LMLocationDB loc in locations)
            {
                LMLocation lmLocation = new LMLocation(loc.Id, frequency, loc.Latitude, loc.Longitude, loc.LMDateTime, loc.Duration, loc.Name);
                LMLocations.Add(lmLocation);
                frequency++;
            }

            return LMLocations;
        }
        #endregion



        public void DeleteAllData()
        {
            SQLiteConnection conn = new SQLiteConnection(DB_PATH);

            // Delete all saved locations
            conn.DeleteAll<LocationDB>();
            conn.DeleteAll<FavoriteDB>();
            conn.DeleteAll<CallLogDB>();
            conn.DeleteAll<LMLocationDB>();
            retrieveMostFrequentLMLocations();
        }
    }
}
