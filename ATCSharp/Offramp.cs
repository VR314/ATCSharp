using NSimulate;

namespace ATCSharp
{
    public class Offramp : Resource
    {
        public int index { get; }
        public Offramp(int index)
        {
            this.index = index;
        }
    }
}