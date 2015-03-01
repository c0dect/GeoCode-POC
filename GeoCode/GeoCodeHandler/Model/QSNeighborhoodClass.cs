using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCodeTest
{
    public class Properties
    {
        public int woe_id { get; set; }
        public string name { get; set; }
        public string name_en { get; set; }
        public string name_adm0 { get; set; }
        public string name_adm1 { get; set; }
        public string name_adm2 { get; set; }
        public string name_lau { get; set; }
        public string name_local { get; set; }
        public double woe_adm0 { get; set; }
        public double woe_adm1 { get; set; }
        public double woe_adm2 { get; set; }
        public double woe_lau { get; set; }
        public double woe_local { get; set; }
        public string woe_ver { get; set; }
        public string placetype { get; set; }
        public double gn_id { get; set; }
        public string gn_name { get; set; }
        public string gn_fcode { get; set; }
        public string gn_adm0_cc { get; set; }
        public string gn_namadm1 { get; set; }
        public double gn_local { get; set; }
        public string gn_nam_loc { get; set; }
        public string woe_funk { get; set; }
        public int quad_count { get; set; }
        public int photo_sum { get; set; }
        public int photo_max { get; set; }
        public int localhoods { get; set; }
        public int local_sum { get; set; }
        public int local_max { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
    }

    public class QSNeighborhoodClass
    {
        public string type { get; set; }
        public Properties properties { get; set; }
        public Geometry geometry { get; set; }
    }
}
