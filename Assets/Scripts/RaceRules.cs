using System;
namespace GalaxyVelocity {
    // Engine-independent race rules. Forward-only progress prevents reverse-line lap exploits.
    public sealed class RaceRules {
        public const float TrackLength=1800;
        public const int Laps=3, GatesPerLap=4;
        public float Progress { get; private set; }
        public int GatesPassed { get; private set; }
        public int Lap => Math.Min(Laps, (int)(Progress/TrackLength)+1);
        public bool Finished => GatesPassed>=Laps*GatesPerLap;
        public int NextGate => GatesPassed%GatesPerLap;
        public void Advance(float meters) {
            if (meters<=0 || Finished) return;
            Progress=Math.Min(TrackLength*Laps,Progress+meters);
            while (Progress >= (GatesPassed+1)*TrackLength/GatesPerLap && GatesPassed<Laps*GatesPerLap) GatesPassed++;
        }
        public void Recover() { Progress=GatesPassed*TrackLength/GatesPerLap; }
        public void Reset() { Progress=0; GatesPassed=0; }
    }
}
