using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp_SimSharp {
    public class Part {
        public Part(string name) {
            this.name = name;
            Occupied = false;
        }

        public string name { get; set; }
        public bool Occupied { get; set; }

        public bool Future { get; set; }
        public override string ToString() {
            return name + "  " + Occupied;
        }
    }
}
