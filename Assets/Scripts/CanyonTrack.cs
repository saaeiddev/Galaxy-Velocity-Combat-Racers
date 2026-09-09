using UnityEngine;
namespace GalaxyVelocity {
    public static class CanyonTrack {
        public const float Width=26;
        public static Vector3 Point(float distance) {
            float t=distance/RaceRules.TrackLength*Mathf.PI*2;
            return new Vector3(Mathf.Sin(t)*285+Mathf.Sin(t*3)*32, 22+Mathf.Sin(t*2)*9, Mathf.Cos(t)*250);
        }
        public static Vector3 Forward(float s) => (Point(s+1)-Point(s-1)).normalized;
        public static Vector3 Right(float s) => Vector3.Cross(Vector3.up,Forward(s)).normalized;
        public static Vector3 Position(float s,float lane,float height=0) => Point(s)+Right(s)*lane+Vector3.up*height;
        public static float Loop(float s) => Mathf.Repeat(s,RaceRules.TrackLength);
        public static bool Crossed(float before,float after,float at) {
            int lap=Mathf.FloorToInt(before/RaceRules.TrackLength);
            float target=lap*RaceRules.TrackLength+at;
            if(target<=before) target+=RaceRules.TrackLength;
            return after>=target;
        }
    }
}
