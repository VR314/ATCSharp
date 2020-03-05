using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp_SimSharp {
    public class Part {
        public Part(string name) {
            this.name = name;
            Occupied = null;
            Future = null;
        }

        public string name { get; set; }
        public Plane Occupied { get; set; }

        public Plane Future { get; set; }
        public override string ToString() {
            if (Occupied == null) {
                return name + " " + null;
            } else {
                return name + "  " + Occupied.ID;
            }
        }
    }
}
