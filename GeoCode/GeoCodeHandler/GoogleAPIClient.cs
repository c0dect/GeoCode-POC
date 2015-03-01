using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeoCodeHandler
{
    public class GoogleAPIClient
    {
        public string FetchGoogleReverseGeocodedResponse(LocationCoordinates locationCoordinates)
        {
            var googleAPIUrl = "https://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&key=" + Constants.APIKey;

            var finalUrl = string.Format(googleAPIUrl, locationCoordinates.Lon, locationCoordinates.Lat);

            HttpWebRequest request = WebRequest.Create(finalUrl) as HttpWebRequest;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                return reader.ReadToEnd();
            }
        }
    }
}
