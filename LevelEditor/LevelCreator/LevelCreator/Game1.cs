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
using System.Xml.Serialization;
using System.IO;
using System.Runtime.InteropServices;

namespace LevelCreator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        CursorClass cursor;
        ObjectMenu objMenu;
        Camera mCamera;
        World mWorld;

        String XmlDest = "C:\\Users\\Edric\\Documents\\TestXML\\test.xml";
        
        public const int Width = 1280;
        public const int Height = 720;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = Height;
            graphics.PreferredBackBufferWidth = Width;
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            mWorld = new World(this);
            mWorld.Height = Height;
            mWorld.Width = Width;
            cursor = new CursorClass(this);
            cursor.Initialize();
            mWorld.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mCamera = new Camera(spriteBatch, this);
            objMenu = new ObjectMenu(this, spriteBatch);

            objMenu.camera = mCamera;
            mWorld.camera = mCamera;
            cursor.camera = mCamera;
            cursor.objMenu = objMenu;
            cursor.world = mWorld;
            mCamera.cursor = cursor;
            mCamera.world = mWorld;
           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                    || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            //Left-shift + Enter to serialize
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                SerializeToXML(XmlDest);
            }
                 

            //always update camera first
            mCamera.Update(gameTime);

            objMenu.Update(gameTime);
            cursor.Update(gameTime);
            mWorld.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// XmlSerializer
        /// </summary>
        /// <param name="filePath">Destination for Xml. Hard-coded at the moment as "XmlDest"</param>
        private void SerializeToXML(String filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WorldObject));
            TextWriter textWriter = new StreamWriter(@filePath);
            foreach (WorldObject obj in mWorld.worldObjectList)
            {
                serializer.Serialize(textWriter, obj);

            }
            textWriter.Close();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            
            spriteBatch.Begin();
            mWorld.Draw(gameTime);
            objMenu.Draw(gameTime);
            cursor.Draw(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
