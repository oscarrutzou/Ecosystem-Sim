using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosystemSim
{
    public static class SceneData
    {
        //All gameObjects in the scene
        public static List<GameObject> gameObjects = new List<GameObject>();
        public static List<GameObject> gameObjectsToAdd = new List<GameObject>();

        public static List<Tile> tiles = new List<Tile>();
        public static List<Herbivore> herbivores = new List<Herbivore>();
        public static List<Predator> predators = new List<Predator>();
        public static List<GameObject> defaults = new List<GameObject>();
    }
}
