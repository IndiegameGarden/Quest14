using TTengine.Core;

namespace Pixie1
{
    /// <summary>
    /// A Spritelet that becomes visible/invisible after a specified time
    /// </summary>
    public class TimedSpritelet: Spritelet
    {
        protected float timeVisible = 0f;
        
        /// <summary>
        /// Time (sec) after which to hide this spritelet automatically again
        /// </summary>
        public float HideTime = 5.5f;

        public TimedSpritelet(string bitmap)
            : base(bitmap)
        {
            //
        }

        public void Show()
        {
            Motion.TargetPos = Level.Current.HELPSCROLL_SHOW_POS;
            timeVisible = 0f;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            timeVisible += p.Dt;
            if (timeVisible > HideTime)
            {
                Motion.TargetPos = Level.Current.HELPSCROLL_HIDE_POS;
            }
        }

    }
}
