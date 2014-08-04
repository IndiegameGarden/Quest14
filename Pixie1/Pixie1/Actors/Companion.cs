using System;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    /**
     * companion of the hero that helps him
     */
    public class Companion: Thing
    {
        public SubsumptionBehavior RootBehavior;
        public ChaseBehavior  ChasingRedGuard, ChasingHero;
        public CombatBehavior Combat;     
        public RandomWanderBehavior Wandering;
        public AttackBehavior Attacking;
        public static float TIME_START_MOVING = 15.0f;

        public static Companion Create()
        {
            return new Companion();
        }
        
        public Companion()
            : base("pixie")
        {
            IsCollisionFree = false;
            
            SetColors(4f, new Color(38, 30, 240), new Color(150, 150, 255));

            Pushing.Force = RandomMath.RandomBetween(1f, 1.5f);

            RootBehavior = new SubsumptionBehavior();
            RootBehavior.Active = false;
            Add(RootBehavior);

            Combat = new CombatBehavior(typeof(RedGuard));
            RootBehavior.Add(Combat);

            ChasingHero = new ChaseBehavior(Level.Current.pixie);
            ChasingHero.ChaseRange = 370f;
            ChasingHero.SatisfiedRange = 6f;
            ChasingHero.MoveSpeed = RandomMath.RandomBetween(1.2f, 1.5f);
            RootBehavior.Add(ChasingHero);

            ChasingRedGuard = new ChaseBehavior(typeof(RedGuard));
            ChasingRedGuard.ChaseRange = 20f;
            ChasingRedGuard.MoveSpeed = RandomMath.RandomBetween(1.1f, 1.5f);
            RootBehavior.Add(ChasingRedGuard);

            Attacking = new AttackBehavior(Level.Current.pixie);
            Attacking.AttackDuration = RandomMath.RandomBetween(1.5f, 2.8f);
            RootBehavior.Add(Attacking);

            Wandering = new RandomWanderBehavior(2.7f, 11.3f);
            Wandering.MoveSpeed = RandomMath.RandomBetween(0.09f, 0.25f);
            RootBehavior.Add(Wandering);
            
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            if (RootBehavior.Active == false && SimTime > TIME_START_MOVING)
                RootBehavior.Active = true;
        }
    }
}
