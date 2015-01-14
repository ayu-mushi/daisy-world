using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;

namespace daisyworld
{
    public class Program
    {
        void invasive(Daisyworld dw1, Daisyworld dw2, int n)
        {
            Livingthing[] ls = dw1.getFirsts(n);
			if(ls != null) for (int i = 0; i != n; i++)
            {
                ls[i].marker = "invader";
                dw2.setMarker(new string[]{"invader"});

                ls[i].reset_age();
                dw2.addNewLivingthing(ls[i]);
            }
        }
        int maxval(OrderedDictionary o) {
            int max = 0;
            foreach (int v in o.Values)
            {
                if (max < v) max = v;
            }
            return max;
        }
        Program()
        {
            Daisyworld[] dws = new Daisyworld[200];
            int[] bestofpopulation = new int[dws.Length];

            int n = 10;
            int height = 50, width = 50; 
            for (int i = 0; i != dws.Length/2; i++)
            {
                dws[i] = new Daisyworld(height, width, 0.5, 0.5, 0.0, 0);
                dws[i].refresh(100);
            }
			for (int i = dws.Length/2; i != dws.Length; i++)
            {
                dws[i] = new Daisyworld(height, width, 0.5, 0, 0, 0);
                dws[i].refresh(100);
            }
            for (int i = 0; i != dws.Length / 2; i++)
            {
                invasive(dws[i], dws[i + (dws.Length / 2)], n);
                dws[i + (dws.Length / 2)].refresh(100);
                bestofpopulation[i + (dws.Length / 2)] = maxval(dws[i + (dws.Length / 2)].populations[0]);
            }
            using (StreamWriter w = new StreamWriter(@"daisyresult.txt")) 
            {
                for (int i = dws.Length/2; i != dws.Length; i++) {
                    foreach(int p in dws[i].populations[0].Values)
					{
						w.Write(p+" ");
					}
				w.WriteLine();
				w.WriteLine();
                }
            }
        }
        static void Main(string[] args)
        {
            new Program();
        }
    }
}
