using System;
using System.Collections.Generic;
using SimSharp;

namespace ATCSharp_SimSharp {

    public class Plane : ActiveObject<Simulation> {
        public readonly int GateIndex;
        public readonly string ID;
        public readonly Direction dirLAND;
        public readonly Direction dirTAKEOFF;

        public enum Direction {
            NORTH,
            SOUTH
        }

        public readonly Algorithm algorithm;

        public enum Algorithm {
            FCFS,
            FWCHECK
        }
        
        public int[] times = new int[4];

        private readonly Process Process;
        private int currIndex = -1;
        private bool leave;
        private bool left;
        private List<Part> parts;
        private Simulation simulation;
        private TimeSpan spawn;

        public Plane(ThreadSafeSimulation env, string ID, int gate, TimeSpan spawn, Algorithm algorithm) : base(env) {
            parts = new List<Part>();
            this.ID = ID;
            this.spawn = spawn;
            GateIndex = gate - 1;
            simulation = env;
            leave = false;
            this.algorithm = algorithm;
            if (GateIndex < Program.NUM_GATES / 2) {
                dirLAND = Direction.NORTH;
                dirTAKEOFF = Direction.SOUTH;
            } else {
                dirLAND = Direction.SOUTH;
                dirTAKEOFF = Direction.NORTH;
            }
            Process = env.Process(Moving());
        }

        private void makePartsList() {
            parts = new List<Part>();
            //these loops create the parts List depending on the location & target of the Plane
            if (leave) {
                if (dirTAKEOFF == Direction.NORTH) {
                    for (int i = GateIndex; i < Program.Taxiways.Count; i++) {
                        parts.Add(Program.Taxiways[i]);
                    }
                    parts.Add(Program.South);
                    parts.Add(Program.Runway);
                } else {
                    for (int i = GateIndex; i >= 0; i--) {
                        parts.Add(Program.Taxiways[i]);
                    }
                    parts.Add(Program.North);
                    parts.Add(Program.Runway);
                }
            } else
                if (dirLAND == Direction.NORTH) {
                parts.Add(Program.Runway);
                parts.Add(Program.South);
                for (int i = Program.Taxiways.Count - 1; i >= GateIndex; i--) {
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

        private bool ChangePart() {
            if (currIndex > 0 && currIndex + 1 == parts.Count) { //end of the current list
                if (!leave) { //if at gate
                    Program.Gates[GateIndex].Occupied = this; Program.Gates[GateIndex].Future = null;
                    return true;
                }
                parts[currIndex - 1].Occupied = null;
                currIndex = 0;
            } else {
                currIndex++;
                if (checkMovement()) {
                    //move to the next one, release from the last one
                    parts[currIndex].Occupied = null; parts[currIndex].Future = null;
                    if (parts.Count > currIndex + 1) { parts[currIndex + 1].Future = this; }
                    if (parts.Count > currIndex + 2) { parts[currIndex + 2].Future = this; }
                    if (parts.Count > currIndex + 3) { parts[currIndex + 3].Future = this; }
                    if (currIndex > 0) {
                        parts[currIndex - 1].Occupied = null;
                    }
                } else {
                    currIndex--;
                }
            }
            return false;
        }

        private bool checkMovement() {
            if (algorithm.Equals(Algorithm.FCFS)) { //CHECKS THE ENTIRE PATH
                for (int i = currIndex; i < parts.Count; i++) {
                    if (parts[i].Occupied != this && parts[i].Occupied != null) {
                        return false;
                    }
                }
                return true;
            } else if (algorithm.Equals(Algorithm.FWCHECK)) {
                int check;
                if (currIndex + 3 > parts.Count) {
                    check = parts.Count;
                } else {
                    check = currIndex + 3;
                }

                for (int i = currIndex; i < check; i++) { //check the 3 in front
                    if (parts[i].Occupied != this && parts[i].Occupied != null) {
                        return false;
                    }
                    if (currIndex > 1 && parts[i].Future != this && parts[i].Future != null) {
                        return false;
                    }
                }

                return true;
            } else {
                return true;
            }
        }

        private IEnumerable<Event> Moving() {
            while (true) {
                //simulation.Log(ID + " is running at " + simulation.Now);
                if ((spawn.Hours * 60 + spawn.Minutes) > simulation.NowD / 60) {
                    if(Program.Taxiways.Count > 0)
                        makePartsList();
                    yield return simulation.Timeout(TimeSpan.FromMinutes((spawn.Hours * 60 + spawn.Minutes) - simulation.NowD / 60));
                } else {
                    int oldIndex = currIndex;
                    if (!left) {
                        if (leave && currIndex + 1 == parts.Count) {
                            yield return simulation.Timeout(TimeSpan.FromMinutes(new Random().Next(2, 5)));
                            parts[currIndex].Occupied = null;
                            simulation.Log(ID + " has left at " + simulation.NowD / 60);
                            times[3] = (int)(simulation.NowD / 60);
                            left = true;
                            yield return simulation.TimeoutD(Program.SimTime.Hours - Environment.Now.Hour);
                        } else if (ChangePart()) { //if part is at GATE
                            times[1] = (int)(simulation.NowD / 60);
                            yield return simulation.Timeout(TimeSpan.FromMinutes(new Random().Next(15, 20)));
                            times[2] = (int)(simulation.NowD / 60);
                            leave = true;
                            Program.Gates[GateIndex].Occupied = null;
                            makePartsList();
                            currIndex = 0;
                        } //else if (currIndex != oldIndex) {  }

                        // moving
                        yield return Environment.Timeout(TimeSpan.FromMinutes(new Random().Next(1, 3)));
                        if (times[0] == 0) { times[0] = (int)(simulation.NowD / 60); }
                    } else { break; }
                }
            }
        }
    }
}