using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MassSpring
{
    public static class Utils 
    {
        public static void ApplyParticleToTransform(IList<Transform> trans_list, IList<Particle> particles)
        {
            if(trans_list.Count != particles.Count)
            {
                throw new System.Exception("Transform count is not equal to particles cout");
            }

            for(int i = 0; i < trans_list.Count; ++i)
            {
                trans_list[i].position = particles[i].pos;
            }
        }
    }
}
