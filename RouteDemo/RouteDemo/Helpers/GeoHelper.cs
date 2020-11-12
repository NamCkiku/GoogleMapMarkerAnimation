using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace RouteDemo.Helpers
{
    /// <summary>
    /// Class dùng để sử lý liên quan đến tọa độ
    /// </summary>
    /// <Modified>
    /// Name     Date         Comments
    /// namth  12/21/2018   created
    /// </Modified>
    /// <Modified>
    /// Name     Date         Comments
    /// namth  12/21/2018   created
    /// </Modified>
    public static class GeoHelper
    {
        public static double DistanceCalculatorCoordinate(double lng, double lat, double lngPre, double latPre)
        {
            double P1X = lng * (Math.PI / 180);
            double P1Y = lat * (Math.PI / 180);
            double P2X = lngPre * (Math.PI / 180);
            double P2Y = latPre * (Math.PI / 180);
            double Kc = 0;
            double Temp = 0;

            Kc = P2X - P1X;
            Temp = Math.Cos(Kc);
            Temp = Temp * Math.Cos(P2Y);
            Temp = Temp * Math.Cos(P1Y);

            Kc = Math.Sin(P1Y);
            Kc = Kc * Math.Sin(P2Y);
            Temp = Temp + Kc;
            Kc = Math.Acos(Temp);
            Kc = Kc * 6376;

            return Kc;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="oldLongitude"></param>
        /// <param name="oldLatitude"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public static byte Bearing(double distance, byte oldBearing, double oldLongitude, double oldLatitude, double longitude, double latitude)
        {
            //If longitude and latitude are not valid, don't change car's direction
            if (longitude == 0 | latitude == 0) return oldBearing;

            //If distance between two cars is too small, retur old Bearing
            if (distance < 0.02)
            {
                return oldBearing;
            }

            byte bearing = 0;

            //Calculate new direction
            double DeltaX = latitude - oldLatitude;
            double DeltaY = longitude - oldLongitude;
            double S = Math.Sqrt(Math.Pow(DeltaX, 2) + Math.Pow(DeltaY, 2));
            double G = Math.Acos(DeltaX / S);
            if (DeltaY < 0) G = 2 * Math.PI - G;
            G = Math.Round(4 * G / Math.PI);
            if (G > 7 || G < 0) G = 0;

            oldLatitude = latitude;
            oldLongitude = longitude;

            try
            { bearing = (byte)G; }
            catch
            { bearing = 0; }

            return bearing;
        }

        public static double ComputeBearing(double lat1, double lng1, double lat2, double lng2)
        {
            var dLon = lng2 - lng1;
            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            return (180.0 / Math.PI) * Math.Atan2(y, x);
        }

        public static double GetRotaion(double start, double end)
        {
            var normalizeEnd = end - start; // rotate start to 0
            var normalizedEndAbs = (normalizeEnd + 360) % 360;

            // -1 = anticlockwise, 1 = clockwise
            var direction = normalizedEndAbs > 180 ? -1 : 1;
            double rotation;
            if (direction > 0)
            {
                rotation = normalizedEndAbs;
            }
            else
            {
                rotation = normalizedEndAbs - 360;
            }
            return rotation;
        }

        /** tính lại góc quay theo chiều đồng hồ */

        public static double ComputeRotation(double fraction, double start, double newRotation)
        {
            double result = fraction * newRotation + start;
            //xử lý tính lại khi góc quay giá trị < 0
            return (result + 360) % 360;
        }

        /** tính hướng */

        public static double ComputeHeading(double fromLat, double fromLng, double toLat, double toLng)
        {
            // http://williams.best.vwh.net/avform.htm#Crs
            double fromLatR = ToRadians(fromLat);

            double fromLngR = ToRadians(fromLng);

            double toLatR = ToRadians(toLat);

            double toLngR = ToRadians(toLng);

            double dLng = toLngR - fromLngR;

            double heading = Math.Atan2(Math.Sin(dLng) * Math.Cos(toLatR),
                Math.Cos(fromLatR) * Math.Sin(toLatR) - Math.Sin(fromLatR) * Math.Cos(toLatR) * Math.Cos(dLng));

            return Wrap(ToDegrees(heading), -180, 180);
        }

        public static double ToRadians(double angdeg)
        {
            return angdeg / 180.0 * Math.PI;
        }

        public static double ToDegrees(double angrad)
        {
            return angrad * 180.0 / Math.PI;
        }

        public static double Wrap(double n, double min, double max)
        {
            return (n >= min && n < max) ? n : (Mod(n - min, max - min) + min);
        }

        public static double Mod(double x, double m)
        {
            return ((x % m) + m) % m;
        }

        public static double ComputeAngleBetween(double fromLat, double fromLng, double toLat, double toLng)
        {
            // Haversine's formula
            double dLat = fromLat - toLat;
            double dLng = fromLng - toLng;
            return 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(dLat / 2), 2) +
                    Math.Cos(fromLat) * Math.Cos(toLat) * Math.Pow(Math.Sin(dLng / 2), 2)));
        }

        public static Position Interpolate(double fraction, Position from, Position to)
        {
            // http://en.wikipedia.org/wiki/Slerp
            double fromLat = ToRadians(from.Latitude);
            double fromLng = ToRadians(from.Longitude);
            double toLat = ToRadians(to.Latitude);
            double toLng = ToRadians(to.Longitude);
            double cosFromLat = Math.Cos(fromLat);
            double cosToLat = Math.Cos(toLat);

            // Computes Spherical interpolation coefficients.
            double angle = ComputeAngleBetween(fromLat, fromLng, toLat, toLng);
            double sinAngle = Math.Sin(angle);
            if (sinAngle < 1E-6)
            {
                return from;
            }
            double a = Math.Sin((1 - fraction) * angle) / sinAngle;
            double b = Math.Sin(fraction * angle) / sinAngle;

            // Converts from polar to vector and interpolate.
            double x = a * cosFromLat * Math.Cos(fromLng) + b * cosToLat * Math.Cos(toLng);
            double y = a * cosFromLat * Math.Sin(fromLng) + b * cosToLat * Math.Sin(toLng);
            double z = a * Math.Sin(fromLat) + b * Math.Sin(toLat);

            // Converts interpolated vector back to polar.
            double lat = Math.Atan2(z, Math.Sqrt(x * x + y * y));
            double lng = Math.Atan2(y, x);
            return new Position(ToDegrees(lat), ToDegrees(lng));
        }

        public static Position LinearInterpolator(double fraction, Position from, Position to)
        {
            double lat = (to.Latitude - from.Latitude) * fraction + from.Latitude;
            double lng = (to.Longitude - from.Longitude) * fraction + from.Longitude;
            return new Position(lat, lng);
        }

        public static double GetInterpolation(double input)
        {
            return (float)(Math.Cos((input + 1) * Math.PI) / 2.0f) + 0.5f;
        }

        /** So sánh khoảng cách giữa 2 điểm là 20m */

        public static bool IsBetweenLatlng(double fromLat, double fromLng, double toLat, double toLng)
        {
            if (IsOriginLocation(fromLat, fromLng) || IsOriginLocation(toLat, toLng))
            {
                return true;
            }

            double MIN_DELTA_LATLNG_CAN_MOVE = 0.0002f;
            double deltaLat = Math.Abs(fromLat - toLat);
            double deltaLng = Math.Abs(fromLng - toLng);

            if (deltaLat > MIN_DELTA_LATLNG_CAN_MOVE || deltaLng > MIN_DELTA_LATLNG_CAN_MOVE)
            {
                return false;
            }

            return true;
        }

        public static bool IsBetweenLatlng(double fromLat, double fromLng, double toLat, double toLng, double delta)
        {
            double deltaLat = Math.Abs(fromLat - toLat);
            double deltaLng = Math.Abs(fromLng - toLng);

            if (deltaLat > delta || deltaLng > delta)
            {
                return false;
            }

            return true;
        }

        /** kiểm tra xem tọa độ này có phải là tọa độ (0,0) */

        public static bool IsOriginLocation(double latitude, double longitude)
        {
            if (latitude != 0 && longitude != 0)
            {
                /* vị trí tâm xích đạo */
                return false;
            }
            return true;
        }

        /** Phương thức này decode từ 1 chuỗi overview_polyline từ google trả về thành danh sách các điểm */

        public static List<Position> DecodePoly(string encoded)
        {
            List<Position> poly = new List<Position>();

            int index = 0, len = encoded.Length;
            int lat = 0, lng = 0;

            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded[(index++)] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);

                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;

                do
                {
                    b = encoded[(index++)] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);

                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                poly.Add(new Position(lat / 1E5, lng / 1E5));
            }
            return poly;
        }

        /// <summary>
        /// Encode it
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static string Encode(IEnumerable<Position> points)
        {
            var str = new StringBuilder();

            void encodeDiff(int diff)
            {
                int shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;

                int rem = shifted;

                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));

                    rem >>= 5;
                }

                str.Append((char)(rem + 63));
            }

            int lastLat = 0;
            int lastLng = 0;

            foreach (var point in points)
            {
                int lat = (int)Math.Round(point.Latitude * 1E5);
                int lng = (int)Math.Round(point.Longitude * 1E5);

                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);

                lastLat = lat;
                lastLng = lng;
            }

            return str.ToString();
        }

        /** Tính góc cho marker với 2 điểm */

        public static double BearingBetweenLocations(double lng, double lat, double lngPre, double latPre)
        {
            double PI = 3.14159;
            double latOne = lat * PI / 180;
            double longOne = lng * PI / 180;
            double latTwo = latPre * PI / 180;
            double longTwo = lngPre * PI / 180;
            double dLon = (longTwo - longOne);
            double y = Math.Sin(dLon) * Math.Cos(latTwo);
            double x = Math.Cos(latOne) * Math.Sin(latTwo) - Math.Sin(latOne) * Math.Cos(latTwo) * Math.Cos(dLon);
            double brng = Math.Atan2(y, x);
            brng = ToDegrees(brng);
            brng = (brng + 360) % 360;
            return brng;
        }

        public static string LatitudeToDergeeMinSec(double lat)
        {
            var ns = "N";
            if (lat < 0)
            {
                lat = -lat;
                ns = "S";
            }

            var d = (int)lat;  // Truncate the decimals
            var t1 = (lat - d) * 60;
            var m = (int)t1;
            var s = Math.Round(((t1 - m) * 60), 4);

            return d + "° " + ((m < 10 ? 0 + m : m) + "' ") + ((s < 10 ? 0 + s : s) + "'' " + ns);
        }

        public static double DergeeMinSecToLatitude(int dergee, int min, double sec)
        {
            var d = dergee;
            var m = ((double)min / 60);
            var s = (sec / 3600);
            return dergee + m + s;
        }

        public static string LongitudeToDergeeMinSec(double lng)
        {
            var ew = "E";
            if (lng < 0)
            {
                lng = -lng;
                ew = "W";
            }

            var d = (int)lng;  // Truncate the decimals
            var t1 = (lng - d) * 60;
            var m = (int)t1;
            var s = Math.Round(((t1 - m) * 60), 4);
            return d + "° " + ((m < 10 ? 0 + m : m) + "' ") + ((s < 10 ? 0 + s : s) + "'' " + ew);
        }

        public static double DergeeMinSecToLongitude(int dergee, int min, double sec)
        {
            var d = dergee;
            var m = ((double)min / 60);
            var s = (sec / 3600);
            return dergee + m + s;
        }

        public static Bounds FromPositions(IEnumerable<Position> positions)
        {
            if (positions == null)
            {
                throw new ArgumentNullException(nameof(positions));
            }

            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;
            var isEmpty = true;

            foreach (var p in positions)
            {
                isEmpty = false;
                minX = Math.Min(minX, p.Longitude);
                minY = Math.Min(minY, p.Latitude);
                maxX = Math.Max(maxX, p.Longitude);
                maxY = Math.Max(maxY, p.Latitude);
            }

            if (isEmpty)
            {
                throw new ArgumentException(@"{nameof(positions)} is empty");
            }

            return new Bounds(new Position(minY, minX), new Position(maxY, maxX));
        }
    }
}