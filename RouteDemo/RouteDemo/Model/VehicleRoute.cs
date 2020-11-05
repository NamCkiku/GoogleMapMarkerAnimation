using System;

namespace RouteDemo.Model
{
    public class VehicleRoute
    {
        public int Index { get; set; }

        public DateTime Time { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }


        public double Heading1 { get; set; }

        public double Heading2 { get; set; }

        public double DeltaAngle { get; set; }

        public float? Direction { get; set; }

        public int Velocity { get; set; }
    }
}