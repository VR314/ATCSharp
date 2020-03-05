using System;
using System.Collections.Generic;
using System.IO;
using SimSharp;

namespace ATCSharp_SimSharp {
    public class Program {
        private const Plane.Algorithm algorithm = Plane.Algorithm.FWCHECK;
        public const int NUM_GATES = 7;
        public static readonly TimeSpan SimTime = TimeSpan.FromHours(10);
        public static List<Part> Gates = new List<Part>();
        public static Part North = new Part("North Off-Ramp");
        public static Part Runway = new Part("Runway");
        public static Part South = new Part("South Off-Ramp");
        public static List<Part> Taxiways = new List<Part>();
        private static List<Plane> planes = new List<Plane>();

        public static void Simulate(int rseed) {
            var start = new DateTime(2000, 1, 1);
            var env = new ThreadSafeSimulation(start, rseed);
            env.Log("== Airport ==");
            generatePlanes(25);
            using (var reader = new StreamReader(@"C:\Users\cheez\Google Drive\10th Grade\Science Fair\ATCSharp\ATCSharp_SimSharp\planes.csv")) {
                reader.ReadLine(); //takes out first line
                while (!reader.EndOfStream) {
                    string[] props = reader.ReadLine().Split(',');
                    planes.Add(new Plane(env, ID: props[1], gate: Convert.ToInt32(props[2]), spawn: TimeSpan.FromMinutes(Convert.ToDouble(props[3])), algorithm));
                }
            }

            // Execute!
            env.Run(SimTime);

            // Analyis/results
            writeResults();
        }
        public static void Main(string[] args) {
            for (int i = 0; i < NUM_GATES; i++) {
                Taxiways.Add(new Part("TS" + i));
                Gates.Add(new Part("Gate " + i));
            }

            Simulate(43);
        }

        static void generatePlanes(int number) {
            using (var writer = new StreamWriter(@"C:\Users\cheez\Google Drive\10th Grade\Science Fair\ATCSharp\ATCSharp_SimSharp\planes.csv")) {
                writer.WriteLine("NUMBER,ID,GATE - 1 INDEX,SPAWNTIME(MIN)"); //header
                for(int i = 0; i < number; i++) {
                    String s = "";
                    s += i.ToString();
                    s += ",";
                    s += (char)(65 + i) + i.ToString();
                    s += ",";
                    s += new Random().Next(1, NUM_GATES).ToString();
                    s += ",";
                    s += new Random().Next(1, number * 20).ToString();
                    writer.WriteLine(s);
                }
            }
        }

        static void writeResults() { //TODO
            foreach (Plane p in planes) {

            }
        }
    }
}