using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using Microsoft.Xna.Framework;

namespace Pixie1.Behaviors
{
    /// <summary>
    /// lets a Thing chase another Thing when it's visible.
    /// </summary>
    public class ChaseBehavior: ThingControl
    {
        /// <summary>
        /// followed target of this chase behavior
        /// </summary>
        public Thing ChaseTarget;
        public Type ChaseTargetType;

        /// <summary>
        /// chase range in pixels
        /// </summary>
        public float ChaseRange = 10.0f;

        /// <summary>
        /// range reached when chaser is satisfied and stops chasing (0 means chase all the way)
        /// </summary>
        public float SatisfiedRange = 0f;

        /// <summary>
        /// if true, inverts the Chase into an Avoid behavior
        /// </summary>
        public bool Avoidance = false;

        protected bool isPauseChase = false;

        public ChaseBehavior(Thing chaseTarget)
        {
            this.ChaseTarget = chaseTarget;
        }

        public ChaseBehavior(Type chaseClass)
        {
            this.ChaseTargetType = chaseClass;
        }

        protected override void OnNextMove()
        {
 	        base.OnNextMove();

            // compute direction towards chase-target
            Vector2 dif = ChaseTarget.Position - ParentThing.Target;
            if (Avoidance)
                dif = -dif;

            // 1) whats-free
            bool isXfree = (dif.X != 0) || !ParentThing.CollidesWithBackground(new Vector2(dif.X, 0f));
            bool isYfree = (dif.Y != 0) || !ParentThing.CollidesWithBackground(new Vector2(0f, dif.Y));

            // choose one direction 1) based on whats free, then 2) semi-randomly, if diagonals would be required
            if (dif.X != 0f && dif.Y != 0f)
            {
                // 2) semi-random
                float r = RandomMath.RandomUnit();
                // tweak probability of move x/y
                float thres = 0.5f;
                if (r > thres && isYfree)
                    dif.X = 0f;
                else if (isXfree)
                    dif.Y = 0f;
                else if (isYfree)
                    dif.X = 0f;
                else if (r > thres)
                    dif.X = 0f;
                else
                    dif.Y = 0f;
                // detect blockage
                if (!isXfree && !isYfree)
                {
                    isPauseChase = true;
                    IsTargetMoveDefined = false;
                }
                else
                {
                    isPauseChase = false;
                    IsTargetMoveDefined = true;
                }
            }
            else
            {
                // cases of needing to move only horiz or vert to target
                // detect blockage
                if (!isXfree || !isYfree)
                {
                    isPauseChase = true;
                    IsTargetMoveDefined = false;
                }
                else
                {
                    isPauseChase = false;
                    IsTargetMoveDefined = true;
                }
            }

            dif.Normalize();
            TargetMove = dif;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // check for dead chase targets
            if (ChaseTarget != null && ChaseTarget.Delete)
                ChaseTarget = null;

            // recheck for nearest chase target
            if (ChaseTargetType != null)
            {
                ChaseTarget = ParentThing.FindNearest(ChaseTargetType);
            }
            
            if (ChaseTarget != null && ChaseTarget.Visible)
            {
                Vector2 dif = ChaseTarget.Position - ParentThing.Target;
                float dist = dif.Length();
                if (dist > 0f && dist <= ChaseRange && dist > SatisfiedRange)
                {
                    // indicate we're chasing
                    if (!isPauseChase)
                        IsTargetMoveDefined = true;
                    AllowNextMove();
                }
            }
        }
    }
}
