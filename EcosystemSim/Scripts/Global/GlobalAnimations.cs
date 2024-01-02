using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EcosystemSim
{
    public enum AnimNames
    {
        TestAnim
    }

    public static class GlobalAnimations
    {
        // Dictionary of all animations
        private static Dictionary<AnimNames, List<Texture2D>> animations = new Dictionary<AnimNames, List<Texture2D>>();
        public static float progress = 0f;

        public static void LoadContent()
        {
            //LoadAnimation(AnimNames.PlayerHandgunIdle, "Player\\Top_Down_Survivor\\handgun\\idle\\survivor-idle_handgun_", 20);
        }

        private static void LoadAnimation(AnimNames animationName, string path, int framesInAnim)
        {
            // Load all frames in the animation
            List<Texture2D> animList = new List<Texture2D>();
            for (int i = 0; i < framesInAnim; i++)
            {
                animList.Add(GameWorld.Instance.Content.Load<Texture2D>(path + i));
            }
            animations[animationName] = animList;

            //-1 since the loading screen icon already has been loaded.
            int totalAnimations = Enum.GetNames(typeof(AnimNames)).Length - 1;
            progress += 1f / totalAnimations; // Update the progress after each animation is loaded
        }

        public static Animation SetAnimation(AnimNames name)
        {
            // Check if the animation exists
            return new Animation(animations[name], name);
        }

    }
}
