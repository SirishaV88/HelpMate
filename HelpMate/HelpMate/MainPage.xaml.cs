//USE_WP8_NATIVE_SQLITE use this for namespace community build isseusing System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location; // Provides the GeoCoordinate class.
using Windows.Devices.Geolocation; //Provides the Geocoordinate class.
using System.Windows.Media;
using System.Windows.Shapes;
using CustomPushpinWp8;
//using System.Windows.MessageBoxButton;
using Microsoft.Phone.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Phone.Maps.Toolkit;
//using System.Windows.Forms.MessageBoxButtons;

namespace HelpMate
{
    public partial class MainPage : PhoneApplicationPage
    {
        Geolocator geo = null;
        DatabaseMgr databaseMgr;
        CallHistory callHistory;
        Favorites favorites;
        LMLocationHelper lmLocationHelper;
        public List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
        List<LMLocation> frequentLMLocations;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            initMap();
            initDatabase();
            initSCM();
            initLM();
            //checkData();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        private void checkData()
        {
            DatabaseMgr databaseMgr = new DatabaseMgr();
            // Delete these three lines
            databaseMgr.retrieveCallLog();
            databaseMgr.retrieveFavorites();
            databaseMgr.retrieveMostFrequentLMLocations();
        }

        #region Initializers

        private void initSCM()
        {
            callHistory = new CallHistory();
            if (!callHistory.existsAny())
            {
                callHistory.CallLogHelper(callHistory.getCallLog());
            }

            favorites = new Favorites();
            if (!favorites.existsAny())
            {
                favorites.FavoriteHelper();
            }
        }

        private void initLM()
        {
            lmLocationHelper = new LMLocationHelper();
            if (!lmLocationHelper.existsAny())
            {
                lmLocationHelper.LMAddHelper();
            }
        }

        private void initMap()
        {
            // Init the ParkMateMap
            ParkMateMap.LandmarksEnabled = true;
            ParkMateMap.PedestrianFeaturesEnabled = true;
        }

        private void initDatabase()
        {
            // Init the database
            databaseMgr = new DatabaseMgr();
            databaseMgr.init();
            //databaseMgr.DeleteAllData();           
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            string strItemIndex;

            if (NavigationContext.QueryString.TryGetValue("goto", out strItemIndex))
            {
                AppTitle.DefaultItem = AppTitle.Items[Convert.ToInt32(strItemIndex)];
                refreshLMLocations();
            }
 
        }

        #endregion

        #region onLoaded event handlers
        
        private void myMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "01b6d5fe-040e-4897-b2bd-fb0250cb060a";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "hgQWR6jRQ4Hh7otCXMDL-Q";
        }

        #endregion

        #region PM methods

        private void DeleteFavorite(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            PhoneCall phoneDataContext = menuItem.DataContext as PhoneCall;
            favorites.DeleteFavorite(phoneDataContext);
            populateFavoritesList();
            
            //var selected = (sender as MenuItem);
            //MessageBox.Show("You chose to  " + menuItem.Header.ToString(),"Result",MessageBoxButton.OK);
        }


            //Do something

        //}

        private async void SaveButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (geo == null)
            {
                geo = new Geolocator();
            }

            geo.DesiredAccuracyInMeters = 25;
            Geoposition pos = await geo.GetGeopositionAsync();
            double lat = pos.Coordinate.Point.Position.Latitude;
            double lon = pos.Coordinate.Point.Position.Longitude;
            double accuracy = pos.Coordinate.Accuracy;
            if (pos.CivicAddress != null)
            {
                string civicAddress = pos.CivicAddress.ToString();
            }

            //textLatitude.Text = "Latitude: " + lat.ToString();
            //textLongitude.Text = "Longitude: " + lon.ToString();
            //textAccuracy.Text = "Accuracy: " + accuracy.ToString();

            showAndCenterMyLocation(lat, lon, 15);

            databaseMgr.AddLocation(lat, lon);

        }

        private void DeleteButton_Click(
            object sender, RoutedEventArgs e)
        {
            databaseMgr.DeleteLocation();
            ParkMateMap.Layers.RemoveAt(0);            

            //// Find current location and center map
            //Tuple<double, double, double> loc = getCurrentLocation().Result;
            //double lat = loc.Item1;
            //double lon = loc.Item2;
            //double accuracy = loc.Item3;

            //centerMap(lat, lon, accuracy);

        }

        protected void showAndCenterMyLocation(double lat, double lon, double accuracy)
        {
            ParkMateMap.Center = new GeoCoordinate(lat, lon);
            ParkMateMap.ZoomLevel = accuracy;

            // Create a small circle to mark the current location.
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(Colors.Blue);
            myCircle.Height = 20;
            myCircle.Width = 20;
            myCircle.Opacity = 50;

            // Create a MapOverlay to contain the circle.
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = myCircle;
            myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = ParkMateMap.Center;

            // Create a MapLayer to contain the MapOverlay.
            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);

            // Add the MapLayer to the Map.
            ParkMateMap.Layers.Add(myLocationLayer);
        }

        private async void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            //Sunmart lat lon = 46.83321,-96.8198878
            // Home lat lon = 46.83016,-96.829341

            double latitude = 0.0;
            double longitude = 0.0;
            string name = "Fargo, ND";

            LocationDB loc = databaseMgr.FindLocation();
            if (loc != null)
            {
                latitude = loc.Latitude;
                longitude = loc.Longitude;
            }

            #region This is using bing maps

            //latitude = 46.83321;
            //longitude = -96.8198878;

            ////// The URI to launch
            //string uriToLaunch = @"bingmaps:?cp="+ latitude + "~" + longitude + "&lvl=17";
            //var uri = new Uri(uriToLaunch);

            //// Launch the URI
            //var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            //if (success)
            //{
            //    // URI launched
            //}
            //else
            //{
            //    // URI launch failed
            //}
            #endregion

            #region This is using any other app
            // Get the values required to specify the destination.            
            //latitude = 46.8949507;
            //longitude = -96.8021357;

            if (loc != null)
            {
                // Assemble the Uri to launch.
                Uri uri = new Uri("ms-drive-to:?destination.latitude=" + latitude +
                    "&destination.longitude=" + longitude + "&destination.name=" + name);

                // Launch the Uri.
                var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            }

            #endregion

        }

        #endregion

        #region SCM methods

        private void populateFavoritesList()
        {
            List<PhoneCall> favoritesData = favorites.GetAllFavorites();
            List<SCM_AlphaKeyGroup<PhoneCall>> DataSource = SCM_AlphaKeyGroup<PhoneCall>.CreateGroups(favoritesData,
                System.Threading.Thread.CurrentThread.CurrentUICulture,
                (PhoneCall s) => { return s.Name; }, true);

            FavoritesList.ItemsSource = DataSource;
        }

        private void SCMLoaded(object sender, RoutedEventArgs e)
        {
            populateFavoritesList();
        }

        private void SuggestButton_Click(object sender, RoutedEventArgs e)
        {
            List<PhoneCall> callLog = callHistory.GetMostFrequentCallLog();
            bool favoriteAdded = false;

            if (callLog != null)
            {
                foreach (PhoneCall call in callLog)
                {
                    if (!favorites.exists(call))
                    {
                        MessageBoxResult res = MessageBox.Show(String.Format("Do you want to add {0}, {1} to favorites", call.Name, call.PhoneNumber), "Add to Favorite?", MessageBoxButton.OKCancel);

                        if (res == MessageBoxResult.OK)
                        {
                            favoriteAdded = true;
                            favorites.AddFavorite(call);
                        }                        
                    }                    
                }
            }

            if (favoriteAdded)
            {
                populateFavoritesList();
                AppTitle.DefaultItem = AppTitle.Items[1];
            }
        }

        private void RemindButton_Click(object sender, RoutedEventArgs e)
        {
            List<PhoneCall> favoriteList = favorites.GetAllFavorites();
            List<PhoneCall> callList = callHistory.getCallLog1();
            List<PhoneCall> toCommunicateList = new List<PhoneCall>();

            foreach(PhoneCall fav in favoriteList)
            {
                bool found = false;
                foreach (PhoneCall call in callList)
                {
                    if (fav.PhoneNumber == call.PhoneNumber)
                    {
                        found = found || true;
                    }
                }

                string callDateTimeStr = fav.CallDateTimeStr;
                DateTime dt = Convert.ToDateTime(callDateTimeStr);
                double totalDays = (DateTime.Today - dt).TotalDays;

                if (!found && totalDays >= 30)
                {
                    toCommunicateList.Add(fav);
                }
            }

            foreach (PhoneCall toComm in toCommunicateList)
            {   
                CustomMessageBox messageBox = new CustomMessageBox()
                {
                    Message = String.Format("Do you want to connect with this person {0} ", toComm.Name, toComm.PhoneNumber),
                    Caption = "Stay connected?",
                    LeftButtonContent = "Call",
                    RightButtonContent = "Text"
                };

                messageBox.Dismissed += (s1, e1) =>
                {
                    switch (e1.Result)
                    {
                        case CustomMessageBoxResult.LeftButton:
                            var phoneTask = new PhoneCallTask
                            {
                                DisplayName = toComm.Name,
                                PhoneNumber = toComm.PhoneNumber.ToString()
                            };
                            phoneTask.Show();
                            Favorites.Update(toComm);
                            populateFavoritesList();

                            break;
                        case CustomMessageBoxResult.RightButton:
                            SmsComposeTask smsComposeTask = new SmsComposeTask();
                            smsComposeTask.To = toComm.PhoneNumber.ToString();
                            smsComposeTask.Body = "Hey how have you been doing!";
                            smsComposeTask.Show();
                            Favorites.Update(toComm);
                            populateFavoritesList();

                            break;
                        case CustomMessageBoxResult.None:
                            // Do something.
                            break;
                        default:
                            break;
                    }
                };

                messageBox.Show();                
            }

        }
        #endregion

        #region LocationMate methods

        private void refreshLocationsOnMap()
        {
            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(FrequentLocationsMap);
            var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;
            if (obj.Items == null || obj.Items.Count == 0)
            {
                obj.ItemsSource = frequentLMLocations;    
                //obj.Items.Clear();
            }            

            if (MyCoordinates != null && MyCoordinates.Count != 0)
            { 
                FrequentLocationsMap.Center = MyCoordinates[MyCoordinates.Count - 1];                
                FrequentLocationsMap.SetView(MyCoordinates[MyCoordinates.Count - 1], 10, MapAnimationKind.Linear);
            }
        }

        private void refreshLMLocations(LMLocation lmLocation = null)
        {
            frequentLMLocations = lmLocationHelper.GetMostFrequentLocations();
            MyCoordinates.Clear();
            foreach (LMLocation lmLoc in frequentLMLocations)
            {
                MyCoordinates.Add(new GeoCoordinate { Latitude = lmLoc.Latitude, Longitude = lmLoc.Longitude });
            }            
            
            LocList.ItemsSource = frequentLMLocations;

            //FrequentLocationsMap.Layers.Clear();            
            //DrawMapMarkers();

            //FrequentLocationsMap.SetView(frequentLMLocations.First<LMLocation>().Coordinate, 12);
            // Set the center of the FrequentLocationsMap           
        }       
        

        private void DrawMapMarkers()
        {
            //FrequentLocationsMap.Layers.Clear();
            MapLayer mapLayer = new MapLayer();
            // Draw marker for current position       

            // Draw markers for location(s) / destination(s)
            foreach (LMLocation lmLoc in frequentLMLocations)
            {
                //DrawMapMarker(MyCoordinates[i], Colors.Red, mapLayer, parklist.parking_details[i].DestinationName);
                UCCustomToolTip _tooltip = new UCCustomToolTip();
                _tooltip.Description = lmLoc.Name;
                _tooltip.DataContext = lmLoc;                
                _tooltip.Menuitem.Click += Menuitem_Click;
                _tooltip.imgmarker.Tap += _tooltip_Tapimg;
                MapOverlay overlay = new MapOverlay();
                overlay.Content = _tooltip;
                overlay.GeoCoordinate = new GeoCoordinate { Latitude = lmLoc.Latitude, Longitude = lmLoc.Longitude };
                overlay.PositionOrigin = new Point(0.0, 1.0);
                mapLayer.Add(overlay);
            }

            FrequentLocationsMap.Layers.Add(mapLayer);
        }

        private void Menuitem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem)sender;
                string selecteditem = item.Tag.ToString();
                var selectedparkdata = frequentLMLocations.Where(s => s.Name == selecteditem).ToList();
                if (selectedparkdata.Count > 0)
                {
                    foreach (var items in selectedparkdata)
                    {

                        //if (Settings.FileExists("LocationDetailItem"))
                        //{
                        //    Settings.DeleteFile("LocationDetailItem");
                        //}
                        //using (IsolatedStorageFileStream fileStream = Settings.OpenFile("LocationDetailItem", FileMode.Create))
                        //{
                        //    DataContractSerializer serializer = new DataContractSerializer(typeof(LocationDetail));
                        //    serializer.WriteObject(fileStream, items);

                        //}
                        //NavigationService.Navigate(new Uri("/MapViewDetailsPage.xaml", UriKind.Relative));
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        private void _tooltip_Tapimg(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                Image item = (Image)sender;
                string selecteditem = item.Tag.ToString();
                var selectedparkdata = frequentLMLocations.Where(s => s.Name == selecteditem).ToList();

                if (selectedparkdata.Count > 0)
                {
                    foreach (var items in selectedparkdata)
                    {
                        ContextMenu contextMenu = ContextMenuService.GetContextMenu(item);
                        contextMenu.DataContext = items;
                        if (contextMenu.Parent == null)
                        {
                            contextMenu.IsOpen = true;

                        }
                        break;
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        private void DeleteLMLocation(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            LMLocation lmLoc = menuItem.DataContext as LMLocation;
            lmLocationHelper.LMLocationDelete(lmLoc);
            refreshLMLocations(lmLoc);
        }

        private void LoadLocations(object sender, RoutedEventArgs e)
        {
            refreshLMLocations();
        }

        private void LoadLocationsOnMap(object sender, RoutedEventArgs e)
        {
            refreshLocationsOnMap();
        }

        private async void NavigateLMLocation(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            LMLocation lmLoc = menuItem.DataContext as LMLocation;

            if (lmLoc != null)
            {
                // Assemble the Uri to launch.
                Uri uri = new Uri("ms-drive-to:?destination.latitude=" + lmLoc.Latitude +
                    "&destination.longitude=" + lmLoc.Longitude + "&destination.name=" + lmLoc.Name);

                // Launch the Uri.
                var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            }
            
        }

        private void CreateLoc_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddLocation.xaml", UriKind.Relative));
            //MessageBoxResult res = MessageBox.Show(String.Format("Do you want to add {0} to Locations list", "NDSU"), "Add to Favorite?", MessageBoxButton.OKCancel);
        }

        private void NotifyClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hey I noticed that you goto NDSU at this time every day", "Location suggestion", MessageBoxButton.OK);
        }

        private void DateChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            
        }       

              
    }
}