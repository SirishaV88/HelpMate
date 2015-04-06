using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;

namespace HelpMate
{
    public partial class AddLocation : PhoneApplicationPage
    {
        Geolocator geo = null;

        public AddLocation()
        {
            InitializeComponent();
        }

        private async void SaveLocation(object sender, RoutedEventArgs e)
        {
            DatabaseMgr dbMgr = new DatabaseMgr();

            if (geo == null)
            {
                geo = new Geolocator();
            }

            geo.DesiredAccuracyInMeters = 25;
            Geoposition pos = await geo.GetGeopositionAsync();
            double lat = pos.Coordinate.Point.Position.Latitude;
            double lon = pos.Coordinate.Point.Position.Longitude;            

            if (locInput.Text != null)
            {
                dbMgr.AddLMLocation(new LMLocation(lat, lon, DateTime.Now, 1000000, locInput.Text));
                NavigationService.Navigate(new Uri("/MainPage.xaml?goto=3", UriKind.Relative));
            }
                        
        }
    }
}