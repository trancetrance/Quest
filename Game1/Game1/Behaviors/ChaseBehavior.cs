﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using Microsoft.Xna.Framework;

namespace Game1.Behaviors
{
    /// <summary>
    /// lets a Thing chase another Thing when it's visible.
    /// </summary>
    public class ChaseBehavior: ThingControl
    {
        /// <summary>
        /// followed target of this chase behavior
        /// </summary>
        public Thing ChaseTarget;
        public Type ChaseTargetType;

        /// <summary>
        /// chase range in pixels
        /// </summary>
        public float ChaseRange = 10.0f;

        /// <summary>
        /// range reached when chaser is satisfied and stops chasing (0 means chase all the way)
        /// </summary>
        public float SatisfiedRange = 0f;

        /// <summary>
        /// if true, inverts the Chase into an Avoid behavior
        /// </summary>
        public bool Avoidance = false;

        public ChaseBehavior(Thing chaseTarget)
        {
            this.ChaseTarget = chaseTarget;
        }

        public ChaseBehavior(Type chaseClass)
        {
            this.ChaseTargetType = chaseClass;
        }

        protected override void OnNextMove()
        {
 	        base.OnNextMove();

            // check for dead chase targets
            if (ChaseTarget != null && ChaseTarget.Delete)
                ChaseTarget = null;

            // recheck for nearest chase target
            if (ChaseTargetType != null)
            {
                ChaseTarget = ParentThing.FindNearest(ChaseTargetType);
            }

            Vector2 dif;
            if (ChaseTarget != null && ChaseTarget.Visible)
            {
                dif = ChaseTarget.Position - ParentThing.Target;
                float dist = dif.Length();
                if (dist > 0f && dist <= ChaseRange && dist > SatisfiedRange)
                {
                    // indicate we're chasing
                    IsTargetMoveDefined = true;
                }
            }
            
            dif = Vector2.Zero;

            // compute direction towards chase-target
            dif = ChaseTarget.Position - ParentThing.Target;
            if (Avoidance)
                dif = -dif;

            // choose one direction semi-randomly, if diagonals would be required
            if (dif.X != 0f && dif.Y != 0f)
            {
                float r = RandomMath.RandomUnit();
                // the larger dif.X wrt dif.Y, the smaller the probability of moving in the Y direction
                float thres = Math.Abs(dif.X) / (Math.Abs(dif.X) + Math.Abs(dif.Y));
                if (r > thres)
                    dif.X = 0f;
                else
                    dif.Y = 0f;
            }
            dif.Normalize();
            TargetMove = dif;
        }

    }
}