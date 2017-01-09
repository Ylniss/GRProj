using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ArcheryGame
{
    public abstract class DrawableGameObject : GameObject
    {
        public Model Model;

        public DrawableGameObject(Game game) : base(game)
        {
        }

        public void LoadContent(ContentManager content, string asset)
        {
            Model = content.Load<Model>(asset);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

    }
}
