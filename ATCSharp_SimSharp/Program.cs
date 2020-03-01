using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using SimSharp;

namespace ATCSharp_SimSharp {
    public class Program {
        public const int NUM_GATES = 5;
        public static Part Runway = new Part("Runway");
        public static Part North = new Part("North Off-Ramp");
        public static Part South = new Part("South Off-Ramp");
        public static List<Part> Taxiways = new List<Part>();
        public static List<Part> Gates = new List<Part>();
        public static readonly TimeSpan SimTime = TimeSpan.FromHours(10);
        //public static List<Part> ALL;
        static void Main(string[] args) {

            for (int i = 0; i < NUM_GATES; i++) {
                Taxiways.Add(new Part("TS" + i));
                Gates.Add(new Part("Gate " + i));
            }

            Simulate(43);
        }

        public static void Simulate(int rseed) {
            // Setup and start the simulation
            // Create an environment and start the setup process
            var start = new DateTime(2014, 2, 1);
            var env = new ThreadSafeSimulation(start, rseed);
            env.Log("== Airport ==");
            IEnumerable<Plane> planes = new List<Plane>();
            using (var reader = new StreamReader(@"C:\Users\cheez\Google Drive\10th Grade\Science Fair\ATCSharp\ATCSharp_SimSharp\planes.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {
                planes = csv.GetRecords<Plane>();
            }

            // Execute!
            env.Run(SimTime);

            // Analyis/results
            env.Log("results after {0} hours.", (env.Now - start).TotalHours);
            //foreach (var plane in planes)
            //    env.Log("{0} made {1} parts.", plane.ID, machine.PartsMade);
        }
        
    }

    public class Part {

        public Part(String name) {
            this.name = name;
            Occupied = false;
        }
        public String name { get; set; }
        public bool Occupied { get; set; }
        //public bool Future { get; set; }
        public override string ToString() {
            return name + "  " + Occupied;
        }
    }

    public enum Direction {
        NORTH,
        SOUTH
    }

    class Plane : ActiveObject<Simulation> {

        public string ID { get; set; }
        public Process Process { get; private set; }
        public readonly Direction dirLAND;
        public readonly Direction dirTAKEOFF;
        public int GateIndex { get; private set; } //STARTS AT 0
        public bool leave { get; private set; }

        private int currIndex = -1;
        public TimeSpan spawn { get; set; }
        private List<Part> parts = new List<Part>();
        public Simulation simulation { get; set; }
        private bool left = false;

        public Plane(ThreadSafeSimulation env, string ID, int gate, TimeSpan spawn) : base(env) {
            this.ID = ID;
            this.spawn = spawn;
            GateIndex = gate;
            simulation = env;
            leave = false;
            if (GateIndex < Program.NUM_GATES / 2) {
                dirLAND = Direction.NORTH;
                dirTAKEOFF = Direction.SOUTH;
            } else {
                dirLAND = Direction.SOUTH;
                dirTAKEOFF = Direction.NORTH;
            }
            makePartsList();

            // Start "working" and "break_machine" processes for this machine.
            Process = env.Process(Moving());
            //env.Process(BreakMachine());
        }

        private IEnumerable<Event> Moving() {
            while (true) {
                //simulation.Log(ID + " is running");
                // change part to be on
                if (spawn.TotalMinutes > simulation.NowD) {
                    yield return Environment.Timeout(TimeSpan.FromMinutes(spawn.TotalMinutes - simulation.NowD));
                }
                int oldIndex = currIndex;
                if (!left) {
                    if (leave && currIndex + 1 == parts.Count) {
                        simulation.Log(ID + " is taking off");
                        yield return Environment.Timeout(TimeSpan.FromMinutes(new Random().Next(2, 5)));
                        parts[currIndex].Occupied = false;
                        simulation.Log(ID + " has left at " + simulation.Now);
                        left = true;
                        yield return Environment.TimeoutD(Program.SimTime.Hours - Environment.Now.Hour);
                    } else if (ChangePart()) { //if part is at GATE
                        yield return Environment.Timeout(TimeSpan.FromMinutes(new Random().Next(15, 20)));
                        leave = true;
                        makePartsList();
                        currIndex = 0;
                    } else if (currIndex != oldIndex) {
                        //simulation.Log(this.ID + " is on " + parts[currIndex].name + " at " + simulation.Now + " \n");
                    }

                    // moving
                    yield return Environment.Timeout(TimeSpan.FromMinutes(new Random().Next(1, 3)));
                } else { break; }

                if (ID == "ID 2") {
                    ID = ID;
                }
            }
        }

        private bool ChangePart() {
            if (currIndex > 0 && currIndex + 1 == parts.Count) { //end of the current list
                if (!leave) { //if at gate
                    Program.Gates[GateIndex].Occupied = true;
                    return true;
                }
                parts[currIndex - 1].Occupied = false;
                currIndex = 0;
            } else {
                currIndex++;
                if (checkMovement()) {
                    //move to the next one, release from the last one
                    parts[currIndex].Occupied = true;
                    if (currIndex > 0) {
                        parts[currIndex - 1].Occupied = false;
                    }
                } else {
                    currIndex--;
                }
            }
            return false;
        }


        private bool checkMovement() { //TODO: FOR FCFS ONLY -- FOR GREEDY USE FUTURE TRACKING
            for (int i = currIndex; i < parts.Count; i++) {
                if (parts[i].Occupied == true) {
                    return false;
                }
            }
            return true;
        }

        public void makePartsList() {
            parts = new List<Part>();
            ///these loops create the parts List depending on the location & target of the Plane
            if (leave) {
                if (dirTAKEOFF == Direction.NORTH) {
                    for (int i = GateIndex; i < Program.NUM_GATES; i++) {
                        parts.Add(Program.Taxiways[i]);
                    }
                    parts.Add(Program.South);
                    parts.Add(Program.Runway);
                } else {
                    for (int i = GateIndex + 1; i >= 0; i--) {
                        parts.Add(Program.Taxiways[i]);
                    }
                    parts.Add(Program.North);
                    parts.Add(Program.Runway);
                }
            } else
                if (dirLAND == Direction.NORTH) {
                parts.Add(Program.Runway);
                parts.Add(Program.South);
                for (int i = Program.NUM_GATES - 1; i >= GateIndex; i--) {
                    parts.Add(Program.Taxiways[i]);
                }
                parts.Add(Program.Gates[GateIndex]);
            } else {
                parts.Add(Program.Runway);
                parts.Add(Program.South);
                for (int i = 0; i < GateIndex; i++) {
                    parts.Add(Program.Taxiways[i]);
                }
                parts.Add(Program.Gates[GateIndex]);
            }
        }
    }
}
