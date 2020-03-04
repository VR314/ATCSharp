using System;
using System.Collections.Generic;
using System.IO;
using SimSharp;

namespace ATCSharp_SimSharp {
    public class Program {
        public const int NUM_GATES = 7;
        public static readonly TimeSpan SimTime = TimeSpan.FromHours(10);
        public static List<Part> Gates = new List<Part>();
        public static Part North = new Part("North Off-Ramp");
        public static Part Runway = new Part("Runway");
        public static Part South = new Part("South Off-Ramp");
        public static List<Part> Taxiways = new List<Part>();

        public static void Simulate(int rseed) {
            // Setup and start the simulation
            // Create an environment and start the setup process
            var start = new DateTime(2014, 2, 1);
            var env = new ThreadSafeSimulation(start, rseed);
            env.Log("== Airport ==");
            List<Plane> planes = new List<Plane>();
            using (var reader = new StreamReader(@"C:\Users\cheez\Google Drive\10th Grade\Science Fair\ATCSharp\ATCSharp_SimSharp\planes.csv")) {
                reader.ReadLine(); //takes out first line
                while (!reader.EndOfStream) {
                    string[] props = reader.ReadLine().Split(',');
                    planes.Add(new Plane(env, props[1], Convert.ToInt32(props[2]), TimeSpan.FromMinutes(Convert.ToDouble(props[3])), Plane.Algorithm.FCFS));
                }
            }

            // Execute!
            env.Run(SimTime);

            // Analyis/results
            env.Log("results after {0} hours.", (env.Now - start).TotalHours);
        }
        public static void Main(string[] args) {
            for (int i = 0; i < NUM_GATES; i++) {
                Taxiways.Add(new Part("TS" + i));
                Gates.Add(new Part("Gate " + i));
            }

            Simulate(43);
        }
    }
}