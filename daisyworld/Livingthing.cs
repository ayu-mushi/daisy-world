using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daisyworld
{
    class Livingthing
    {
        double albedo_of_adam; //初期色。値の変異は「アダムの値」から揺らぎすぎる事は無いようにする

        public double albedo; //色素を作るには資源を使わなければならない。色素無しなら0.5、grayとなる
        int max_age;
        int age;
        public int number; // 生物nである.
        protected double[] vs_livingthing_interaction_of_adam;
        public double[] vs_livingthing_interaction; //対生物相互作用: 周りに生物nが居るとき一定確率で寿命UP [-0.5,+0.5]
        public string marker; // 子孫まで付き纏う印
        int numofhabitats = 4;

        public Livingthing()
        {
            age = 0;
        }
        public void reset_age() {
            age = 0;
        }
        public void getYouger() { //わかがえる
            age--;
        }
        public void happy_birthday() //としをとる
        {
            age++;
        }
        public bool is_nows_the_time_to_retire() //もう寿命?
        {
            return age <= max_age;
        }
       
        public Livingthing getchild(Random r)
        {
            Livingthing child = new Livingthing();
            child.marker = this.marker;
            child.age = 0;
            child.max_age = this.max_age;
            child.albedo_of_adam = this.albedo_of_adam;
            child.vs_livingthing_interaction_of_adam = this.vs_livingthing_interaction_of_adam;

            double mut = (r.NextDouble()/4)-(1/8);

            int m1 = r.Next(-1, this.vs_livingthing_interaction_of_adam.Length);
            int m2 = r.Next(-1, this.vs_livingthing_interaction_of_adam.Length);

            child.albedo = this.albedo;
            child.vs_livingthing_interaction = new double[this.vs_livingthing_interaction_of_adam.Length];
            for (int i = 0; i != child.vs_livingthing_interaction.Length; i++) {
                child.vs_livingthing_interaction[i] = this.vs_livingthing_interaction[i];
            }

            if (m1 == -1) {
                if (child.albedo >= 0.5)
                {
                    if (child.albedo + mut < child.albedo_of_adam + 0.25) child.albedo += mut;
                    else child.albedo -= mut;
                }
                else
                {
                    if (child.albedo - mut > child.albedo_of_adam - 0.25) child.albedo -= mut;
                    else child.albedo += mut;
                }
            }
            else
            {
                if (child.vs_livingthing_interaction_of_adam[m1] + mut < child.vs_livingthing_interaction_of_adam[m1] + 0.25)
                {child.vs_livingthing_interaction[m1] += mut;}
                else
                { child.vs_livingthing_interaction[m1] -= mut;}
            }

            if (m2 == -1)
            {
                if (child.albedo >= 0.5)
                {
                    if (child.albedo - mut > child.albedo_of_adam - 0.25) child.albedo -= mut;
                    else child.albedo += mut;
                }
                else
                {
                    if (child.albedo + mut < child.albedo_of_adam + 0.25) child.albedo += mut;
                    else child.albedo -= mut;
                }
            }
            else
            {
                if (child.vs_livingthing_interaction_of_adam[m2] - mut < child.vs_livingthing_interaction_of_adam[m2] - 0.25)
                {child.vs_livingthing_interaction[m2] -= mut;}
                else
                { child.vs_livingthing_interaction[m2] += mut; }
            }
            return child;
        }
    }
    class wdaisy : Livingthing
    {
        public double albedo_of_adam = 0.75;
        public int max_age = 25;
        public wdaisy()
        {
            vs_livingthing_interaction_of_adam = new double[] { 0, 0.75, -0.5, 0.5 };
            number = 0;
            albedo = albedo_of_adam;
            vs_livingthing_interaction = vs_livingthing_interaction_of_adam;
        }    
    }
    class bdaisy : Livingthing
    {
        public double albedo_of_adam = 0.25;
        public int max_age = 25;
        public bdaisy() 
        {
            vs_livingthing_interaction_of_adam = new double[] { 0, 0.75, -0.5, 0.5 };
            number = 1;
            albedo = albedo_of_adam;
            vs_livingthing_interaction = vs_livingthing_interaction_of_adam;
        }
    }
    class rabbit : Livingthing 
    {
        public double albedo_of_adam = 0.5; // gray
        public int max_age = 10;
        public rabbit()
        {
            vs_livingthing_interaction_of_adam = new double[] { 0.5, 0.5, 0.5, -0.5 };
            number = 2;
            albedo = albedo_of_adam;
            vs_livingthing_interaction = vs_livingthing_interaction_of_adam;
        }
    }
    class fox : Livingthing
    {
        public double albedo_of_adam = 0.5; // gray
        public int max_age = 10;
        public fox()
        {
            vs_livingthing_interaction_of_adam = new double[] { 0, 0, 0.7, 0.3 };
            number = 3;
            albedo = albedo_of_adam;
            vs_livingthing_interaction = vs_livingthing_interaction_of_adam;
        }
    }
}