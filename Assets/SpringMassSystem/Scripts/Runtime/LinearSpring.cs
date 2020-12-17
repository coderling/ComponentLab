using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MassSpring
{
    [Serializable]
    public class LinearSpring : IMassSpringData
    {
        public SystemParams sys_params;
        public SystemParams system_params { get { return sys_params; } }
        public Simulator simulator;

        private List<Particle> _particles;
        public IList<Particle> particles { get { return _particles; } }
        private List<Spring> _springs;
        public IList<Spring> springs { get { return _springs; } }

        public LinearSpring()
        {
            sys_params = new SystemParams();
            _particles = new List<Particle>();
            _springs = new List<Spring>();
            simulator = new Simulator();
            simulator.data = this;
        }

        public void AddParticle(Particle p)
        {
            p.pre_pos = p.pos;
            _particles.Add(p);
            int index = _particles.Count - 1;
            if(index > 0)
            {
                var sp = new Spring();
                sp.p1 = _particles[index - 1];
                sp.p2 = _particles[index];
                sp.static_length = (sp.p1.pos - sp.p2.pos).magnitude;
                _springs.Add(sp);
                p.is_pined = false;
            }
            else
            {
                p.is_pined = true;
            }
        }
    }
}
