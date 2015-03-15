using System;
using TTengine.Core;
using Microsoft.Xna.Framework;
using Pixie1.Actors;

namespace Pixie1.Behaviors
{
    /**
     * when enabled, moves into the direction the leader of attack is facing
     */
    public class AttackBehavior: ThingControl
    {
        public Thing Leader = null;
        public float AttackDuration = 4.5f;
        public bool IsAttacking = false;
        public float CurrentAttackDuration = 0f;

        protected string[] attackString = new string[] { "Attack!", "Forward, men!", "Go!", "Knights!", "To arms!", "Attack!", 
            "Kill them!", "Cover me!", "Engage!", "ATTACK!!", "Swords!", "Drive them back!",
            "Begone, red knaves!", "Red traitors, die!", "Strike now!", "STRIKE!" ,
            "A thousand battles,\na thousand victories!", "We shall prevail!", "Arthur, to your service!",
            "Squash the red vermin.", "Blue and Gold! For honor!", "For justice!", "For the King!"};

        public AttackBehavior(Thing leader)
        {
            Leader = leader;
        }

        public void TriggerAttack() {
            if (!IsAttacking)
            {
                (ParentThing as Knight).ChasingHero.SatisfiedRange = 11f; // more range allowed during attack
                if (Level.Current.Subtitles.Children.Count <= 2)
                    Level.Current.Subtitles.Show(0, attackString[RandomMath.RandomIntBetween(0, attackString.Length - 1)], 1.5f);
                IsAttacking = true;
                CurrentAttackDuration = 0f;
            }
            else
            {
                // if already attacking, allow more range with new attack
                (ParentThing as Knight).ChasingHero.SatisfiedRange++;
                CurrentAttackDuration = 0f;
            }
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            if (IsAttacking)
            {                
                CurrentAttackDuration += p.Dt;
                if (CurrentAttackDuration > AttackDuration)
                {
                    IsAttacking = false;
                    CurrentAttackDuration = 0f;
                }
                AllowNextMove();
            }
            else
            {
                (ParentThing as Knight).ChasingHero.SatisfiedRange = 6f; // more range allowed during attack
            }
        }

        protected override void OnNextMove() 
        {
            base.OnNextMove();

            // only attack if not blocked there.
            if (IsAttacking && !ParentThing.CollidesWithBackground(Leader.FacingDirection))
            {
                TargetMove = Leader.FacingDirection;
                IsTargetMoveDefined = true;                
            }
        }

    }
}
