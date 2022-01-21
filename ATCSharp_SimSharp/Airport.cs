using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Airport {
	public List<Plane> Planes = new();
	public List<Part> Parts = new();
	public List<Gate> Gates = new();
	public List<Taxiway> Taxiways = new();
	public List<Runway> Runways = new();

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

		this.Runways.AddRange(Parts.FindAll((item) => item.GetType() == typeof(Runway)).Select((item) => (Runway)item).ToList());
		this.Taxiways.AddRange(Parts.FindAll((item) => item.GetType() == typeof(Taxiway)).Select((item) => (Taxiway)item).ToList());
		Console.WriteLine(this);
	}
	public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented,
		new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
}
