using SimSharp;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

// INFO: USE OPTIMIZED RELEASE BINARY FOR GETTING DATA

public class Program {
	public readonly static TimeSpan SimDuration = TimeSpan.FromHours(12);
	public static int NUM_GATES { get; private set; }
	public static int NUM_PLANES { get; private set; }
	public static double NUM_TIMES { get; private set; }
	public static DateTime StartTime { get; private set; } = new DateTime(2000, 1, 1);
	public static Simulation Env { get; private set; } = new ThreadSafeSimulation(StartTime, 1);
	public static List<Part> Taxiways { get; private set; } = new();
	public static List<Plane> Planes { get; private set; } = new();

	public static void Main() {
		Console.WriteLine(new Taxiway("test taxiway"));
		Console.WriteLine(new Runway("test runway", Direction.NORTH));
		Console.WriteLine(new Plane(Algorithm.DLimited, "test", StartTime.AddMinutes(5)));
		Env.Run(SimDuration);
	}

	public static double Simulate(int seed) {
		Console.WriteLine("\n\n== Airport ==");
		// generate planes
		// generate airport

		// Execute!
		Env.Run(SimDuration);

		return Planes.Max(p => p.Data.TakeoffTime);
	}
}