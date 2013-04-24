using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;


namespace LevelCreator
{
    public class ObjectMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch sb;

        private List<WorldObject> objIconList { get; set; } //can't think of a better name
        public Camera camera { get; set; }
        public WorldObject currObject;
        private WorldObject iconObj;

        private Texture2D iconBack;
        private int currIcon;
        private KeyboardState currState, prevState;
        private Vector2 iconPos;
        private Vector2 cameraPos;

        public ObjectMenu(Game game, SpriteBatch sb) 
            : base(game)
        {
            this.sb = sb;
            objIconList = new List<WorldObject>();
            iconBack = game.Content.Load<Texture2D>("iconBack");
            LoadMenu(game.Content, "Content/ObjectList.xml");
            currState = Keyboard.GetState();
            
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        private void LoadMenu(ContentManager content, String wordFile)
        {
            using (XmlReader reader = XmlReader.Create(new StreamReader(wordFile)))
            {
                XDocument xml = XDocument.Load(reader);
                XElement root = xml.Root;
                foreach (XElement elem in root.Elements())
                {
                    WorldObject obj = new WorldObject();
                    foreach (XElement innerElem in elem.Elements())
                    {
                        XMLParse.AddValueToClassInstance(innerElem, obj);
                    }

                    obj.setTexture(content.Load<Texture2D>(obj.TextureName));
                    
                    obj.objScale = 1f/(obj.Texture.Width) * 100;
                    objIconList.Add(obj);
                }
            }

            //init icon background
            iconObj = new WorldObject();
            iconObj.setTexture(iconBack);
            iconObj.color = Color.Gray;
            iconObj.objScale = 1f / (iconObj.Texture.Width) * 100;
            iconObj.PositionInWorldSpace = iconPos;
        }

        public override void Update(GameTime gameTime)
        {
            prevState = currState;
            currState = Keyboard.GetState();

            //Tab to iterate through object list
            if(currState.IsKeyDown(Keys.Tab) && prevState.IsKeyUp(Keys.Tab) && currState.IsKeyUp(Keys.LeftShift))
            {
                currIcon = (currIcon + 1) % objIconList.Count;
                
            }

            //Shift-tab to iterate backwards through object list
            if ((currState.IsKeyDown(Keys.Tab) && currState.IsKeyDown(Keys.LeftShift)) && 
                (prevState.IsKeyUp(Keys.Tab)))
            {
                
                currIcon = (currIcon - 1);
                if (currIcon < 0)
                    currIcon = objIconList.Count - 1;

            }
            
            currObject = objIconList[currIcon];

            if (cameraPos != camera.Position)
            {
                cameraPos = camera.Position;
                foreach (WorldObject obj in objIconList)
                {
                    float newX = (camera.Position.X - camera.world.Width/2) + (obj.Texture.Width/2) * obj.objScale;
                    float newY = (camera.Position.Y - camera.world.Height/2) + 50;
                    Vector2 pos = new Vector2(newX, newY);
                    obj.PositionInWorldSpace = pos;
                }

            }

            iconObj.PositionInWorldSpace = new Vector2(currObject.PositionInWorldSpace.X, currObject.PositionInWorldSpace.Y);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            camera.Draw(iconObj);
            camera.Draw(currObject);
            base.Draw(gameTime);
        }  
    }
}
