using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LevelCreator
{
    public class Camera
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public CursorClass cursor { get; set; }
        public World world { get; set; }
        public bool cameraMoving;
        public int xOffset;
        public int yOffset; 

        private KeyboardState currentKeys, lastKeys;
        private SpriteBatch mSpriteBatch;
        private int mWheelPrevVal;
        private const float zoomIncrement = 0.2f;
        private const int cameraMoveDist = 10;
        

        public float textTime;

        public Camera(SpriteBatch sb, Game game)
        {

            mWheelPrevVal = Mouse.GetState().ScrollWheelValue;

            mSpriteBatch = sb;
            Position = new Vector2(Game1.Width / 2, Game1.Height / 2);
            Rotation = 0.0f;
            Zoom = 1.0f;
            currentKeys = Keyboard.GetState();
            xOffset = 0;
            yOffset = 0; 
        }

        public void Begin()
        {
            mSpriteBatch.Begin();
        }

        public void End()
        {
            mSpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            textTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            lastKeys = currentKeys;
            currentKeys = Keyboard.GetState();
            
            moveCamera();
            checkForModifyWorldSize();
        }


        /// <summary>
        /// Tracks camera movements.
        /// Keys.Z: Zoom in
        /// Keys.X: Zoom out
        /// Keys.M: Return camera to starting position
        /// Arrow Keys: Move the camera in the specified direction
        /// </summary>
        private void moveCamera()
        {
            Vector2 newPos = Position;

            if (currentKeys.IsKeyDown(Keys.Z) && lastKeys.IsKeyUp(Keys.Z) && Zoom < 5.0f
                 || mWheelPrevVal < Mouse.GetState().ScrollWheelValue)
            {
                mWheelPrevVal = Mouse.GetState().ScrollWheelValue;
                Zoom += zoomIncrement;
            }

            if (currentKeys.IsKeyDown(Keys.X) && lastKeys.IsKeyUp(Keys.X) && Zoom > 0.25f
                 || mWheelPrevVal > Mouse.GetState().ScrollWheelValue)
            {
                mWheelPrevVal = Mouse.GetState().ScrollWheelValue;
                Zoom -= zoomIncrement;
            }

            if (currentKeys.IsKeyDown(Keys.Right))
            {
                newPos.X += cameraMoveDist;
                xOffset += cameraMoveDist;
            }

            if (currentKeys.IsKeyDown(Keys.Left))
            {
                newPos.X -= cameraMoveDist;
                xOffset -= cameraMoveDist;
            }

            if (currentKeys.IsKeyDown(Keys.Up))
            {
                newPos.Y -= cameraMoveDist;
                yOffset -= cameraMoveDist;
            }

            if (currentKeys.IsKeyDown(Keys.Down))
            {
                newPos.Y += cameraMoveDist;
                yOffset += cameraMoveDist;
            }

            //Return camera to original position and reset offsets
            if (currentKeys.IsKeyDown(Keys.M))
            {
                xOffset = 0;
                yOffset = 0;
                newPos = new Vector2(Game1.Width / 2, Game1.Height / 2);
            }

            Position = newPos;
        }

        /// <summary>
        /// Keys.Q: Increase grid x axis
        /// Keys.W: Decrease grid x axis
        /// Keys.A: Increase grid y axis
        /// Keys.S: Decrease grid y axis
        /// Arrow Keys: Move the camera in the specified direction
        /// </summary>
        public void checkForModifyWorldSize()
        {
            bool modified = false;

            if (currentKeys.IsKeyDown(Keys.Q) && lastKeys.IsKeyUp(Keys.Q))
            {
                world.Width += world.GridWidth;
                modified = true;
                
            }

            if (currentKeys.IsKeyDown(Keys.W) && lastKeys.IsKeyUp(Keys.W) && world.Width - world.GridWidth > 0)
            {
                world.Width -= world.GridWidth;
                modified = true;
            }

            if (currentKeys.IsKeyDown(Keys.A) && lastKeys.IsKeyUp(Keys.A))
            {
                world.Height += world.GridHeight;
                modified = true;
            }

            if (currentKeys.IsKeyDown(Keys.S) && lastKeys.IsKeyUp(Keys.S) && world.Height- world.GridHeight > 0)
            {
                world.Height -= world.GridHeight;
                modified = true;
            }

            if (modified)
            {
                world.destroyGrid();
                world.createGrid();
            }

        }

        public void Draw(WorldObject obj)
        {
            Vector2 objPosInCameraSpace = obj.PositionInWorldSpace - Position;

            objPosInCameraSpace = new Vector2(objPosInCameraSpace.X * (float)Math.Cos(-Rotation) -
                                              objPosInCameraSpace.Y * (float)Math.Sin(-Rotation),
                                              objPosInCameraSpace.X * (float)Math.Sin(-Rotation) +
                                              objPosInCameraSpace.Y * (float)Math.Cos(-Rotation));
            objPosInCameraSpace *= Zoom;

            Vector2 objPosInScreenSpace = objPosInCameraSpace + new Vector2(Game1.Width / 2, Game1.Height / 2);
            

            float rotationInScreenSpace = obj.RotationInWorldSpace - Rotation;

            if (obj.objScale != 0)
            {
                mSpriteBatch.Draw(obj.Texture, objPosInScreenSpace, null, obj.color, rotationInScreenSpace, obj.TextureOrigin, obj.objScale, SpriteEffects.None, 0);
            }
            else
            {
                if (obj.spEffect == SpriteEffects.None)
                    mSpriteBatch.Draw(obj.Texture, objPosInScreenSpace, null, obj.color, rotationInScreenSpace, obj.TextureOrigin, Zoom, SpriteEffects.None, 0);
                else
                    mSpriteBatch.Draw(obj.Texture, objPosInScreenSpace, null, obj.color, rotationInScreenSpace, obj.TextureOrigin, Zoom, obj.spEffect, 0);
            }
        }
    }
}

