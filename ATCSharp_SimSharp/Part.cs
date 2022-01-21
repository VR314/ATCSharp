using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System;
using System.Collections.Generic;


/* Vision:
 *  - each runway has a corresponding parallel runway
 *  - each runway/parallel complex is connected to the main Gate/TW complex via a connecting taxiway
 *  - each Part has a +/- direction, indicating towards/away from the GTW complex, each Part has a Tuple of Parts it's connected to, depending on the direction
 *      - decision-making, get a tree of each Part's connecteds in the direction you want (UP TO A CERTAIN DEPTH)
 *      - implement time-blocking queue
 *  - assign free gate at landing => eventually change this to plane 'types' (representing airlines or plane size) that can only go to certain gates, and pick the gate only when within scope of the GTW complex
 *  - when done GTW processing, pick a random runway and begin taxiing (how to prevent conflicts when planes are far away)
 *      - REFINE PRINCIPLES OF DECENTRALIZATION / GOALS / ALGORITHMS
 *  - checking when there is a traffic jam!
 */

public struct TimeBlock {
	public Plane plane;
	public int startTime;
	public int endTime;

	public int length {
		get {
			return (int)(endTime - startTime);
		}
	}
}

public abstract class Part {
	public string Name { get; init; }

	/// <summary>
	/// [[Positive], [Negative]]
	/// </summary>
	public List<Part>[] Connected { get; } = new List<Part>[2] { new List<Part>(), new List<Part>() };
	public List<Plane> Planes { get; set; } = new();
	public int Capacity { get; init; }
	public bool Occupied {
		get {
			if (Planes.Count > Capacity) {
				Console.Error.WriteLine("MORE PLANES THAN POSSIBLE ON " + Name);
			}
			return Planes.Count == Capacity;
		}
	}

	public Part(string name, int capacity = 1) {
		this.Name = name;
		this.Capacity = capacity;
	}

	public override string ToString() => JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented,
		new JsonConverter[] { new StringEnumConverter() });
}