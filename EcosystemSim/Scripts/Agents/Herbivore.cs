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
            if (target != null && target is Tile tile && tile.tileType == TileType.Water)
            {
                if (currentState != AgentState.Drinking && thirstMeter <= amountBeforeSearch)
                {
                    currentState = AgentState.Drinking;
                    drinkingTimer = 1;
                }
                else
                {
                    currentState = AgentState.IdleWalk;
                }
            } else if (target != null && target is Plant)
            {
                currentState = AgentState.Eating;
                eatingTimer = 1;
            }
        }
    }
}
