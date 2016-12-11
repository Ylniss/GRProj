using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ArcheryGame.GameObjects
{
    public class Wall : DrawableGameObject
    {
        public Wall(Game game, Vector3 position) : base(game)
        {
            LoadContent(game.Content, "sciana1");
            Position = position;
        }
    }
}
