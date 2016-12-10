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
                return Matrix.CreateRotationX(MathHelper.ToRadians(90) + RotationInRadians.X) *
                    Matrix.CreateRotationZ(RotationInRadians.Z) *
                     Matrix.CreateRotationY(RotationInRadians.Y)
                    * Matrix.CreateTranslation(Position);
            }
        }

        public Arrow(Game game, Vector3 position) : base(game)
        {
            fired = false;
            Position = position;
            Velocity.X = 0.5f;

            RotationInRadians.X = MathHelper.ToRadians(90);
            RotationInRadians.Z = MathHelper.ToRadians(45);

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

               Position += lookAtOffset * 10.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

        }

        public void Fire()
        {
            fired = true;
        }


    }
}
