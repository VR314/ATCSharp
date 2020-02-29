using NSimulate;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp
{
    public class TaxiSection : Resource
    {
        public int index { get; }
        public bool future { get; set; } //USED IN GREEDY
        public Gate gate { get; }
        public TaxiSection(int index)
        {
            this.index = index;
            gate = new Gate(index);
        }
        public new int Allocated { get; set; } //TODO: if plane is Allocated & its index == this.index, then Allocated-- and Gate.Allocated++
    }
}
