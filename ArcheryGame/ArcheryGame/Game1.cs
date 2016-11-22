using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ArcheryGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera Camera;
        Floor floor;

        BasicEffect effect;

        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        float ms = 20;
        float total = 0;

        Archer archer;
        Arrow arrow;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.Title = "Archer3D";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            archer = new Archer(this);
            //archer.Position = new Vector3(0,0, 0);
   
            // Here we instance the camera, setting its position, target, rotation, whether it is orthographic,
            // then finally the near and far plane distances from the camera.
            Camera = new Camera(this, new Vector3(10, 1, 300), Vector3.Zero, Vector3.Zero, false, 1, 1000);
            Camera.Initialize();
           


        }

        protected override void Initialize()
        {
            ArcheryGame.Services.Initialize(this, graphics.GraphicsDevice, Camera);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), graphics.GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);
            viewMatrix = Matrix.CreateLookAt(Camera.Position, Camera.Target, new Vector3(0f, 1f, 0f));
            worldMatrix = Matrix.CreateWorld(Camera.Target, Vector3.Forward, Vector3.Up);

            floor = new Floor(graphics.GraphicsDevice, 300, 300);
            effect = new BasicEffect(graphics.GraphicsDevice);
            arrow = new Arrow(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            archer.LoadContent(Content, "mech1");
            arrow.LoadContent(Content, "Arrow");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            total += gameTime.ElapsedGameTime.Milliseconds;

            if (total >= ms)
            {
                total -= ms;
                var keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Up))
                    Camera.Position += Vector3.Forward;
                else if (keyboardState.IsKeyDown(Keys.Down))
                    Camera.Position += Vector3.Backward;
                if (keyboardState.IsKeyDown(Keys.Right))
                    Camera.Position += Vector3.Right;
                else if (keyboardState.IsKeyDown(Keys.Left))
                    Camera.Position += Vector3.Left;
            }

            arrow.Update(gameTime);

            viewMatrix = Matrix.CreateLookAt(Camera.Position, Camera.Target, Vector3.Up);

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

            floor.Draw(Camera, effect);
            base.Draw(gameTime);
        }
    }
}
