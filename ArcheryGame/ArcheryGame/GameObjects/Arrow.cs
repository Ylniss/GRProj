﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ArcheryGame
{
    public class Arrow : DrawableGameObject
    {
        public float Weight;

        private bool fired;
        private float elapsed;

        public Arrow(Game game) : base(game)
        {
            fired = false;
            Velocity.Y = 0.5f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fired)
            {    
                Position.X = Velocity.X * (float)Math.Cos(RotationInRadians.X) * elapsed * 0.000001f;
                Position.Y = Velocity.Y * (float)Math.Cos(RotationInRadians.Y) * elapsed * 0.000001f;
            }
           
        }
    }
}
