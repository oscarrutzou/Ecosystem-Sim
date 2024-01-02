using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EcosystemSim
{
    public class Animation
    {
        public AnimNames animationName { get; private set; }
        public List<Texture2D> frames { get; private set; }
        public int currentFrame { get; private set; }
        public Action onAnimationDone;
        public float frameRate = 20f;
        private float frameDuration;
        private float timer;

        public Animation(List<Texture2D> frames, AnimNames animationName)
        {
            this.frames = frames;
            currentFrame = 0;
            this.animationName = animationName;
        }


        public void AnimationUpdate()
        {
            // Calculate the frame duration based on the frame rate
            frameDuration = 1f / frameRate;

            // Add the elapsed time since the last frame to the timer
            timer += (float)GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds;
            if (timer > frameDuration)
            {
                timer -= frameDuration;
                currentFrame = (currentFrame + 1) % frames.Count;
                CheckAnimationDone();
            }
        }

        /// <summary>
        /// Check if animation is done
        /// </summary>
        private void CheckAnimationDone()
        {
            if (currentFrame == frames.Count - 1)
            {
                onAnimationDone?.Invoke();
            }
        }
    }
}
