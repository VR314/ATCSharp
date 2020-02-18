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
                // instantate a new simulator
                var simulator = new Simulator();

                context.Register<Runway>(new Runway() { Capacity = 1 });
                context.Register<Gates>(new Gates() { Capacity = 6 });

                IEnumerable<Plane> planes = GeneratePlanes(context);

                // the simulation will run until:
                //     -there are no processes with work remaining
                //     -a terminate instruction has been issued; 
                //     -or and SimuationEndTrigger has fired
                simulator.Simulate();
                OutputResults(planes);
            }
        }

        static IEnumerable<Plane> GeneratePlanes(SimulationContext context) //MAKE PLANES
        {
            Random r = new Random();
            List<Plane> planes = new List<Plane>();






            return planes;
        }


        static void OutputResults(IEnumerable<Plane> planes)
        {

        }
    }

    public class Taxiway
    {
        public bool Full { get; set; }
    }
}
