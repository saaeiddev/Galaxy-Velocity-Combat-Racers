using UnityEngine;
namespace GalaxyVelocity {
    public sealed class Racer : MonoBehaviour {
        public Pilot Pilot; public int PilotIndex; public bool IsPlayer;
        public RaceRules Race=new RaceRules();
        public float Speed,Lane,Lateral,Height,Vertical,Boost=100,Health=100,Shield=100;
        public float AbilityCooldown,AbilityTime,Invulnerable,FireCooldown,MissileCooldown,FinishTime=-1;
        public int Missiles=3, Kills; public bool Boosting,Drifting;
        float damageAge=10, aiSeed; GameSession game;
        public void Setup(GameSession session,int pilot,bool player,int grid) {
            game=session; PilotIndex=pilot; Pilot=Pilot.All[pilot];IsPlayer=player;
            aiSeed=grid*1.7f;Lane=(grid%3-1)*7; Race.Advance(grid*9); Place();
        }
        public void Tick(float dt) {
            if(Race.Finished) { if(FinishTime<0)FinishTime=game.RaceTime; return; }
            float steer=IsPlayer?((Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)?1:0)):Mathf.Clamp((Mathf.Sin(game.RaceTime*.6f+aiSeed)*8-Lane)*.35f,-1,1);
            bool throttle=!IsPlayer||Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow);
            bool brake=IsPlayer&&(Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow));
            Drifting=IsPlayer&&Input.GetKey(KeyCode.Space)&&Mathf.Abs(steer)>.1f&&Speed>30&&Height<=.05f;
            Boosting=(IsPlayer?Input.GetKey(KeyCode.LeftShift):Mathf.Sin(game.RaceTime+aiSeed)>.85f)&&Boost>1&&throttle;
            Boost=Mathf.Clamp(Boost+(Boosting?-29:15)*dt,0,100);
            float max=Pilot.Speed*(Boosting?1.52f:1)*(PilotIndex==1&&AbilityTime>0?1.25f:1);
            Speed=Mathf.MoveTowards(Speed,brake?0:throttle?max:0,dt*(brake?100:throttle?36:18));
            if(Drifting)Speed=Mathf.Max(0,Speed-10*dt);
            float target=steer*Pilot.Handling*(Drifting?1.5f:1)*(Height>0?.7f:1);
            Lateral=Mathf.Lerp(Lateral,target,1-Mathf.Exp(-dt*(Drifting?3:9)));
            Lane+=Lateral*dt;
            if(Mathf.Abs(Lane)>CanyonTrack.Width/2-1) {
                Lane=Mathf.Sign(Lane)*(CanyonTrack.Width/2-1);Lateral*=-.3f;Speed*=1-1.5f*dt;Damage(16*dt);
            }
            float before=Race.Progress;Race.Advance(Speed*dt);
            foreach(float pad in game.BoostPads) if(CanyonTrack.Crossed(before,Race.Progress,pad)&&Mathf.Abs(Lane)<6) {Speed=Mathf.Max(Speed,Pilot.Speed*1.65f);Boost=Mathf.Min(100,Boost+22);}
            foreach(float ramp in game.Ramps) if(CanyonTrack.Crossed(before,Race.Progress,ramp)&&Height<=.1f)Vertical=17;
            if(Height>0||Vertical>0) {Vertical-=27*dt;Height=Mathf.Max(0,Height+Vertical*dt);if(Height<=0)Vertical=0;}
            foreach(float barrier in game.Barriers)if(CanyonTrack.Crossed(before,Race.Progress,barrier)&&Lane>2&&Height<3){Damage(24);Speed*=.6f;}
            FireCooldown-=dt;MissileCooldown-=dt;AbilityCooldown=Mathf.Max(0,AbilityCooldown-dt);AbilityTime-=dt;Invulnerable-=dt;damageAge+=dt;
            if(damageAge>4)Shield=Mathf.Min(100,Shield+9*dt);
            if((IsPlayer?(Input.GetMouseButton(0)||Input.GetKey(KeyCode.J)):Mathf.Sin(game.RaceTime*2+aiSeed)>.4f)&&FireCooldown<=0)Fire(false);
            if(IsPlayer&&(Input.GetMouseButtonDown(1)||Input.GetKeyDown(KeyCode.K)))Fire(true);
            if(IsPlayer&&Input.GetKeyDown(KeyCode.E))Ability();
            if(IsPlayer&&Input.GetKeyDown(KeyCode.R))Recover();
            Place();
        }
        public void Place() {
            transform.position=CanyonTrack.Position(Race.Progress,Lane,1.5f+Height);
            transform.rotation=Quaternion.LookRotation(CanyonTrack.Forward(Race.Progress))*Quaternion.Euler(-Vertical*.5f,Lateral*.7f,-Lateral*(Drifting?1.5f:.7f));
        }
        public void Fire(bool missile) {
            if(missile){if(Missiles<=0||MissileCooldown>0)return;Missiles--;MissileCooldown=.65f;}
            else {if(FireCooldown>0)return;FireCooldown=Pilot.FireInterval*(PilotIndex==1&&AbilityTime>0?.55f:1);}
            game.Combat.Fire(this,missile);if(IsPlayer)game.Audio.Shot(missile);
        }
        public void Ability() {
            if(AbilityCooldown>0)return;AbilityCooldown=14;
            if(PilotIndex==0){Shield=Mathf.Min(100,Shield+45);Invulnerable=3;AbilityTime=3;}
            else if(PilotIndex==1)AbilityTime=5;
            else {Boost=100;Invulnerable=4;AbilityTime=4;}
            if(IsPlayer)game.Notify(Pilot.Ability);
        }
        public bool Damage(float amount) {
            if(Invulnerable>0||Race.Finished)return false;
            damageAge=0;float absorb=Mathf.Min(Shield,amount);Shield-=absorb;Health-=amount-absorb;
            if(Health<=0){Recover();return true;}return false;
        }
        public void Recover() {
            Health=100;Shield=60;Speed=0;Lane=0;Height=0;Vertical=0;Invulnerable=3;Race.Recover();
            if(IsPlayer)game.Notify("RECOVERED AT LAST CHECKPOINT");Place();
        }
    }
}
