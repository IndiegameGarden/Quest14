using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pixie1.Actors;
using Pixie1.Behaviors;

namespace Pixie1.Toys
{
    /// <summary>
    /// to test spell usage with key in castle
    /// </summary>
    public class TruthSerum: Toy
    {
        Thing SerumTarget = null;

        public TruthSerum()
            : base()
        {
            UsedUponPickup = false;
            UseTimeMax = 130f;
            SetColors(1.9f, Color.MediumSeaGreen, Color.Chartreuse);
        }

        public override string ToyName()
        {
            return "Serum of Compulsion";
        }

        public override string ToyHint()
        {
            return "thrust into your foe and...";
        }

        public override void StartUsing()
        {
            base.StartUsing();

            // check what I'm facing
            List<Thing> facing = ParentThing.DetectCollisions(ParentThing.FacingDirection);
            foreach (Thing t in facing)
            {
                if (t is RedGuard || t is Servant)
                {
                    SerumTarget = t;
                    break; // only attack one thing at a time
                }
            }

            if (SerumTarget == null)
            {
                Level.Current.Subtitles.Show(2, "I can't use the serum here.", 3f);
                IsUsed = false;
                UsesLeft++;
            }
            else
            {
                String t = "";
                PathFindToTargetBehavior pathFindBehavior = new PathFindToTargetBehavior()
                {
                    ChaseTarget = Level.Current.king,
                    ChaseRange = 1970f,
                    SatisfiedRange = 3f,
                    Duration = UseTimeMax
                };

                if (SerumTarget is RedGuard)
                {
                    t += "Red Guard";
                    var rg = SerumTarget as RedGuard;
                    rg.ComplexBehavior.Active = false;
                    rg.AddNextUpdate(pathFindBehavior);
                }
                if (SerumTarget is Servant)
                {
                    t += "Servant";
                    var sv = SerumTarget as Servant;
                    sv.ComplexBehavior.Active = false;
                    sv.AddNextUpdate(pathFindBehavior);
                }
                t += ", show me - WHERE is Arthur?!";
                Level.Current.Subtitles.Show(7, t, 4f);
            }
        }

        public override void StopUsing()
        {
            base.StopUsing();
            if (SerumTarget != null)
            {
                if (SerumTarget is RedGuard)
                {
                    var rg = SerumTarget as RedGuard;
                    rg.ComplexBehavior.Active = true;
                }
                if (SerumTarget is Servant)
                {
                    var sv = SerumTarget as Servant;
                    sv.ComplexBehavior.Active = true;
                }
                SerumTarget = null;
            }
        }


    }
}
