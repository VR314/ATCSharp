using NSimulate;
using NSimulate.Instruction;
using System.Collections.Generic;

namespace ATCSharp {
    public class TaxiActivity : Activity {
        private bool leave;
        private Plane plane;
        private List<Resource> parts = new List<Resource>();
        public TaxiActivity(bool l, Plane p) {
            this.leave = l;
            plane = p;
        }
        public override IEnumerator<InstructionBase> Simulate() {
            int index = plane.Characteristics.GateIndex;

            ///these loops create the parts List depending on the location & target of the Plane
            if (leave) {
                if (plane.Characteristics.TakeoffDirection == Globals.Direction.NORTH) {
                    for (int i = index; i < Globals.NUM_GATES; i++) {
                        parts.Add(Globals.taxiways[i]);
                    }
                    parts.Add(Globals.south);
                    parts.Add(Globals.runway);
                } else {
                    for (int i = index; i >= 0; i--) {
                        parts.Add(Globals.taxiways[i]);
                    }
                    parts.Add(Globals.north);
                    parts.Add(Globals.runway);
                }
            } else
                if (plane.Characteristics.LandDirection == Globals.Direction.NORTH) {
                parts.Add(Globals.south);
                for (int i = Globals.NUM_GATES; i >= index; i--) {
                    parts.Add(Globals.taxiways[i]);
                }
            } else {
                parts.Add(Globals.south);
                for (int i = 0; i < index; i++) {
                    parts.Add(Globals.taxiways[i]);
                }
            }

            while (plane.Characteristics.CurrentIndex != parts.Count) {
                AllocateInstruction<TaxiSection> allocate = new AllocateInstruction<TaxiSection>(1);
                yield return allocate;
            }
        }

        private bool checkMovement() { //TODO: FOR FCFS ONLY -- FOR GREEDY USE FUTURE TRACKING
            foreach (Resource r in parts) {
                if (r.Allocated == 1) { //ASSUMING Allocated means the number on that Resource
                    return false;
                }
            }
            return true;
        }
    }
}
