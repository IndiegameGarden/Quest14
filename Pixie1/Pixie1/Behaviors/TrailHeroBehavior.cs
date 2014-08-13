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
        /// whether I am on the trail of Hero currently, or not.
        /// </summary>
        public bool IsOnTrail = false;

        /// <summary>
        /// how long to pause trailing when I find myself fully blocked.
        /// </summary>
        public float TimeToPauseWhenFullyBlocked = 7f;

        /// <summary>
        /// index of position in Hero's PositionLog that is currently being used as target.
        /// </summary>
        protected int heroPositionLogTrackIndex = 0;
        
        /// <summary>
        /// whether the hero-following led to a full block (no more movement options towards target) last move.
        /// </summary>
        protected bool isFullyBlockedState = false;

        protected float timeInFullyBlockedState = 0f;

        protected override void OnNextMove()
        {
            base.OnNextMove();

            // compute situation
            Vector2 vMe = ParentThing.Target;
            Vector2 vTarget = Hero.GetPositionLog(heroPositionLogTrackIndex);
            Vector2 vMeToTarget = vTarget - vMe;
            Vector2 vHero = Level.Current.hero.Target;
            Vector2 vMeToHero = vHero - vMe;
            float distToHero = vMeToHero.Length();
            float distToTarget = vMeToTarget.Length();

            // reached target already? Then advance target.
            if (distToTarget == 0f)
            {
                if (heroPositionLogTrackIndex < Hero.PositionLogIndex)
                {
                    heroPositionLogTrackIndex++;    // advance to next place of hero
                    // update some info
                    vTarget = Hero.GetPositionLog(heroPositionLogTrackIndex);
                    vMeToTarget = vTarget - vMe;
                    distToTarget = vMeToTarget.Length();
                }
                IsOnTrail = true;
            }

            // if trail-point not close, re-select target by looping over entire Hero's trail
            if (!IsOnTrail)
            {
                int idxBest = heroPositionLogTrackIndex;
                float dmin = distToTarget;
                int maxIters = Math.Min(Hero.LOG_LENGTH, Hero.PositionLogIndex);
                for (int i = 0; i < maxIters; i++)
                {
                    float d = (Hero.PositionLog[i] - vMe).Length();
                    if (d < dmin)
                    {
                        idxBest = i;
                        dmin = d;
                    }
                }
                heroPositionLogTrackIndex = idxBest;
                // update some info
                vTarget = Hero.GetPositionLog(heroPositionLogTrackIndex);
                vMeToTarget = vTarget - vMe;
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
            bool isBlockedX = (dif.X != 0f) && ParentThing.CollidesWithBackground(difX);
            bool isBlockedY = (dif.Y != 0f) && ParentThing.CollidesWithBackground(difY);
            bool isFullyBlocked = (isBlockedX && (dif.Y == 0f)) ||
                                    (isBlockedY && (dif.X == 0f)) ||
                                    (isBlockedX && isBlockedY);            

            // if my moves are blocked, and hero is far enough away that cheating won't be noticed, jump the line
            // to the target, if target square is free.
            if (isFullyBlocked && distToHero > this.CheatRange && distTargetToHero > this.CheatRange
                && !ParentThing.CollidesWithSomething(vMeToTarget))
            {
                ParentThing.PositionAndTarget = vTarget;
                isFullyBlockedState = false;
                IsOnTrail = true;
            }
            else if (isFullyBlocked)
            {
                // mark that i'm blocked - so seems I'm off the trail.
                isFullyBlockedState = true;
                IsOnTrail = false;
            }
            else if (!isFullyBlockedState ||  timeInFullyBlockedState > TimeToPauseWhenFullyBlocked)
            {
                isFullyBlockedState = false;
                timeInFullyBlockedState = 0f;
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
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // keep time in state
            if (isFullyBlockedState)
                timeInFullyBlockedState += p.Dt;

            // some vars
            Vector2 vHero = Level.Current.hero.Target;
            Vector2 vMeToHero = vHero - ParentThing.Target;
            Vector2 vTarget = Hero.GetPositionLog(heroPositionLogTrackIndex);
            Vector2 vMeToTarget = vTarget - vHero;
            float distToHero = vMeToHero.Length();
            float distToTarget = vMeToTarget.Length();

            // check if in range to operate the behavior
            if (distToHero <= ChaseRange && distToHero > SatisfiedRange)
            {
                AllowNextMove();
                IsTargetMoveDefined = !isFullyBlockedState;
            }

        }
    }
}
