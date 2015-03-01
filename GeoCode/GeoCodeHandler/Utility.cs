using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCodeHandler
{
    public static class Utility
    {
        public static string GetJSONString(List<LocationCoordinates> locationCoordinates)
        {
            var geoJsonString = new StringBuilder().Append("[");

            foreach (var coordinate in locationCoordinates)
            {
                geoJsonString.Append("[").Append(coordinate.Lat + ", " + coordinate.Lon).Append("], ");
            }

            return geoJsonString.ToString().Trim().TrimEnd(',') + "]";
        }
    }
}
