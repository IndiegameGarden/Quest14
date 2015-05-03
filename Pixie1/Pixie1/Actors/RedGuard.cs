using System;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    public class RedGuard: Thing
    {
        // behaviors - the things that red guards do 
        public SubsumptionBehavior ComplexBehavior;
        public CombatBehavior Combat;
        public BlinkBehavior Blinking;
        public ChaseBehavior  Chasing;
        public ChaseBehavior ChasingComp;
        public AlwaysTurnRightBehavior Turning;
        public RandomWanderBehavior Wandering;

        // combat damage
        public float    MinDamage = 0.9f,
                        MaxDamage = 2.5f;
        protected string[] attackString = new string[] { "Take this, golden villain!", "We hurt the leader!", "Galad bleeds!", "Our swords struck true!",
            "To the grave, traitor!", "Die, golden scum!" , "He stumbles!", "Kill him!", "Take the leader!",
            "Death to all Pixelot!", "We got him!", "Victory to the sorceress!" };

        public static RedGuard Create()
        {
            return new RedGuard(Level.Current.hero);
        }

        public RedGuard(Thing chaseTarget)
            : base("pixie")
        {
            IsCollisionFree = false;
            DrawInfo.DrawColor = new Color(255, 10, 4);
            Health = 12f;

            ComplexBehavior = new SubsumptionBehavior();
            Add(ComplexBehavior);

            Combat = new CombatBehavior(typeof(Knight));
            ComplexBehavior.Add(Combat);

            // chase knights that are very close
            ChasingComp = new ChaseBehavior(typeof(Knight));
            ChasingComp.MoveSpeed = RandomMath.RandomBetween(0.43f, 0.65f);
            ChasingComp.ChaseRange = 2f; // RandomMath.RandomBetween(12f, 40f);
            ComplexBehavior.Add(ChasingComp);

            // chase hero
            Chasing = new ChaseBehavior(chaseTarget);
            Chasing.MoveSpeed = RandomMath.RandomBetween(0.47f, 0.75f);
            Chasing.ChaseRange = 14f; // RandomMath.RandomBetween(12f, 40f);
            ComplexBehavior.Add(Chasing);

            Turning = new AlwaysTurnRightBehavior(); // patrolling
            Turning.MoveSpeed = Chasing.MoveSpeed; //RandomMath.RandomBetween(0.57f, 1.05f);
            Turning.MoveSpeed = 0.7f;
            ComplexBehavior.Add(Turning);

            Wandering = new RandomWanderBehavior(2.7f, 11.3f);
            Wandering.MoveSpeed = 0.7f;
            ComplexBehavior.Add(Wandering);
            
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            if (TargetMove.LengthSquared() > 0)
            {
                if (CollidesWhenThisMoves(Level.Current.hero, TargetMove))
                {
                    if (Level.Current.Subtitles.Children.Count <= 4 && Level.Current.hero.Health > 0f)
                    {
                        Level.Current.Sound.PlayRandomCombatSound(0.2f, 0.3f);
                        Level.Current.Subtitles.Show(3, "(Red:) " + attackString[RandomMath.RandomIntBetween(0, attackString.Length - 1)], 3.5f, Color.IndianRed);
                        var damage = RandomMath.RandomBetween(MinDamage, MaxDamage);
                        HurtBehavior.Apply(Level.Current.hero, damage, MaxDamage);
                        Level.Current.hero.Health -= damage;
                    }
                }
            }

        }

        protected override void OnDies()
        {
            base.OnDies();
            Level.Current.Sound.PlayDiedSound(0.24f);
            DrawInfo.DrawColor = new Color(170, 80, 82);
            ComplexBehavior.Active = false; // disable any moves
        }
    }
}
