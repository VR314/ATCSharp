using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Runway : Part {
	//  ensure that the runway is one-direction only for simplicity
	public Direction PrefDirection;

	public Runway(string name, Direction preferredDirection, int capacity = 1) : base(name, capacity) {
		this.PrefDirection = preferredDirection;
	}
}
