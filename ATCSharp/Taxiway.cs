using NSimulate;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATCSharp {
    public class Taxiway : Resource {

        public Taxiway(List<TaxiSection> sections) {
            this.sections = sections;
        }

        public List<TaxiSection> sections { get;  }
    }
}
