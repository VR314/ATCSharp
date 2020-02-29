using NSimulate;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp {
    public class Gate : Resource {
        public int index { get; }

        public Gate(int i) {
            index = i;
        }
    }
}
