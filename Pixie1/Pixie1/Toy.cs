using System;

using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1.Behaviors;

namespace Pixie1
{
    /// <summary>
    /// base class for a Toy that can be picked up, left behind, used (and then it is active for a while), sometimes with 
    /// multiple shots before it is expended. Typically some special power, effect or weapon.
    /// </summary>
    public abstract class Toy: Thing
    {
        /// <summary>
        /// if my Parent is a Thing, this holds it. If null, there is no Parent which is a Thing
        /// </summary>
        public Thing ParentThing = null;

        public bool CanBePickedUp = true;

        /// <summary>
        /// if true this Toy is immediately used (once) by ParentThing upon pickup
        /// </summary>
        public bool UsedUponPickup = true;

        /// <summary>
        /// whether the Toy is being used right now (true) or not (false)
        /// </summary>
        public bool IsUsed = false;

        /// <summary>
        /// true when has been used for the first time, false if never used yet
        /// </summary>
        public bool IsFirstUsed = false;

        /// <summary>
        /// whether the Toy usage is being actively triggered now (true) or not (false).
        /// Triggering is for example by keypress or by the AI of a Thing.
        /// </summary>
        public bool IsTriggered = false;

        /// <summary>
        /// time this Toy can be used at most (remains active) in a single usage
        /// </summary>
        public float UseTimeMax = 5f;

        /// <summary>
        /// How long the Toy is in use since last activation. 0 if not in use.
        /// </summary>
        public float UseTime = 0f;

        /// <summary>
        /// how many times using this Toy has still left (e.g. 'shots')
        /// </summary>
        public int UsesLeft = 1;

        SubtitleText toyExplanationMessage = null;
        SubtitleText currentToyMsg = null;

        public Toy()
            : base()
        {          
            // nothing yet.
        }

        public abstract string ToyName();

        public virtual string ToyHint()
        {
            return "";
        }

        /// <summary>
        /// a ParentThing starts using the Toy
        /// </summary>
        public virtual void StartUsing()
        {
            if (!IsFirstUsed)
                Level.Current.Sound.PlayUseToySound();
            IsFirstUsed = true;
            IsUsed = true;
            UseTime = 0f;
            UsesLeft--;
        }

        /// <summary>
        /// a ParentThing stops using the Toy (or Toy times out)
        /// </summary>
        public virtual void StopUsing()
        {
            UseTime = 0f;
            IsUsed = false;
            IsTriggered = false;
        }

        protected String UsesLeftMsg()
        {
            string s = "";
            if (UsesLeft > 0)
                s = "(" + UsesLeft + " more)";
            return s;
        }

        protected void ShowToyMsg(string text, float duration)
        {
            if (currentToyMsg != null)
                currentToyMsg.Delete = true;
            currentToyMsg = Level.Current.Subtitles.Show(1, text, duration);
        }

        protected override void  OnNewParent()
        {
 	         base.OnNewParent();
             if (Parent is Thing)
             {
                 ParentThing = Parent as Thing;
                 ParentThing.ToyActive = this;
                 //Level.Current.hero.ShowInventory();
                 if (UsedUponPickup && UsesLeft > 0)
                 {
                     StartUsing();
                 }
             }
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            if(IsUsed)
                UseTime += p.Dt;    
            if (UseTime > UseTimeMax)
            {
                StopUsing();                                
            }

            // check if the last message, if any, about this toy is already done and new instance can be started
            Thing pixie = Level.Current.hero;
            bool pixieFacingToy = CollidesWhenOtherMoves(pixie, pixie.FacingDirection);
            if (toyExplanationMessage != null && toyExplanationMessage.Delete && !pixieFacingToy)
            {
                toyExplanationMessage = null;
            }

            // pixie facing this Toy == print info message
            if (ParentThing == null && toyExplanationMessage==null && pixieFacingToy)
            {
                toyExplanationMessage = Level.Current.Subtitles.Show(5,SayToyName(), 2.1f);
            }

            // collision with pixie = pickup            
            if (ParentThing==null && Collides(pixie))
            {
                Level.Current.Sound.PlayPickupSound();
                if (CanBePickedUp)
                {
                    pixie.Add(this);
                    Visible = false;
                }
                else
                {
                    if (UsesLeft > 0)
                    {
                        StartUsing();
                    }
                }
            }
        }

        protected virtual string SayToyName()
        {
            string tname = ToyName();
            if (tname.Length > 0)
            {
                string s = "\"" + tname + "\"";
                string thint = ToyHint();
                if (thint.Length > 0)
                    s += "\n(" + thint + ")";
                return s;
            }
            else
                return "";
        }

    }
}
