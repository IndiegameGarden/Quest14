using System;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    /**
     * knight that helps the hero
     */
    public class Knight: Thing
    {
        public SubsumptionBehavior ComplexBehavior;
        public ChaseBehavior  ChasingRedGuard, ChasingHero;
        public CombatBehavior Combat;
        public AlwaysTurnRightBehavior WallFollowing;
        public RandomWanderBehavior Wandering;
        public AttackBehavior Attacking;
        public static float TIME_START_MOVING = 12.6f;

        public static Knight Create()
        {
            return new Knight();
        }
        
        public Knight()
            : base("pixie")
        {
            IsCollisionFree = false;
            
            SetColors(4f, new Color(28, 20, 230), new Color(70, 70, 255));

            Pushing.Force = RandomMath.RandomBetween(1f, 1.5f);

            ComplexBehavior = new SubsumptionBehavior();
            ComplexBehavior.Active = false;
            Add(ComplexBehavior);

            Combat = new CombatBehavior(typeof(RedGuard));
            ComplexBehavior.Add(Combat);

            ChasingHero = new ChaseBehavior(Level.Current.hero);
            ChasingHero.ChaseRange = 370f;
            ChasingHero.SatisfiedRange = 6f;
            ChasingHero.MoveSpeed = RandomMath.RandomBetween(1.2f, 1.5f);
            ComplexBehavior.Add(ChasingHero);

            ChasingRedGuard = new ChaseBehavior(typeof(RedGuard));
            ChasingRedGuard.ChaseRange = 20f;
            ChasingRedGuard.MoveSpeed = RandomMath.RandomBetween(1.1f, 1.5f);
            ComplexBehavior.Add(ChasingRedGuard);

            Attacking = new AttackBehavior(Level.Current.hero);
            Attacking.AttackDuration = RandomMath.RandomBetween(1.5f, 2.8f);
            ComplexBehavior.Add(Attacking);

            /*
            WallFollowing = new AlwaysTurnRightBehavior();
            WallFollowing.MoveSpeed = ChasingHero.MoveSpeed;
            RootBehavior.Add(WallFollowing);
            */

            Wandering = new RandomWanderBehavior(2.7f, 11.3f);
            Wandering.MoveSpeed = RandomMath.RandomBetween(0.09f, 0.25f);
            ComplexBehavior.Add(Wandering);
            
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            if (ComplexBehavior.Active == false && SimTime > TIME_START_MOVING)
                ComplexBehavior.Active = true;
        }
    }
}
