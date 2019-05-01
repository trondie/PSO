using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO_Final
{
    public class Problem
    {
        public delegate double Fitness(List<double> pos, int dim);
        public delegate void Initialize_delegate(ref List<Particle> pos, int dim);
        public Fitness fitnessDelegate;
        public Initialize_delegate initDelegate;
        private int dim;
        public Problem(string problem, int dim)
        {
            this.dim = dim;
            if (problem.Equals("circle"))
            {
                fitnessDelegate = new Fitness(fitness_circle);
            }
        }
        
        public static double fitness_circle(List<double> pos, int dim)
        {
            double value = 0.0;
            for (int i = 0; i < dim; ++i)
                value += pos[i] * pos[i];

            return Math.Abs(value);
        }
    }

}
