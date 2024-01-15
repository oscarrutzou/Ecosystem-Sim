using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosystemSim
{
    public enum HerbivoreType
    {
        Rabbit,
    }

    public class Herbivore : Agent
    {
        public HerbivoreType type;
        public Herbivore(Vector2 pos, HerbivoreType type)
        {
            position = pos;
            this.type = type;
            SwitchType();
            currentState = AgentState.IdleWalk;
        }

        private void SwitchType()
        {
            switch (type)
            {
                case HerbivoreType.Rabbit:
                    texture = GlobalTextures.textures[TextureNames.Bunny];
                    break;
            }
        }

        public override void ActionOnTargetFound()
        {
            //Brug a* i stedet for.
            if (target != null && target is Tile)
            {
                currentState = AgentState.Drinking;
                drinkingTimer = 1;
            } else if (target != null && target is Plant)
            {
                currentState = AgentState.Eating;
                eatingTimer = 1;
            }
        }
    }
}
