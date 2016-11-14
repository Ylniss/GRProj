using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ArcheryGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ArcheryGame.Services.Initialize(this, graphics.GraphicsDevice, Camera);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), graphics.GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);
            viewMatrix = Matrix.CreateLookAt(Camera.Position, Camera.Target, new Vector3(0f, 1f, 0f));
            worldMatrix = Matrix.CreateWorld(Camera.Target, Vector3.Forward, Vector3.Up);
            floor = new Floor(graphics.GraphicsDevice, 300, 300);
            effect = new BasicEffect(graphics.GraphicsDevice);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            archer.LoadContent(Content, "mech1");
          //  archer2.LoadContent(Content, "mech1");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
                    Camera.Velocity += Vector3.Forward;
                else if (keyboardState.IsKeyDown(Keys.Down))
                    Camera.Velocity += Vector3.Backward;
                else if (keyboardState.IsKeyDown(Keys.Right))
                    Camera.Velocity += Vector3.Right;
                else if (keyboardState.IsKeyDown(Keys.Left))
                    Camera.Velocity += Vector3.Left;
            }

            viewMatrix = Matrix.CreateLookAt(Camera.Position, Camera.Target, Vector3.Up);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //foreach (var gameObj in Components)
            //{
            //    Model model = null;
            //    if (gameObj is DrawableGameObject)
            //        model = (gameObj as DrawableGameObject).Model;
            //    else
            //        continue;
              
            //    //foreach (ModelMesh mesh in model.Meshes)
            //    //{
            //    //    foreach (BasicEffect effect in mesh.Effects)
            //    //    {
            //    //        //effect.EnableDefaultLighting();
            //    //        effect.AmbientLightColor = new Vector3(1f, 0, 0);
            //    //        effect.View = viewMatrix;
            //    //        effect.World = worldMatrix;
            //    //        effect.Projection = projectionMatrix;
            //    //    }
            //    //    mesh.Draw();
            //    //}
            //}

            floor.Draw(Camera, effect);
            base.Draw(gameTime);
        }
    }
}
