using NUnit.Framework;
using UnityEngine;
namespace GalaxyVelocity.Tests {
    public class RaceRulesTests {
        [Test] public void FinishRequiresTwelveOrderedGates(){var r=new RaceRules();r.Advance(5399);Assert.IsFalse(r.Finished);Assert.AreEqual(11,r.GatesPassed);r.Advance(1);Assert.IsTrue(r.Finished);Assert.AreEqual(3,r.Lap);}
        [Test] public void ReverseCannotFarmLaps(){var r=new RaceRules();r.Advance(500);r.Advance(-600);Assert.AreEqual(500,r.Progress);Assert.AreEqual(1,r.GatesPassed);}
        [Test] public void RecoveryDoesNotGrantProgress(){var r=new RaceRules();r.Advance(790);r.Recover();Assert.AreEqual(450,r.Progress);Assert.AreEqual(1,r.GatesPassed);}
        [Test] public void FastProjectileUsesSweptCollision(){Assert.IsTrue(CombatSystem.Near(Vector3.zero,Vector3.forward*100,Vector3.forward*50,2));Assert.IsFalse(CombatSystem.Near(Vector3.zero,Vector3.forward*100,new Vector3(10,0,50),2));}
        [Test] public void TrackSeamIsContinuous(){Assert.Less(Vector3.Distance(CanyonTrack.Point(0),CanyonTrack.Point(1800)),.01f);Assert.IsTrue(CanyonTrack.Crossed(1798,1803,0));}
    }
}
