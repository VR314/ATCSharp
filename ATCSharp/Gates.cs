using NSimulate;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp
{
    public class Gates : Resource
    {
        public bool[] gates = new bool[6] { false, false, false, false, false, false };

        public Gates() { }
    }
}
