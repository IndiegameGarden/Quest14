using System;
using System.Collections.Generic;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    public class Hero: Thing
    {

        public List<Knight> Knights = new List<Knight>();

        protected float health = 10f;

        public Hero()
            : base("pixie")
        {            
            IsCollisionFree = false;
            SetColors(4f, Color.DarkGoldenrod, new Color(230, 210, 10));
            Velocity = 1.5f;

            Pushing.Force = 10f; // force higher than companions.

        }

        public float Health
        {
            get
            {
                return health;
            }

            set
            {
                health = value;
                if (health <= 0f)
                    Level.Current.LoseLevel();
            }
        }

        public void LeadAttack()
        {
            foreach (Knight c in Knights)
                c.Attacking.TriggerAttack();
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
        }

    }
}
