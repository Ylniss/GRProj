using System;
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

        public Arrow(Game game, Vector3 position) : base(game)
        {
            fired = false;
            Position = position;
            Velocity.X = 0.5f;

            RotationInRadians.X = MathHelper.ToRadians(45);
            RotationInRadians.Y = MathHelper.ToRadians(45);

            

            ScalePercent = new Vector3(1, 1, 1);
            //arrow.LoadContent(Content, "Arrow");
         //   LoadContent(Content, "Arrow");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fired)
            {
                var direction = new Vector3(Position.X + Velocity.X * elapsed, Position.Y + Velocity.Y * elapsed,
                                      Position.Z + Velocity.Z * elapsed) - Position;

                direction.Normalize(); //make it a unit vector
                var angle = (float)Math.Atan2(-direction.X, direction.Y);
                Position += direction * 10.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }

        }

        public void Fire()
        {
            fired = true;
        }


    }
}
