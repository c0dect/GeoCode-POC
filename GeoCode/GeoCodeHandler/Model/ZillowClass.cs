using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCodeTest.Zillow
{
    public class Properties
{
    public string STATE { get; set; }
    public string COUNTY { get; set; }
    public string CITY { get; set; }
    public string NAME { get; set; }
    public double REGIONID { get; set; }
}

public class Geometry
{
    public string type { get; set; }    
    public List<List<List<double>>> coordinates { get; set; }
}

public class ZillowCLass
{
    public string type { get; set; }
    public Properties properties { get; set; }
    public Geometry geometry { get; set; }
}
}
