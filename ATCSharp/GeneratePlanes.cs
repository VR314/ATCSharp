using NSimulate;
using NSimulate.Instruction;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp
{
    class GeneratePlanes : Process
    {
        public override IEnumerator<InstructionBase> Simulate()
        {
            
            //
            // this method is implemented as an iterator that uses _yield return_ statements to return instructions to the simulator
            // instructions may:
            //   -  SCHEDULE ACTIVITIES

            // e.g. wait until a notification is raised... in this case a notification that an alarm is ringing
        //yield return new WaitNotificationInstruction<AlarmRingingNotification>(); - checks NOTIFICATION - useless in this class?

            // request a resource (if resources are not available, the process will be blocked here until they become available
        //var allocateResourceXInstruction = new AllocateInstruction<ResourceX>(1); 
        //yield return allocateResourceXInstruction;

            // wait for a period of time
            yield return new WaitInstruction(10); // how long to wait for each action

            // release a previously allocated resource
        //yield return new ReleaseInstruction<ResourceX>(allocateResourceXInstruction); - remove from runway, gate, etc. after some time
        }
    }
}
