using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ArcheryGame.GameObjects
{
    public class Target : DrawableGameObject
    {
        private Random random = new Random();

        public Target(Game game, Vector3 position) : base(game)
        {
            LoadContent(game.Content, "target1");
            Position = position;
            ScalePercent = new Vector3(2, 2, 1);
        }

        public void RandomizePosition(int maxX, int maxZ)
        {
            int X = random.Next(maxX);
            int Z = random.Next(maxZ);
            int Y = random.Next(15, 20);

            Position = new Vector3(X, Y, -Z);
        }
    }
}
