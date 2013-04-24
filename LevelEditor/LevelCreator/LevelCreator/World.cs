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
    public class World : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D gridTexture;
        WorldObject gridObj;

        public List<WorldObject> worldObjectList;
        public List<WorldObject> gridList;
        public List<WorldObject> deleteList;

        public Camera camera { get; set; }

        //set to world size
        public float Width { get; set; }
        public float Height { get; set; }
        public int GridHeight;
        public int GridWidth;

        Game mGame;

        public World(Game game)
            : base(game)
        {
            mGame = game;
           
        }

        public override void Initialize()
        {
            gridTexture = mGame.Content.Load<Texture2D>("gridBG");
            GridHeight = gridTexture.Height;
            GridWidth = gridTexture.Width; 
            gridList = new List<WorldObject>();
            worldObjectList = new List<WorldObject>();
            deleteList = new List<WorldObject>();
            createGrid();
            base.Initialize();
        }

        public void createGrid()
        {
            float widthLeft = Width;
            float heightLeft = Height;

            for (float i = 0; i < widthLeft; i += gridTexture.Width)
            {
                for (float j = 0; j < heightLeft; j += gridTexture.Height)
                {
                    gridObj = new WorldObject();
                    gridObj.setTexture(gridTexture);
                    gridObj.PositionInWorldSpace = new Vector2(i + gridTexture.Width/2, j + gridTexture.Height/2);
                    gridList.Add(gridObj);
                }
            }
        }

        //Hacky method; testing. 
        public void destroyGrid()
        {
            gridList = new List<WorldObject>();
        }

        public override void Update(GameTime gameTime)
        {   
            //Updates worldObjectList to remove shift-leftclicked objs
            foreach (WorldObject obj in deleteList)
            {
                worldObjectList.Remove(obj);
            }
            //resets list if list contains already deleted objs
            if(deleteList.Count > 0)
                deleteList = new List<WorldObject>();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (WorldObject grid in gridList)
            {
                camera.Draw(grid);
            }
            foreach (WorldObject obj in worldObjectList)
            {
                camera.Draw(obj);
            }
            base.Draw(gameTime);
        }        
    }
}
