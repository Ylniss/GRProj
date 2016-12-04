using ArcheryGame.GameObjects;
using ArcheryGame.TerrainGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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

        private Arrow arrow;

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

            archer = new Archer(this, new Vector3(0, 30, 0), Vector3.Zero, 10);
            archer.Initialize();

            terrain = new TerrainGenerator(Content, "grass", "heightmap");
            terrain.Initialize();

            //floor = new Floor(graphics.GraphicsDevice, 300, 300);
            effect = new BasicEffect(graphics.GraphicsDevice);
            arrow = new Arrow(this);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), graphics.GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);
            viewMatrix = camera.View;
            worldMatrix = Matrix.Identity;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            arrow.LoadContent(Content, "Arrow");
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

            archer.Update(gameTime);
            camera.Update(gameTime);
            arrow.Update(gameTime);
       
            viewMatrix = camera.View;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawModels();

            terrain.DrawTerrain(viewMatrix, projectionMatrix);

            var cameraPosition = camera.Position;
            var message = string.Format("Camera X: {0}\nCamera Y: {1}\nCamera Z: {2}", cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, Vector2.Zero, Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawModels()
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

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

                worldMatrix = Matrix.CreateScale(gameObject.ScalePercent) * 
                    Matrix.CreateRotationX(gameObject.RotationInRadians.X) * Matrix.CreateRotationY(gameObject.RotationInRadians.Y) * Matrix.CreateRotationZ(gameObject.RotationInRadians.Z) * 
                    Matrix.CreateTranslation(gameObject.Position);

                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(1f, 0, 0);
                        effect.View = viewMatrix;
                        effect.World = worldMatrix;
                        effect.Projection = projectionMatrix;
                    }
                    mesh.Draw();
                }
            }
        }

    }
}
