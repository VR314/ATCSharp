namespace ATCSharp_SimSharp {

    public class Part {
        public Part(string name) {
            this.name = name;
            Occupied = null;
            Future = null;
        }

        public string name { get; set; }
        public Plane Occupied { get; set; }
        public Plane Future { get; set; }

        public override string ToString() {
            if (Future == null) {
                return name + " ";
            } else {
                return name + "  " + Future.ID;
            }
        }
    }
}