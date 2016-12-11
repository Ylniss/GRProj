using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using ArcheryGame.GameObjects;

namespace ArcheryGame.TerrainGeneration
{
    public class TerrainGenerator
    {
        public int TerrainWidth;
        public int TerrainLength;
        public float[,] HeightData;

        VertexBuffer terrainVertexBuffer;
        IndexBuffer terrainIndexBuffer;
        VertexDeclaration terrainVertexDeclaration;

        Effect effect;

        Texture2D sandTexture;
        Texture2D rockTexture;
        Texture2D grassTexture;

        private ContentManager content;
        private Game game;

        private string assetGrassTexture;
        private string assetSandTexture;
        private string assetRockTexture;
        private string assetHeightMap;

        private List<Wall> walling = new List<Wall>();

        public TerrainGenerator(Game game, string assetGrassTexture, string assetSandTexture, string assetRockTexture, string assetHeightMap)
        {
            this.game = game;
            this.content = game.Content;

            this.assetGrassTexture = assetGrassTexture;
            this.assetSandTexture = assetSandTexture;
            this.assetRockTexture = assetRockTexture;
            this.assetHeightMap = assetHeightMap;
        }

        public void DrawTerrain(Matrix currentViewMatrix, Matrix currentProjectionMatrix)
        {
            effect.CurrentTechnique = effect.Techniques["MultiTextured"];
            effect.Parameters["xTexture0"].SetValue(sandTexture);
            effect.Parameters["xTexture1"].SetValue(grassTexture);
            effect.Parameters["xTexture2"].SetValue(rockTexture);

            Matrix worldMatrix = Matrix.Identity;
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xView"].SetValue(currentViewMatrix);
            effect.Parameters["xProjection"].SetValue(currentProjectionMatrix);

            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xAmbient"].SetValue(0.4f);
            effect.Parameters["xLightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Services.Graphics.Indices = terrainIndexBuffer;
                Services.Graphics.SetVertexBuffer(terrainVertexBuffer);

                Services.Graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, terrainVertexBuffer.VertexCount / 3);
            }
        }

        public void GenerateWall()
        {
            for (int i = 0; i < TerrainLength / 10; i += 10)
            {
                walling.Add(new Wall(game, new Vector3 (i, 5f, 0f)));

            }


            foreach (Wall wall in walling)
            {
                wall.Initialize();
            }
        }

        public void Initialize()
        {
            effect = content.Load<Effect>("Effect");

            grassTexture = content.Load<Texture2D>(assetGrassTexture);
            sandTexture = content.Load<Texture2D>(assetSandTexture);
            rockTexture = content.Load<Texture2D>(assetRockTexture);

            LoadVertices();
        }

        private void LoadVertices()
        {
            Texture2D heightMap = content.Load<Texture2D>(assetHeightMap);
            LoadHeightData(heightMap);

            VertexMultitextured[] terrainVertices = SetUpTerrainVertices();
            int[] terrainIndices = SetUpTerrainIndices();
            terrainVertices = CalculateNormals(terrainVertices, terrainIndices);
            CopyToTerrainBuffers(terrainVertices, terrainIndices);
            terrainVertexDeclaration = new VertexDeclaration(VertexMultitextured.VertexDeclaration.VertexStride, VertexMultitextured.VertexElements);
        }

        private void LoadHeightData(Texture2D heightMap)
        {
            float minimumHeight = float.MaxValue;
            float maximumHeight = float.MinValue;

            TerrainWidth = heightMap.Width;
            TerrainLength = heightMap.Height;

            Color[] heightMapColors = new Color[TerrainWidth * TerrainLength];
            heightMap.GetData(heightMapColors);

            HeightData = new float[TerrainWidth, TerrainLength];
            for (int x = 0; x < TerrainWidth; x++)
                for (int y = 0; y < TerrainLength; y++)
                {
                    HeightData[x, y] = heightMapColors[x + y * TerrainWidth].R;
                    if (HeightData[x, y] < minimumHeight) minimumHeight = HeightData[x, y];
                    if (HeightData[x, y] > maximumHeight) maximumHeight = HeightData[x, y];
                }

            for (int x = 0; x < TerrainWidth; x++)
                for (int y = 0; y < TerrainLength; y++)
                    HeightData[x, y] = (HeightData[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * 30.0f;
        }

        private VertexMultitextured[] SetUpTerrainVertices()
        {
            VertexMultitextured[] terrainVertices = new VertexMultitextured[TerrainWidth * TerrainLength];

            for (int x = 0; x < TerrainWidth; x++)
            {
                for (int y = 0; y < TerrainLength; y++)
                {
                    terrainVertices[x + y * TerrainWidth].Position = new Vector3(x, HeightData[x, y], -y);
                    terrainVertices[x + y * TerrainWidth].TextureCoordinate.X = x / 30.0f;
                    terrainVertices[x + y * TerrainWidth].TextureCoordinate.Y = y / 30.0f;

                    terrainVertices[x + y * TerrainWidth].TexWeights.X = MathHelper.Clamp(1.0f - Math.Abs(HeightData[x, y] - 0) / 8.0f, 0, 1);
                    terrainVertices[x + y * TerrainWidth].TexWeights.Y = MathHelper.Clamp(1.0f - Math.Abs(HeightData[x, y] - 12) / 6.0f, 0, 1);
                    terrainVertices[x + y * TerrainWidth].TexWeights.Z = MathHelper.Clamp(1.0f - Math.Abs(HeightData[x, y] - 20) / 6.0f, 0, 1);

                    float total = terrainVertices[x + y * TerrainWidth].TexWeights.X;
                    total += terrainVertices[x + y * TerrainWidth].TexWeights.Y;
                    total += terrainVertices[x + y * TerrainWidth].TexWeights.Z;

                    terrainVertices[x + y * TerrainWidth].TexWeights.X /= total;
                    terrainVertices[x + y * TerrainWidth].TexWeights.Y /= total;
                    terrainVertices[x + y * TerrainWidth].TexWeights.Z /= total;
                }
            }

            return terrainVertices;
        }

        private int[] SetUpTerrainIndices()
        {
            int[] indices = new int[(TerrainWidth - 1) * (TerrainLength - 1) * 6];
            int counter = 0;
            for (int y = 0; y < TerrainLength - 1; y++)
            {
                for (int x = 0; x < TerrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * TerrainWidth;
                    int lowerRight = (x + 1) + y * TerrainWidth;
                    int topLeft = x + (y + 1) * TerrainWidth;
                    int topRight = (x + 1) + (y + 1) * TerrainWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }

            return indices;
        }

        private VertexMultitextured[] CalculateNormals(VertexMultitextured[] vertices, int[] indices)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();

            return vertices;
        }

        private void CopyToTerrainBuffers(VertexMultitextured[] vertices, int[] indices)
        {
            terrainVertexBuffer = new VertexBuffer(Services.Graphics, VertexMultitextured.VertexDeclaration, vertices.Length * VertexMultitextured.VertexDeclaration.VertexStride, BufferUsage.WriteOnly);
            terrainVertexBuffer.SetData(vertices);

            terrainIndexBuffer = new IndexBuffer(Services.Graphics, typeof(int), indices.Length, BufferUsage.WriteOnly);
            terrainIndexBuffer.SetData(indices);
        }
    }
}
