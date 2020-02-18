using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp
{
    public class PlaneStatistics
    {
        public PlaneStatistics() { }

        //DATA NEEDED TO TRACK

        public long? Spawn { get; set; }

        public long? LandClearance { get; set; }

        public long? DoneRunway { get; set; }

        public long? GateClearance { get; set; }

        public long? GateDischarge { get; set; }
    }
}
