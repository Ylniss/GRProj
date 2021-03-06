﻿using ArcheryGame.TerrainGeneration;
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
        private float arrowForce;

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

        public float ArrowForce
        {
            get { return arrowForce; }
            set
            {
                if (value > 20)
                    arrowForce = 20;
                else if (value < 0)
                    arrowForce = 0;
                else
                    arrowForce = value;
            }
        }

        private float mouseClipPoint;

        //defines speed of movement
        float speed = 10;
        float rotationSpeed = 0.1f;

        private Vector3 mouseRotationBuffer;
        private MouseState currentMouseState;
        private MouseState previoustMouseState;
        private MouseState prevMouseState;

        private bool readyToShoot = false;

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
            mouseClipPoint = Game.Window.ClientBounds.Height / 2;
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

            if (mouseState.LeftButton == ButtonState.Pressed && !readyToShoot)
            {
                readyToShoot = true;
                mouseClipPoint = mouseState.Position.Y;
             }

            if (mouseState.LeftButton == ButtonState.Released && readyToShoot)
            {
                readyToShoot = false;
                previoustMouseState = currentMouseState;

                if(ArrowForce > 0)
                    Shoot();
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
                //Normalizacja wektora zeby po skosach nie przemieszczać się szybciej.
                moveVector.Normalize();

                moveVector *= dt * speed;

                Move(moveVector);
            }

            if (!readyToShoot)
            {
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
            }

            ArrowForce = (mouseState.Position.Y - mouseClipPoint) / 25f;

            base.Update(gameTime);
        }

        /// <summary>
        /// Uwzględnienie rotacji kamery.
        /// </summary>
        /// <param name="moveVector"></param>
        /// <returns></returns>
        private Vector3 IncludeRotation(Vector3 moveVector)
        {
            var rotation = Matrix.CreateRotationY(Rotation.Y);

            Vector3 movement = new Vector3(moveVector.X, moveVector.Y, moveVector.Z);

            movement = Vector3.Transform(movement, rotation);

            return Position + movement;
        }

        private void Move(Vector3 moveVector)
        {
            Position = IncludeRotation(moveVector);
            Rotation = Rotation;
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
            arrow.Fire(ArrowForce);
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
