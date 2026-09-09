using UnityEngine;
namespace GalaxyVelocity {
    public sealed class GalaxyHUD : MonoBehaviour {
        public GameSession Game;GUIStyle small,title,large,button;Texture2D portrait;int portraitIndex=-1;
        void Init(){if(small!=null)return;small=new GUIStyle(GUI.skin.label){fontSize=16};small.normal.textColor=new Color(.76f,.85f,.92f);title=new GUIStyle(small){fontSize=36,fontStyle=FontStyle.Bold};large=new GUIStyle(title){fontSize=66};button=new GUIStyle(GUI.skin.button){fontSize=18,padding=new RectOffset(16,16,12,12)};}
        void Label(float x,float y,float w,float h,string s,GUIStyle style=null){GUI.Label(new Rect(x,y,w,h),s,style??small);}
        void Panel(float x,float y,float w,float h){GUI.color=new Color(.025f,.055f,.095f,.9f);GUI.DrawTexture(new Rect(x,y,w,h),Texture2D.whiteTexture);GUI.color=Color.white;}
        void Bar(float x,float y,string name,float value,Color c){Label(x,y,220,25,name+"  "+Mathf.CeilToInt(value));GUI.color=new Color(.14f,.19f,.25f);GUI.DrawTexture(new Rect(x,y+27,220,5),Texture2D.whiteTexture);GUI.color=c;GUI.DrawTexture(new Rect(x,y+27,220*value/100,5),Texture2D.whiteTexture);GUI.color=Color.white;}
        void Portrait(int index){if(portraitIndex==index)return;portraitIndex=index;if(portrait)Destroy(portrait);portrait=new Texture2D(64,64);Color[] data=new Color[4096];for(int y=0;y<64;y++)for(int x=0;x<64;x++){
            float dx=(x-32)/23f,dy=(y-30)/24f;Color c=new Color(.03f,.07f,.12f);
            if(dx*dx+dy*dy<1)c=Pilot.All[index].Color;
            if(y>44&&y<61&&((x>13&&x<23)||(x>41&&x<51)))c=Pilot.All[index].Color;
            if(y>29&&y<36&&x>13&&x<51)c=index==0?Color.green:Color.cyan;
            if(y>14&&y<25&&x>22&&x<42)c=index==1?Color.yellow:Color.white;data[y*64+x]=c;
        }portrait.SetPixels(data);portrait.Apply();}
        void OnGUI(){if(!Game||!Game.Player)return;Init();GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(Screen.width/1280f,Screen.height/720f,1));var p=Game.Player;Portrait(p.PilotIndex);
            Label(32,24,650,28,"GV / GALAXY VELOCITY     •     ASTERION CANYON");Label(1000,24,260,28,"COMBAT RACERS / PROTOTYPE");
            if(Game.State==SessionState.Selection){Panel(32,110,530,565);Label(56,132,470,90,"RACE THE EDGE.\nOWN THE GALAXY.",title);Label(56,232,460,30,"SELECT YOUR PILOT");
                for(int i=0;i<3;i++){GUI.backgroundColor=Pilot.All[i].Color;if(GUI.Button(new Rect(56,276+i*65,480,53),(Game.Selected==i?"●  ":"")+Pilot.All[i].Name+"   /   "+Pilot.All[i].Ability,button))Game.Select(i);}GUI.backgroundColor=Color.white;
                Label(56,475,460,60,p.Pilot.Description);Label(56,534,460,28,"SPEED "+p.Pilot.Speed+"   HANDLING "+p.Pilot.Handling+"   3 LAPS / 6 RACERS");if(GUI.Button(new Rect(56,586,480,60),"LAUNCH RACE  →",button))Game.Begin();return;}
            Panel(32,83,220,130);GUI.DrawTexture(new Rect(46,100,64,64),portrait);Label(120,102,135,30,"POSITION");Label(120,123,135,58,Game.Rank(p)+" / 6",title);Label(48,178,190,25,"LAP "+p.Race.Lap+" / 3    CP "+p.Race.NextGate);
            Panel(1000,83,248,100);Label(1020,100,210,25,"RACE TIME");Label(1020,126,230,45,System.TimeSpan.FromSeconds(Game.RaceTime).ToString(@"mm\:ss\.ff"),title);
            Panel(32,535,265,150);Bar(52,550,"HULL",p.Health,new Color(1,.35f,.14f));Bar(52,602,"SHIELD",p.Shield,Color.cyan);
            Panel(1000,478,248,207);Label(1018,486,220,85,Mathf.RoundToInt(p.Speed*3.6f).ToString(),large);Label(1018,561,210,25,"KM/H     "+(p.Drifting?"DRIFT":p.Height>0?"AIRBORNE":"ANTIGRAV"));Bar(1014,606,"BOOST",p.Boost,Color.cyan);
            Panel(440,600,440,85);Label(460,610,420,25,"J / LASER    K / MISSILE × "+p.Missiles+"    KILLS "+p.Kills);Label(460,646,420,25,"E / "+p.Pilot.Ability+"  "+(p.AbilityCooldown<=0?"READY":Mathf.CeilToInt(p.AbilityCooldown)+"s"));
            Label(410,695,620,25,"WASD / DRIVE   SHIFT / BOOST   SPACE / DRIFT   ESC / PAUSE");
            if(Game.MessageTime>0)Label(400,130,700,40,Game.Message,title);
            if(Game.State==SessionState.Countdown)Label(598,270,300,110,Mathf.CeilToInt(Game.Countdown).ToString(),large);
            if(Game.State==SessionState.Paused||Game.State==SessionState.Finished){Panel(360,205,560,330);Label(400,230,500,70,Game.State==SessionState.Paused?"RACE PAUSED":"FINISH / P"+Game.Rank(p),title);Label(400,300,500,35,"TIME "+Game.RaceTime.ToString("F2")+"s     DRONE / RACER KILLS "+p.Kills);
                if(GUI.Button(new Rect(400,355,480,55),Game.State==SessionState.Paused?"RESUME":"RACE AGAIN",button)){if(Game.State==SessionState.Paused)Game.TogglePause();else Game.Begin();}
                if(GUI.Button(new Rect(400,430,480,55),"PILOT SELECT",button))Game.Menu();}
        }
    }
}
