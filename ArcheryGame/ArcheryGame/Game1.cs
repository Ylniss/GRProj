using ArcheryGame.GameObjects;
using ArcheryGame.TerrainGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace ArcheryGame
{
    public class Game1 : Game
    {
        private SpriteFont font;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Camera camera;
        private Archer archer;
        private BasicEffect effect;

        private Matrix projectionMatrix;
        private Matrix viewMatrix;
        private Matrix worldMatrix;

        private TerrainGenerator terrain;

        //  private Arrow arrow;
        private Skydome sky;

        private Target target;

        private int score = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.Title = "Archer3D";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            camera = new Camera(this, new Vector3(0, 30, 0), Vector3.Zero);
            camera.Initialize();

            ArcheryGame.Services.Initialize(this, GraphicsDevice, camera);

            terrain = new TerrainGenerator(this, "sand", "grass", "stone", "heightmap4");
            terrain.Initialize();
            terrain.GenerateWall();

            archer = new Archer(this, new Vector3(15, 30, -15), Vector3.Zero, 10, terrain);
            archer.Initialize();

            sky = new Skydome(this);
            sky.Initialize();

            target = new Target(this, new Vector3(20, 15, -20));
            target.Initialize();

            effect = new BasicEffect(graphics.GraphicsDevice);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), graphics.GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);
            viewMatrix = camera.View;
            worldMatrix = Matrix.Identity;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //arrow.LoadContent(Content, "Arrow");
            font = Content.Load<SpriteFont>("Standard");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
                return;
            }
            if (IsActive)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.K))
                {
                    archer.Arrows.Clear();
                    var arrows = Components.Where(x => x.GetType() == typeof(Arrow)).ToList();

                    for (int i = 0; i < arrows.Count(); i++)
                        Components.Remove(arrows[i]);

                }

                archer.Update(gameTime);
                camera.Update(gameTime);
                sky.Update(gameTime);

                viewMatrix = camera.View;
                for (int i = 0; i < archer.Arrows.Count; i++)
                {
                    if (archer.Arrows[i].Position.Y < -20)
                    {
                        Components.Remove(archer.Arrows[i]);
                        archer.Arrows.RemoveAt(i);
                        --score;
                        continue;
                    }

                    if (IsCollision(archer.Arrows[i].Model, archer.Arrows[i].WorldMatrix, target.Model, target.WorldMatrix))
                    {
                      
                        target.RandomizePosition(terrain.TerrainLength, terrain.TerrainWidth);
                        score += (Int32) Vector3.Distance(archer.Position, archer.Arrows[i].Position) / 10;
                        Components.Remove(archer.Arrows[i]);
                        archer.Arrows.RemoveAt(i);
                    }
                }

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            DrawModels();

            terrain.DrawTerrain(viewMatrix, projectionMatrix);

            //var cameraPosition = camera.Position;
            //var message = string.Format("Camera X: {0}\nCamera Y: {1}\nCamera Z: {2}\nCamera Rotation{3}\n", cameraPosition.X, cameraPosition.Y, cameraPosition.Z,camera.Rotation);
            //string message2 = string.Empty;
            //int i = 1;
            //foreach (var item in archer.Arrows)
            //{

            //    message2 +=  string.Format("Arrow number: {0}, Position: {1}\n RotationInRadians: {2}\nRotationAcceleration: {3}\nRotationVelocity: {3}\n",i, item.Position, item.RotationInRadians, item.RotationAcceleration, item.RotationVelocity);
            //    i++;
            //}
            spriteBatch.Begin();
            //spriteBatch.DrawString(font, message, Vector2.Zero, Color.Black);
            //spriteBatch.DrawString(font, message2, new Vector2(0, 80), Color.Black);

            spriteBatch.DrawString(font, "Score: " + score, new Vector2(0, 80), Color.Yellow);
            spriteBatch.DrawString(font, String.Format("Arrow force: {0:0.00} %", archer.ArrowForce * 5), new Vector2(0, 160), Color.Yellow);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawModels()
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            var rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;

            foreach (var gameObj in Components)
            {
                //TODO model łucznika.
                if (gameObj is Archer) continue;

                Model model = null;
                if (gameObj is DrawableGameObject)
                    model = (gameObj as DrawableGameObject).Model;
                else
                    continue;

                var gameObject = (gameObj as DrawableGameObject);


                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(1f, 0, 0);
                        effect.View = viewMatrix;
                        effect.World = gameObject.WorldMatrix;
                        effect.Projection = projectionMatrix;
                    }
                    mesh.Draw();
                }
            }
        }

        private bool IsCollision(Model model1, Matrix world1, Model model2, Matrix world2)
        {
            for (int meshIndex1 = 0; meshIndex1 < model1.Meshes.Count; meshIndex1++)
            {
                BoundingSphere sphere1 = model1.Meshes[meshIndex1].BoundingSphere;
                sphere1 = sphere1.Transform(world1);

                sphere1.Radius = 0.001f;

                for (int meshIndex2 = 0; meshIndex2 < model2.Meshes.Count; meshIndex2++)
                {
                    BoundingSphere sphere2 = model2.Meshes[meshIndex2].BoundingSphere;
                    sphere2 = sphere2.Transform(world2);

                    if (sphere1.Intersects(sphere2))
                        return true;
                }
            }
            return false;
        }
    }
}
