using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// since this is immutable, remove and re-create when editing
public struct TimeBlock {
	public Part Part { get; set; }
	public Plane Plane { get; set; }
	public int StartTime { get; set; }
	public int EndTime { get; set; }

	public int length {
		get {
			return (int)(EndTime - StartTime);
		}
	}
}

public class Airport {
	public List<Plane> Planes = new();
	public List<Plane> CompletedPlanes = new();
	public List<Part> Parts = new();
	public List<Gate> Gates = new();
	public List<Taxiway> Taxiways = new();
	public List<Runway> Runways = new();
	// TODO: keep this sorted in time order
	public List<TimeBlock> TimeBlocks { get; set; } = new();

	// TODO: make this a proper constructor
	public Airport(List<Plane> planes, List<Part> parts, List<Gate> gates, List<Link> links) {
		this.Planes = planes;
		this.Parts = parts;
		this.Gates = gates;
		// since a --> b is the positive direction
		foreach (Link l in links) {
			// a is negative to b
			l.b.Connected[(int)Direction.NEGATIVE].Add(l.a);
			// b is positive to a
			l.a.Connected[(int)Direction.POSITIVE].Add(l.b);
		}

		Runways.AddRange(Parts.FindAll((item) => item.GetType() == typeof(Runway)).Select((item) => (Runway)item).ToList());
		Taxiways.AddRange(Parts.FindAll((item) => item.GetType() == typeof(Taxiway)).Select((item) => (Taxiway)item).ToList());
	}

	public void Instantiate() {
		foreach (Plane p in Planes) {
			p.Instantiate();
		}

		foreach (Taxiway t in Taxiways) {
			if (t.Gate != null) {
				t.Gate.Taxiway = t;
			}
		}
	}

	public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented,
		new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
}
