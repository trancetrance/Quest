﻿using System;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Game1;
using Game1.Behaviors;
using TreeSharp;

namespace Game1.Actors
{
    public class RedGuard: Thing
    {
        // behaviors - the things that red guards do 
        public BlinkBehavior Blinking;
        public ChaseBehavior  Chasing;
        public ChaseBehavior ChasingComp;
        public AlwaysTurnRightBehavior Turning;
        public RandomWanderBehavior Wandering;

        protected string[] attackString = new string[] { "Take this, golden villain!", "We hurt him!", "He bleeds!", "Our swords struck true!",
            "He bleeds!", "To the grave, golden traitor!", "Die, golden scum!" , "He stumbles!"};

        public static RedGuard Create()
        {
            return new RedGuard(Level.Current.pixie);
        }

        public static RedGuard CreateCloaky()
        {
            RedGuard p = new RedGuard(Level.Current.pixie);
            p.IsCloaky = true;
            return p;
        }

        bool isCloaky = false;

        public RedGuard(Thing chaseTarget)
            : base("pixie")
        {
            IsCollisionFree = false;
            DrawInfo.DrawColor = new Color(255, 10, 4);

            var sub = new PrioritySelector();
            Add(sub);

            // chase companions that are very close
            ChasingComp = new ChaseBehavior(typeof(Companion));
            ChasingComp.MoveSpeed = RandomMath.RandomBetween(0.43f, 0.65f);
            ChasingComp.ChaseRange = 1.5f; // RandomMath.RandomBetween(12f, 40f);
            sub.AddChild(ChasingComp);

            // chase hero
            Chasing = new ChaseBehavior(chaseTarget);
            Chasing.MoveSpeed = RandomMath.RandomBetween(0.47f, 0.75f);
            Chasing.ChaseRange = 15f; // RandomMath.RandomBetween(12f, 40f);
            sub.AddChild(Chasing);

            Turning = new AlwaysTurnRightBehavior(); // patrolling
            Turning.MoveSpeed = Chasing.MoveSpeed; //RandomMath.RandomBetween(0.57f, 1.05f);
            Turning.MoveSpeed = 0.7f;
            sub.AddChild(Turning);

            Wandering = new RandomWanderBehavior(2.7f, 11.3f);
            Wandering.MoveSpeed = 0.7f;
            sub.AddChild(Wandering);
            
        }

        /// <summary>
        /// set 'cloaky' status, a cloaky is a hardly visible bad pixel
        /// </summary>
        public bool IsCloaky
        {
            get
            {
                return isCloaky;
            }
            set
            {
                if (IsCloaky == value)
                    return;
                // if change - swap dutycycle
                Blinking.DutyCycle = 1f - Blinking.DutyCycle;
                isCloaky = value;                
            }
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            if (TargetMove.LengthSquared() > 0)
            {
                if (CollidesWhenThisMoves(Level.Current.pixie, TargetMove))
                {
                    if (Level.Current.Subtitles.Children.Count <= 4)
                    {
                        Level.Current.Subtitles.Show(3, attackString[RandomMath.RandomIntBetween(0, attackString.Length - 1)], 3.5f, Color.IndianRed);
                        Level.Current.pixie.Health -= RandomMath.RandomBetween(1f, 3f);
                    }
                }
            }
        }
    }
}