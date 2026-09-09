/* Dependency-free WebGL 3D renderer. All geometry is authored procedurally. */
(function(){'use strict';
const {add,sub,mul,dot,norm,cross,position,point,forward,right,pilots}=GV;
const I=()=>new Float32Array([1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1]);
function perspective(fov,aspect,near,far){const f=1/Math.tan(fov/2),m=new Float32Array(16);m[0]=f/aspect;m[5]=f;m[10]=(far+near)/(near-far);m[11]=-1;m[14]=2*far*near/(near-far);return m}
function view(eye,target){const z=norm(sub(eye,target)),x=norm(cross([0,1,0],z)),y=cross(z,x);return new Float32Array([x[0],y[0],z[0],0,x[1],y[1],z[1],0,x[2],y[2],z[2],0,-dot(x,eye),-dot(y,eye),-dot(z,eye),1])}
function model(p,yaw=0,roll=0,scale=1){const c=Math.cos(yaw),s=Math.sin(yaw),a=Math.cos(roll),b=Math.sin(roll);return new Float32Array([c*a*scale,b*scale,-s*a*scale,0,-c*b*scale,a*scale,s*b*scale,0,s*scale,0,c*scale,0,...p,1])}
class Mesh{
 constructor(){this.data=[]}
 tri(a,b,c,color,em=0){const n=norm(cross(sub(b,a),sub(c,a)));for(const p of [a,b,c])this.data.push(...p,...n,...color,em)}
 quad(a,b,c,d,color,em=0){this.tri(a,b,c,color,em);this.tri(a,c,d,color,em)}
 box(p,size,color,em=0,yaw=0){const [x,y,z]=size.map(v=>v/2),co=Math.cos(yaw),si=Math.sin(yaw),pts=[[-x,-y,-z],[x,-y,-z],[x,y,-z],[-x,y,-z],[-x,-y,z],[x,-y,z],[x,y,z],[-x,y,z]].map(q=>add(p,[q[0]*co+q[2]*si,q[1],-q[0]*si+q[2]*co]));for(const f of [[0,3,2,1],[4,5,6,7],[0,4,7,3],[1,2,6,5],[3,7,6,2],[0,1,5,4]])this.quad(...f.map(i=>pts[i]),color,em)}
 sphere(p,size,color,rows=8,cols=12,em=0){const pt=(i,j)=>{let a=i/rows*Math.PI,b=j/cols*Math.PI*2;return add(p,[Math.sin(a)*Math.cos(b)*size[0]/2,Math.cos(a)*size[1]/2,Math.sin(a)*Math.sin(b)*size[2]/2])};for(let i=0;i<rows;i++)for(let j=0;j<cols;j++)this.quad(pt(i,j),pt(i+1,j),pt(i+1,j+1),pt(i,j+1),color,em)}
 spire(p,r,h,color,seed){const n=7,lower=[],upper=[];for(let i=0;i<n;i++){let a=i/n*Math.PI*2;lower.push(add(p,[Math.cos(a)*r,0,Math.sin(a)*r]));upper.push(add(p,[Math.cos(a+.3)*r*.65+seed,h*(.88+.12*Math.sin(i*3+seed)),Math.sin(a+.3)*r*.65]))}for(let i=0;i<n;i++)this.quad(lower[i],lower[(i+1)%n],upper[(i+1)%n],upper[i],color);for(let i=1;i<n-1;i++)this.tri(upper[0],upper[i],upper[i+1],color.map(v=>v*1.12))}
 wedge(p,w,h,l,c){let v=[[-w,-h,-l],[w,-h,-l],[w,-h,l],[-w,-h,l],[-w,h,-l],[w,h,-l],[w*.38,h*.1,l],[-w*.38,h*.1,l]].map(q=>add(p,q));for(const f of [[0,3,2,1],[4,5,6,7],[0,1,5,4],[3,7,6,2],[0,4,7,3],[1,2,6,5]])this.quad(...f.map(i=>v[i]),c)}
}
const cyan=[.1,.78,1],white=[.86,.9,.92],dark=[.06,.09,.14];
function vehicle(index){const m=new Mesh,c=pilots[index].color;m.wedge([0,0,.5],1.15,.38,2.7,c);m.wedge([0,.15,1.6],.34,.29,1.4,white);m.sphere([0,.6,-.5],[1.5,1,1.7],dark);
 for(const side of [-1,1]){m.wedge([side*1.9,-.1,-.4],.38,.35,2,white);m.box([side*1.9,.28,-.4],[.2,.05,2.5],c);m.box([side*1.9,0,-2.45],[.54,.44,.15],cyan,1);m.box([side*1.9,-.03,-3.2],[.24,.16,1.6],cyan,1);m.box([side*1.1,.38,.7],[.21,.23,2.7],dark);m.box([side*1.1,.38,2.05],[.18,.15,.12],cyan,1);m.box([side*1.08,-.13,1.55],[.07,.08,1.8],cyan,1);m.wedge([side*1.23,.8,-1.8],.08,.7,.8,c)}
 m.sphere([0,1,-.7],[.85,.75,.68],c);const fur=index===0?[.94,.42,.1]:index===1?[.08,.3,.67]:[.74,.65,.85];m.sphere([0,1.62,-.5],[.88,.85,.82],fur);m.sphere([0,1.43,-.04],[.59,.32,.4],index===1?[1,.68,.1]:white,5,8);m.box([0,1.75,-.14],[.83,.18,.17],index===0?[.3,1,.2]:cyan,.6);
 for(const side of [-1,1]){m.wedge([side*.32,2.04,-.5],.15,.38,.19,fur);m.box([side*.47,1.66,-.5],[.15,.4,.45],dark)}return m}
class Renderer{
 constructor(canvas){this.canvas=canvas;const gl=canvas.getContext('webgl',{antialias:true,alpha:false,preserveDrawingBuffer:true});if(!gl)throw new Error('WebGL is unavailable. Enable hardware acceleration in your browser.');this.gl=gl;
 const vs=`attribute vec3 aPos;attribute vec3 aNormal;attribute vec3 aColor;attribute float aGlow;uniform mat4 uModel,uView,uProjection;varying vec3 vColor;varying float vFog;void main(){vec4 world=uModel*vec4(aPos,1.0);vec4 camera=uView*world;vec3 n=normalize(mat3(uModel)*aNormal);float diffuse=max(dot(n,normalize(vec3(-0.5,0.85,0.3))),0.0);vec3 light=vec3(.4,.48,.6)+vec3(.82,.72,.57)*diffuse;vColor=mix(aColor*light,aColor*1.6,aGlow);vFog=1.0-exp(-pow(length(camera.xyz)*.0018,2.0));gl_Position=uProjection*camera;}`;
 const fs=`precision mediump float;varying vec3 vColor;varying float vFog;void main(){gl_FragColor=vec4(mix(vColor,vec3(.49,.61,.72),clamp(vFog,0.0,.95)),1.0);}`;
 const shader=(type,src)=>{const s=gl.createShader(type);gl.shaderSource(s,src);gl.compileShader(s);if(!gl.getShaderParameter(s,gl.COMPILE_STATUS))throw new Error(gl.getShaderInfoLog(s));return s};
 this.program=gl.createProgram();gl.attachShader(this.program,shader(gl.VERTEX_SHADER,vs));gl.attachShader(this.program,shader(gl.FRAGMENT_SHADER,fs));gl.linkProgram(this.program);if(!gl.getProgramParameter(this.program,gl.LINK_STATUS))throw new Error(gl.getProgramInfoLog(this.program));gl.useProgram(this.program);
 this.loc={};for(const name of ['uModel','uView','uProjection'])this.loc[name]=gl.getUniformLocation(this.program,name);for(const name of ['aPos','aNormal','aColor','aGlow'])this.loc[name]=gl.getAttribLocation(this.program,name);
 gl.enable(gl.DEPTH_TEST);gl.disable(gl.CULL_FACE);gl.clearColor(.38,.57,.75,1);
 this.world=this.upload(this.buildWorld());this.vehicles=[0,1,2].map(i=>this.upload(vehicle(i)));const drone=new Mesh;drone.sphere([0,0,0],[2,1.2,2],dark);drone.box([0,0,0],[5,.2,1],[.29,.32,.36]);drone.box([0,0,-1],[.8,.3,.18],[1,.12,.06],1);this.drone=this.upload(drone);
 this.pickups=[cyan,[.75,.22,1],[.2,1,.5]].map(c=>{const m=new Mesh;m.box([0,0,0],[1.7,1.7,1.7],dark);for(let s of [-1,1]){m.box([s*.85,0,0],[.08,1.8,1.8],c,1);m.box([0,s*.85,0],[1.8,.08,1.8],c,1)}m.box([0,0,-.9],[.8,.8,.08],c,1);return this.upload(m)});
 this.bolts=[cyan,[1,.18,.05],[1,.65,.05]].map(c=>{const m=new Mesh;m.box([0,0,0],[.16,.16,3],c,1);return this.upload(m)});const f=new Mesh;f.box([0,0,0],[.35,.35,.35],[1,.65,.15],1);this.particle=this.upload(f);this.eye=[0,35,270];this.target=[0,25,250];this.frames=0;
 }
 upload(mesh){const gl=this.gl,buffer=gl.createBuffer();gl.bindBuffer(gl.ARRAY_BUFFER,buffer);gl.bufferData(gl.ARRAY_BUFFER,new Float32Array(mesh.data),gl.STATIC_DRAW);return {buffer,count:mesh.data.length/10}}
 draw(mesh,m){const gl=this.gl;gl.bindBuffer(gl.ARRAY_BUFFER,mesh.buffer);for(const [name,size,offset] of [['aPos',3,0],['aNormal',3,12],['aColor',3,24],['aGlow',1,36]]){gl.enableVertexAttribArray(this.loc[name]);gl.vertexAttribPointer(this.loc[name],size,gl.FLOAT,false,40,offset)}gl.uniformMatrix4fv(this.loc.uModel,false,m);gl.drawArrays(gl.TRIANGLES,0,mesh.count)}
 buildWorld(){const m=new Mesh;let seed=9317;const rand=()=>{seed=(Math.imul(seed,1664525)+1013904223)>>>0;return seed/4294967296};
 m.box([0,-66,0],[2400,12,2400],[.43,.3,.22]);
 for(let i=0;i<360;i++){let s=i*5,a=point(s),b=point(s+5),f=forward(s),yaw=Math.atan2(f[0],f[2]);let ra=right(s),rb=right(s+5);const v=[add(a,mul(ra,-13)),add(a,mul(ra,13)),add(b,mul(rb,13)),add(b,mul(rb,-13))];m.quad(...v,[.12,.17,.21]);
  for(const side of [-1,1]){m.box(position(s,side*13,.6),[.2,.2,GV.len(sub(b,a))+.2],cyan,.9,yaw);if(i%6===0)m.box(position(s,side*13,-1),[.4,3,.4],[.34,.38,.4])}
  if(i%3===0)for(const lane of [-4.3,4.3])m.box(position(s,lane,.04),[.13,.04,2.5],[.63,.7,.72],0,yaw);
  if(i%12===0){m.box(position(s,0,-40),[5,80,5],[.25,.27,.3]);m.box(position(s,0,-2),[28,3,3],[.29,.32,.34],0,yaw)}
 }
 for(let i=0;i<190;i++){const s=rand()*1800,lane=(rand()>.5?1:-1)*(38+rand()*170),h=35+rand()*120;const c=[.48+rand()*.14,.29+rand()*.08,.2+rand()*.08];m.spire(position(s,lane,-70),14+rand()*22,h,c,rand()*8)}
 for(let i=0;i<46;i++){let h=55+rand()*165,p=[(rand()-.5)*245,h/2-40,(rand()-.5)*225],w=7+rand()*9;m.box(p,[w,h,w],[.25,.32,.38]);m.box(add(p,[0,h/2+5,0]),[w*.4,12,w*.4],[.36,.43,.47]);for(let j=0;j<3;j++)m.box(add(p,[0,h*.2+j*9,0]),[w+.15,.5,w+.15],cyan,.8)}
 // Landmark gateway beyond the city.
 for(let i=0;i<70;i++){let t=i/69*Math.PI,p=[Math.cos(t)*175,Math.sin(t)*175+20,-270];m.box(p,[12,12,13],[.4,.47,.52],0,-t);if(i%3===0)m.box(add(p,[0,0,7]),[5,5,.2],cyan,1)}
 m.sphere([-420,440,-800],[410,410,410],[.65,.68,.81],24,40);m.sphere([-205,490,-720],[74,74,74],[.78,.8,.85],12,20);
 for(let i=0;i<4;i++){const s=i*450,f=forward(s),yaw=Math.atan2(f[0],f[2]);for(const side of [-1,1])m.box(position(s,side*14,6),[1,13,1],[.34,.4,.45],0,yaw);m.box(position(s,0,12),[29,2.2,1.2],dark,0,yaw);m.box(position(s,0,10.8),[27,.16,1.3],i?cyan:[1,.51,.2],1,yaw);for(let j=-12;j<13;j+=2)m.box(position(s,j,12),[1,.7,1.3],i?cyan:white,.6,yaw);if(i===0)for(let x=-12;x<13;x+=2)for(let z=0;z<3;z++)m.box(position(s+z*1.1,x,.07),[1.8,.06,1],(x/2+z)%2?white:dark)}
 for(const s of [80,380,820,1260,1620]){const f=forward(s),yaw=Math.atan2(f[0],f[2]);m.box(position(s,0,.08),[10,.05,12],[.35,.2,.09],0,yaw);for(let z=-4;z<=4;z+=3)for(let side of [-1,1])m.box(position(s+z,side*1.5,.14),[.3,.08,4],[1,.57,.12],1,yaw+side*.75)}
 for(const s of [320,1080]){let a=position(s-12,-13,.1),b=position(s-12,13,.1),c=position(s,13,2),d=position(s,-13,2);m.quad(a,b,c,d,[.36,.42,.45]);for(const side of [-1,1])m.box(position(s,side*12,2.1),[.2,.15,2],cyan,1)}
 for(const s of [620,1430]){const f=forward(s),yaw=Math.atan2(f[0],f[2]);m.box(position(s,7,2),[9,3,.18],[.7,.13,.06],.6,yaw);for(const lane of [2.5,11.5])m.box(position(s,lane,2),[.3,4,.6],[1,.45,.1],1,yaw)}
 for(let i=0;i<8;i++){let p=[355+i*14,70+i*7,-160+i*35];m.box(p,[22,3,22],dark);m.box(add(p,[0,1.7,0]),[23,.25,23],cyan,.5)}return m;
 }
 render(game,dt){const gl=this.gl,canvas=this.canvas,dpr=Math.min(window.devicePixelRatio||1,1.5),w=Math.round(canvas.clientWidth*dpr),h=Math.round(canvas.clientHeight*dpr);if(canvas.width!==w||canvas.height!==h){canvas.width=w;canvas.height=h;gl.viewport(0,0,w,h)}
 const p=game.player,pos=game.pos(p),f=forward(p.race.progress),r=right(p.race.progress),selection=game.state==='selection';
 let eye=selection?add(add(pos,mul(f,10)),add(mul(r,10),[0,6,0])):add(add(pos,mul(f,p.boosting?-17:-14)),[0,6.7,0]);
 let target=selection?add(pos,mul(r,-3.8)):add(add(pos,mul(f,16)),[0,1.5,0]);
 const smooth=1-Math.exp(-dt*7);this.eye=this.eye.map((v,i)=>v+(eye[i]-v)*smooth);this.target=this.target.map((v,i)=>v+(target[i]-v)*smooth);
 gl.clear(gl.COLOR_BUFFER_BIT|gl.DEPTH_BUFFER_BIT);gl.uniformMatrix4fv(this.loc.uView,false,view(this.eye,this.target));gl.uniformMatrix4fv(this.loc.uProjection,false,perspective((p.boosting?77:65)*Math.PI/180,w/h,.1,2500));this.draw(this.world,I());
 for(const racer of game.racers){const fwd=forward(racer.race.progress),yaw=Math.atan2(fwd[0],fwd[2])+racer.lateral*.01;this.draw(this.vehicles[racer.pilot],model(game.pos(racer),yaw,-racer.lateral*.015))}
 for(const d of game.drones)if(d.respawn<=0)this.draw(this.drone,model(game.dronePos(d),game.time*.5));
 for(const item of game.pickups)if(item.timer<=0)this.draw(this.pickups[item.type],model(position(item.s,item.lane,2+Math.sin(game.time*2)*.3),game.time,Math.sin(game.time)*.15));
 for(const s of game.shots)this.draw(this.bolts[s.missile?2:s.owner===game.player?0:1],model(s.p,Math.atan2(s.dir[0],s.dir[2])));
 for(const fx of game.fx)this.draw(this.particle,model(fx.p,fx.life,fx.life,fx.life*2));this.frames++;
 }
}
window.GVRenderer=Renderer;
})();
