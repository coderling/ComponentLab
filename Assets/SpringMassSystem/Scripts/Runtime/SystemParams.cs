using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MassSpring
{
    [Serializable]
    public class SystemParams 
    {
        public Vector3 gravity = Vector3.up * -9.8f;
        public float damping = 0f;
        public float update_rate = 60f;
    }
}
