using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MassSpring
{
    public interface IMassSpringData
    {
        IList<Particle> particles { get; }
        IList<Spring> springs { get; }
        SystemParams system_params { get; }
    }

    public class Simulator 
    {
        public IMassSpringData data;

        private float m_time = 0f;
        public void Simulate(float t)
        {
            float dt  = 1.0f / data.system_params.update_rate;
            int simulate_count = 0;
            m_time += t;
            while(m_time >= dt)
            {
                m_time -= dt;
                ++simulate_count;
            }
            for(int i = 0; i < simulate_count; ++i)
            {
                InnerSimulate(dt);
            }

            foreach(var sp in data.springs)
            {
                Debug.DrawLine(sp.p1.pos, sp.p2.pos);
            }
        }

        private void InnerSimulate(float dt)
        {
            //Verlet
            // next_pos = cur_pos + (cur_pos - pre_pos) * (1 - damping) + accel * dt * dt
            foreach(var p in data.particles)
            {
                if (p.is_pined)
                    continue;

                var accel = GetParticleAccel(p);
                Vector3 pos = p.pos + (p.pos - p.pre_pos) * (1 - data.system_params.damping) + accel * dt * dt;
                p.pre_pos = p.pos;
                p.pos = pos;
            }

            foreach(var sp in data.springs)
            {
                var subdir = sp.p1.pos - sp.p2.pos;
                var sub = (subdir).magnitude - sp.static_length;
                subdir.Normalize();
                subdir = subdir * sub;

                if (!sp.p1.is_pined)
                {
                    subdir = subdir / 2;
                    sp.p1.pos -= subdir;
                }
                if (!sp.p2.is_pined)
                    sp.p2.pos += subdir;
            }
        }

        private Vector3 GetParticleAccel(Particle p)
        {
            return (data.system_params.gravity + p.force);
        }
    }
}
