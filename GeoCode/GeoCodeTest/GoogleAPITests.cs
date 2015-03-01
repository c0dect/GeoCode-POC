using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using GeoCodeTest.Zillow;
using ClipperLib;

namespace GeoCodeTest
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using GeoCodeHandler;


    [TestClass]
    public class GoogleAPITests
    {

        [TestMethod]
        public async Task TestReverseGeocodingAPI()
        {
            var googleAPIUrl = "https://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&key=AIzaSyCSfoAREd1e6a-WLOoYFHiV5W3majI1q7I";
            var latLong = new LocationCoordinates { Lat = "45.634421", Lon = "-122.567960" };
            
            var finalUrl = string.Format(googleAPIUrl, latLong.Lat, latLong.Lon);



            HttpWebRequest request = WebRequest.Create(finalUrl) as HttpWebRequest;

            // Get response
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                var temp = reader.ReadToEnd();
                // Console application output
                //Console.WriteLine(reader.ReadToEnd());
            }


            var webRequest = WebRequest.Create(finalUrl);
            //var result = webRequest.GetResponse();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(finalUrl))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                var result = await content.ReadAsStringAsync();

            }
        } 

        [TestMethod]
        public void TestClipperLibrary()
        {

            double scale = 1024.0;
            double minValue = Int64.MinValue / scale;
            double maxValue = Int64.MaxValue / scale;

            var polyOne = new List<LocationCoordinates>();
            
            var polyTwo = new List<LocationCoordinates>();
            
            var qsPoly = new DataContractJsonSerializer(typeof(QSNeighborhoodClass));
            var zillowPoly = new DataContractJsonSerializer(typeof(ZillowCLass));

            var qsContent = (QSNeighborhoodClass)qsPoly.ReadObject(File.OpenRead(@"D:\CodeBase\ClarifiGeoCode\QSJson.json"));
            var zillowContent = (QSNeighborhoodClass)qsPoly.ReadObject(File.OpenRead(@"D:\CodeBase\ClarifiGeoCode\ZillowJson.json"));
            
            foreach (var geoPair in qsContent.geometry.coordinates[0])
            {
                polyOne.Add(new LocationCoordinates { Lat = geoPair[0].ToString(), Lon = geoPair[1].ToString() });
            }

            foreach (var geoPair in zillowContent.geometry.coordinates[0])
            {
                polyTwo.Add(new LocationCoordinates { Lat = geoPair[0].ToString(), Lon = geoPair[1].ToString() });
            }

            // var qsPoints = new List<List<IntPoint>>() { new List<IntPoint>()};
            Paths qsPoints = new Paths(1) { new Path(4) };
            foreach (var item in polyOne)
            {
                qsPoints[0].Add(new IntPoint(to_long(Convert.ToDouble(item.Lat), minValue, maxValue, scale), to_long(Convert.ToDouble(item.Lon), minValue, maxValue, scale)));
            }

            //var zillowPoints = new List<List<IntPoint>>() { new List<IntPoint>() };
            Paths zillowPoints = new Paths(1) { new Path(4) };
            foreach (var item in polyTwo)
            {
                zillowPoints[0].Add(new IntPoint(to_long(Convert.ToDouble(item.Lat), minValue, maxValue, scale), to_long(Convert.ToDouble(item.Lon), minValue, maxValue, scale)));
            }

            var clipper = new Clipper();
            clipper.AddPaths(qsPoints, PolyType.ptSubject, true);
            clipper.AddPaths(zillowPoints, PolyType.ptClip, true);
            Paths solution = new Paths();
            var ans = clipper.Execute(ClipType.ctXor, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            var geoJsonString = "[";

            for (int i = 0; i < solution[0].Count; i++)
            {
                geoJsonString += "[" + Convert.ToDouble(solution[0][i].X / scale).ToString() + ", " + Convert.ToDouble(solution[0][i].Y / scale).ToString() + "], ";
            }

            geoJsonString = geoJsonString.Trim().TrimEnd(',') + "]";

            File.AppendAllText(@"D:\CodeBase\ClarifiGeoCode\IntersectPoly.txt", geoJsonString);
            var test = solution;

        }

        [TestMethod]
        public void TestLineSegmentPolygonIntersectionTest()
        {
            double scale = 1024.0;
            double minValue = Int64.MinValue / scale;
            double maxValue = Int64.MaxValue / scale;

            var polyOne = new List<LocationCoordinates>();

            var polyTwo = new List<LocationCoordinates>();

            Paths qsPoints = new Paths(1) { new Path(4) };
            qsPoints[0].Add(new IntPoint { X = 0, Y = 0 });
            qsPoints[0].Add(new IntPoint { X = 0, Y = 50 });
            qsPoints[0].Add(new IntPoint { X = 50, Y = 50 });
            qsPoints[0].Add(new IntPoint { X = 50, Y = 100 });
            qsPoints[0].Add(new IntPoint { X = 0, Y = 100 });
            qsPoints[0].Add(new IntPoint { X = 0, Y = 150 });
            qsPoints[0].Add(new IntPoint { X = 100, Y = 150 });
            qsPoints[0].Add(new IntPoint { X = 100, Y = 0 });
            qsPoints[0].Add(new IntPoint { X = 0, Y = 0 });

            
            Paths zillowPoints = new Paths(1) { new Path(4) };
            zillowPoints[0].Add(new IntPoint { X = 20, Y = -10 });
            zillowPoints[0].Add(new IntPoint { X = 20, Y = 160 });
            zillowPoints[0].Add(new IntPoint { X = 30, Y = 160 });
            zillowPoints[0].Add(new IntPoint { X = 30, Y = -10 });
            zillowPoints[0].Add(new IntPoint { X = 20, Y = -10 });

            //var clipper = new Clipper();
            //clipper.AddPaths(qsPoints, PolyType.ptSubject, true);
            //clipper.AddPaths(zillowPoints, PolyType.ptClip, true);

            //Paths solution = new Paths();
            //var ans = clipper.Execute(ClipType.ctIntersection, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            var result = new PolygonOperations().GetPolygonResult(qsPoints, zillowPoints, ClipType.ctIntersection);
            Assert.IsNotNull(result);
        }

        private long to_long(double v, double minValue, double maxValue, double scale)
        {
            if (v < 0)
            {
                if (v >= minValue)
                    return (long)(v * scale - 0.5);
            }

            return (long)(v * scale + 0.5);
        }
    }
}
