using System.Collections.Generic;
using UnityEngine;
namespace GalaxyVelocity {
    public sealed class CombatSystem {
        sealed class Shot {public GameObject Visual;public Vector3 Position,Direction;public Racer Owner;public Transform Target;public float Life;public bool Missile;}
        public sealed class Drone {public GameObject Visual;public float S,Lane,Health=60,Respawn,Fire;}
        readonly List<Shot> shots=new List<Shot>(); public readonly List<Drone> Drones=new List<Drone>();
        readonly GameSession game;
        public CombatSystem(GameSession session){game=session;for(int i=0;i<8;i++){
            var root=new GameObject("Hostile drone");ProceduralArt.Part("Core",root.transform,Vector3.zero,new Vector3(2,1,2),new Color(.15f,.17f,.2f),PrimitiveType.Sphere);
            ProceduralArt.Part("Red optic",root.transform,new Vector3(0,0,-1),new Vector3(.8f,.3f,.2f),Color.red,PrimitiveType.Cube,true);
            ProceduralArt.Part("Cross wings",root.transform,Vector3.zero,new Vector3(5,.2f,1),new Color(.25f,.29f,.35f));
            Drones.Add(new Drone{Visual=root,S=170+i*205,Lane=i%2==0?-6:6,Fire=i*.4f});
        }}
        public void Fire(Racer owner,bool missile){
            if(shots.Count>=160)return;
            Vector3 origin=owner.transform.position+owner.transform.forward*3;
            Transform target=null;float nearest=140;
            foreach(var r in game.Racers)if(r!=owner&&!r.Race.Finished)Consider(r.transform,origin,owner.transform.forward,ref target,ref nearest);
            foreach(var d in Drones)if(d.Respawn<=0)Consider(d.Visual.transform,origin,owner.transform.forward,ref target,ref nearest);
            var direction=target?(target.position-origin).normalized:owner.transform.forward;
            Spawn(origin,direction,owner,missile,missile?target:null);
        }
        void Consider(Transform t,Vector3 origin,Vector3 forward,ref Transform result,ref float nearest){
            var delta=t.position-origin;float dist=delta.magnitude;
            if(dist<nearest&&Vector3.Dot(delta.normalized,forward)>.85f){result=t;nearest=dist;}
        }
        void Spawn(Vector3 origin,Vector3 direction,Racer owner,bool missile,Transform target){
            var o=ProceduralArt.Part(missile?"Homing missile":"Laser bolt",null,origin,new Vector3(missile?.35f:.12f,missile?.35f:.12f,missile?1.5f:4),missile?new Color(1,.5f,.05f):owner&&owner.IsPlayer?Color.cyan:Color.red,PrimitiveType.Cube,true);
            shots.Add(new Shot{Visual=o,Position=origin,Direction=direction,Owner=owner,Missile=missile,Target=target,Life=missile?4:1.6f});
        }
        public void Tick(float dt){
            foreach(var d in Drones){
                if(d.Respawn>0){d.Respawn-=dt;d.Visual.SetActive(false);if(d.Respawn<=0){d.Health=60;d.Visual.SetActive(true);}continue;}
                d.Visual.transform.position=CanyonTrack.Position(d.S,d.Lane,3+Mathf.Sin(game.RaceTime*2+d.S)*.7f);d.Visual.transform.Rotate(0,30*dt,0);
                d.Fire-=dt;var p=game.Player;Vector3 delta=p.transform.position-d.Visual.transform.position;
                if(delta.magnitude<100&&d.Fire<=0){d.Fire=2.2f;Spawn(d.Visual.transform.position,delta.normalized,null,false,null);}
            }
            for(int i=shots.Count-1;i>=0;i--){var s=shots[i];s.Life-=dt;
                if(s.Missile&&s.Target&&s.Target.gameObject.activeInHierarchy)s.Direction=Vector3.RotateTowards(s.Direction,(s.Target.position-s.Position).normalized,3*dt,0);
                Vector3 previous=s.Position;s.Position+=s.Direction*(s.Missile?190:290)*dt;bool hit=false;
                foreach(var r in game.Racers)if(r!=s.Owner&&!r.Race.Finished&&Near(previous,s.Position,r.transform.position,2.6f)){
                    bool killed=r.Damage(s.Missile?70:17);if(killed&&s.Owner)s.Owner.Kills++;hit=true;break;
                }
                if(!hit&&s.Owner)foreach(var d in Drones)if(d.Respawn<=0&&Near(previous,s.Position,d.Visual.transform.position,2.4f)){
                    d.Health-=s.Missile?70:22;if(d.Health<=0){d.Respawn=8;s.Owner.Kills++;if(s.Owner.IsPlayer)game.Notify("DRONE DESTROYED");}hit=true;break;
                }
                if(hit||s.Life<=0){Object.Destroy(s.Visual);shots.RemoveAt(i);}else{s.Visual.transform.position=s.Position;s.Visual.transform.rotation=Quaternion.LookRotation(s.Direction);}
            }
        }
        public static bool Near(Vector3 a,Vector3 b,Vector3 point,float radius){Vector3 ab=b-a;float t=Mathf.Clamp01(Vector3.Dot(point-a,ab)/Mathf.Max(.0001f,ab.sqrMagnitude));return (a+ab*t-point).sqrMagnitude<radius*radius;}
        public void Clear(){foreach(var s in shots)Object.Destroy(s.Visual);shots.Clear();foreach(var d in Drones)Object.Destroy(d.Visual);Drones.Clear();}
    }
}
