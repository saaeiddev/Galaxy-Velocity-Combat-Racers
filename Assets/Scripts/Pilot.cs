using UnityEngine;
namespace GalaxyVelocity {
    public sealed class Pilot {
        public readonly string Name, Species, Ability, Description;
        public readonly float Speed, Handling, FireInterval;
        public readonly Color Color;
        public Pilot(string name, string species, string ability, string description, float speed, float handling, float fireInterval, Color color) {
            Name=name; Species=species; Ability=ability; Description=description;
            Speed=speed; Handling=handling; FireInterval=fireInterval; Color=color;
        }
        public static readonly Pilot[] All = {
            new Pilot("ROOK SOLAR", "VULPINE / COMMANDER", "AEGIS PULSE", "Restore 45 shield and gain 3 seconds of immunity.", 78, 20, .17f, new Color(1,.23f,.12f)),
            new Pilot("KITE AZURE", "FALCON / INTERCEPTOR", "OVERDRIVE", "Gain 5 seconds of additional speed and rapid fire.", 86, 17, .12f, new Color(.12f,.5f,1)),
            new Pilot("NYX VESPER", "LUNAR LYNX / PHASE RUNNER", "PHASE SHIFT", "Refill boost and phase through damage for 4 seconds.", 75, 26, .19f, new Color(.65f,.23f,1))
        };
    }
}
