using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System;
using System.Collections.Generic;

namespace ATCSharp_SimSharp {

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


    public class Part {
        public Part(string name) {
            this.Name = name;
            this.Planes = new List<Plane>();
            this.Connected = Array.Empty<Part>();
            this.Capacity = 1;
            Occupied = null;
            Future = null;
        }

        public string Name { get; set; }

        public Part[] Connected { get; }

        public List<Plane> Planes { get; }

        public int Capacity { get; }

        // underscore to avoid naming collision
        public bool OccupiedTemp {
            get {
                return Planes.Count == Capacity;
            }
        }

        // TODO: make an array of planes, and make Occupied a get() that returns whether planes.length == capacity
        // TODO: implement capacity, time duration for taxi, connected tuple, and time-blocking data structure
        /* TODO: REMOVE */
        public Plane Occupied { get; set; }
        /* TODO: TO REMOVE */
        public Plane Future { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented,
            new Newtonsoft.Json.JsonConverter[] { new StringEnumConverter() });
    }
}