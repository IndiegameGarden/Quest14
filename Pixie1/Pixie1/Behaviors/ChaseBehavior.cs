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
            if (dif.Length() == 0f)
                return;

            Vector2 difX = new Vector2(dif.X, 0f);
            Vector2 difY = new Vector2(0f, dif.Y);
            if (difX.Length() > 0f)
                difX.Normalize();
            if (difY.Length() > 0f)
                difY.Normalize();

            // whats-free
            bool isBlockedX = (difX.X != 0f) && ParentThing.ChecksCollisions && ParentThing.CollidesWithBackground(difX);
            bool isBlockedY = (difY.Y != 0f) && ParentThing.ChecksCollisions && ParentThing.CollidesWithBackground(difY);
            bool isFullyBlocked = (isBlockedX && (dif.Y == 0f)) ||
                                    (isBlockedY && (dif.X == 0f)) ||
                                    (isBlockedX && isBlockedY);
            bool isDiagonal = (dif.X != 0f) && (dif.Y != 0f);

            // choose one direction 1) based on whats free, then 2) semi-randomly, if diagonals would be required
            isPauseChase = false;
            if (isFullyBlocked)
            {
                dif = new Vector2(difY.Y, difX.X); // try a swap to get around obstacle
            }
            else if (isBlockedX)         // choose Y move if x is blocked
                dif = difY;
            else if (isBlockedY)    // choose X move if y is blocked
                dif = difX;
            else if (isDiagonal)   // choose random x/y if a diagonal move would be required
            {
                float r = RandomMath.RandomUnit();
                float thres = 0.5f;
                if (r > thres)
                    dif = difX;
                else
                    dif = difY;
            }
            else
            {
                dif.Normalize();
            }

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
            
            if (ChaseTarget != null && ChaseTarget.Visible && ChaseTarget.Health > 0f)
            {
                Vector2 dif = ChaseTarget.Position - ParentThing.Target;
                float dist = dif.Length();
                if (dist <= ChaseRange && dist > SatisfiedRange)
                {
                    // indicate we're chasing
                    IsTargetMoveDefined = !isPauseChase;
                    AllowNextMove();
                }
            }
        }
    }
}
