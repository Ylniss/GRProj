using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ArcheryGame.GameObjects
{
    public class Skydome : DrawableGameObject
    {
        public Skydome(Game game) : base(game)
        {
            LoadContent(game.Content, "skydome1");
            ScalePercent = new Vector3(900, 900, 900);
            Position.Y = -50f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Position.X = Services.Camera.Position.X;
            Position.Z = Services.Camera.Position.Z;

            RotationInRadians.Y += 0.0001f;

        }
    }
}
