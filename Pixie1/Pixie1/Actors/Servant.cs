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

            // avoid knights - done by a 'reversed' chase
            AvoidingKnights = new ChaseBehavior(typeof(Knight));
            AvoidingKnights.MoveSpeed = RandomMath.RandomBetween(0.43f, 0.65f);
            AvoidingKnights.ChaseRange = RandomMath.RandomBetween(10f, 17f);
            AvoidingKnights.ReverseBehavior();
            ComplexBehavior.Add(AvoidingKnights);

            // avoid hero - done by a 'reversed' chase
            AvoidingHero = new ChaseBehavior(typeof(Hero));
            AvoidingHero.MoveSpeed = RandomMath.RandomBetween(0.43f, 0.65f);
            AvoidingHero.ChaseRange = RandomMath.RandomBetween(10f, 17f);
            AvoidingHero.ReverseBehavior();
            ComplexBehavior.Add(AvoidingHero);

            Wandering = new RandomWanderBehavior(9.7f, 14.3f);
            Wandering.MoveSpeed = 0.3f;
            ComplexBehavior.Add(Wandering);
        }

    }
}
