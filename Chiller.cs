using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulador_CAG
{
    class Chiller
    {
        public bool state;
        private double power, p_set, e_inp;
        public double Tinp, Tout;

        public Chiller()
        {
            state = false;
            power = 0;
            p_set = 0;
        }

        public void Control(bool new_state)
        {
            state = new_state;
            if (!state)
                power = 0;
        }

        public void Calc(double t_set, double load)
        {
            if (load <= 0)
                load = 0.01;

            if (state)
            {
                double err = Tout - t_set;
                p_set = p_set + (err * 0.5);
                if (p_set > 10000)
                    p_set = 10000;
                if (p_set < 0)
                    p_set = 0;
#if DEBUG
                Console.WriteLine("Pow {0:F}", p_set);
#endif
            }
            else
            {
                p_set = 0;
            }
            
            double errPow = p_set - power;
            if (errPow > 10)
                errPow = 10;
            if (errPow < -10)
                errPow = -10;
            power += errPow;

            double dT = power / load;
            if (dT > 4)
            {
                power--;
                dT = 4;
            }
            if (power <= 0)
                power = 0;
#if DEBUG
            Console.WriteLine("dT {0:F}", dT);
#endif
            Tout = Tinp - dT;
        }
    }
}
