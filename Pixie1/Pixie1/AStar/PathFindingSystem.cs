using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Pixie1.AStar
{
    /// <summary>
    /// a method call with the job result
    /// </summary>
    /// <param name="job"></param>
    public delegate void PathResultDelegate(PathFindingJob job);

    public class MyPathNode : IPathNode<Object>
    {
        public Int32 X;
        public Int32 Y;
        public bool IsWall;
        public bool IsWalkable(Object unused)
        {
            return !IsWall;
        }
    }

    public class PathFindingSystem
    {
        Queue<PathFindingJob> jobQ = new Queue<PathFindingJob>();
        Thread thread;
        bool isRunning = true;
        SpatialAStar<MyPathNode, Object> aStar;
        MyPathNode[,] grid ;
        LevelBackground levelBg;

        /// <summary>
        /// create a new path-finding system that operates on the given level map and given
        /// threshold for passable blocks in the map. More systems can be created for different
        /// maps or thresholds.
        /// </summary>
        /// <param name="levelBg"></param>
        /// <param name="passableIntensityThreshold"></param>
        public PathFindingSystem(LevelBackground levelBg, int passableIntensityThreshold)
        {
            this.levelBg = levelBg;
            var tx = levelBg.Texture;
            grid = new MyPathNode[tx.Width, tx.Height];
            for (int x = 0; x < tx.Width; x++)
            {
                for (int y = 0; y < tx.Height; y++)
                {
                    var col = levelBg.SamplePixel(new Vector2(x,y));
                    grid[x, y] = new MyPathNode()
                    {
                        IsWall = ((col.R + col.G + col.B) < passableIntensityThreshold),
                        X = x,
                        Y = y,
                    };
                }
            }

            aStar = new SpatialAStar<MyPathNode, Object>(grid);
            StartSystemThread();
        }

        protected void StartSystemThread()
        {
            thread = new Thread(new ThreadStart(RunSystemMainloop));
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
        }

        public void StopSystem()
        {
            isRunning = false;
        }

        public bool IsBusy()
        {
            lock (jobQ)
            {
                return jobQ.Count > 0;
            }
        }

        /// <summary>
        /// Add a new path-finding job to the queue of the system
        /// </summary>
        /// <param name="job">job to enqueue</param>
        public void AddJob(PathFindingJob job)
        {
            lock (jobQ)
            {
                jobQ.Enqueue(job);
            }
        }

        protected void RunSystemMainloop()
        {
            while (isRunning)
            {
                PathFindingJob job = null;
                lock (jobQ)
                {
                    if (jobQ.Count > 0)
                    {
                        job = jobQ.Dequeue();
                    }
                }
                if (job != null)
                {
                    job.Result = aStar.Search(new System.Drawing.Point((int)Math.Round(job.From.X), (int)Math.Round(job.From.Y)),
                                 new System.Drawing.Point((int)Math.Round(job.To.X), (int)Math.Round(job.To.Y)), levelBg);
                    job.Callback(job);
                }
                else
                {
                    Thread.Sleep(20);
                }
            }
        }
    }
}
