﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcheryGame
{
    public sealed class Services
    {
        #region Fields
        private static Services instance = null;
        private static GraphicsDevice graphics;
        private static Random randomNumber;
        #endregion

        #region Properties
        /// <summary>
        /// This is used to get the Services Instance
        /// Instead of using the mInstance this will do the check to see if the Instance is valid
        /// where ever you use it. It is also private so it will only get used inside the engine services.
        /// </summary>
        private static Services Instance
        {
            get
            {
                //Make sure the Instance is valid
                if (instance != null)
                {
                    return instance;
                }

                throw new InvalidOperationException("The Engine Services have not been started!");
            }
        }

        public static Camera Camera
        {
            get;
            private set;
        }

        public static GraphicsDevice Graphics
        {
            get { return graphics; }
        }

        public static Random RandomNumber
        {
            get { return randomNumber; }
        }
        /// <summary>
        /// Returns the window size in pixels, of the height.
        /// </summary>
        /// <returns>int</returns>
        public static int WindowHeight
        {
            get { return graphics.ScissorRectangle.Height; }
        }
        /// <summary>
        /// Returns the window size in pixels, of the width.
        /// </summary>
        /// <returns>int</returns>
        public static int WindowWidth
        {
            get { return graphics.ScissorRectangle.Width; }
        }
        #endregion
        #region Constructor
        /// <summary>
        /// This is the constructor for the Services
        /// You will note that it is private that means that only the Services can only create itself.
        /// </summary>
        private Services(Game game)
        {

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This is used to start up Panther Engine Services.
        /// It makes sure that it has not already been started if it has been it will throw and exception
        /// to let the user know.
        /// 
        /// You pass in the game class so you can get information needed.
        /// </summary>
        /// <param name="game">Reference to the game class.</param>
        /// <param name="graphicsDevice">Reference to the graphic device.</param>
        /// <param name="Camera">For passing the reference of the camera when instanced.</param>
        public static void Initialize(Game game, GraphicsDevice graphicsDevice, Camera camera)
        {
            //First make sure there is not already an instance started
            if (instance == null)
            {
                //Create the Engine Services
                instance = new Services(game);
                //Reference the camera to the property.
                Camera = camera;
                graphics = graphicsDevice;
                randomNumber = new Random();
                return;
            }

            throw new Exception("The Engine Services have already been started.");
        }
        #endregion
    }
}
