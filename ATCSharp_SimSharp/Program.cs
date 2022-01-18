using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimSharp;

namespace ATCSharp_SimSharp {

    public class Program {
        private const Plane.Algorithm algorithm = Plane.Algorithm.FCFS;
        public static int NUM_GATES;
        public static int NUM_PLANES;
        public static double NUM_TIMES;
        public static readonly TimeSpan SimTime = TimeSpan.FromHours(10);
        public static List<Part> Gates = new List<Part>();
        public static Part North = new Part("North Off-Ramp");
        public static Part Runway = new Part("Runway");
        public static Part South = new Part("South Off-Ramp");
        public static List<Part> Taxiways = new List<Part>();
        private static List<Plane> planes = new List<Plane>();

        public static double Simulate(int rseed, int iteration) {
            Gates = new List<Part>();
            North = new Part("North Off-Ramp");
            Runway = new Part("Runway");
            South = new Part("South Off-Ramp");
            Taxiways = new List<Part>();
            planes = new List<Plane>();


            DateTime start = new DateTime(2000, 1, 1);
            ThreadSafeSimulation env = new ThreadSafeSimulation(start, rseed);
            Console.WriteLine("\n\n== Airport ==");
            using (StreamReader reader = new StreamReader(Path.Combine(System.Environment.CurrentDirectory, @"planes1.csv"))) {
                reader.ReadLine(); //takes out first line
                while (!reader.EndOfStream) {
                    string[] props = reader.ReadLine().Split(',');
                    planes.Add(new Plane(env, ID: props[1], gate: Convert.ToInt32(props[2]), spawn: TimeSpan.FromMinutes(Convert.ToDouble(props[3])), algorithm));
                }
            }

            for (int i = 0; i <= planes.Max(p => p.GateIndex); i++) {
                Taxiways.Add(new Part("TS" + i));
                Gates.Add(new Part("Gate " + i));
            }

            // Execute!
            env.Run(SimTime);

            return planes.Max(p => p.times[3]);
        }

        public static void Main(string[] args) {
            Random r = new Random();
            for (int j = 0; j < 1; j++) {
                double avg = 0;
                avg += Simulate(j, iteration: j);

                // Analyis/results
                // writeResults(avg);
                Console.WriteLine(avg);
            }
        }

        /// <summary>
        /// Generates a set of Planes with random attributes and writes them to the "planes.csv" file
        /// </summary>
        private static void generatePlanes(int number) {
            for (int i = 0; i < number; i++) {
                NUM_PLANES = new Random(i).Next(1, 25);
                NUM_GATES = new Random(i).Next(1, 10);
                using (var writer = new StreamWriter(System.IO.File.Create(@"scenarios\" + i + ".csv"))) {
                    writer.WriteLine("NUMBER,ID,GATE - 1 INDEX,SPAWNTIME(MIN)"); //header
                    for (int j = 0; j < NUM_PLANES; j++) {
                        String s = "";
                        s += j.ToString();
                        s += ",";
                        s += (char)(65 + (j % 26)) + j.ToString();
                        s += ",";
                        s += new Random(j).Next(1, NUM_GATES).ToString();
                        s += ",";
                        s += new Random(j).Next(1, number * 5).ToString();
                        writer.WriteLine(s);
                    }
                }
            }
        }

        private static void writeResults(double avg) { //TODO
            using (var writer = new StreamWriter(@"C:\Users\cheez\Google Drive\10th Grade\Science Fair\ATCSharp\ATCSharp_SimSharp\resultsFCFS1.csv", true)) {
                writer.WriteLine(algorithm + "," + NUM_GATES + "," + NUM_PLANES + "," + avg);
            }
        }
    }
}