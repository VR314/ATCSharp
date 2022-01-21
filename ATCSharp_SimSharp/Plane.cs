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
	POSITIVE,
	NEGATIVE
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
	private Part currentPart;

	public PlaneData Data = new PlaneData();
	public Direction CurrDirection { get; }
	public bool Completed { get; private set; } = false;
	private Queue<Part> partsQueue = new();
	private Part target => partsQueue.ToArray()[partsQueue.Count - 1];

	public Plane(Algorithm algorithm, string ID, DateTime spawnTime) : base(Program.Env) {
		this.ID = ID;
		this.spawnTime = spawnTime;
		this.algorithm = algorithm;
		this.process = simulation.Process(Moving());
	}

	// run on all planes after Airport is defined
	public void Instantiate() {
		currentPart = Program.Airport.Runways[0];
		MakePartsQueue();
	}

	// for now, search for all parts in the positive direction
	private void MakePartsQueue() {
		Part p = currentPart;
		partsQueue.Enqueue(p);
		do {
			p = p.Connected[(int)Direction.POSITIVE][0];
			partsQueue.Enqueue(p);
		} while (p.Connected[(int)Direction.POSITIVE].Count > 0);
	}

	private void ChangePart() {
		Part oldPart = currentPart;
		Part nextPart = partsQueue.Dequeue();
		oldPart.Planes.Remove(this);
		nextPart.Planes.Add(this);
		currentPart = nextPart;
	}

	private bool CheckMovement() {
		if (!Completed && partsQueue.Count > 0 && !partsQueue.Peek().Occupied) {
			return true;
		} else if (partsQueue.Count == 0) {
			Program.Airport.Planes.Remove(this);
			Program.Airport.CompletedPlanes.Add(this);
			Completed = true;
			currentPart.Planes.Remove(this);
			simulation.Log($"{simulation.NowD / 60}\t{ID} is completed!");
			return false;
		} else {
			return false;
		}
	}

	private IEnumerable<Event> Moving() {
		// only runs 24 hours max
		if (simulation.NowD / 60 > 60 * spawnTime.Hour + spawnTime.Minute) {
			yield return simulation.Timeout(TimeSpan.FromMinutes((simulation.NowD / 60) - (60 * spawnTime.Hour + spawnTime.Minute)));
		}
		simulation.Log($"{simulation.NowD / 60}\t{ID} starting at {currentPart.Name}");
		while (true) {
			if (CheckMovement()) {
				ChangePart();
				simulation.Log($"{simulation.NowD / 60}\t{ID} --> {currentPart.Name}");
				yield return simulation.Timeout(TimeSpan.FromMinutes(5));
			} else {
				if (Completed) {
					yield return simulation.Timeout(TimeSpan.FromMinutes(1000000000));
				} else {
					// simulation.Log($"{simulation.NowD / 60}\t{ID} == {currentPart.Name}");
					yield return simulation.Timeout(TimeSpan.FromMinutes(1));
				}
			}
		}
	}

	public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented,
		new JsonConverter[] { new StringEnumConverter() });
}