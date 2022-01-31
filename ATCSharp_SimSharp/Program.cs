using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SimSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CsvHelper;
using System.Globalization;

// INFO: USE OPTIMIZED RELEASE BINARY FOR GETTING DATA
public struct Link {
	public Part pA {
		get {
			string search = a;
			return Program.Airport.Parts.Find((part) => part.Name == search);
		}
	}
	public Part pB {
		get {
			string search = b;
			return Program.Airport.Parts.Find((part) => part.Name == search);
		}
	}

	public string a;
	public string b;
}

public class SimSummary {
	public double maxTakeoffTime { get; set; }
	public double avgIdleTime { get; set; }
	public double maxIdleTime { get; set; }
}

public class Program {
	public static Algorithm Alg = Algorithm.DGlobal;
	public readonly static TimeSpan SimDuration = TimeSpan.FromHours(23);
	public static Simulation Env { get; private set; }
	public static Airport Airport { get; set; }

	// make sure this is set to false when debugging in real-time
	public static bool LogByTime { get; } = false && RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
	public static string[] Logs { get; set; }
	public static string[] AirportStates { get; set; }
	public static List<SimSummary> SimSummaries { get; set; } = new();

	public static void Main() {
		SimulateForData();
	}

	public static void Debug() {
		Env = new ThreadSafeSimulation(randomSeed: 1, defaultStep: TimeSpan.FromMinutes(1));
		Logs = new string[(int)SimDuration.TotalMinutes];
		AirportStates = new string[(int)SimDuration.TotalMinutes];


		List<Gate> gates = new() {
			new Gate("G1"),
			new Gate("G2"),
			new Gate("G3"),
			new Gate("G4"),
		};

		List<Part> parts = new() {
			new Runway("R_TL"),
			new Runway("R_BL"),
			new Taxiway("T0"),
			new Taxiway("T1"),
			new Taxiway("T2+G1", gates[0]),
			new Taxiway("T3+G2", gates[1]),
			// -- dividing --
			new Taxiway("T4+G3", gates[2]),
			new Taxiway("T5+G4", gates[3]),
			new Taxiway("T6"),
			new Taxiway("T7"),
			new Runway("R_TR"),
			new Runway("R_BR"),
		};

		// a --> b is the "positive" direction, so b.connected[1] += a; && a.connected[0] += b;
		List<Link> links = new() {
			new Link { a = "R_BL", b = "T1" },
			new Link { a = "R_TL", b = "T0" },
			new Link { a = "T0", b = "T1" },
			new Link { a = "T1", b = "T2+G1" },
			new Link { a = "T2+G1", b = "T3+G2" },
			//--dividing airport in half: left runway can only go to left half gates (<= n/2)--
			new Link { a = "R_BR", b = "T6" },

			new Link { a = "R_TR", b = "T7" },
			new Link { a = "T7", b = "T6" },
			new Link { a = "T6", b = "T5+G4" },
			new Link { a = "T5+G4", b = "T4+G3" },
		};

		List<Plane> planes = new() {
			new Plane(Alg, "P1", TimeSpan.FromMinutes(5)),
			new Plane(Alg, "P2", TimeSpan.FromMinutes(6)),
			new Plane(Algorithm.DLimited, "P3", TimeSpan.FromMinutes(7)),
		};

		Airport = new Airport(planes, parts, gates, links);
		Airport.Instantiate();
		// Console.WriteLine("Running...");
		Env.Run(SimDuration);
		if (LogByTime) {
			Console.Clear();
			List<string> logs = Logs.Where(x => x != null).ToList();
			List<string> states = AirportStates.Where(x => x != null).ToList();
			int i = 0;
			int hMax = 0;
			for (int x = 0; x < logs.Count; x++) {
				hMax = Math.Max((logs[i].Count(c => c == '\n') + states[i].Count(c => c == '\n') / 5) * 5, hMax);
			}

			while (i < logs.Count) {
				Console.SetWindowSize(Console.WindowWidth, hMax);
				Console.SetWindowPosition(0, i * Console.WindowHeight);
				Console.SetCursorPosition(0, i * Console.WindowHeight);
				Console.WriteLine(logs[i]);
				Console.WriteLine("--------------------------\n");
				Console.Write(states[i]);
				var ch = Console.ReadKey(false).Key;
				switch (ch) {
					case ConsoleKey.Enter: // exit
						i = logs.Count;
						Console.WriteLine();
						break;
					case ConsoleKey.UpArrow:
						i = Math.Min(i + 1, logs.Count - 1);
						break;
					case ConsoleKey.RightArrow:
						i = Math.Min(i + 1, logs.Count - 1);
						break;
					case ConsoleKey.LeftArrow:
						i = Math.Max(i - 1, 0);
						break;
					case ConsoleKey.DownArrow:
						i = Math.Max(i - 1, 0);
						break;
				}
			}
		}
	}

	public static void SimulateForData() {
		Alg = Algorithm.DLimited;
		for (int i = 0; i < 1000;) {
			SimSummary? output = Simulate(5);
			if (output != null) {
				i++;
				Console.WriteLine(i);
				SimSummaries.Add(output);
			}
		}
		Console.WriteLine("---");
		string path = @"test.csv";
		using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write))) {
			writer.WriteLine("LIMITED");
			string data = String.Join(",", "avgIdle", "maxIdle", "maxTakeoff");
			writer.WriteLine(data);
			foreach (SimSummary summary in SimSummaries) {
				data = String.Join(",", summary.avgIdleTime, summary.maxIdleTime, summary.maxTakeoffTime);
				writer.WriteLine(data);
			}
		}

		SimSummaries = new();
		Alg = Algorithm.DGlobal;
		for (int i = 0; i < 1000;) {
			SimSummary? output = Simulate(5);
			if (output != null) {
				i++;
				Console.WriteLine(i);
				SimSummaries.Add(output);
			}
		}
		Console.WriteLine("---");
		path = @"test.csv";
		using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write))) {
			writer.WriteLine();
			writer.WriteLine();
			writer.WriteLine("GLOBAL");
			string data = String.Join(",", "avgIdle", "maxIdle", "maxTakeoff");
			writer.WriteLine(data);
			foreach (SimSummary summary in SimSummaries) {
				data = String.Join(",", summary.avgIdleTime, summary.maxIdleTime, summary.maxTakeoffTime);
				writer.WriteLine(data);
			}
		}
	}

	public static SimSummary? Simulate(int numPlanes) {
		Env = new ThreadSafeSimulation(randomSeed: 1, defaultStep: TimeSpan.FromMinutes(1));
		Logs = new string[(int)SimDuration.TotalMinutes];
		AirportStates = new string[(int)SimDuration.TotalMinutes];


		List<Gate> gates = new() {
			new Gate("G1"),
			new Gate("G2"),
			new Gate("G3"),
			new Gate("G4"),
		};

		List<Part> parts = new() {
			new Runway("R_TL"),
			new Runway("R_BL"),
			new Taxiway("T0"),
			new Taxiway("T1"),
			new Taxiway("T2+G1", gates[0]),
			new Taxiway("T3+G2", gates[1]),
			// -- dividing --
			new Taxiway("T4+G3", gates[2]),
			new Taxiway("T5+G4", gates[3]),
			new Taxiway("T6"),
			new Taxiway("T7"),
			new Runway("R_TR"),
			new Runway("R_BR"),
		};

		// a --> b is the "positive" direction, so b.connected[1] += a; && a.connected[0] += b;
		List<Link> links = new() {
			new Link { a = "R_BL", b = "T1" },
			new Link { a = "R_TL", b = "T0" },
			new Link { a = "T0", b = "T1" },
			new Link { a = "T1", b = "T2+G1" },
			new Link { a = "T2+G1", b = "T3+G2" },
			//--dividing airport in half: left runway can only go to left half gates (<= n/2)--
			new Link { a = "R_BR", b = "T6" },

			new Link { a = "R_TR", b = "T7" },
			new Link { a = "T7", b = "T6" },
			new Link { a = "T6", b = "T5+G4" },
			new Link { a = "T5+G4", b = "T4+G3" },
		};

		List<Plane> planes = GeneratePlanes(numPlanes);

		Airport = new Airport(planes, parts, gates, links);
		Airport.Instantiate();
		Env.Run(SimDuration);

		// TODO: find alternative: currently ignoring stalemates...
		if (Airport.CompletedPlanes.Count != planes.Count) {
			return null;
		}

		return new SimSummary {
			maxIdleTime = Airport.CompletedPlanes.Max(p => p.Data.TotalIdleTime),
			avgIdleTime = Airport.CompletedPlanes.Average(p => p.Data.TotalIdleTime),
			maxTakeoffTime = Airport.CompletedPlanes.Max(p => p.Data.TakeoffTime),
		};
	}

	public static List<Plane> GeneratePlanes(int numPlanes) {
		Random rnd = new Random();
		List<Plane> planes = new List<Plane>();
		for (int i = 0; i < numPlanes; i++) {
			planes.Add(new Plane(Alg, $"P{i}", TimeSpan.FromMinutes(rnd.Next(i * 50))));
		}
		return planes;
	}
}