using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using System.Threading.Tasks;

namespace GeoCodeHandler
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public class PolygonOperations
    {
        private static double _scale = 1024.0;
        public List<List<LocationCoordinates>> GetPolygonIntersection(List<LocationCoordinates> polygonOne, List<LocationCoordinates> polygonTwo)
        {
            var locationCoordinates = new List<List<LocationCoordinates>>();

            Paths polygonOnePath = GetClipperPolygonPath(polygonOne);
            Paths polygonTwoPath = GetClipperPolygonPath(polygonTwo);

            locationCoordinates = GetPolygonResult(polygonOnePath, polygonTwoPath, ClipType.ctIntersection);
            return locationCoordinates;
        }

        public List<List<LocationCoordinates>> GetPolygonResult(Paths subjectPolygon, Paths clipperPolygon, ClipType clipType)
        {
            var locationCoordinates = new List<List<LocationCoordinates>>();

            var clipper = new Clipper();
            clipper.AddPaths(subjectPolygon, PolyType.ptSubject, true);
            clipper.AddPaths(clipperPolygon, PolyType.ptClip, true);
            Paths solution = new Paths();
            var ans = clipper.Execute(clipType, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            foreach(var polygon in solution)
            {
                var coordinates = new List<LocationCoordinates>();
                for (int i = 0; i < polygon.Count; i++)
                    coordinates.Add(new LocationCoordinates { Lat = Convert.ToDouble(solution[0][i].X / _scale).ToString(), Lon = Convert.ToDouble(solution[0][i].Y / _scale).ToString() });
                locationCoordinates.Add(coordinates);
            }
            
            return locationCoordinates;
        }

        public List<LocationCoordinates> GetDifferencePolygon(List<LocationCoordinates> subjectPolygon, List<LocationCoordinates> clipperPolygon)
        {
            var locationCoordinates = new List<List<LocationCoordinates>>();

            Paths subjectPaths = GetClipperPolygonPath(subjectPolygon);
            Paths clipperPaths = GetClipperPolygonPath(clipperPolygon);

            locationCoordinates = GetPolygonResult(subjectPaths, clipperPaths, ClipType.ctDifference);

            if (!locationCoordinates.Any())
                return null;

            return locationCoordinates.First();
        }

        private Paths GetClipperPolygonPath(List<LocationCoordinates> polygon)
        {
            
            double minValue = Int64.MinValue / _scale;
            double maxValue = Int64.MaxValue / _scale;

            Paths polygonPath = new Paths(1) { new Path(4) };
            foreach (var item in polygon)
                polygonPath[0].Add(new IntPoint(to_long(Convert.ToDouble(item.Lat), minValue, maxValue, _scale), to_long(Convert.ToDouble(item.Lon), minValue, maxValue, _scale)));

            return polygonPath;
        }

        internal static long to_long(double v, double minValue, double maxValue, double scale)
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
