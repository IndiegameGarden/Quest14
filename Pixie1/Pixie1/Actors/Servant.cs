using System;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    public class Servant: Thing
    {
        public SubsumptionBehavior ComplexBehavior;
        public ChaseBehavior AvoidingHero;
        public ChaseBehavior AvoidingKnights;
        public RandomWanderBehavior Wandering;

        public static Servant Create()
        {
            return new Servant();
        }

        public Servant()
            : base("pixie")
        {
            IsCollisionFree = false;
            Pushing.Force = 0f; // servants cant push others.
            DrawInfo.DrawColor = Color.Yellow;

            ComplexBehavior = new SubsumptionBehavior();
            Add(ComplexBehavior);

            // avoid other things
            AvoidingKnights = new ChaseBehavior(typeof(Knight));
            AvoidingKnights.MoveSpeed = RandomMath.RandomBetween(0.43f, 0.65f);
            AvoidingKnights.ChaseRange = 11f; // RandomMath.RandomBetween(12f, 40f);
            AvoidingKnights.ReverseBehavior();
            ComplexBehavior.Add(AvoidingKnights);

            // avoid other things
            AvoidingHero = new ChaseBehavior(typeof(Hero));
            AvoidingHero.MoveSpeed = RandomMath.RandomBetween(0.43f, 0.65f);
            AvoidingHero.ChaseRange = 11f; // RandomMath.RandomBetween(12f, 40f);
            AvoidingHero.ReverseBehavior();
            ComplexBehavior.Add(AvoidingHero);

            Wandering = new RandomWanderBehavior(9.7f, 14.3f);
            Wandering.MoveSpeed = 0.3f;
            ComplexBehavior.Add(Wandering);
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            /*
            if (TargetMove.LengthSquared() > 0)
            {
                if (CollidesWhenThisMoves(Level.Current.pixie, TargetMove))
                {
                    if (Level.Current.Subtitles.Children.Count <= 4)
                    {
                        Level.Current.Subtitles.Show(3, attackString[RandomMath.RandomIntBetween(0, attackString.Length - 1)], 3.5f, Color.IndianRed);
                    }
                }
            }
             */
        }
    }
}
