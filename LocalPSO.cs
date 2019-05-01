using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO_Final
{
    class LocalPSO : PSO
    {
        private int neighborhood_count;
        private bool inertia_decrease;

        public LocalPSO(int neighbors, int particles, double lower_lim, double max_lim, int dim, double c1, double c2, double inertia, double min_vel, double max_vel, string problem, bool init_specific, bool plot_results, bool draw, bool inertia_decrease)
         : base(particles, lower_lim, max_lim, dim, c1, c2, inertia, min_vel, max_vel, problem, init_specific, plot_results, draw){
            this.neighborhood_count = neighbors + 1;
            this.inertia_decrease = inertia_decrease;
        }
        /* The following functions are overrided by this particular PSO */

        public override void compare_neighbors() 
        {
            List<int> closest_indices = new List<int>();

            for (int p = 0; p < swarm.Count(); ++p)
            {
                double reference_length = 0.0;
                for (int k = 0; k < dim; ++k)
                    reference_length += (swarm[p].position[k] * swarm[p].position[k]);
                reference_length = Math.Abs(reference_length);
                for (int i = 0; i < neighborhood_count - 1; ++i)
                {
                    double shortest = double.MaxValue;
                    for (int p_c = 0; p_c < swarm.Count(); ++p_c)
                    {
                        if (p == p_c)
                            continue;
                        if (closest_indices.Contains(p_c))
                            continue;

                        double temp_len = 0.0;
                        for (int k = 0; k < dim; ++k)
                        {
                            double value = swarm[p_c].position[k] - swarm[p].position[k];
                            temp_len += (value * value);
                        }
                        temp_len = Math.Sqrt(temp_len);
                        if (temp_len < shortest)
                        {
                            closest_indices.Add(i);
                            shortest = temp_len;
                        }
                    }
                }
                //Now we have the neighbors - Update
                List<Particle> neighbors = new List<Particle>();
                for (int i = 0; i < closest_indices.Count(); ++i)
                {
                    neighbors.Add(swarm[closest_indices[i]]);
                }
                Particle temp = swarm[p];
                update_neighborhood_particle(ref neighbors, ref temp);
                swarm[p] = temp;
            }
        }
        public override void update_neighborhood_particle(ref List<Particle> neighbors, ref Particle particle) 
        {
            double particle_fitness = this.problem.fitnessDelegate.Invoke(particle.position, dim);
           
            for (int n_p = 0; n_p < neighbors.Count(); ++n_p)
            {
                double temp_fitness = this.problem.fitnessDelegate.Invoke(neighbors[n_p].position, dim);
                if (particle.best_global_fitness > temp_fitness)
                {
                    particle.best_global_fitness = temp_fitness;
                    if (temp_fitness < this.current_global_best)
                        this.current_global_best = temp_fitness;
                    for (int i = 0; i < dim; ++i)
                        particle.best_glob_pos[i] = neighbors[n_p].position[i];
                }
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
            double inertia_decrease_rate = 3.0 * ((inertia - min_inertia) / iterations);
            for (int i = 0; i < iterations; ++i)
            {
                if (draw)
                {
                    System.Threading.Thread.Sleep(10);
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
                if (inertia_decrease)
                    this.inertia -= (inertia <= min_inertia) ? inertia : inertia_decrease_rate;
            }
            this.inertia = this.initial_inertia;
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
