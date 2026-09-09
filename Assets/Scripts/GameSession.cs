using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace GalaxyVelocity {
    public enum SessionState {Selection,Countdown,Racing,Paused,Finished}
    public sealed class GameSession : MonoBehaviour {
        public SessionState State=SessionState.Selection;public Racer Player;public readonly List<Racer> Racers=new List<Racer>();
        public CombatSystem Combat;public PickupSystem Pickups;public GameAudio Audio;public float RaceTime,Countdown,MessageTime;public string Message="";
        public int Selected;public readonly float[] BoostPads={80,380,820,1260,1620},Ramps={320,1080},Barriers={620,1430};
        SessionState beforePause;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Boot(){if(!FindFirstObjectByType<GameSession>())new GameObject("Galaxy Velocity / systems").AddComponent<GameSession>();}
        void Start(){
            Application.targetFrameRate=120;QualitySettings.vSyncCount=1;
            new GameObject("Asterion Canyon").AddComponent<CanyonWorld>().Build(this);
            var cam=Camera.main;if(!cam)cam=new GameObject("Main Camera").AddComponent<Camera>();cam.tag="MainCamera";cam.nearClipPlane=.1f;cam.farClipPlane=2400;cam.backgroundColor=new Color(.36f,.55f,.76f);cam.clearFlags=CameraClearFlags.SolidColor;
            if(!cam.GetComponent<AudioListener>())cam.gameObject.AddComponent<AudioListener>();cam.GetUniversalAdditionalCameraData().renderPostProcessing=true;cam.gameObject.AddComponent<ChaseCamera>().Game=this;
            Audio=gameObject.AddComponent<GameAudio>();Audio.Game=this;gameObject.AddComponent<GalaxyHUD>().Game=this;Select(0);cam.transform.position=Player.transform.position+Vector3.up*8;
        }
        public void Select(int index){Selected=index;if(Player)Destroy(Player.gameObject);var o=ProceduralArt.Vehicle(index);Player=o.AddComponent<Racer>();Player.Setup(this,index,true,0);}
        public void Begin(){
            Combat?.Clear();Pickups?.Clear();foreach(var r in Racers)if(r&&r!=Player)Destroy(r.gameObject);Racers.Clear();Select(Selected);Racers.Add(Player);
            for(int i=1;i<6;i++){var r=ProceduralArt.Vehicle(i%3).AddComponent<Racer>();r.Setup(this,i%3,false,i);Racers.Add(r);}
            Combat=new CombatSystem(this);Pickups=new PickupSystem(this);RaceTime=0;Countdown=3;State=SessionState.Countdown;Notify("ALL SYSTEMS ONLINE");
        }
        void Update(){
            float dt=Mathf.Min(Time.deltaTime,.05f);MessageTime-=dt;
            if(Input.GetKeyDown(KeyCode.Escape))TogglePause();
            if(State==SessionState.Countdown){Countdown-=dt;if(Countdown<=0)State=SessionState.Racing;return;}
            if(State!=SessionState.Racing)return;RaceTime+=dt;
            foreach(var r in Racers)r.Tick(dt);Combat.Tick(dt);Pickups.Tick(dt);
            if(Player.Race.Finished){Player.FinishTime=RaceTime;State=SessionState.Finished;Notify("RACE COMPLETE");}
        }
        public void TogglePause(){if(State==SessionState.Racing||State==SessionState.Countdown){beforePause=State;State=SessionState.Paused;}else if(State==SessionState.Paused)State=beforePause;}
        public int Rank(Racer racer){int rank=1;foreach(var r in Racers)if(r!=racer&&(r.Race.Progress>racer.Race.Progress||(r.Race.Finished&&r.FinishTime>=0&&(racer.FinishTime<0||r.FinishTime<racer.FinishTime))))rank++;return rank;}
        public void Menu(){Combat?.Clear();Pickups?.Clear();foreach(var r in Racers)if(r&&r!=Player)Destroy(r.gameObject);Racers.Clear();State=SessionState.Selection;Select(Selected);}
        public void Notify(string text){Message=text;MessageTime=2.4f;}
        void OnApplicationFocus(bool focus){if(!focus&&(State==SessionState.Racing||State==SessionState.Countdown))TogglePause();}
    }
}
