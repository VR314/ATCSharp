using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Taxiway : Part {
	public Gate Gate { get; init; }
	public Taxiway(string name, Gate gate = null, int capacity = 1) : base(name, capacity) {
		this.Gate = gate;
	}
}
