using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Gate : Part {
	// TODO: how to keep its associated Taxiway without circular parameters
	// public Taxiway Taxiway { get; init; }
	public Gate(string name, Taxiway taxiway, int capacity = 1) : base(name, capacity) {
		// this.Taxiway = taxiway;
	}
}
