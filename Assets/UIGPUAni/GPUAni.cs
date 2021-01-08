using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIGPUAni
{
    public class GPUAni : MonoBehaviour
    {
        GPUAniGraphic[] graphics;
        private void Awake()
        {
            graphics = gameObject.GetComponentsInChildren<GPUAniGraphic>();
        }

        private void OnEnable()
        {
            Play();
        }

        public void Play()
        {
            foreach(var g in graphics)
            {
                g.Play();
            }

        }
    }
}
