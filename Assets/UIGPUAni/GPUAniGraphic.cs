using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIGPUAni
{
    [RequireComponent(typeof(Graphic))]
    public class GPUAniGraphic : BaseMeshEffect, IMaterialModifier
    {
        public GPUAnimation gpu_animation;
        public int offset;
        public float animation_length;

        public void Play()
        {
            if (graphic != null)
            {
                graphic.SetVerticesDirty();
            }
        }

        static UIVertex tm_vertex = new UIVertex();
        public override void ModifyMesh(VertexHelper vh)
        {
            // uv1 = (animation_length, offset)
            Vector3 normal = new Vector3(animation_length, offset, Time.timeSinceLevelLoad);
            int index_count = vh.currentVertCount;
            for(int i = 0; i < index_count; ++i)
            {
                vh.PopulateUIVertex(ref tm_vertex, i);
                tm_vertex.normal = normal;
                vh.SetUIVertex(tm_vertex, i);
            }
        }
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            return gpu_animation.material;
        }
    }

}
