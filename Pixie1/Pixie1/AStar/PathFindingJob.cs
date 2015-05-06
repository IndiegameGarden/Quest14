using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pixie1.AStar
{
    public class PathFindingJob
    {
        public Vector2 From;
        public Vector2 To;
        public LinkedList<MyPathNode> Result;
        public PathResultDelegate Callback;
    }

}
