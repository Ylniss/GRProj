using ArcheryGame.TerrainGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ArcheryGame.GameObjects
{
    public class Archer : DrawableGameObject
    {
        private TerrainGenerator terrain;

        private Vector3 position;
        private Vector3 rotation;

        private Bow bow;

        new public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                Services.Camera.Position = position;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                Services.Camera.Rotation = rotation;
            }
        }

        public List<Arrow> Arrows { get; set; }

        //defines speed of movement
        float speed = 10;
        float rotationSpeed = 0.1f;

        private Vector3 mouseRotationBuffer;
        private MouseState currentMouseState;
        private MouseState previoustMouseState;
        private MouseState prevMouseState;

        private bool shoot = false;

        public Archer(Game game, Vector3 position, Vector3 rotation, float speed, TerrainGenerator terrain) 
            : base(game)
        {
            Position = position;
            Rotation = rotation;
            this.speed = speed;

            this.terrain = terrain;

            //bow = new Bow(game, position);

	        Arrows = new List<Arrow>();
            previoustMouseState = Mouse.GetState();
        }

        public override void Initialize()
        {
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);

            prevMouseState = Mouse.GetState();

            //bow.Initialize();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var moveVector = Vector3.Zero;

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            currentMouseState = mouseState;

            CheckCollisionWithMapBorders();

            if (mouseState.LeftButton == ButtonState.Pressed && !shoot)
            {
                Shoot();
                shoot = true;
             }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                shoot = false;
            }

            if (keyboardState.IsKeyDown(Keys.W))
                moveVector.Z = 1.0f;

            if (keyboardState.IsKeyDown(Keys.S))
                moveVector.Z = -1.0f;

            if (keyboardState.IsKeyDown(Keys.A))
                moveVector.X = 1.0f;

            if (keyboardState.IsKeyDown(Keys.D))
                moveVector.X = -1.0f;

            position.Y = CalculateHeight(10.0f);

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
            var rotation = Matrix.CreateRotationY(Rotation.Y);

            Vector3 movement = new Vector3(ammount.X, ammount.Y, ammount.Z);

            movement = Vector3.Transform(movement, rotation);

            return Position + movement;
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

        private void Shoot()
        {
            var arrow = new Arrow(Game,Position);
            Arrows.Add(arrow);
            arrow.LoadContent(Game.Content, "Arrow");
            arrow.Position = Position;
            arrow.Direction = Rotation;
            arrow.RotationInRadians = new Vector3(Rotation.X, Rotation.Y, Rotation.Z); 
            //    arrow.Direction.Normalize();
            arrow.Fire();
        }

        private void CheckCollisionWithMapBorders()
        {
            int offset = 5;

            if (Position.X < offset)
                Position = new Vector3(offset, position.Y, position.Z);
            if (Position.X > terrain.TerrainWidth - offset)
                Position = new Vector3(terrain.TerrainWidth - offset, position.Y, position.Z);

            offset *= -1;

            if (Position.Z > offset)
                Position = new Vector3(position.X, position.Y, offset);
            if (Position.Z < -terrain.TerrainWidth - offset)
                Position = new Vector3(position.X, position.Y, -terrain.TerrainWidth - offset);
        }

        private float CalculateHeight(float offset)
        {
            float[] closestPoints = new float[5];

            closestPoints[0] = terrain.HeightData[(int)position.X, (int)-position.Z];
            closestPoints[1] = terrain.HeightData[(int)position.X + 1, (int)-position.Z];
            closestPoints[2] = terrain.HeightData[(int)position.X, (int)-position.Z + 1];
            closestPoints[3] = terrain.HeightData[(int)position.X - 1, (int)-position.Z];
            closestPoints[4] = terrain.HeightData[(int)position.X, (int)-position.Z - 1];

            float height = 0f;
            foreach (float point in closestPoints)
            {
                height += point;
            }
            height /= closestPoints.Length;
            height += offset;

            return height;
        }
    }
}
