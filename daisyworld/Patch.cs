using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daisyworld
{
    class Patch
    {
        public double temperature;
        public Livingthing l;
        double albedo_of_surface = 0.40; //地面の反射率
        public int i, j;

        public Patch()
        {
            
        }
        int[] neighborix(int i)
        {
            if (i == 0)
                return new int[]{i - 1, j - 1};
            if (i == 1)
                return new int[]{i - 1, j};
            if (i == 2)
                return new int[]{i - 1, j + 1};
            if (i == 3)
                return new int[]{i, j - 1};
            if (i == 4)
                return new int[]{i, j + 1};
            if (i == 5)
                return new int[]{i + 1, j - 1};
            if (i == 6)
                return new int[]{i + 1, j};
            if (i == 7)
                return new int[]{i + 1, j + 1};
            else
                return null;
        }

        Patch neighbor(Daisyworld dw, int i) 
        {
            int[] neighborixarr = neighborix(i);
            return dw.get(neighborixarr[0], neighborixarr[1]);
        }

        const int numofneighbor=8;

        public void interaction_of_livingthings(Daisyworld dw, Random r)//決められた相互作用の仕方に因って寿命が上がり下がりする
        {
            for (int i = 0; i != numofneighbor; i++) 
            {
                if (this.neighbor(dw, i).isliving() && r.NextDouble() < Math.Abs(this.l.vs_livingthing_interaction[this.neighbor(dw, i).l.number])) {
                    if (this.l.vs_livingthing_interaction[this.neighbor(dw, i).l.number] > 0) {
                        this.l.getYouger(); //寿命をあげる
                    }
                    else if (this.l.vs_livingthing_interaction[this.neighbor(dw, i).l.number] < 0)
                    {
                        this.l.happy_birthday(); //寿命を下げる
                    }
                }
            }
        }

        public void calc_temperature(Daisyworld dw){
            double absorbed_luminosity = 0;
            double local_heating = 0;

            if (this.isliving())
            {
                absorbed_luminosity = (1 - this.l.albedo) * dw.solar_luminosity; //生きている場合生物の吸収-反射値
            }
            else
            {
                absorbed_luminosity = (1 - this.albedo_of_surface) * dw.solar_luminosity;//死んでいる場合地面の吸収-反射値
            }

            if (absorbed_luminosity > 0)
            {
                local_heating = 72 * Math.Log(absorbed_luminosity) + 80;
                // Math.Logは1引数の場合自然対数になる
            }
            else
            {
                local_heating = 80;
            }
            this.temperature = (temperature + local_heating) / 2;
        }

        List<int[]> notlivingNeighborixes(Daisyworld dw) //死んでいる隣人indexes
        {
            List<int[]> m_notlivingNeighbor = new List<int[]>();

            for(int i = 0; i != numofneighbor; i++)
            {
                if (this.neighbor(dw, i).isliving())
                {
                    m_notlivingNeighbor.Add(this.neighborix(i));
                }
            }

            return m_notlivingNeighbor;
        }

        // 生と死
        public void check_survivability(Daisyworld dw, Random r)
        {
            double seed_threshold = 0; //子を産むかの閾値
            int[] seeding_place = new int[2];

            this.l.happy_birthday();

            if (this.l.is_nows_the_time_to_retire())
            {
                this.die();
            }
            else
            {
                seed_threshold =
                    (0.1457 * temperature) - (0.0032 * (Math.Pow(temperature, 2.0)) - 0.6443);
                
                //近傍に在る誰も居ない土地の中からランダムに選択し其処に子を産む
                if (r.NextDouble() < seed_threshold)
                    //r.NextDouble: 0.0から1.0の実数を得る
                {
                    if (this.notlivingNeighborixes(dw).Count != 0)
                    {
                        seeding_place = // oneof 生きていない隣人
                            this.notlivingNeighborixes(dw)[r.Next(0, this.notlivingNeighborixes(dw).Count - 1)];
                        dw.get(seeding_place[0], seeding_place[1]).birth(this.l, r);
                    }
                }
            }
        }

        public bool isliving()
        {
            return this.l != null;
        }

        void die()
        {
            this.l = null;
        }

        public void birth(Livingthing parent, Random r){
            l = parent.getchild(r);
        }
    }
}
