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
        public Vector3 Direction { get; set; }

        public override Matrix WorldMatrix
        {
            get
            {
                return Matrix.CreateRotationX(RotationInRadians.X) *
                    Matrix.CreateRotationY(RotationInRadians.Y)*
                    Matrix.CreateRotationZ(RotationInRadians.Z) *
                     Matrix.CreateTranslation(Position);
            }
        }

        public Arrow(Game game, Vector3 position) : base(game)
        {
            fired = false;
            Position = position;

            RotationInRadians.X = MathHelper.ToRadians(0);
            RotationInRadians.Z = MathHelper.ToRadians(0);

            ScalePercent = new Vector3(1, 1, 1);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fired)
            {
     

               var rotationMatrix = Matrix.CreateRotationX(Direction.X) * Matrix.CreateRotationY(Direction.Y);
            
               Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);

                var speed = 20f;

                lookAtOffset = Vector3.Transform(lookAtOffset, Matrix.CreateTranslation(new Vector3(
                    lookAtOffset.X * speed * elapsed * (float)Math.Cos(-Direction.X),
                    speed * elapsed * (float)Math.Sin(-Direction.X) - (0.5f * 9.81f * elapsed * elapsed),
                    lookAtOffset.Z * speed * elapsed * (float)Math.Cos(-Direction.X))));

                Position += lookAtOffset * 10.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

        }

        public void Fire()
        {
            fired = true;
        }


    }
}
