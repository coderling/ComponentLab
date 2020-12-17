using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MassSpring
{
    public class Particle
    {
        public Vector3 pos;
        public Vector3 pre_pos;
        public float m = 1;
        public Vector3 force;
        public bool is_pined = false;
    }
}
