using SimSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ATCSharp_SimSharp {
    public class Program {
        public const int NUM_GATES = 5;
        public static Part Runway = new Part("Runway");
        public static Part North = new Part("North Off-Ramp");
        public static Part South = new Part("South Off-Ramp");
        public static List<Part> Taxiways = new List<Part>();
        public static List<Part> Gates = new List<Part>();
        public static readonly TimeSpan SimTime = TimeSpan.FromHours(2);
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
            var env = new Simulation(start, rseed);
            env.Log("== Airport ==");
            var planes = Enumerable.Range(0, 2).Select(x => new Plane(env, "ID " + x, x % 5)).ToArray();

            // Execute!
            env.Run(SimTime);

            // Analyis/results
            env.Log("Machine shop results after {0} days.", (env.Now - start).TotalDays);
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
        public Direction dirLAND { get; private set; }
        public Direction dirTAKEOFF { get; private set; }
        public int GateIndex { get; private set; } //STARTS AT 0
        private int currIndex = -1;
        public bool leave { get; private set; }
        private List<Part> parts = new List<Part>();
        private Simulation simulation;

        public Plane(Simulation env, string ID, int gate) : base(env) {
            this.ID = ID;
            GateIndex = gate;
            simulation = env;

            if (GateIndex < Program.NUM_GATES / 2) {
                dirLAND = Direction.NORTH;
                dirTAKEOFF = Direction.SOUTH;
            } else {
                dirLAND = Direction.SOUTH;
                dirTAKEOFF = Direction.NORTH;
            }

            // Start "working" and "break_machine" processes for this machine.
            Process = env.Process(Moving());
            this.leave = false;
            //env.Process(BreakMachine());
        }

        private IEnumerable<Event> Moving() {
            while (true) {
                // change part to be on
                int oldIndex = currIndex;
                makePartsList();

                if(leave && currIndex + 1 == parts.Count) {
                    yield return Environment.TimeoutD(Program.SimTime.Hours - Environment.Now.Hour);
                } else if (ChangePart()) { //if part is at target TW 
                    yield return Environment.Timeout(TimeSpan.FromMinutes(2));
                    makePartsList();
                    parts[currIndex].Occupied = false;
                    Program.Gates[GateIndex].Occupied = true;
                    simulation.Log(this.ID + " is on " + Program.Gates[GateIndex].name + " at " + simulation.Now + " \n");
                } else if (currIndex != oldIndex) {
                    foreach (Part p in parts) {
                        Console.WriteLine(p);
                    }
                    simulation.Log(this.ID + " is on " + parts[currIndex].name + " at " + simulation.Now + " \n");
                }

                // Start movement
                var doneIn = TimeSpan.FromMinutes(2);
                while (doneIn > TimeSpan.Zero) {
                    // moving
                    yield return Environment.Timeout(doneIn);
                    doneIn = TimeSpan.Zero; // Set to 0 to exit while loop.
                }
            }
        }

        private bool ChangePart() {
            if (currIndex > 0 && currIndex + 1 == parts.Count) {
                parts[currIndex - 1].Occupied = false;
                currIndex = 0;
                if (!leave) {
                    leave = true;
                    Program.Taxiways[GateIndex].Occupied = true;
                    return true;
                }
            } else {
                if (++currIndex >= 0 && checkMovement()) {
                    //move to the next one, release from the last one
                    parts[currIndex].Occupied = true;
                    if (currIndex > 0) {
                        parts[currIndex - 1].Occupied = false;
                    }
                }
            }
            return false;
        }

        
        private bool checkMovement() { //TODO: FOR FCFS ONLY -- FOR GREEDY USE FUTURE TRACKING
            for (int i = currIndex; i < parts.Count; i++) {
                if (parts[i].Occupied) {
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
            } else {
                parts.Add(Program.Runway);
                parts.Add(Program.South);
                for (int i = 0; i < GateIndex; i++) {
                    parts.Add(Program.Taxiways[i]);
                }
            }
        }
    }
}
