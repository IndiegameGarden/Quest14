using System;
using System.Collections.Generic;
using TTengine.Core;
using Microsoft.Xna.Framework;

namespace Pixie1.Behaviors
{
    /**
     * when engaged in close proximity to an enemy, plays sounds (and maybe do hitpoints etc)
     */
    public class CombatBehavior: ThingControl
    {
        public Type EnemyType;
        public bool IsCombat = false;
        public bool WasCombat = false;

        public CombatBehavior(Type enemyType)
        {
            EnemyType = enemyType;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            AllowNextMove();
        }

        protected override void OnNextMove() 
        {
            base.OnNextMove();

            // check for enemy facing
            List<Thing> facing = ParentThing.DetectCollisions(ParentThing.FacingDirection);
            WasCombat = IsCombat;
            IsCombat = false;
            float randomVal = RandomMath.RandomUnit();
            foreach(Thing t in facing) 
            {
                if (t.GetType() == EnemyType)
                {
                    IsCombat = true;
                    if(!WasCombat || randomVal < 0.08f)
                        t.Health -= RandomMath.RandomBetween(0.8f, 1.6f);
                    break; // only attack one thing at a time
                }
            }

            if (IsCombat)
            {
                if (!WasCombat || randomVal < 0.08f)
                    Level.Current.Sound.PlayRandomCombatSound(0.2f, 0.3f);
            }
        }

    }
}
