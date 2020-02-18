using NSimulate;
using NSimulate.Instruction;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp
{
    public class TaxiActivity : Activity
    {
        public override IEnumerator<InstructionBase> Simulate()
        {
            AllocateInstruction<Taxiway> allocateTW = new AllocateInstruction<Taxiway>(10); //time to take over taxiway while landing
            yield return allocateTW;
        }
    }
}
