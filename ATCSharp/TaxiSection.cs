using NSimulate;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp
{
    class TaxiSection : Resource
    {
        public int index { get; }
        public bool future { get; set; }
        public TaxiSection(int index)
        {
            this.index = index;
        }
    }
}
