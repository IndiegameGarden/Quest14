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
        public float CheatRange = 10f;

        /// <summary>
        /// index of position in Hero's PositionLog that is currently being used as target.
        /// </summary>
        protected int heroPositionLogTrackIndex = 0;
        protected bool isFullyBlockedState = false;

        protected override void OnNextMove()
        {
            base.OnNextMove();

            // compute situation
            Vector2 vTarget = Hero.GetPositionLog(heroPositionLogTrackIndex);
            Vector2 vMeToTarget = vTarget - ParentThing.Target;
            Vector2 vHero = Level.Current.hero.Target;
            Vector2 vMeToHero = vHero - ParentThing.Target;
            float distToHero = vMeToHero.Length();
            float distToTarget = vMeToTarget.Length();

            // reached target already? Then advance target.
            if (distToTarget == 0f && heroPositionLogTrackIndex < Hero.PositionLogIndex)
            {
                heroPositionLogTrackIndex++;    // advance to next place of hero
                // update some info
                vTarget = Hero.GetPositionLog(heroPositionLogTrackIndex);
                vMeToTarget = vTarget - ParentThing.Target;
                distToTarget = vMeToTarget.Length();
            }
            
            // compute whether blocked
            Vector2 vTargetToHero = vHero - vTarget;
            Vector2 dif = vMeToTarget;
            Vector2 difX = new Vector2(dif.X, 0f);
            Vector2 difY = new Vector2(0f, dif.Y);
            if (difX.Length() > 0f)
                difX.Normalize();
            if (difY.Length() > 0f)
                difY.Normalize();
            float distTargetToHero = vTargetToHero.Length();
            bool isBlockedX = (dif.X != 0f) && ParentThing.CollidesWithSomething(difX);
            bool isBlockedY = (dif.Y != 0f) && ParentThing.CollidesWithSomething(difY);
            bool isFullyBlocked = (isBlockedX && (dif.Y == 0f)) ||
                                    (isBlockedY && (dif.X == 0f)) ||
                                    (isBlockedX && isBlockedY);
            
            /*
            if (isFullyBlocked && heroPositionLogTrackIndex >0)
            {
                // not yet close to target. So, test if there is a closer target on trail.
                int idx = heroPositionLogTrackIndex - 1;

                Vector2 vTargetMin1 = Hero.GetPositionLog(idx);
                Vector2 vMeToTargetMin1 = vTargetMin1 - ParentThing.Target;
                float distToTargetMin1 = vMeToTargetMin1.Length();
                if (distToTargetMin1 > 1f && distToTargetMin1 < distToTarget)    // YES, that one is closer.
                {
                    heroPositionLogTrackIndex = idx; // so use the closer target.
                    // update some info
                    vTarget = Hero.GetPositionLog(heroPositionLogTrackIndex);
                    vMeToTarget = vTarget - ParentThing.Target;
                    distToTarget = vMeToTarget.Length();
                }
            }
             */
            

            if (isFullyBlocked && distToHero > this.CheatRange && distTargetToHero > this.CheatRange
                && !ParentThing.CollidesWithSomething(vMeToTarget))
            {
                // if my moves are blocked, and hero is far enough away to be cheating, jump the line
                // to the target, if target square is free.
                ParentThing.PositionAndTarget = vTarget;
                isFullyBlockedState = true;
            }
            else if (isFullyBlocked)
            {
                // skip my move if blocked - do another move
                isFullyBlockedState = true;
            }
            else
            {
                isFullyBlockedState = false;
                if (isBlockedX)         // choose Y move if x is blocked
                    dif = difY;
                else if (isBlockedY)    // choose X move if y is blocked
                    dif = difX;
                else if ((dif.X != 0f) && (dif.Y != 0f))   // choose random x/y if a diagonal move would be required
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
            IsTargetMoveDefined = !isFullyBlockedState;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            Vector2 vHero = Level.Current.hero.Target;
            Vector2 vMeToHero = vHero - ParentThing.Target;
            float distToHero = vMeToHero.Length();
            if (distToHero <= ChaseRange && distToHero > SatisfiedRange)
            {
                IsTargetMoveDefined = !isFullyBlockedState;
                AllowNextMove();
            }
            if (distToHero <= SatisfiedRange && heroPositionLogTrackIndex>0 )
            {
                // select last hero's position to track and slightly go before that
                int idx  = Hero.PositionLogIndex;

                Vector2 vTarget = Hero.GetPositionLog(idx);
                Vector2 vMeToTarget = vTarget - ParentThing.Target;
                float distToTarget = vMeToTarget.Length();

                float newDistToTarget = 0f;
                do
                {
                    idx--;
                    // update some info
                    vTarget = Hero.GetPositionLog(idx);
                    vMeToTarget = vTarget - ParentThing.Target;
                    newDistToTarget = vMeToTarget.Length();
                    if (newDistToTarget < distToTarget)
                        heroPositionLogTrackIndex = idx;
                } while (newDistToTarget < distToTarget);
                
            }
        }
    }
}
