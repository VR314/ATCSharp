using NSimulate;
using NSimulate.Instruction;
using System.Collections.Generic;

namespace ATCSharp
{
    public class Plane : Process
    {
        public PlaneCharacteristics Characteristics { get; set; }
        public PlaneStatistics Statistics { get; set; }

        public Plane(PlaneCharacteristics c)
        {
            Characteristics = c;
            Statistics = new PlaneStatistics();

        }

        public override IEnumerator<InstructionBase> Simulate()
        {
            if(Characteristics.Spawn <= Context.TimePeriod)
            {
                //wait for spawn time
                yield return new WaitInstruction((long)Context.TimePeriod - (long)Characteristics.Spawn);
            }

            Statistics.Spawn = Context.TimePeriod;
            AllocateInstruction<Runway> allocateRunway = new AllocateInstruction<Runway>(1);
            yield return allocateRunway;

            Statistics.LandClearance = Context.TimePeriod;

            yield return new WaitInstruction((long)Characteristics.RunwayDuration);

            Statistics.DoneRunway = Context.TimePeriod;

            ReleaseInstruction<Runway> releaseRunway = new ReleaseInstruction<Runway>(allocateRunway);

            //------------------------------------------------------------------------------------------------------------
            //                          GATE

            TaxiActivity taxi = new TaxiActivity(true, this);
            yield return new ScheduleActivityInstruction(taxi, Context.TimePeriod);

            Statistics.GateClearance = Context.TimePeriod;

            yield return new WaitInstruction((long)Characteristics.GateDuration);

            Statistics.GateDischarge = Context.TimePeriod;

            ReleaseInstruction<TaxiSection> releaseGate = new ReleaseInstruction<TaxiSection>(allocateGate);
            yield return releaseGate;

            taxi = new TaxiActivity(false, this);
            yield return new ScheduleActivityInstruction(taxi, Context.TimePeriod);
        }
    }
}