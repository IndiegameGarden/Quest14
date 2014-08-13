using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pixie1.Behaviors;
using Microsoft.Xna.Framework;

namespace Pixie1.Actors
{
    public class Boss: Thing
    {
        public ChaseBehavior Chasing;

        public Boss(): base("boss")
        {
            PositionAndTarget = new Vector2(520f, 290f);

            // chase hero
            Chasing = new ChaseBehavior(Level.Current.hero);
            Chasing.MoveSpeed = RandomMath.RandomBetween(0.18f, 0.24f);
            Chasing.ChaseRange = 30f; // RandomMath.RandomBetween(12f, 40f);
            Chasing.SatisfiedRange = 1f;
            Add(Chasing);

            DrawInfo.Center = new Vector2(0.5f, 0.5f);

        }

        protected override void OnUpdate(ref TTengine.Core.UpdateParams p)
        {
            base.OnUpdate(ref p);
            Hero hero = Level.Current.hero;
            Vector2 dif = (hero.Target - Target);
            if (dif.Length() <= 8.5f)
            {
                dif.Normalize();
                Vector2 smiteVector = dif* p.Dt * 25f;
                hero.Target += smiteVector;
            }
        }

    }
}
