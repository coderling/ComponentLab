using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MassSpring;

namespace MassSpring.Test
{
    public class TestLinearSpring : MonoBehaviour
    {
        public LinearSpring spring;
        public GameObject[] particle_gameobjects;
        private List<Transform> trans_list;
        public GameObject root;
        private void Awake()
        {
            trans_list = new List<Transform>();
            foreach(var gb in particle_gameobjects)
            {
                var p = new Particle();
                p.pos =  gb.transform.position;
                spring.AddParticle(p);
                trans_list.Add(gb.transform);
            }
        }

        private void Update()
        {
            var root_p = spring.particles[0];
            root_p.pre_pos = root_p.pos;
            root_p.pos = root.transform.position; 
        }

        private void LateUpdate()
        {
            spring.simulator.Simulate(Time.deltaTime);
            Utils.ApplyParticleToTransform(trans_list, spring.particles);
        }
    }
}
