using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using Microsoft.Xna.Framework;
using Pixie1.Actors;

namespace Pixie1.Behaviors
{
    public class TrailHeroBehavior: ThingControl
    {
        /// <summary>
        /// chase range in pixels
        /// </summary>
        public float ChaseRange = 150.0f;

        /// <summary>
        /// range reached when chaser is satisfied and stops chasing (0 means chase all the way)
        /// </summary>
        public float SatisfiedRange = 2f;

        /// <summary>
        /// min range that I have to be away from hero before I start cheating in my moves i.e. jump over solid blocks
        /// </summary>
        public float CheatRange = 100f;

        /// <summary>
        /// index of position in Hero's PositionLog that is currently being used as target.
        /// </summary>
        protected uint heroPositionLogTrackIndex = UInt32.MaxValue;

        protected override void OnNextMove()
        {
            base.OnNextMove();

            if (heroPositionLogTrackIndex == UInt32.MaxValue) // first-time init
            {
                heroPositionLogTrackIndex = Hero.PositionLogIndex;
            }

            // compute situation
            Vector2 vTarget = Hero.PositionLog[Hero.CalcPositionLogIndex(0)];
            Vector2 vMeToTarget = vTarget - ParentThing.Target;
            Vector2 vHero = Level.Current.hero.Target;
            Vector2 vMeToHero = vHero - ParentThing.Target;
            float distToHero = vMeToHero.Length();
            float distToTarget = vMeToTarget.Length();

            // reached target already? Then advance target.
            if (distToTarget <= 1f)
            {
                int iters = 0;
                while (distToTarget <= 1f && heroPositionLogTrackIndex < Hero.PositionLogIndex)
                {
                    heroPositionLogTrackIndex++;    // advance to next place of hero

                    // update some info
                    vTarget = Hero.PositionLog[heroPositionLogTrackIndex % Hero.LOG_LENGTH];
                    vMeToTarget = vTarget - ParentThing.Target;
                    distToTarget = vMeToTarget.Length();
                    iters++;
                    if (iters > 5) break;
                }
            }
            else if (heroPositionLogTrackIndex >0)
            {
                // not yet close to target. So, test if there is a closer target on trail.
                uint idx = heroPositionLogTrackIndex - 1;

                Vector2 vTargetMin1 = Hero.PositionLog[idx % Hero.LOG_LENGTH];
                Vector2 vMeToTargetMin1 = vTargetMin1 - ParentThing.Target;
                float distToTargetMin1 = vMeToTargetMin1.Length();
                if (distToTargetMin1 < distToTarget)    // YES, that one is closer.
                {
                    heroPositionLogTrackIndex = idx; // so use the closer target.
                    // update some info
                    vTarget = Hero.PositionLog[heroPositionLogTrackIndex % Hero.LOG_LENGTH];
                    vMeToTarget = vTarget - ParentThing.Target;
                    distToTarget = vMeToTarget.Length();
                }
            }

            // check if move blocks
            Vector2 vTargetToHero = vHero - vTarget;
            Vector2 dif = vMeToTarget;
            Vector2 difX = new Vector2(dif.X, 0f);
            Vector2 difY = new Vector2(0f, dif.Y);
            if(difX.Length()>0f)
                difX.Normalize();
            if (difY.Length() > 0f) 
                difY.Normalize();
            float distTargetToHero = vTargetToHero.Length();
            bool isBlockedX = (dif.X!=0f) && ParentThing.CollidesWithSomething(difX);
            bool isBlockedY = (dif.Y!=0f) && ParentThing.CollidesWithSomething(difY);
            bool isFullyBlocked = (isBlockedX && (dif.Y == 0f)) || 
                                    (isBlockedY && (dif.X == 0f)) || 
                                    (isBlockedX && isBlockedY);

            if (isFullyBlocked && distToHero > this.CheatRange && distTargetToHero > this.CheatRange)
            {
                // if my moves are blocked, and hero is far enough away to be cheating, jump the line
                // to the target.
                ParentThing.PositionAndTarget = vTarget;                
            }
            else
            {
                if (isBlockedX)         // choose Y move if x is blocked
                {
                    dif = difY;
                }
                else if (isBlockedY)    // choose X move if y is blocked
                {
                    dif = difX;
                }
                else if ((dif.X != 0f) && (dif.Y != 0f) )   // choose random x/y if a diagonal move would be required
                {
                    float r = RandomMath.RandomUnit();
                    float thres = 0.5f;
                    if (r > thres)
                        dif = difX;
                    else
                        dif = difY;
                }
            }

            // do the move
            if(dif.Length()>0f)
                dif.Normalize();
            TargetMove = dif;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            Vector2 vHero = Level.Current.hero.Target;
            Vector2 vMeToHero = vHero - ParentThing.Target;
            float distToHero = vMeToHero.Length();
            if (distToHero <= ChaseRange && distToHero > SatisfiedRange)
            {
                IsTargetMoveDefined = true;
                AllowNextMove();
            }
        }
    }
}
