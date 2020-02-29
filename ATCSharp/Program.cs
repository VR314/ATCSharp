using System;
using System.Collections.Generic;
using NSimulate;

namespace ATCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new SimulationContext(isDefaultContextForProcess: true))
            {
                // instantiate a new simulator
                var simulator = new Simulator();

                context.Register<Runway>(Globals.runway);
                context.Register<Offramp>(Globals.north);
                context.Register<Offramp>(Globals.south);

                for(int i = 0; i < Globals.NUM_GATES; i++) //north to south
                {
                    Globals.taxiway.sections.Add(new TaxiSection(index: i) { Capacity = 1 });
                }
                context.Register<Taxiway>(Globals.taxiway);


                IEnumerable<Plane> planes = GeneratePlanes();
                foreach(Plane p in planes)
                {
                    context.Register<Plane>(p);
                }

                // the simulation will run until:
                //     -there are no processes with work remaining
                //     -a terminate instruction has been issued; 
                //     -or and SimuationEndTrigger has fired
                simulator.Simulate();
                OutputResults(planes);
            }
        }

        static IEnumerable<Plane> GeneratePlanes()
        {
            return new List<Plane>();
        }


        static void OutputResults(IEnumerable<Plane> planes)
        {

        }
    }

    public static class Globals
    {
        public const int NUM_GATES = 5;
        public static Runway runway = new Runway() { Capacity = 1 };
        public static Taxiway taxiway = new Taxiway(null);
        public static Offramp north = new Offramp(0) { Capacity = 1 };
        public static Offramp south = new Offramp(1) { Capacity = 1 };
        public static Offramp[] offramps = new Offramp[] { north, south };

        public enum Direction
        {
            NORTH,
            SOUTH
        }
    }
}
