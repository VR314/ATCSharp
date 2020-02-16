using System;
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

                // the simulation will run until:
                //     -there are no processes with work remaining
                //     -a terminate instruction has been issued; 
                //     -or and SimuationEndTrigger has fired
                simulator.Simulate();
            }
        }
    }
}
