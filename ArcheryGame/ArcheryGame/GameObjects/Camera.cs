using Microsoft.Xna.Framework;

namespace ArcheryGame
{
    /// <summary>
    /// Kamera z widoku pierwszej osoby.
    /// </summary>
    public class Camera : GameComponent
    {
        public Matrix View { get { return Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up); } }

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


        public Camera(Game game, Vector3 position, Vector3 rotation)
            : base(game)
        {
            Position = position;
            Rotation = rotation;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 1, 100);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Aktualizuje kierunek spoglądania kamery.
        /// </summary>
        private void UpdateLookAt()
        {
            var rotationMatrix = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);

            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);

            cameraLookAt = Position + lookAtOffset;
        }
    }

}
