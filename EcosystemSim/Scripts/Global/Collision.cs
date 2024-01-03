using Microsoft.Xna.Framework;

namespace EcosystemSim
{
    static public class Collision
    {
        /// <summary>
        /// Bool if the sender object id colliding with the other gameObject, using a collisionBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="other">The target of a collision check</param>
        /// <returns></returns>
        /// 
        static public bool IntersectBox(GameObject sender, GameObject other)
        {
            //checks if the sender is colliding with the other object
            if (sender == other
                || sender == null
                || other == null) return false;

            return sender.collisionBox.Intersects(other.collisionBox);
        }

        static public bool ContainsBox(GameObject sender, GameObject other)
        {
            //checks if the sender is colliding with the other object
            if (sender == other
                || sender == null
                || other == null) return false;

            return other.collisionBox.Contains(sender.collisionBox);
        }

    }
}
