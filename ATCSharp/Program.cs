using System;
using System.Collections.Generic;
using NSimulate;

namespace ATCSharp
{
    class Program
    {
        public const int NUM_GATES = 5;
        static void Main(string[] args)
        {
            using (var context = new SimulationContext(isDefaultContextForProcess: true))
            {
                // instantate a new simulator
                var simulator = new Simulator();

                context.Register<Runway>(new Runway() { Capacity = 1 });

                Offramp north = new Offramp(0) { Capacity = 1 };
                context.Register<Offramp>(north);
                Offramp south = new Offramp(1) { Capacity = 1 };
                context.Register<Offramp>(south);
                Offramp[] offramps = new Offramp[] { north, south };

                List<TaxiSection> taxiways = new List<TaxiSection>();
                for(int i = 0; i < NUM_GATES; i++) //north to south
                {
                    TaxiSection temp = new TaxiSection(index: i) { Capacity = 1 };
                    context.Register<TaxiSection>(temp);
                    taxiways.Add(temp);
                }

                IEnumerable<Plane> planes = new List<Plane>();
                

                // the simulation will run until:
                //     -there are no processes with work remaining
                //     -a terminate instruction has been issued; 
                //     -or and SimuationEndTrigger has fired
                simulator.Simulate();
                OutputResults(planes);
            }
        }


        static void OutputResults(IEnumerable<Plane> planes)
        {

        }
    }
}
