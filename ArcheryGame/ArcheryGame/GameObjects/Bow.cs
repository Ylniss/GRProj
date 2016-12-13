using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ArcheryGame.GameObjects
{
    public class Bow : DrawableGameObject
    {
        public Bow(Game game, Vector3 position) : base(game)
        {
            LoadContent(game.Content, "bow");
            Position = position;
            ScalePercent = new Vector3(1, 10, 5);
        }
    }
}
