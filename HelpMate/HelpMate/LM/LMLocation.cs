using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpMate
{
    class LMLocation
    {
        public int Id { get; set; }

        public int Frequency { get; set; }
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public DateTime LMDateTime { get; set; }
        
        public int Duration { get; set; }

        public GeoCoordinate Coordinate { get; set; }

        public string Name { get; set; }

        // Constructor
        public LMLocation()
        {
        }
        public LMLocation(double lat, double lon, DateTime lmDateTime, int duration, string name)
        {
            this.Latitude = lat;
            this.Longitude = lon;
            this.LMDateTime = lmDateTime;
            this.Duration = duration;
            this.Coordinate = new GeoCoordinate(this.Latitude, this.Longitude);
            this.Name = name;
        }

        public LMLocation(int id, int frequency, double lat, double lon, DateTime lmDateTime, int duration, string name)
        {
            this.Id = id;
            this.Frequency = frequency;
            this.Latitude = lat;
            this.Longitude = lon;
            this.LMDateTime = lmDateTime;
            this.Duration = duration;            
            this.Coordinate = new GeoCoordinate(this.Latitude, this.Longitude);
            this.Name = name;
        }

    }
}
