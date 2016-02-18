using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FootballHedge
{
    class ComboBets
    {
        public void FindOptimal()
        {
            int count = 3;
            double[,] koef = new double[count, 2];
            koef[0, 0] = 2.95;
            koef[0, 1] = 2.9;
            koef[1, 0] = 3.6;
            koef[1, 1] = 3.5;
            koef[2, 0] = 2.8;
            koef[2, 1] = 2.7;
            //koef[3, 0] = 3.2;
            //koef[3, 1] = 3.1;

            double s0 = 200;
            double[] rez = new double[count];

            double Koef = 1;
            for (int i = 0; i < count; i++) { Koef *= koef[i, 0]; }

            double TotalLay = 1;
            for (int i = 0; i < count; i++) { TotalLay *= koef[i, 1]; }

            rez[0] = s0 * Koef / (1.0101*TotalLay);

            for (int i = 1; i < count; i++)
            {
                double buf = 0;
                if (i > 1)
                {
                    for (int j = i - 1; j >= 1; j--)
                    {
                        buf += rez[j] * (koef[j, 1] - 1);
                    }
                }
                if(i == count-1) rez[i] = (s0 * Koef - rez[0] * koef[0, 1] - buf) / (koef[i, 1] - 1);
                else  rez[i] = (rez[0]*koef[0,1]+buf);

                Debug.WriteLine("calc " + i.ToString() + "  " + buf.ToString() + "  " + rez[i].ToString());
            }
            for (int i = 0; i < count; i++)
            {
                Debug.WriteLine(i.ToString() + ": " + rez[i].ToString());
            }
        }
    }
}
