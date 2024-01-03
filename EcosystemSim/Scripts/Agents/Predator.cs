using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosystemSim
{
    public class Predator: Agent
    {
        public Predator(Vector2 pos) {
            position = pos;
            texture = GlobalTextures.textures[TextureNames.Fox];
        }
    }
}
