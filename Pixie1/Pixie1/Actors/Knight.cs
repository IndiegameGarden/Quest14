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
        protected bool isFreeToMoveFromSquareTable = false; // start 'stuck' to square table

        public static Knight Create()
        {
            return new Knight();
        }
        
        public Knight()
            : base("pixie")
        {
            IsCollisionFree = false;
            
            SetColors(4f, new Color(28, 20, 230), new Color(76, 76, 255));
            Health = 13f;

            Pushing.Force = RandomMath.RandomBetween(1f, 1.5f);

            ComplexBehavior = new SubsumptionBehavior();
            ComplexBehavior.Active = false; // initially, knight stays near square table.
            Add(ComplexBehavior);

            Combat = new CombatBehavior(typeof(RedGuard));
            ComplexBehavior.Add(Combat);

            ChasingHero = new ChaseBehavior(Level.Current.hero);
            ChasingHero.ChaseRange = 20f;
            ChasingHero.SatisfiedRange = 8f;
            ChasingHero.MoveSpeed = RandomMath.RandomBetween(1.54f, 1.87f);
            ComplexBehavior.Add(ChasingHero);

            var tb = new PathFindToTargetBehavior()
            {
                ChaseTarget = Level.Current.hero,
                ChaseRange = 970f,    
                SatisfiedRange = ChasingHero.ChaseRange - 2f
            };
            ComplexBehavior.Add(tb);

            ChasingRedGuard = new ChaseBehavior(typeof(RedGuard),true);
            ChasingRedGuard.ChaseRange = 20f;
            ChasingRedGuard.MoveSpeed = RandomMath.RandomBetween(1.1f, 1.5f);
            ComplexBehavior.Add(ChasingRedGuard);

            Attacking = new AttackBehavior(Level.Current.hero);
            Attacking.AttackDuration = RandomMath.RandomBetween(1.9f, 2.95f);
            ComplexBehavior.Add(Attacking);

            Wandering = new RandomWanderBehavior(15.7f, 23.3f);
            Wandering.MoveSpeed = RandomMath.RandomBetween(0.001f, 0.02f);
            ComplexBehavior.Add(Wandering);
        }

        protected override void OnDies()
        {
            base.OnDies();
            var dist = (this.Position - Level.Current.hero.Position).Length();
            Level.Current.Sound.PlayDiedSound(0.9f, dist);
            ColorFx.Active = false;
            DrawInfo.DrawColor = new Color(214, 245, 245);
            ComplexBehavior.Active = false;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            if (!isFreeToMoveFromSquareTable && SimTime > TIME_START_MOVING && Health > 0f)
            {
                isFreeToMoveFromSquareTable = true;
                ComplexBehavior.Active = true;
            }
        }
    }
}
