using UnityEngine;
namespace GalaxyVelocity {
    public sealed class ChaseCamera : MonoBehaviour {
        public GameSession Game;Camera lens;
        void Awake(){lens=GetComponent<Camera>();}
        void LateUpdate(){
            if(!Game||!Game.Player)return;var p=Game.Player;float dt=Time.unscaledDeltaTime;
            Vector3 forward=CanyonTrack.Forward(p.Race.Progress);
            Vector3 desired=p.transform.position-forward*(p.Boosting?16:13)+Vector3.up*6;
            if(Game.State==SessionState.Selection)desired=p.transform.position+forward*10+CanyonTrack.Right(p.Race.Progress)*9+Vector3.up*5;
            transform.position=Vector3.Lerp(transform.position,desired,1-Mathf.Exp(-dt*6));
            transform.LookAt(p.transform.position+forward*(Game.State==SessionState.Selection?0:12)+Vector3.up);
            lens.fieldOfView=Mathf.Lerp(lens.fieldOfView,p.Boosting?78:64,dt*4);
        }
    }
}
