using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Runway : Part {
	// is this needed for Runways?
	// public List<TimeBlock> TimeBlocks { get; set; } = new();

	public Runway(string name, int capacity = 1) : base(name, capacity) {

	}
}
