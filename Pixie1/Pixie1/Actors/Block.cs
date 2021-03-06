﻿using System;
using System.Collections.Generic;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    /**
     * a block that can be pushed around
     */
    public class Block: Thing
    {

        public Block()
            : base("pixie")
        {            
            IsCollisionFree = false;
            DrawInfo.DrawColor = Color.Brown;
            DrawInfo.LayerDepth = 0.6f;
            Pushing.Force = 0f; // no push force of my own.
        }

        public static Block Create() {
            return new Block();
        }
    }
}
