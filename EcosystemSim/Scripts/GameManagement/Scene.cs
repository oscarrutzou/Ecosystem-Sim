using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

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
            AddNewGameObjects();

            // Use List instead of LinkedList
            List<GameObject> gameObjects = new List<GameObject>(SceneData.gameObjects);
            List<GameObject> objectsToRemove = new List<GameObject>();

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.isRemoved)
                {
                    objectsToRemove.Add(gameObject);
                }
                else
                {
                    if (InputManager.mouseOutOfBounds) break;
                    gameObject.animation?.AnimationUpdate();
                    gameObject.Update();
                }
            }

            foreach (var gameObject in objectsToRemove)
            {
                gameObjects.Remove(gameObject);
                RemoveFromCategory(gameObject);
            }

            SceneData.gameObjects = new List<GameObject>(gameObjects);
        }

        private void AddNewGameObjects()
        {
            foreach (GameObject objectToAdd in SceneData.gameObjectsToAdd)
            {
                SceneData.gameObjects.Add(objectToAdd);
                AddToCategory(objectToAdd);
            }

            SceneData.gameObjectsToAdd.Clear();
        }

        private void AddToCategory(GameObject gameObject)
        {
            switch (gameObject)
            {
                case Tile tile:
                    SceneData.tiles.Add(tile);
                    break;
                case Plant plant:
                    SceneData.plants.Add(plant);
                    break;
                case Herbivore herbivore:
                    SceneData.herbivores.Add(herbivore);
                    break;
                case Predator predator:
                    SceneData.predators.Add(predator);
                    break;
                default:
                    SceneData.defaults.Add(gameObject);
                    break;
            }
        }

        private void RemoveFromCategory(GameObject gameObject)
        {
            switch (gameObject)
            {
                case Tile tile:
                    SceneData.tiles.Remove(tile);
                    break;
                case Plant plant:
                    SceneData.plants.Remove(plant);
                    break;
                case Herbivore herbivore:
                    SceneData.herbivores.Remove(herbivore);
                    break;
                case Predator predator:
                    SceneData.predators.Remove(predator);
                    break;
                default:
                    SceneData.defaults.Remove(gameObject);
                    break;
            }
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


    }
}
