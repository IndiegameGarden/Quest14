using System;
using System.Collections.Generic;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    /**
     * pink arthur, the king to be rescued
     */
    public class PinkArthur: Thing
    {
        public bool isFollowHero = false;
        public ChaseBehavior Following;

        public PinkArthur()
            : base("pixie")
        {            
            IsCollisionFree = false;
            DrawInfo.DrawColor = Color.Pink;
            Health = 15f;

            Following = new ChaseBehavior(Level.Current.hero);
            Following.Active = false;
            Following.SatisfiedRange = 3f;
            Following.ChaseRange = 21f;
        }

        protected override void OnNewParent()
        {
            base.OnNewParent();
            Add(Following);
        }

        protected override void OnDies()
        {
            base.OnDies();
            Following.Active = false;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // enable the following behavior - or not
            Following.Active = isFollowHero;

            // check when to start following our rescuer
            float dist = (Level.Current.hero.Position - Position).Length();
            if (dist < 1.5f)
            {
                Level.Current.FoundPinkArthur();
                // start following hero
                if (!isFollowHero)
                    isFollowHero = true;
            }

            // check win position
            if (Position.X <= 44f && Position.Y <= 74f)
            {
                Level.Current.WinLevel();
                isFollowHero = false;
            }
        }

    }
}
