using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

using SimSharp;

using System;
using System.Collections.Generic;

/* Algorithms:
 * - Decentralized Limited: like FCFS with bigger scope
 * - Decentralized Global: Using Time-Blocking Queues (requires multiple paths between points? -- or just forces more efficient waiting?)
 */


public enum Direction {
	NORTH,
	SOUTH
}
public enum Algorithm {
	DLimited,
	DGlobal
}

public enum State {
	LANDING,
	TAXI_IN,
	GATE,
	TAXI_OUT,
	TAKEOFF
}

public struct PlaneData {
	public int LandingTime { get; init; }
	public int TakeoffTime { get; init; }
	public int GateArrivalTime { get; init; }
}

public class Plane : ActiveObject<Simulation> {
	public readonly string ID;
	public readonly int GateIndex;
	public readonly Algorithm algorithm;
	private readonly DateTime spawnTime;
	private readonly Simulation simulation = Program.Env;
	private readonly Process process;

	public PlaneData Data = new PlaneData();
	public Direction CurrDirection { get; }
	public bool Completed { get; private set; } = false;
	private Queue<Part> partsQueue = new();

	public Plane(Algorithm algorithm, string ID, DateTime spawnTime) : base(Program.Env) {
		this.ID = ID;
		this.spawnTime = spawnTime;
		this.algorithm = algorithm;
		this.process = simulation.Process(Moving());
	}

	private void MakePartsQueue() {

	}

	private void ChangePart() {

	}

	private bool CheckMovement() {
		return true;
	}

	private IEnumerable<Event> Moving() {
		while (true) {
			var x = simulation.Timeout(TimeSpan.FromMinutes((simulation.NowD / 60) + 5));
			Console.WriteLine("interval: " + (simulation.NowD / 60 + 5));
			yield return x;
			simulation.Log(ID + " running at " + simulation.NowD / 60);
		}
	}

	public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented,
		new JsonConverter[] { new StringEnumConverter() });
}