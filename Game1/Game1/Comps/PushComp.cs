﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using TTengine.Comps;
using Microsoft.Xna.Framework;

namespace Game1.Behaviors
{
    /// <summary>
    /// lets a ThingComp push another ThingComp and being pushed
    /// </summary>
    public class PushComp: Comp
    {
        /// <summary>
        /// relative force of pushing (strength of unit related). 0f is no pushing force at all but can be pushed.
        /// </summary>
        public float Force = 1.0f;

        public Vector2 pushFromOthers = Vector2.Zero;
        public Vector2 pushFromOthersRemainder = Vector2.Zero;

        public PushComp()
        {
        }

        /// <summary>
        /// receive push force from neighbor Things
        /// </summary>
        /// <param name="dir"></param>
        public void BePushed(Vector2 dir)
        {
            if (Force < 5f)  // FIXME hack to let pixie not be pushed
                pushFromOthers += dir;
        }

        protected override void OnNextMove()
        {
			// check if there is push from others. This push can build up over time, only
			// released upon a next move
            float dist = pushFromOthers.Length();
            if (dist > 0f )
            {
                // yes - check if push direction square occupied
                Vector2 dif = pushFromOthers;              

                // choose dominant direction, if diagonals would be required
                if (Math.Abs(dif.X) > Math.Abs(dif.Y))
                    dif.Y = 0f;
                else
                    dif.X = 0f;
                dif.Normalize();
               	
                // if that square is taken, transfer my push to the ThingComp there with my own Force
                List<ThingComp> lt = ParentThing.DetectCollisions(dif);
                foreach (ThingComp t in lt)
                {
                    t.Pushing.BePushed(dif);
                }

                // if the square being pushed to is free, allow the move to go there
                if (lt.Count == 0)
                {                    
                    TargetMove = dif;
                    IsTargetMoveDefined = true;
                }
            }

            // reset the push buildup
            pushFromOthers = Vector2.Zero;

        }
    }
}
