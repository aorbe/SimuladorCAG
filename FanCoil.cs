using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulador_CAG
{
    class FanCoil
    {
        public bool state, mode;
        private double Vset, load;
        public double Tinp, Tout, Vpos;
        
        public FanCoil(double initial_load)
        {
            state = false;
            mode = false;
            load = initial_load;
            Tout = 25.0;
            Vpos = 0;
            Vset = 0;
        }

        public void Control(bool new_state, bool new_mode)
        {
            if (!new_state && state)
            { // Desligando

            }
            if (new_state && !state)
            { // Ligando

            }
            state = new_state;
            if (!new_mode && mode)
            { // Auto -> Manual

            }
            if (new_mode && !mode)
            { // Manual -> Auto

            }
            mode = new_mode;
        }


        public void Calc(double t_set, double v_set)
        {
            // Diferença Tinp-Tout é função de Vpos
            double calc_pos = (100 - Vpos + load) * 0.001;
            if (calc_pos < 0)
                calc_pos = 0;
            Tout += (Tinp - Tout) * calc_pos;
            if (state)
            {
                if (mode)
                {
                    double err = Tout - t_set;
                    double new_pos = Vset + (err * 0.05);
                    if (new_pos > 100)
                        new_pos = 100;
                    if (new_pos < 0)
                        new_pos = 0;
                    Vset = new_pos;
#if DEBUG
                    Console.WriteLine("Vset {0:F}", Vset);
#endif
                }
                else
                {
                    Vset = v_set;
                    if (Vset > 100)
                        Vset = 100;
                    if (Vset < 0)
                        Vset = 0;
                }
            }
            else
            {
                Vset = 0;
            }
            double errPos = Vset - Vpos;
            if (errPos > 1)
                errPos = 1;
            if (errPos < -1)
                errPos = -1;
            Vpos += errPos;
        }
    }

}
