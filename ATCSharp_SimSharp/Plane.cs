using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

using SimSharp;

using System;
using System.Collections.Generic;
using System.Linq;

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
	WAITING,
	LANDING,
	TAXI_IN,
	GATE,
	TAXI_OUT,
	TAKEOFF
}

public enum Half {
	LEFT,
	RIGHT
}

// class instead of struct b/c structs are immutable :(
public class PlaneData {
	public double LandingTime { get; set; } = -1;
	public double TakeoffTime { get; set; } = -1;
	public double GateArrivalTime { get; set; } = -1;
	public double TotalIdleTime { get; set; } = 0;
}

public class Plane : ActiveObject<Simulation> {
	public readonly string ID;
	public readonly int GateNumber;
	public readonly Algorithm Algorithm;
	private readonly TimeSpan spawnTime;
	private readonly Simulation simulation = Program.Env;
	private Part currentPart;

	public State State { get; private set; } = State.WAITING;
	public PlaneData Data { get; private set; } = new PlaneData();
	public Direction CurrDirection { get; private set; } = Direction.POSITIVE;
	public bool Completed { get; private set; } = false;
	private Queue<Part> partsQueue = new();
	private Queue<Part> origPartsQueue = new();
	private Half half;
	private Part target => partsQueue.ToArray()[partsQueue.Count - 1];
	private List<TimeBlock> timeBlocks => Program.Airport.TimeBlocks.FindAll((tb) => tb.Plane.Equals(this));

	public Plane(Algorithm algorithm, string ID, TimeSpan spawnTime) : base(Program.Env) {
		this.ID = ID;
		this.spawnTime = spawnTime;
		this.Algorithm = algorithm;
	}

	public Plane(Algorithm algorithm, string ID, TimeSpan spawnTime, Runway runway) : base(Program.Env) {
		this.ID = ID;
		this.spawnTime = spawnTime;
		this.Algorithm = algorithm;
		this.currentPart = runway;
	}

	// run after Airport is defined
	public void Instantiate() {
		// TODO: determine gate randomly, choose runway that corresponds to gate? OR determine runway randomly, search all positive paths and pick an open gate (that isn't time-blocked!)
		if (currentPart == null) {
			int rw = new Random().Next(Program.Airport.Runways.Count);
			currentPart = Program.Airport.Runways[rw];
		}


		if (Program.Airport.Runways.IndexOf((Runway)currentPart) < Math.Floor(Program.Airport.Runways.Count / 2.0)) {
			half = Half.LEFT;
		} else {
			half = Half.RIGHT;
		}

		simulation.Process(Moving());
	}

	private void Log(string message) {
		int time = (int)(simulation.NowD);
		if (Program.LogByTime) {
			if (Program.Logs[time] == null) {
				Program.Logs[time] = $"{simulation.NowD}\n";
			}
			Program.Logs[time] += $"\n{message}";

			string s = "";
			foreach (Part part in Program.Airport.Parts) {
				s += $"\n{part.Name}\t";
				foreach (Plane p in part.Planes) {
					s += $"{p.ID}";
				}
			}
			Program.AirportStates[time] = s;
		} else {
			simulation.Log($"{simulation.NowD}\t{message}");
		}
	}

	private bool MakePartsQueue() {
		switch (State) {
			case State.WAITING:
				List<Gate> availableGates = Program.Airport.Gates;
				int removeIndex = 0;
				int removeAmount = (Program.Airport.Gates.Count / 2);
				if (half == Half.LEFT) {
					removeIndex = (Program.Airport.Gates.Count / 2);
					removeAmount = availableGates.Count - removeIndex;
				}
				availableGates.RemoveRange(removeIndex, removeAmount);
				availableGates = availableGates.FindAll((g) => !g.Targeted);
				if (availableGates.Count == 0) {
					return false;
				}
				int index = new Random().Next(availableGates.Count);
				Taxiway target = availableGates[index].Taxiway;
				Part p = currentPart;
				partsQueue.Enqueue(p);
				do {
					p = p.Connected[(int)Direction.POSITIVE][0];
					partsQueue.Enqueue(p);
				} while (p.Connected[(int)Direction.POSITIVE].Count > 0 && !p.Equals(target));
				// partsQueue.Enqueue(target);
				partsQueue.Enqueue(target.Gate);
				target.Gate.Targeted = true;
				origPartsQueue = new Queue<Part>(partsQueue);
				return true;
			case State.GATE:
				// TODO: pick a target runway, go positive from there, and reverse the queue
				List<Part> reversedPartsList = origPartsQueue.Reverse().ToList();
				foreach (Part part in reversedPartsList) {
					partsQueue.Enqueue(part);
				}
				partsQueue.Dequeue();
				// throw new Exception("AT THE GATE -- UNIMPLEMENTED PARTS QUEUE");
				return true;
			default:
				throw new Exception("MakePartsQueue() called at unexpected time");
		}
		return false;
	}

	private void ChangePart() {
		Part oldPart = currentPart;
		Part nextPart = partsQueue.Dequeue();
		oldPart.Planes.Remove(this);
		nextPart.Planes.Add(this);
		currentPart = nextPart;
		if (Algorithm == Algorithm.DGlobal) {
			// update TimeBlocks
		}
	}

	private bool CheckMovement() {
		if (!Completed && partsQueue.Count > 0 && !partsQueue.Peek().Occupied) {
			// TODO: algorithm implementation here
			switch (Algorithm) {
				case Algorithm.DLimited:
					// Console.WriteLine();
					return true;
				case Algorithm.DGlobal:
					// Console.WriteLine();
					return true;
			}
			return true;
		} else if (Completed) {
			currentPart.Planes.Remove(this);
			return false;
		} else if (partsQueue.Count == 0) { // just finished current partsQueue
			if (State == State.TAXI_IN) {
				State = State.GATE;
			} else {
				Program.Airport.Planes.Remove(this);
				Program.Airport.CompletedPlanes.Add(this);
				Completed = true;
				currentPart.Planes.Remove(this);
				Log($"{ID} is completed!");
				Log($"{ID} has total idle time: {Data.TotalIdleTime}");
			}
			// TODO: increment state, re-make parts list based on state
			return false;
		} else {
			return false;
		}
	}

	// add variable distribution of movement times, or does that inherently interfere with time-blocking?
	private IEnumerable<Event> Moving() {
		// only runs 24 hours max
		// wait until spawnTime
		while (!MakePartsQueue()) {
			yield return simulation.Timeout(TimeSpan.FromMinutes(1));
		}

		if (simulation.NowD < 60 * spawnTime.Hours + spawnTime.Minutes) {
			yield return simulation.Timeout(TimeSpan.FromMinutes((60 * spawnTime.Hours + spawnTime.Minutes) - (simulation.NowD)));
		}

		State = State.LANDING;

		// wait until runway is open
		while (currentPart.Occupied) {
			Data.TotalIdleTime++;
			yield return simulation.Timeout(TimeSpan.FromMinutes(1));
		}

		// set to landing state
		Log($"{ID} starting at {currentPart.Name}");
		Data.LandingTime = simulation.NowD;
		State = State.TAXI_IN;

		// taxi in
		while (State == State.TAXI_IN) {
			if (CheckMovement()) {
				ChangePart();
				Log($"{ID} --> {currentPart.Name}");
				yield return simulation.Timeout(TimeSpan.FromMinutes(3));
			} else {
				Data.TotalIdleTime++;
				yield return simulation.Timeout(TimeSpan.FromMinutes(1));
			}
		}

		while (!MakePartsQueue()) {
			yield return simulation.Timeout(TimeSpan.FromMinutes(1));
		}
		State = State.TAXI_OUT;

		while (State == State.TAXI_OUT) {
			if (CheckMovement()) {
				ChangePart();
				Log($"{ID} --> {currentPart.Name}");
				yield return simulation.Timeout(TimeSpan.FromMinutes(3));
			} else {
				Data.TotalIdleTime++;
				yield return simulation.Timeout(TimeSpan.FromMinutes(1));
			}
		}
	}

	/*
	 * 
				if (Completed) {
					yield return simulation.Timeout(TimeSpan.FromMinutes(Program.SimDuration.TotalMinutes - simulation.NowD + 1));
				} else {
					// .Log($"{ID} == {currentPart.Name}");
					Data.TotalIdleTime++;
					yield return simulation.Timeout(TimeSpan.FromMinutes(1));
				}
	*/

	// to serialize only some fields, replace this with new { } and fill in only needed parameters 
	public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented,
		new JsonConverter[] { new StringEnumConverter() });
}