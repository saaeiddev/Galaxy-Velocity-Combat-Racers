using System.Collections.Generic;
using UnityEngine;
namespace GalaxyVelocity {
    public static class ProceduralArt {
        static readonly Dictionary<string,Material> cache=new Dictionary<string,Material>();
        public static Material Mat(Color c,bool glow=false) {
            string key=c.ToString()+glow;
            if(cache.TryGetValue(key,out var m)&&m)return m;
            m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=c;
            m.SetFloat("_Smoothness",.5f);m.SetFloat("_Metallic",glow?0:.35f);
            if(glow){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",c*3);}
            cache[key]=m;return m;
        }
        public static GameObject Part(string name,Transform parent,Vector3 p,Vector3 scale,Color c,PrimitiveType type=PrimitiveType.Cube,bool glow=false) {
            var o=GameObject.CreatePrimitive(type);o.name=name;o.transform.SetParent(parent,false);o.transform.localPosition=p;o.transform.localScale=scale;
            o.GetComponent<Renderer>().sharedMaterial=Mat(c,glow);if(Application.isPlaying)Object.Destroy(o.GetComponent<Collider>());else Object.DestroyImmediate(o.GetComponent<Collider>());return o;
        }
        public static GameObject Vehicle(int index) {
            var root=new GameObject(Pilot.All[index].Name+" / racer");var t=root.transform;Color c=Pilot.All[index].Color;
            var white=new Color(.82f,.89f,.92f);var dark=new Color(.055f,.08f,.12f);var cyan=new Color(.05f,.8f,1);
            Part("Armored keel",t,Vector3.zero,new Vector3(2.3f,.65f,4.8f),c);
            Part("Forward wedge",t,new Vector3(0,-.05f,2.6f),new Vector3(1.5f,.42f,1.5f),white).transform.localRotation=Quaternion.Euler(0,45,0);
            Part("Cockpit",t,new Vector3(0,.5f,-.4f),new Vector3(1.5f,.7f,1.6f),dark,PrimitiveType.Sphere);
            for(int side=-1;side<=1;side+=2){
                Part("Outrigger",t,new Vector3(side*1.8f,-.1f,-.5f),new Vector3(.7f,.7f,3.7f),white);
                Part("Laser cannon",t,new Vector3(side*1.3f,.35f,.8f),new Vector3(.23f,.3f,2.6f),dark);
                Part("Thruster",t,new Vector3(side*1.8f,0,-2.45f),new Vector3(.5f,.42f,.2f),cyan,PrimitiveType.Cube,true);
                Part("Ion ribbon",t,new Vector3(side*1.8f,-.03f,-3.2f),new Vector3(.23f,.18f,1.5f),cyan,PrimitiveType.Cube,true);
                Part("Energy trim",t,new Vector3(side*1.14f,-.15f,1),new Vector3(.08f,.08f,2),cyan,PrimitiveType.Cube,true);
                Part("Wing",t,new Vector3(side*1.2f,.65f,-1.6f),new Vector3(.12f,1.1f,1.4f),c).transform.localRotation=Quaternion.Euler(-20,0,side*-18);
            }
            Part("Pilot armor",t,new Vector3(0,1,-.65f),new Vector3(.8f,.75f,.6f),c,PrimitiveType.Sphere);
            Color fur=index==0?new Color(.93f,.4f,.1f):index==1?new Color(.08f,.3f,.72f):new Color(.73f,.65f,.87f);
            Part("Pilot head",t,new Vector3(0,1.6f,-.55f),new Vector3(.9f,.85f,.8f),fur,PrimitiveType.Sphere);
            Part(index==1?"Golden beak":"White muzzle",t,new Vector3(0,1.42f,-.04f),new Vector3(.58f,.3f,.4f),index==1?new Color(1,.7f,.1f):white,PrimitiveType.Sphere);
            Part("Visor",t,new Vector3(0,1.73f,-.17f),new Vector3(.8f,.2f,.12f),index==0?Color.green:cyan,PrimitiveType.Cube,true);
            for(int side=-1;side<=1;side+=2)Part("Ear / crest",t,new Vector3(side*.34f,2.07f,-.55f),new Vector3(.2f,.6f,.3f),fur).transform.localRotation=Quaternion.Euler(-12,0,-side*18);
            return root;
        }
    }
}
