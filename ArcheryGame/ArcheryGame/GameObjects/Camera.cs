using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcheryGame
{
    /// <summary>
    /// Kamera z widoku pierwszej osoby.
    /// </summary>
    public class Camera : GameComponent
    {
        public Matrix View
        {
            get {  return Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);  }
        }

        public Matrix Projection { get; protected set; }

        public Vector3 Position
        {
            get { return cameraPosition; }
            set
            {
                cameraPosition = value;
                UpdateLookAt();
            }
        }

        public Vector3 Rotation
        {
            get { return cameraRotation; }
            set
            {
                cameraRotation = value;
                UpdateLookAt();
            }
        }

        private Vector3 cameraPosition;
        private Vector3 cameraRotation;
        private Vector3 cameraLookAt;

        private Vector3 mouseRotationBuffer;

        private MouseState currentMouseState;
        private MouseState previoustMouseState;

   
        //defines speed of camera movement
        float speed = 10;
        float rotationSpeed = 0.1f;

        MouseState prevMouseState;

        public Camera(Game game, Vector3 position, Vector3 rotation, float speed)
            : base(game)
        {

            Position = position;
            Rotation = rotation;
            this.speed = speed;

            previoustMouseState = Mouse.GetState();

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 1, 100);
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);

            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var moveVector = Vector3.Zero;

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            currentMouseState = mouseState;

            if (keyboardState.IsKeyDown(Keys.W))
                moveVector.Z = 1;

            if (keyboardState.IsKeyDown(Keys.S))
                moveVector.Z = -1;

            if (keyboardState.IsKeyDown(Keys.A))
                moveVector.X = 1;

            if (keyboardState.IsKeyDown(Keys.D))
                moveVector.X = -1;

            if (moveVector != Vector3.Zero)
            {
                //normalizacja wektora zeby po skosach nie latac szybciej
                moveVector.Normalize();

                moveVector *= dt * speed;

                Move(moveVector);
            }

            //Ruch myszką
            float deltaX;
            float deltaY;

            if (currentMouseState != previoustMouseState)
            {
                deltaX = currentMouseState.X - Game.GraphicsDevice.Viewport.Width / 2;
                deltaY = currentMouseState.Y - Game.GraphicsDevice.Viewport.Height / 2;

                mouseRotationBuffer.X -= rotationSpeed * deltaX * dt;
                mouseRotationBuffer.Y -= rotationSpeed * deltaY * dt;

                if (mouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));

                if (mouseRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(75.0f));

                Rotation = new Vector3(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)),
                    MathHelper.WrapAngle(mouseRotationBuffer.X), 0);

                deltaX = 0;
                deltaY = 0;
            }

            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

            previoustMouseState = currentMouseState;

            base.Update(gameTime);
        }

        private Vector3 PreviewMove(Vector3 ammount)
        {
            var rotation = Matrix.CreateRotationY(cameraRotation.Y);

            Vector3 movement = new Vector3(ammount.X, ammount.Y, ammount.Z);

            movement = Vector3.Transform(movement, rotation);

            return cameraPosition + movement;
        }

        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }

        private void MoveTo(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        private void UpdateLookAt()
        {
            var rotationMatrix = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);

            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
       

            cameraLookAt = Position + lookAtOffset;
        }
    }

}
