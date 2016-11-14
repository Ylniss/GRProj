using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ArcheryGame
{
    public class Floor
    {
        private readonly GraphicsDevice device;
        private readonly int floorWidth;
        private readonly int floorHeight;
        private VertexBuffer floorBuffer;

        private const int titleColorCount = 4;

        private Color[] floorColors = new Color[titleColorCount] { Color.Blue, Color.Black, Color.White, Color.Red };

        public Floor(GraphicsDevice device, int width, int height)
        {
            this.device = device;
            floorWidth = width;
            floorHeight = height;

            BuildFloorBuffer();
        }

        public void Draw(Camera camera, BasicEffect effect)
        {
            effect.VertexColorEnabled = true;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.World = Matrix.Identity;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(floorBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, floorBuffer.VertexCount / 3);
            }
        }

        private void BuildFloorBuffer()
        {
            var vertexList = new List<VertexPositionColor>();
            int counter = 0;

            for (int x = 0; x < floorWidth; x++)
            {
                counter++;
                for (int z = 0; z < floorHeight; z++)
                {
                    counter++;

                    foreach (var vertex in FloorTile(x, z, floorColors[counter % titleColorCount]))
                        vertexList.Add(vertex);
                }
            }

            floorBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, vertexList.Count, BufferUsage.None);
            floorBuffer.SetData(vertexList.ToArray());
        }

        private List<VertexPositionColor> FloorTile(int xOffset, int zOffset, Color titleColor)
        {
            var list = new List<VertexPositionColor>();
            list.Add(new VertexPositionColor(new Vector3(0 + xOffset, 0, 0 + zOffset), titleColor));
            list.Add(new VertexPositionColor(new Vector3(1 + xOffset, 0, 0 + zOffset), titleColor));
            list.Add(new VertexPositionColor(new Vector3(0 + xOffset, 0, 1 + zOffset), titleColor));
            list.Add(new VertexPositionColor(new Vector3(1 + xOffset, 0, 0 + zOffset), titleColor));
            list.Add(new VertexPositionColor(new Vector3(1 + xOffset, 0, 1 + zOffset), titleColor));
            list.Add(new VertexPositionColor(new Vector3(0 + xOffset, 0, 1 + zOffset), titleColor));

            return list;
        }
    }
}
