using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp
{
    public class PlaneCharacteristics
    {

        public PlaneCharacteristics() {  }

        //INSTANCE VARIABLES OF A PLANE
        public long? Spawn { get; set; }

        public long? RunwayDuration { get; set; }

        public long? GateDuration { get; set; }

        public long? TaxiDuration { get; set; } //CALCULATE NOT JUST GIVEN

    }
}
