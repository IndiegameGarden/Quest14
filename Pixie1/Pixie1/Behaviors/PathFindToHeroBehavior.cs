using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using Microsoft.Xna.Framework;
using Pixie1.Actors;
using Pixie1.AStar;

namespace Pixie1.Behaviors
{
    public class PathFindToHeroBehavior: ThingControl
    {
        /// <summary>
        /// chase range in pixels
        /// </summary>
        public float ChaseRange = 150.0f;

        /// <summary>
        /// range reached when chaser is satisfied and stops chasing (0 means chase all the way)
        /// </summary>
        public float SatisfiedRange = 2f;

        LinkedListNode<MyPathNode> pathPointer = null;
        PathFindingJob job = null;

        protected override void OnNextMove()
        {
            base.OnNextMove();

            // follow path finding trail
            Vector2 vNext = new Vector2(pathPointer.Value.X, pathPointer.Value.Y);
            Vector2 dif = vNext - ParentThing.Target;
            if (pathPointer == job.Result.Last)
            {
                // path is done. No next move.
                job = null;
            }
            else
            {
                pathPointer = pathPointer.Next;
            }

            // do the move
            if(dif.Length()>0f)
                dif.Normalize();
            TargetMove = dif;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // some vars
            Vector2 vHero = Level.Current.hero.Target;
            Vector2 vMeToHero = vHero - ParentThing.Target;
            float distToHero = vMeToHero.Length();

            // check if in range to operate the behavior
            if (distToHero <= ChaseRange && distToHero > SatisfiedRange)
            {
                // check if a path is already planned...
                if (job != null && job.Result != null)
                {
                    AllowNextMove();
                    IsTargetMoveDefined = true;
                }
                // if not, plan a new path is PathFinder is not busy.
                else if (job == null && !Level.Current.PathFinder.IsBusy())
                {
                    var newJob = new PathFindingJob()
                    {
                        From = ParentThing.Target,
                        To = vHero,
                        Callback = PathFinderCallback
                    };
                    Level.Current.PathFinder.AddJob(newJob);
                }
            }
            else
            {
                // not in the right range. Delete any previous paths to reset.
                pathPointer = null;
                job = null;
            }

        }

        /// <summary>
        /// process result of PathFinder job - NOTE runs in PathFinder's thread.
        /// </summary>
        /// <param name="job"></param>
        public void PathFinderCallback(PathFindingJob job)
        {
            this.pathPointer = job.Result.First;
            this.job = job; // NOTE job should be assigned last. This triggers the other (game) thread to use value of job.
        }

    }
}
