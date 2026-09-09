/* Deterministic fixed-step arcade simulation; shared by rendering and Node tests. */
(function(root){
'use strict';
const TAU=Math.PI*2,LENGTH=1800,LAPS=3,WIDTH=26;
const pilots=[
 {name:'Rook Solar',species:'VULPINE · COMMANDER',ability:'Aegis pulse',desc:'Restore 45 shield. Ignore damage for 3 seconds.',speed:78,handling:20,fire:.17,color:[1,.23,.12]},
 {name:'Kite Azure',species:'FALCON · INTERCEPTOR',ability:'Overdrive',desc:'Five seconds of higher speed and rapid fire.',speed:86,handling:17,fire:.12,color:[.12,.5,1]},
 {name:'Nyx Vesper',species:'LUNAR LYNX · PHASE RUNNER',ability:'Phase shift',desc:'Refill boost. Phase through damage for 4 seconds.',speed:75,handling:26,fire:.19,color:[.65,.23,1]}
];
const clamp=(x,a,b)=>Math.max(a,Math.min(b,x)),mix=(a,b,t)=>a+(b-a)*t,mod=(n,m)=>((n%m)+m)%m;
const add=(a,b)=>a.map((v,i)=>v+b[i]),sub=(a,b)=>a.map((v,i)=>v-b[i]),mul=(a,k)=>a.map(v=>v*k),dot=(a,b)=>a.reduce((s,v,i)=>s+v*b[i],0),len=a=>Math.hypot(...a),norm=a=>mul(a,1/(len(a)||1)),cross=(a,b)=>[a[1]*b[2]-a[2]*b[1],a[2]*b[0]-a[0]*b[2],a[0]*b[1]-a[1]*b[0]];
function point(s){const t=s/LENGTH*TAU;return [Math.sin(t)*285+Math.sin(t*3)*32,22+Math.sin(t*2)*9,Math.cos(t)*250]}
const forward=s=>norm(sub(point(s+1),point(s-1))),right=s=>norm(cross([0,1,0],forward(s)));
const position=(s,lane,h=0)=>add(add(point(s),mul(right(s),lane)),[0,h,0]);
function crossed(a,b,at){let target=Math.floor(a/LENGTH)*LENGTH+at;if(target<=a)target+=LENGTH;return b>=target}
function swept(a,b,p,r){const ab=sub(b,a),t=clamp(dot(sub(p,a),ab)/Math.max(.0001,dot(ab,ab)),0,1);return len(sub(add(a,mul(ab,t)),p))<r}
class RaceRules{
 constructor(){this.progress=0;this.gates=0}
 get lap(){return Math.min(LAPS,Math.floor(this.progress/LENGTH)+1)}
 get finished(){return this.gates>=LAPS*4}
 advance(m){if(m<=0||this.finished)return;this.progress=Math.min(LENGTH*LAPS,this.progress+m);while(this.gates<12&&this.progress>=(this.gates+1)*450)this.gates++}
 recover(){this.progress=this.gates*450}
}
function racer(pilot,grid=0){return {pilot,grid,race:new RaceRules(),lane:(grid%3-1)*7,speed:0,lateral:0,height:0,vertical:0,boost:100,hull:100,shield:100,ammo:3,fire:0,missile:0,cooldown:0,ability:0,immune:0,damageAge:10,kills:0,finish:-1,boosting:false,drifting:false}}
class Game{
 constructor(){this.selected=0;this.state='selection';this.time=0;this.racers=[racer(0)];this.player.lane=0;this.shots=[];this.drones=[];this.pickups=[];this.fx=[];this.events=[];this.pads=[80,380,820,1260,1620];this.ramps=[320,1080];this.barriers=[620,1430];this.stats={shots:0,hits:0,pickups:0,jumps:0,pads:0}}
 get player(){return this.racers[0]}
 select(i){this.selected=i;this.racers=[racer(i)];this.player.lane=0}
 start(){this.racers=Array.from({length:6},(_,i)=>racer(i===0?this.selected:i%3,i));this.player.lane=0;this.racers.forEach((r,i)=>r.race.advance(i*9));this.time=0;this.countdown=3;this.state='countdown';this.shots=[];this.fx=[];this.events=[];this.stats={shots:0,hits:0,pickups:0,jumps:0,pads:0};this.drones=Array.from({length:8},(_,i)=>({s:170+i*205,lane:i%2?-6:6,hp:60,respawn:0,fire:i*.4}));this.pickups=Array.from({length:12},(_,i)=>({s:100+i*140,lane:(i%3-1)*7,type:i%3,timer:0}));this.notify('ALL SYSTEMS ONLINE')}
 notify(text){this.events.push(text)}
 pause(){if(this.state==='paused')this.state=this.beforePause;else if(['racing','countdown'].includes(this.state)){this.beforePause=this.state;this.state='paused'}}
 rank(r=this.player){return 1+this.racers.filter(o=>o!==r&&(o.race.progress>r.race.progress||(o.race.finished&&o.finish>=0&&(r.finish<0||o.finish<r.finish)))).length}
 pos(r){return position(r.race.progress,r.lane,1.5+r.height)}
 dronePos(d){return position(d.s,d.lane,3+Math.sin(this.time*2+d.s)*.7)}
 recover(r){r.hull=100;r.shield=60;r.speed=0;r.lane=0;r.height=0;r.vertical=0;r.immune=3;r.race.recover();if(r===this.player)this.notify('RECOVERED AT LAST CHECKPOINT')}
 damage(r,n){if(r.immune>0||r.race.finished)return false;r.damageAge=0;const absorb=Math.min(n,r.shield);r.shield-=absorb;r.hull-=n-absorb;if(r===this.player)this.notify('INCOMING FIRE');if(r.hull<=0){this.burst(this.pos(r),[1,.3,.05]);this.recover(r);return true}return false}
 ability(r){if(r.cooldown>0)return;r.cooldown=14;if(r.pilot===0){r.shield=Math.min(100,r.shield+45);r.immune=3;r.ability=3}else if(r.pilot===1)r.ability=5;else{r.boost=100;r.immune=4;r.ability=4}if(r===this.player)this.notify(pilots[r.pilot].ability.toUpperCase())}
 fire(r,missile=false){if(this.shots.length>=160)return;if(missile){if(r.ammo<=0||r.missile>0)return;r.ammo--;r.missile=.65}else {if(r.fire>0)return;r.fire=pilots[r.pilot].fire*(r.pilot===1&&r.ability>0?.55:1)}
  const dir=forward(r.race.progress),origin=add(this.pos(r),mul(dir,3));let target=null,nearest=140;
  for(const obj of [...this.racers.filter(o=>o!==r&&!o.race.finished),...this.drones.filter(d=>d.respawn<=0)]){const p=obj.race?this.pos(obj):this.dronePos(obj),delta=sub(p,origin),dist=len(delta);if(dist<nearest&&dot(norm(delta),dir)>.85){nearest=dist;target=obj}}
  const aim=target?norm(sub(target.race?this.pos(target):this.dronePos(target),origin)):dir;
  this.shots.push({p:origin,dir:aim,owner:r,target:missile?target:null,missile,life:missile?4:1.6});if(r===this.player){this.stats.shots++;this.events.push(missile?'sound:missile':'sound:laser')}
 }
 burst(p,c){for(let i=0;i<14;i++)this.fx.push({p:[...p],v:[Math.sin(i*17)*12,(i%5)*3,Math.cos(i*7)*12],life:.65,c})}
 tick(dt,input={}){
  if(this.state==='countdown'){this.countdown-=dt;if(this.countdown<=0){this.state='racing';this.notify('GO · CLAIM THE CANYON')}return}
  if(this.state!=='racing')return;this.time+=dt;
  for(const r of this.racers){if(r.race.finished){if(r.finish<0)r.finish=this.time;continue}const player=r===this.player,p=pilots[r.pilot],seed=r.grid*1.7;
   const steer=player?(input.steer||0):clamp((Math.sin(this.time*.6+seed)*8-r.lane)*.35,-1,1),throttle=player?!!input.throttle:true,brake=player&&input.brake;
   r.drifting=player&&input.drift&&Math.abs(steer)>.1&&r.speed>30&&r.height<=.05;
   r.boosting=(player?input.boost:Math.sin(this.time+seed)>.85)&&r.boost>1&&throttle;
   r.boost=clamp(r.boost+(r.boosting?-29:15)*dt,0,100);
   const max=p.speed*(r.boosting?1.52:1)*(r.pilot===1&&r.ability>0?1.25:1),target=brake?0:throttle?max:0,accel=dt*(brake?100:throttle?36:18);
   r.speed+=clamp(target-r.speed,-accel,accel);if(r.drifting)r.speed=Math.max(0,r.speed-10*dt);
   r.lateral=mix(r.lateral,steer*p.handling*(r.drifting?1.5:1)*(r.height>0?.7:1),1-Math.exp(-dt*(r.drifting?3:9)));r.lane+=r.lateral*dt;
   if(Math.abs(r.lane)>12){r.lane=Math.sign(r.lane)*12;r.lateral*= -.3;r.speed*=1-1.5*dt;this.damage(r,16*dt)}
   const before=r.race.progress;r.race.advance(r.speed*dt);
   for(const pad of this.pads)if(crossed(before,r.race.progress,pad)&&Math.abs(r.lane)<6){r.speed=Math.max(r.speed,p.speed*1.65);r.boost=Math.min(100,r.boost+22);if(player){this.stats.pads++;this.notify('ION PAD · VELOCITY SURGE')}}
   for(const ramp of this.ramps)if(crossed(before,r.race.progress,ramp)&&r.height<=.1){r.vertical=17;if(player)this.stats.jumps++}
   if(r.height>0||r.vertical>0){r.vertical-=27*dt;r.height=Math.max(0,r.height+r.vertical*dt);if(r.height<=0)r.vertical=0}
   for(const b of this.barriers)if(crossed(before,r.race.progress,b)&&r.lane>2&&r.height<3){this.damage(r,24);r.speed*=.6}
   for(const key of ['fire','missile','ability','immune'])r[key]-=dt;r.cooldown=Math.max(0,r.cooldown-dt);r.damageAge+=dt;if(r.damageAge>4)r.shield=Math.min(100,r.shield+9*dt);
   if(player?input.fire:Math.sin(this.time*2+seed)>.4)this.fire(r);
   if(player&&input.missile)this.fire(r,true);if(player&&input.ability)this.ability(r);if(player&&input.recover)this.recover(r);
  }
  for(const d of this.drones){if(d.respawn>0){d.respawn-=dt;if(d.respawn<=0)d.hp=60;continue}d.fire-=dt;const delta=sub(this.pos(this.player),this.dronePos(d));if(len(delta)<100&&d.fire<=0){d.fire=2.2;this.shots.push({p:this.dronePos(d),dir:norm(delta),owner:null,target:null,missile:false,life:1.6})}}
  for(let i=this.shots.length-1;i>=0;i--){const s=this.shots[i];s.life-=dt;if(s.missile&&s.target&&!(s.target.respawn>0)){const aim=norm(sub(s.target.race?this.pos(s.target):this.dronePos(s.target),s.p));s.dir=norm(add(mul(s.dir,1-Math.min(1,dt*4)),mul(aim,Math.min(1,dt*4))))}const old=s.p;s.p=add(s.p,mul(s.dir,(s.missile?190:290)*dt));let hit=false;
   for(const r of this.racers)if(r!==s.owner&&!r.race.finished&&swept(old,s.p,this.pos(r),2.6)){const killed=this.damage(r,s.missile?70:17);if(killed&&s.owner)s.owner.kills++;hit=true;break}
   if(!hit&&s.owner)for(const d of this.drones)if(d.respawn<=0&&swept(old,s.p,this.dronePos(d),2.4)){d.hp-=s.missile?70:22;if(d.hp<=0){d.respawn=8;s.owner.kills++;this.burst(this.dronePos(d),[1,.5,.08]);if(s.owner===this.player)this.notify('DRONE DESTROYED')}hit=true;break}
   if(hit){this.burst(s.p,s.missile?[1,.5,.1]:[.1,.8,1]);if(s.owner===this.player)this.stats.hits++}if(hit||s.life<=0)this.shots.splice(i,1);
  }
  for(const item of this.pickups){item.timer-=dt;if(item.timer>0)continue;for(const r of this.racers)if(Math.abs(mod(r.race.progress,LENGTH)-item.s)<5&&Math.abs(r.lane-item.lane)<3&&r.height<4){if(item.type===0)r.ammo=Math.min(9,r.ammo+3);else if(item.type===1)r.shield=Math.min(100,r.shield+40);else r.hull=Math.min(100,r.hull+35);item.timer=8;if(r===this.player){this.stats.pickups++;this.notify(['+3 MISSILES','SHIELD RESTORED','HULL REPAIRED'][item.type])}break}}
  for(let i=this.fx.length-1;i>=0;i--){const f=this.fx[i];f.life-=dt;f.p=add(f.p,mul(f.v,dt));if(f.life<=0)this.fx.splice(i,1)}
  if(this.player.race.finished){this.player.finish=this.time;this.state='finished';this.notify('RACE COMPLETE')}
 }
}
const API={Game,RaceRules,pilots,point,position,forward,right,crossed,swept,clamp,mix,mod,add,sub,mul,dot,len,norm,cross,LENGTH};
if(typeof module!=='undefined')module.exports=API;else root.GV=API;
})(typeof globalThis!=='undefined'?globalThis:this);
