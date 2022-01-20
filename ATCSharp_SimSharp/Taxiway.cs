﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Taxiway : Part {
	//TODO: keep this sorted in time order
	public List<TimeBlock> TimeBlocks { get; set; } = new();

	// if this is null, then it isn't a Gated-Taxiway
	// TODO: OR make a new object called a Gated-Taxiway that can handle both planes passing through and in "parking" to avoid circular reference
	public Gate Gate { get; init; }
	public Taxiway(string name, Gate gate = null, int capacity = 1) : base(name, capacity) {
		this.Gate = gate;
	}
}
