using ClipperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCodeHandler
{
    public class PolygonCreator
    {
        public List<LocationCoordinates> GetEstimatedPolygon(List<LocationCoordinates> polygonOne, List<LocationCoordinates> polygonTwo, string locationName)
        {
            var polygonOperations = new PolygonOperations();

            var polygonIntersection = polygonOperations.GetPolygonIntersection(polygonOne, polygonTwo);
            System.Diagnostics.Debug.WriteLine(Utility.GetJSONString(polygonIntersection[0]));
            if (polygonIntersection == null || !polygonIntersection.Any())
                return new List<LocationCoordinates>();

            //var polyOneDiff = polygonOne;
            //var polyTwoDiff = polygonTwo;
            //foreach(var polygon in polygonIntersection)
            //{
            //    polyOneDiff = polygonOperations.GetDifferencePolygon(polyOneDiff, polygon);
            //    polyTwoDiff = polygonOperations.GetDifferencePolygon(polyTwoDiff, polygon);
            //}
            var polyOneDiff = polygonOperations.GetDifferencePolygon(polygonOne, polygonIntersection[0]);
            System.Diagnostics.Debug.WriteLine(Utility.GetJSONString(polyOneDiff));

            var polyTwoDiff = polygonOperations.GetDifferencePolygon(polygonTwo, polygonIntersection[0]);
            System.Diagnostics.Debug.WriteLine(Utility.GetJSONString(polyTwoDiff));

            var polyOneAccuracy = GetPolygonAccuracy(polyOneDiff, locationName);
            var polyTwoAccuracy = GetPolygonAccuracy(polyTwoDiff, locationName);

            if (polyOneAccuracy > polyTwoAccuracy)
                return polygonOne;
            return polygonTwo;
        }

        private double GetPolygonAccuracy(List<LocationCoordinates> polygon, string locationName)
        {
            double scale = 1024.0;
            double minValue = Int64.MinValue / scale;
            double maxValue = Int64.MaxValue / scale;

            var successPoints = 0;
            var totalPoints = 0;

            var minLonValue = polygon.Min(latLong => latLong.Lon);
            var maxLonValue = polygon.Max(latLong => latLong.Lon);
            var minLatValue = PolygonOperations.to_long(Convert.ToDouble(polygon.Min(latLong => latLong.Lat)), minValue, maxValue, scale);
            var maxLatValue = PolygonOperations.to_long(Convert.ToDouble(polygon.Max(latLong => latLong.Lat)), minValue, maxValue, scale);
            //var maxLatValue = polygon.Max(latLong => latLong.Lat);
            var latValue = maxLatValue;

            for (var i = Math.Abs(minLatValue); i < Math.Abs(maxLatValue); i += Constants.StepSize)
            {
                var rectangle = CreateRectangle(latValue, minLonValue, maxLonValue, scale);
                System.Diagnostics.Debug.WriteLine(Utility.GetJSONString(rectangle));

                var polygonIntersections = new PolygonOperations().GetPolygonIntersection(polygon, rectangle);

                foreach (var polygonIntersection in polygonIntersections)
                {
                    System.Diagnostics.Debug.WriteLine(Utility.GetJSONString(polygonIntersection));

                    if (CheckIfRectangleIsValid(polygonIntersection, locationName))
                        successPoints++;
                    totalPoints++;
                }
                latValue += Constants.StepSize;
            }

            return ((double)successPoints/(double)totalPoints);
        }

        private bool CheckIfRectangleIsValid(List<LocationCoordinates> polygon, string locationName)
        {
            var minLonValue = polygon.Min(latLong => latLong.Lon);
            var maxLonValue = polygon.Max(latLong => latLong.Lon);
            var avgLonValue = (Convert.ToDouble(minLonValue) + Convert.ToDouble(maxLonValue)) / 2;
            var reverseGeocodedGoogleResponse = new GoogleAPIClient().FetchGoogleReverseGeocodedResponse(new LocationCoordinates { Lat = polygon[0].Lat, Lon = avgLonValue.ToString() });

            if (reverseGeocodedGoogleResponse.Contains(locationName))
                return true;

            return false;
        }

        private List<LocationCoordinates> CreateRectangle(long latValue, string minLonValue, string maxLonValue, double scale)
        {
            var minLatValue = Convert.ToDouble(latValue / scale).ToString();
            var maxLatValue = Convert.ToDouble((latValue + Constants.Width) / scale).ToString();

            return new List<LocationCoordinates>
            {
                new LocationCoordinates{ Lat = minLatValue, Lon = minLonValue},
                new LocationCoordinates{ Lat = maxLatValue, Lon = minLonValue},
                new LocationCoordinates{ Lat = maxLatValue, Lon = maxLonValue},
                new LocationCoordinates{ Lat = minLatValue, Lon = maxLonValue},
                new LocationCoordinates{ Lat = minLatValue, Lon = minLonValue},
            };
        }
    }
}
