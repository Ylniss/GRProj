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
<<<<<<< HEAD
            RotationInRadians.X = MathHelper.ToRadians(45);
            RotationInRadians.Y = MathHelper.ToRadians(45);
           // RotationInRadians.Z = MathHelper.ToRadians(-90);
=======
           // RotationInRadians.X = MathHelper.ToRadians(45);
            RotationInRadians.X = MathHelper.ToRadians(Services.Camera.Rotation.X);
            
>>>>>>> origin/master
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
<<<<<<< HEAD
                var direction = new Vector3(Position.X + Velocity.X * elapsed, Position.Y + Velocity.Y * elapsed,
                                      Position.Z + Velocity.Z * elapsed) - Position;

                direction.Normalize(); //make it a unit vector
                var angle = (float)Math.Atan2(-direction.X, direction.Y);
                Position += direction * 10.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
               // Position.X = Position.X + Velocity.X  * elapsed;
              //   Position.Y = Position.Y  + Velocity.Y * (float)Math.Cos(RotationInRadians.Y) * elapsed * 0.01f;
             //   Position.Z = Position.Z + Velocity.Z * elapsed ;
=======
                Position.X = Position.X + Velocity.X * (float)Math.Cos(RotationInRadians.X) * elapsed;
                Position.Y = Position.Y  + Velocity.Y * (float)Math.Sin(RotationInRadians.Y) * elapsed * 0.01f;
>>>>>>> origin/master
            }

        }

        public void Fire()
        {
            fired = true;
        }


    }
}
