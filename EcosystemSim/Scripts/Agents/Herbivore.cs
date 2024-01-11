using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosystemSim
{
    public class Herbivore : Agent
    {
        public Herbivore(Vector2 pos)
        {
            position = pos;
            texture = GlobalTextures.textures[TextureNames.Bunny];
            currentState = AgentState.Search;
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
