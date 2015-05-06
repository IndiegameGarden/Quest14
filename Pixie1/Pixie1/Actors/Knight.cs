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
        public static float TIME_START_MOVING = 11.6f;

        public static Knight Create()
        {
            return new Knight();
        }
        
        public Knight()
            : base("pixie")
        {
            IsCollisionFree = false;
            
            SetColors(4f, new Color(28, 20, 230), new Color(76, 76, 255));
            Health = 15f;

            Pushing.Force = RandomMath.RandomBetween(1f, 1.5f);

            ComplexBehavior = new SubsumptionBehavior();
            ComplexBehavior.Active = false;
            Add(ComplexBehavior);

            Combat = new CombatBehavior(typeof(RedGuard));
            ComplexBehavior.Add(Combat);

            ChasingHero = new ChaseBehavior(Level.Current.hero);
            ChasingHero.ChaseRange = 24f; // 110f;
            ChasingHero.SatisfiedRange = 8f;
            ChasingHero.MoveSpeed = RandomMath.RandomBetween(1.54f, 1.87f);
            ComplexBehavior.Add(ChasingHero);

            //TrailHeroBehavior tb = new TrailHeroBehavior();
            PathFindToHeroBehavior tb = new PathFindToHeroBehavior();
            tb.ChaseRange = 970f;
            //tb.CheatRange = ChasingHero.ChaseRange + 8f;
            tb.SatisfiedRange = ChasingHero.ChaseRange - 2f;
            ComplexBehavior.Add(tb);

            ChasingRedGuard = new ChaseBehavior(typeof(RedGuard));
            ChasingRedGuard.ChaseRange = 20f;
            ChasingRedGuard.MoveSpeed = RandomMath.RandomBetween(1.1f, 1.5f);
            ComplexBehavior.Add(ChasingRedGuard);

            Attacking = new AttackBehavior(Level.Current.hero);
            Attacking.AttackDuration = RandomMath.RandomBetween(1.5f, 2.8f);
            ComplexBehavior.Add(Attacking);

            WallFollowing = new AlwaysTurnRightBehavior();
            WallFollowing.MoveSpeed = ChasingHero.MoveSpeed * 0.7f;
            //ComplexBehavior.Add(WallFollowing);

            Wandering = new RandomWanderBehavior(15.7f, 23.3f);
            Wandering.MoveSpeed = RandomMath.RandomBetween(0.001f, 0.02f);
            ComplexBehavior.Add(Wandering);
        }

        protected override void OnDies()
        {
            base.OnDies();
            Level.Current.Sound.PlayDiedSound(0.3f);
            ColorFx.Active = false;
            ComplexBehavior.Active = false;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            if (ComplexBehavior.Active == false && SimTime > TIME_START_MOVING)
                ComplexBehavior.Active = true;
        }
    }
}
