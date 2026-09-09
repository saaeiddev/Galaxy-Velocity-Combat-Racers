using UnityEngine;
namespace GalaxyVelocity {
    public sealed class GameAudio : MonoBehaviour {
        AudioSource engine,effects;AudioClip laser,missile;public GameSession Game;
        void Awake(){engine=gameObject.AddComponent<AudioSource>();effects=gameObject.AddComponent<AudioSource>();engine.clip=Tone(70,1,.25f);engine.loop=true;engine.volume=.05f;engine.Play();laser=Tone(740,.13f,.4f);missile=Tone(130,.35f,.6f);}
        static AudioClip Tone(float hz,float seconds,float level){int count=(int)(22050*seconds);float[] data=new float[count];for(int i=0;i<count;i++){float t=(float)i/count;data[i]=Mathf.Sin(i*hz*Mathf.PI*2/22050*(1-.3f*t))*level*(1-t);}var c=AudioClip.Create("Synthesized original SFX",count,1,22050,false);c.SetData(data,0);return c;}
        public void Shot(bool heavy){effects.PlayOneShot(heavy?missile:laser,.15f);}
        void Update(){if(Game&&Game.Player){engine.pitch=.5f+Game.Player.Speed/65;engine.volume=Game.State==SessionState.Racing?.06f:0;}}
    }
}
