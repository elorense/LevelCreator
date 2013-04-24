using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LevelCreator
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class CursorClass : Microsoft.Xna.Framework.DrawableGameComponent
    {
        MouseState currentMouseState;
        Texture2D cursorTexture, grabTexture;
        Game mGame;
        Vector2 textureOrigin;
        WorldObject wCursor;
        WorldObject objDragged;
        
        public ObjectMenu objMenu;
        public Vector2 cursorPos;
        public World world { get; set; }
        public Camera camera { get; set; }

        public CursorClass(Game game)
            : base(game)
        {
            mGame = game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            cursorTexture = mGame.Content.Load<Texture2D>("reg_cursor");
            grabTexture = mGame.Content.Load<Texture2D>("grab_cursor");
            wCursor = new WorldObject(cursorTexture);
            wCursor.setTexture(cursorTexture);
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState currentKeys = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
            
            cursorPos.X = currentMouseState.X + camera.xOffset;
            cursorPos.Y = currentMouseState.Y + camera.yOffset;

            if (currentMouseState.LeftButton == ButtonState.Pressed
                    && currentKeys.IsKeyUp(Keys.LeftShift))
            {
                wCursor.setTexture(grabTexture);

                //Check if cursor is trying to drag an object already on screen
                foreach (WorldObject obj in world.worldObjectList)
                {
                    if (wCursor.Collides(obj) && objDragged == null)
                    {
                        objDragged = obj;
                    }
                }

                //Check if cursor is clicking an icon of object in objMenu
               
                if (wCursor.Collides(objMenu.currObject) && objDragged == null)
                {
                    objDragged = new WorldObject();
                    objDragged.setTexture(objMenu.currObject.Texture);
                    objDragged.TextureName = objMenu.currObject.TextureName;
                }
                

                //Causes selected object to follow cursor
                if (objDragged != null)
                {
                    objDragged.PositionInWorldSpace = cursorPos;
                }
            }
            //If leftshift and left mouse are pressed, delete object in world
            else if (currentMouseState.LeftButton == ButtonState.Pressed
                    && currentKeys.IsKeyDown(Keys.LeftShift))
            {
                foreach (WorldObject obj in world.worldObjectList)
                {
                    if (wCursor.Collides(obj) && objDragged == null)
                    {
                        world.deleteList.Add(obj);
                        
                    }
                }
            }
            else
            {
                //Drop object and place in list of object in world
                if (objDragged != null && !world.worldObjectList.Contains(objDragged))
                    world.worldObjectList.Add(objDragged);
                objDragged = null;
                wCursor.setTexture(cursorTexture);
            }

            wCursor.PositionInWorldSpace = cursorPos;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if(objDragged != null)
                camera.Draw(objDragged);

            camera.Draw(wCursor);
        }
    }
}
