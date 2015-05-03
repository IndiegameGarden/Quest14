using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;

namespace Pixie1.Behaviors
{
    /// <summary>
    /// A quick blinking animation, shown when a character is hurt/hit
    /// </summary>
    public class HurtBehavior: BlinkBehavior
    {
        private const float TIME_PERIOD = 0.1f, 
                            DUTY_CYCLE = 0.5f,
                            MAX_TIME_DURATION = 0.4f;

        /// <summary>
        /// create a new HurtBehavior with given intensity of effect
        /// </summary>
        /// <param name="intensity">between 0 (no effect) and 1 (max intensity)</param>
        public HurtBehavior(float intensity)
            : base(TIME_PERIOD, DUTY_CYCLE)
        {
            if (intensity > 1f)
                intensity = 1f;
            if (intensity < 0f)
                intensity = 0f;
            this.Duration = intensity * MAX_TIME_DURATION; // limited time duration.
        }

        public static void Apply(Thing t, float healthDamage, float maxDamage)
        {
            t.AddNextUpdate(new HurtBehavior(healthDamage / maxDamage));
        }

    }
}
