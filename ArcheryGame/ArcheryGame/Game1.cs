using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ArcheryGame
{
    public class Game1 : Game
    {
        SpriteFont font;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;
        Floor floor;

        BasicEffect effect;

        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        Arrow arrow;

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
            camera = new Camera(this, new Vector3(0, 2, 0), Vector3.Zero, 10);
            camera.Initialize();

            floor = new Floor(graphics.GraphicsDevice, 300, 300);
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

            camera.Update(gameTime);
            arrow.Update(gameTime);
       
            viewMatrix = camera.View;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var gameObj in Components)
            {
                Model model = null;
                if (gameObj is DrawableGameObject)
                    model = (gameObj as DrawableGameObject).Model;
                else
                    continue;

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

            floor.Draw(camera, effect);

            var cameraPosition = camera.Position;
            var message = string.Format("Camera X: {0}\nCamera Y: {1}\nCamera Z: {2}", cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, Vector2.Zero, Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
