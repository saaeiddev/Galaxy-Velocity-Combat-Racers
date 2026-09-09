using System.Collections.Generic;
using UnityEngine;
namespace GalaxyVelocity {
    public sealed class PickupSystem {
        sealed class Pickup {public float S,Lane,Timer;public int Type;public GameObject Visual;}
        readonly List<Pickup> items=new List<Pickup>();readonly GameSession game;
        public PickupSystem(GameSession g){game=g;for(int i=0;i<12;i++){
            int type=i%3;float s=100+i*140,lane=(i%3-1)*7;
            var o=ProceduralArt.Part(type==0?"Missile pickup":type==1?"Shield pickup":"Repair pickup",null,CanyonTrack.Position(s,lane,2),Vector3.one*1.8f,type==0?Color.cyan:type==1?new Color(.7f,.2f,1):Color.green,PrimitiveType.Cube,true);
            items.Add(new Pickup{S=s,Lane=lane,Type=type,Visual=o});
        }}
        public void Tick(float dt){foreach(var item in items){
            item.Timer-=dt;item.Visual.SetActive(item.Timer<=0);if(item.Timer>0)continue;
            item.Visual.transform.Rotate(30*dt,70*dt,0);
            foreach(var r in game.Racers){float delta=Mathf.Abs(CanyonTrack.Loop(r.Race.Progress)-item.S);
                if(delta<5&&Mathf.Abs(r.Lane-item.Lane)<3&&r.Height<4){
                    if(item.Type==0)r.Missiles=Mathf.Min(9,r.Missiles+3);else if(item.Type==1)r.Shield=Mathf.Min(100,r.Shield+40);else r.Health=Mathf.Min(100,r.Health+35);
                    item.Timer=8;if(r.IsPlayer)game.Notify(item.Type==0?"+3 MISSILES":item.Type==1?"SHIELD RESTORED":"HULL REPAIRED");break;
                }
            }
        }}
        public void Clear(){foreach(var p in items)Object.Destroy(p.Visual);items.Clear();}
    }
}
