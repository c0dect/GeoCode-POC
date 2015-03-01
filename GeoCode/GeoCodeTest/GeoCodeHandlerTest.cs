using GeoCodeHandler;
using GeoCodeTest.Zillow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace GeoCodeTest
{
    [TestClass]
    public class GeoCodeHandlerTest
    {
        [TestMethod]
        public void TestGeoCodeHandler()
        {
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

            var solution = new PolygonCreator().GetEstimatedPolygon(polyOne, polyTwo, "Marrion");
        }
    }
}
