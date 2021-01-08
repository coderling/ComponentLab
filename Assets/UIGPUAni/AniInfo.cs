using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIGPUAni
{
    public class GPUAnimation :ScriptableObject
    {
        public Texture2D animation_texture;
        public Material material;
        public Vector2Int range;
        public float space_width;
    }
}
