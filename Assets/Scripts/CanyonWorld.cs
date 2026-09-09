using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace GalaxyVelocity {
    public sealed class CanyonWorld : MonoBehaviour {
        public void Build(GameSession game) {
            Random.InitState(9317);
            RenderSettings.ambientLight=new Color(.4f,.48f,.61f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.49f,.55f,.64f);RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=.0018f;
            var sun=new GameObject("Binary sun / key").AddComponent<Light>();sun.type=LightType.Directional;sun.color=new Color(1,.79f,.59f);sun.intensity=2;sun.shadows=LightShadows.Soft;sun.transform.rotation=Quaternion.Euler(35,-40,0);
            var volume=new GameObject("Cinematic volume").AddComponent<Volume>();volume.isGlobal=true;volume.profile=ScriptableObject.CreateInstance<VolumeProfile>();
            var bloom=volume.profile.Add<Bloom>();bloom.intensity.Override(.45f);bloom.threshold.Override(1);
            var tone=volume.profile.Add<Tonemapping>();tone.mode.Override(TonemappingMode.ACES);
            Color road=new Color(.1f,.14f,.18f),cyan=new Color(.03f,.75f,1),stone=new Color(.49f,.27f,.18f);
            for(int i=0;i<360;i++) {
                float s=i*5;Vector3 a=CanyonTrack.Point(s),b=CanyonTrack.Point(s+5);float length=Vector3.Distance(a,b)+.2f;
                var deck=ProceduralArt.Part("Circuit deck",transform,(a+b)/2,new Vector3(26,.7f,length),road);deck.transform.rotation=Quaternion.LookRotation(b-a);
                foreach(int side in new[]{-1,1}){
                    var rail=ProceduralArt.Part("Energy guard",transform,CanyonTrack.Position(s,side*13,1),new Vector3(.18f,.2f,length),cyan,PrimitiveType.Cube,true);rail.transform.rotation=deck.transform.rotation;
                }
                if(i%4==0){var mark=ProceduralArt.Part("Lane marker",transform,CanyonTrack.Position(s,0,.4f),new Vector3(.25f,.03f,2),new Color(.65f,.72f,.78f));mark.transform.rotation=deck.transform.rotation;}
                if(i%12==0)ProceduralArt.Part("Bridge pier",transform,a-Vector3.up*35,new Vector3(7,70,7),new Color(.25f,.28f,.3f));
            }
            for(int i=0;i<150;i++) {
                float s=Random.Range(0,1800);float lane=(Random.value>.5f?1:-1)*Random.Range(35,170);float h=Random.Range(30,125);
                var rock=ProceduralArt.Part("Basalt spire",transform,CanyonTrack.Position(s,lane,-38+h*.5f),new Vector3(Random.Range(18,45),h,Random.Range(20,50)),stone*Random.Range(.7f,1.3f));rock.transform.rotation=Quaternion.Euler(Random.Range(-9,9),Random.Range(0,180),Random.Range(-9,9));
            }
            for(int i=0;i<35;i++){
                float h=Random.Range(55,190);var p=new Vector3(Random.Range(-130,130),h/2-40,Random.Range(-130,130));
                ProceduralArt.Part("Citadel",transform,p,new Vector3(12,h,12),new Color(.23f,.31f,.39f));
                ProceduralArt.Part("Citadel beacon",transform,p+Vector3.up*h*.4f,new Vector3(12.2f,.7f,12.2f),cyan,PrimitiveType.Cube,true);
            }
            ProceduralArt.Part("Alien planet",transform,new Vector3(-500,470,-900),Vector3.one*370,new Color(.46f,.55f,.77f),PrimitiveType.Sphere);
            ProceduralArt.Part("Moon",transform,new Vector3(-260,540,-850),Vector3.one*80,new Color(.73f,.78f,.82f),PrimitiveType.Sphere);
            for(int i=0;i<4;i++)Gate(i*450,i==0?"FINISH / GALAXY VELOCITY":"CHECKPOINT 0"+i,cyan);
            foreach(float pad in game.BoostPads){var p=ProceduralArt.Part("Boost pad",transform,CanyonTrack.Position(pad,0,.42f),new Vector3(10,.06f,11),new Color(1,.42f,.06f),PrimitiveType.Cube,true);p.transform.rotation=Quaternion.LookRotation(CanyonTrack.Forward(pad));}
            foreach(float ramp in game.Ramps){var p=ProceduralArt.Part("Launch ramp",transform,CanyonTrack.Position(ramp-4,0,1),new Vector3(25,.5f,10),new Color(.25f,.32f,.38f));p.transform.rotation=Quaternion.LookRotation(CanyonTrack.Forward(ramp))*Quaternion.Euler(-12,0,0);}
            foreach(float b in game.Barriers){var p=ProceduralArt.Part("Energy barrier / avoid right lane",transform,CanyonTrack.Position(b,7,2),new Vector3(9,3,.3f),new Color(1,.15f,.1f),PrimitiveType.Cube,true);p.transform.rotation=Quaternion.LookRotation(CanyonTrack.Forward(b));}
            for(int i=0;i<7;i++)ProceduralArt.Part("Floating relay platform",transform,new Vector3(350+i*13,80+i*6,-100+i*26),new Vector3(22,3,22),road);
        }
        void Gate(float s,string text,Color color){
            var root=new GameObject(text);root.transform.position=CanyonTrack.Point(s);root.transform.rotation=Quaternion.LookRotation(CanyonTrack.Forward(s));root.transform.parent=transform;
            foreach(int side in new[]{-1,1})ProceduralArt.Part("Gate tower",root.transform,new Vector3(side*14,6,0),new Vector3(1,13,1),color,PrimitiveType.Cube,true);
            ProceduralArt.Part("Gate header",root.transform,new Vector3(0,12,0),new Vector3(29,2,1),new Color(.07f,.1f,.16f));
            var label=new GameObject("Label").AddComponent<TextMesh>();label.transform.SetParent(root.transform,false);label.transform.localPosition=new Vector3(0,12,-.6f);label.transform.localRotation=Quaternion.Euler(0,180,0);label.text=text;label.fontSize=50;label.characterSize=.15f;label.anchor=TextAnchor.MiddleCenter;label.color=Color.white;
        }
    }
}
