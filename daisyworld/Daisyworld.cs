using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;

namespace daisyworld
{
    class Daisyworld
    {
        Patch[][] m_daisyworld;
        int n, m;
        int global_temperature = 0;//各局所温度の平均
        int global_time = 0;
        public double solar_luminosity = 0.6;
        String[] markers;
        public OrderedDictionary[] populations;
        public OrderedDictionary dynamics_of_temperature = new OrderedDictionary(); //温度の遷移表
        Random r;
        public double start_p_bdaisies;
        public double start_p_wdaisies;
        public double start_p_rabbits;
        public double start_p_foxes;

        public Daisyworld(int n, int m, 
			double start_p_bdaisies=0.2,
			double start_p_wdaisies=0.2,
			double start_p_rabbits=0.1,
			double start_p_foxes=0.05)
        {
			this.start_p_bdaisies=start_p_bdaisies;
			this.start_p_wdaisies=start_p_wdaisies;
			this.start_p_rabbits=start_p_rabbits;
			this.start_p_foxes=start_p_foxes;
			
            r = new Random();
            m_daisyworld = new Patch[n][];
            this.n = n; this.m = m;

            double factor;
            double wall;
            for (int i = 0; i != n; i++)
            {
                m_daisyworld[i] = new Patch[m];
                for (int j = 0; j != m; j++)
                {
                    m_daisyworld[i][j] = new Patch();
                    m_daisyworld[i][j].i = i;
                    m_daisyworld[i][j].j = j;

                    factor = r.NextDouble();
                    wall = 0;
                    if (factor < (wall += start_p_bdaisies))
                    {
						m_daisyworld[i][j].l = new bdaisy();
                    }
                    else if (factor < (wall += start_p_wdaisies))
                    {
                        m_daisyworld[i][j].l = new wdaisy();
                    }
                    else if (factor < (wall += start_p_rabbits))
                    {
                        m_daisyworld[i][j].l = new rabbit();
                    }
                    else if (factor < (wall += start_p_foxes))
                    {
                        m_daisyworld[i][j].l = new fox();
                    }
                }
            }
        }

        double get_global_temperature()
        {
            double s = 0;
            for (int i = 0; i != n; i++)
            {
                for (int j = 0; j != m; j++)
                {
                    s += this.get(i, j).temperature;
                }
            }
            return s / (n * m);
        }

        public void write_dynamics_of_global_temperature(StreamWriter w) {
            foreach (double t in dynamics_of_temperature.Values) {
                w.Write(t + " ");
            }
            w.WriteLine();
        }

        public void refresh()
        {
            if (markers != null)
            {
                for (int i = 0; i != markers.Length; i++)
                {
                    populations[i].Add(global_time, takeACensusByMaker(markers[i]));
                }
            }
            for (int i = 0; i != n; i++)
            {
                for (int j = 0; j != m; j++)
                {
                    m_daisyworld[i][j].calc_temperature(this);

                    if (m_daisyworld[i][j].isliving())
                    {
                        m_daisyworld[i][j].check_survivability(this, r);
                    }

                    if (m_daisyworld[i][j].isliving())
                    {
                        this.get(i, j).interaction_of_livingthings(this, r);
                    }
                }
            }
            dynamics_of_temperature.Add(global_time, get_global_temperature());

            global_time++;
            solar_luminosity+=0.00001;
        }

        public void refresh(int i)
        {
            while (i--!=0)
            {
                refresh();
            }
        }

        public Livingthing getFirst()
        {
            Patch p;
            for (int i = 0; i != n; i++)
            {
                for (int j = 0; j != m; j++)
                {
                    p = this.get(i, j);
                    if (p.isliving()) { 
                        return p.l;
                    }
                }
            }
            return null;
        }

		public Livingthing[] getFirsts(int N)
        {
			Livingthing[] firsts = new Livingthing[N];
            Patch p;
			int x = 0;
            for (int i = 0; i != n; i++)
            {
                for (int j = 0; j != m; j++)
                {
					if(x==N){return firsts;}
					
                    p = this.get(i, j);
                    if (p.isliving()) { 
						firsts[x] = p.l;
                        x++;
                    }
                }
            }
            return null;
        }
		
        public Patch get(int i, int j)
        {
            int x=i, y=j;
            if (i < 0) { x = n + i; }
            else if (n <= i) { x = i % n; }
            
            if (j < 0) { y = m + j; }
            else if (m <= j) { y = j % m; }

            return m_daisyworld[Math.Abs(x)][Math.Abs(y)];
        }

        int takeACensusByMaker(String marker) //marker付きの数を調べる
        {
            int result = 0;
            for (int i = 0; i != n; i++)
            {
                for (int j = 0; j != m; j++)
                {
                    if (this.get(i,j).l!=null&&this.get(i, j).l.marker == marker)
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        public void setMarker(String[] markers) //markerを付ける
        {
            this.markers = markers;
            populations = new OrderedDictionary[markers.Length];
            for (int i = 0 ; i != markers.Length; i++) 
            {
                populations[i] = new OrderedDictionary();
            }
        }

        public void addNewLivingthing(Livingthing l) 
        {
            for (int i = 0; i != n; i++) 
            {
                for (int j = 0; j != m; j++) 
                {
                    if (!this.get(i, j).isliving()) {
                        this.get(i, j).l = l;
                        return;
                    }
                }
            }
        }

        override public String ToString() {
            string str="";
            for (int i = 0; i != n; i++) 
            {
                if (i != 0) str += System.Environment.NewLine;
                for (int j = 0; j != m; j++) 
                {
                    if (j == 0) str += " ";
                    if (this.get(i, j).l == null) str += ".";
                    else str+=this.get(i, j).l.number;
                }
            }
            return str;
        }
    }
}
