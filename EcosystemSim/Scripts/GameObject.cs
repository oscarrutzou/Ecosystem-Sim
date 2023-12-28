using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosystemSim
{
    public abstract class GameObject
    {
        public bool isRemoved;


        public virtual void Update() { }


        public virtual void Draw()
        {

        }
    }
}
