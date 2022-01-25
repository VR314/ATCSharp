using SimSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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

public class Program {
	public readonly static TimeSpan SimDuration = TimeSpan.FromHours(23);
	public static Simulation Env { get; private set; } = new ThreadSafeSimulation(randomSeed: 1, defaultStep: TimeSpan.FromMinutes(1));
	public static Airport Airport { get; set; }

	public static bool LogByTime { get; } = true && RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
	public static string[] Logs { get; set; } = new string[(int)SimDuration.TotalMinutes];
	public static string[] AirportStates { get; set; } = new string[(int)SimDuration.TotalMinutes];

	public static void Main() {
		List<Plane> planes = new() {
			new Plane(Algorithm.DLimited, "P1", TimeSpan.FromMinutes(5)),
			new Plane(Algorithm.DLimited, "P2", TimeSpan.FromMinutes(6)),
			new Plane(Algorithm.DLimited, "P3", TimeSpan.FromMinutes(7)),
		};

		List<Gate> gates = new() {
			new Gate("G1"),
			new Gate("G2"),
			new Gate("G3"),
			new Gate("G4"),
		};

		List<Part> parts = new() {
			new Runway("R0FL"),
			new Taxiway("T1"),
			new Taxiway("T2+G1", gates[0]),
			new Taxiway("T3+G2", gates[1]),
			// -- dividing --
			new Taxiway("T4+G3", gates[2]),
			new Taxiway("T5+G4", gates[3]),
			new Taxiway("T6"),
			new Taxiway("T7"),
			new Runway("R1BR"),
			new Runway("R2TR"),
		};

		// a --> b is the "positive" direction, so b.connected[1] += a; && a.connected[0] += b;
		List<Link> links = new() {
			new Link { a = "R0FL", b = "T1" },
			new Link { a = "T1", b = "T2+G1" },
			new Link { a = "T2+G1", b = "T3+G2" },
			//--dividing airport in half: left runway can only go to left half gates (<= n/2)--
			new Link { a = "R1BR", b = "T6" },

			new Link { a = "R2TR", b = "T7" },
			new Link { a = "T7", b = "T6" },
			new Link { a = "T6", b = "T5+G4" },
			new Link { a = "T5+G4", b = "T4+G3" },
		};

		// TODO: after validating algorithms, make this a large, quad-runway airport with many gates and see how behavior changes
		Airport = new Airport(planes, parts, gates, links);
		Airport.Instantiate();

		Env.Run(SimDuration);
		if (LogByTime) {
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
						return;
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

	public static double Simulate(int seed) {
		Console.WriteLine("\n\n== Airport ==");
		// generate planes
		// generate airport

		// Execute!
		Env.Run(SimDuration);

		return Airport.Planes.Max(p => p.Data.TakeoffTime);
	}
}