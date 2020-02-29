namespace ATCSharp
{
    public class PlaneCharacteristics
    {

        public PlaneCharacteristics() {  }

        //INSTANCE VARIABLES OF A PLANE
        public long? Spawn { get; set; }

        public int GateIndex { get; set; }

        public int CurrentIndex { get; set; }

        public long? RunwayDuration { get; set; }

        public long? GateDuration { get; set; }

        public long? TaxiDuration { get; set; } //CALCULATE NOT JUST GIVEN
        /// <summary>
        /// TO WHICH DIRECTION
        /// </summary>
        public Globals.Direction TakeoffDirection { get; set; }

        /// <summary>
        /// FROM WHICH DIRECTION
        /// </summary>
        public Globals.Direction LandDirection { get; set; }

    }
}
