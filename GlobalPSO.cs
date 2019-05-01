using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO_Final
{
    public class GlobalPSO : PSO
    {
        private bool inertia_decrease;

        public GlobalPSO(int neighbors, int particles, double lower_lim, double max_lim, int dim, double c1, double c2, double inertia, double min_vel, double max_vel, string problem, bool init_specific, bool plot_results, bool draw, bool inertia_decrease)
            : base(particles, lower_lim, max_lim, dim, c1, c2, inertia, min_vel, max_vel, problem, init_specific, plot_results, draw)
        {
            this.inertia_decrease = inertia_decrease;
        }

        //The following functions are overrided by a particular PSO
        public override void compare_neighbors() 
        {
            double temp_best = double.MaxValue;
            int best_index = 0;
            //Run through particles and find the lowest value
            for (int i = 0; i < swarm.Count(); ++i)
            {
                double temp_particle_best = swarm[i].best_local_fitness;
                if (temp_best > temp_particle_best)
                {
                    temp_best = temp_particle_best;
                    best_index = i;
                }
            }
            //If the current global best value so far is larger, then update the value 
            if (this.current_global_best > temp_best)
            {
                //Set all global values in the swarm to the lowest value
                for (int i = 0; i < swarm.Count(); ++i)
                {
                    swarm[i].best_global_fitness = temp_best;
                    for (int k = 0; k < this.dim; ++k)
                    {
                        swarm[i].best_glob_pos[k] = swarm[best_index].position[k];
                    }
                }
                this.current_global_best = temp_best;
            }
        }

        public override void change_particle_velocity(ref Particle particle) 
        {
            double r1 = rand_gen.NextDouble();
            double r2 = rand_gen.NextDouble();

            for (int i = 0; i < dim; ++i)
            {
                particle.velocity[i] = inertia * particle.velocity[i] + ((C1 * r1 * (particle.best_local_pos[i] - particle.position[i]))
                    + (C2 * r2 * (particle.best_glob_pos[i] - particle.position[i])));
                if (Math.Abs(particle.velocity[i]) < min_velocity)
                {
                    particle.velocity[i] = (particle.velocity[i] > 0.0) ? min_velocity : -min_velocity;
                }
                if (Math.Abs(particle.velocity[i]) > max_velocity)
                {
                    particle.velocity[i] = (particle.velocity[i] > 0.0) ? max_velocity : -max_velocity;
                }
            }  
        }
        //Start the swarm and return a solution
        public double start_swarm_task1(int iterations, double threshold)
        {
            ++this.runs;
            this.fitnesses.Add(current_global_best);
            double inertia_decrease_rate = (inertia - min_inertia) / iterations;
            for (int i = 0; i < iterations; ++i)
            {
                if (draw)
                {
                   // System.Threading.Thread.Sleep(10);
                    form.Invalidate();
                    draw_swarm1();
                }
                compare_all_individual();
                compare_neighbors();
                change_all_velocities();
                change_all_positions();
                ++this.iterations_performed;
                if (this.plot_results)
                {
                    this.fitnesses.Add(current_global_best);
                }
                System.Console.WriteLine(this.current_global_best);
                if (current_global_best < threshold)
                {
                    write_iteration_count();
                    this.initial_inertia = this.inertia;
                    return this.current_global_best;
                }
               // print_fitnesses();
                if (inertia_decrease)
                    this.inertia = (inertia <= min_inertia) ? inertia : (inertia - inertia_decrease_rate);
            }
            this.initial_inertia = this.inertia;
            return this.current_global_best;
        }
        private void write_iteration_count()
        {
            System.Console.WriteLine(" ");
            System.Console.Write("Iterations : ");
            System.Console.Write(this.iterations_performed);
            System.Console.WriteLine(" ");
        }
    }
}
