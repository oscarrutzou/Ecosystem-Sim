using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EcosystemSim
{
    public enum Scenes
    {
        MainMenu,
        LoadingScreen,
        GameScene,
        EndMenu,
    }
    public enum LayerDepth
    {
        Background,
        Grid,
        Agents,
        ScreenOverLay,
        GuiObjects,
        GuiText,
        FullOverlay
    }

    public abstract class Scene
    {
        // We have a data stored on each scene, to make it easy to add and remove gameObjects
        public bool hasFadeOut;
        public bool isPaused;

        public abstract void Initialize();

        /// <summary>
        /// The base update on the scene handles all the gameobjects and calls Update on them all. 
        /// </summary>
        public virtual void Update()
        {
            LinkedList<GameObject> gameObjects = new LinkedList<GameObject>(SceneData.gameObjects);

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.isRemoved)
                {
                    gameObjects.Remove(gameObject);
                }
                else
                {
                    //gameObject.animation?.AnimationUpdate();
                    gameObject.Update();
                }
            }

            SceneData.gameObjects = new List<GameObject>(gameObjects);
        }

        public virtual void DrawInWorld()
        {
            //DrawSceenColor();

            //// Draw all GameObjects that is not Gui in the active scene.
            foreach (GameObject gameObject in SceneData.gameObjects)
            {
                //if (gameObject is not Gui)
                //{
                gameObject.Draw();
                //}
            }
        }

        public virtual void DrawOnScreen()
        {
            // Draw all Gui GameObjects in the active scene.
            //foreach (GameObject guiGameObject in SceneData.guis)
            //{
            //    guiGameObject.Draw();
            //}
            //DrawCursor();
        }



        #region Instantiate and Object LayerDepths
        public void Instantiate(GameObject gameObject)
        {
            SceneData.gameObjects.Add(gameObject);

        }

        #endregion

        #region Sort Objects


        private void RemoveObjectsFromList()
        {
            SceneData.defaults.RemoveAll(defultsObj => defultsObj.isRemoved);
        }

        /// <summary>
        /// Add all lists to the the gameObjects list that contains all gameObjects on the scene
        /// </summary>
        private void AddObjectsToList()
        {

            SceneData.gameObjects.AddRange(SceneData.defaults);
        }

        ///// <summary>
        ///// Sorts the gameObjects and adds them to the correct lists
        ///// </summary>
        ///// <param name="gameObjectsToAdd"></param>
        private void SortIntoCategories(GameObject gameObj)
        {
            switch (gameObj)
            {

                default:
                    SceneData.defaults.Add(gameObj);
                    break;
            }
        }
        #endregion
        //private void DrawSceenColor()
        //{
        //    if (Global.currentScene == Global.world.scenes[Scenes.MainMenu]
        //        || Global.currentScene == Global.world.scenes[Scenes.LoadingScreen]
        //        || Global.currentScene == Global.world.scenes[Scenes.EndMenu])
        //    {
        //        Global.graphics.GraphicsDevice.Clear(Color.DarkRed);
        //    }
        //    else if (Global.currentScene == Global.world.scenes[Scenes.ElevatorMenu])
        //    {
        //        Global.graphics.GraphicsDevice.Clear(Color.Silver);
        //    }
        //    else
        //    {
        //        Global.graphics.GraphicsDevice.Clear(Color.Black);
        //    }
        //}

        //private void DrawCursor()
        //{
        //    Vector2 pos = new Vector2(InputManager.mousePositionOnScreen.X - GlobalTextures.textures[TextureNames.CrossHair].Width / 2, InputManager.mousePositionOnScreen.Y - GlobalTextures.textures[TextureNames.CrossHair].Height / 2);
        //    Global.spriteBatch.Draw(GlobalTextures.textures[TextureNames.CrossHair], pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        //}


    }
}
