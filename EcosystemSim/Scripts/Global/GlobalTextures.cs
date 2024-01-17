using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosystemSim
{
    public enum TextureNames
    {
        TestTile,
        TestTileNonWalk,
        TileEmpty,
        TileWater,
        TileGrassy,
        TilePlain,
        GreensGrass,
        GreensMushroom,
        GreensYellowFlower,
        GreensRedFlower,
        Fox,
        Bunny,
        UISearchRad100,
    }

    // Dictionary of all textures
    public static class GlobalTextures
    {
        public static Dictionary<TextureNames, Texture2D> textures { get; private set; }
        public static SpriteFont defaultFont { get; private set; }


        public static void LoadContent()
        {
            ContentManager content = GameWorld.Instance.Content;
            // Load all textures
            textures = new Dictionary<TextureNames, Texture2D>
            {
                {TextureNames.TestTile, content.Load<Texture2D>("World\\TestTile") },
                {TextureNames.TestTileNonWalk, content.Load<Texture2D>("World\\TestTileNonWalkable") },
                {TextureNames.TileEmpty, content.Load<Texture2D>("World\\TileEmpty") },
                {TextureNames.TileWater, content.Load<Texture2D>("World\\TileWater") },
                {TextureNames.TilePlain, content.Load<Texture2D>("World\\TilePlain") },
                {TextureNames.TileGrassy, content.Load<Texture2D>("World\\TileGrassy") },
                {TextureNames.GreensGrass, content.Load<Texture2D>("World\\GreensGrass") },
                {TextureNames.GreensMushroom, content.Load<Texture2D>("World\\TestMushroom") },
                {TextureNames.GreensYellowFlower, content.Load<Texture2D>("World\\YellowFlower") },
                {TextureNames.GreensRedFlower, content.Load<Texture2D>("World\\RedFlower") },

                {TextureNames.Fox, content.Load<Texture2D>("Agents\\tile_fox") },
                {TextureNames.Bunny, content.Load<Texture2D>("Agents\\tile_bunny") },
                {TextureNames.UISearchRad100, content.Load<Texture2D>("UI\\AgentUI\\SearchRad100") },


            };

            // Load all fonts
            defaultFont = content.Load<SpriteFont>("Fonts\\Arial");
        }
    }
}
