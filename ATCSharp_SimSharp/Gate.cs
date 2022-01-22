using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Gate : Part {
	// TODO: how to keep its associated Taxiway without circular parameters
	public Taxiway Taxiway { get; set; }
	public Gate(string name, int capacity = 1) : base(name, capacity) {

	}

	// run on all Gates after Airport is defined
	public void Instantiate(Taxiway taxiway) {
		this.Taxiway = taxiway;
	}
}
