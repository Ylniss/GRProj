﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcheryGame
{
    public abstract class GameObject : DrawableGameComponent
    {
        #region Fields
        private float frameTime;
        // Doing these as fields is almost twice as fast as if they were properties. 
        // Also, sense XYZ are fields they do not get data binned as a property.
        public Vector3 Position;
        public Vector3 Acceleration;
        public Vector3 Velocity;
        public Vector3 RotationInRadians;
        public Vector3 ScalePercent;
        public Vector3 RotationVelocity;
        public Vector3 RotationAcceleration;
        #endregion

        #region Constructor
        /// <summary>
        /// This gets the Positioned Object ready for use, initializing all the fields.
        /// </summary>
        /// <param name="game">The game class</param>
        public GameObject(Game game)
            : base(game)
        {
            game.Components.Add(this);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Allows the game component to be updated.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Velocity += Acceleration * frameTime;
            Position += Velocity * frameTime;
            RotationVelocity += RotationAcceleration * frameTime;
            RotationInRadians += RotationVelocity * frameTime;
        }
        #endregion
    }
}
