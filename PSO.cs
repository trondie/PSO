using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace PSO_Final
{
    public class Particle
    {
        public List<double> position;
        public List<double> velocity;
        public List<double> best_glob_pos;
        public List<double> best_local_pos;
        public double best_global_fitness;
        public double best_local_fitness;

        public Particle(ref List<double> position, int dimension)
        {
            this.position = new List<double>(position);
            this.best_glob_pos = new List<double>(position);
            this.best_local_pos = new List<double>(position);
        }
        public Particle(ref List<double> position, ref List<double> velocity, int dimension)
        {
            this.position = new List<double>(position);
            this.velocity = new List<double>(velocity);
            this.best_glob_pos = new List<double>(position);
            this.best_local_pos = new List<double>(position);
        }
    }
    public class PSO
    {
        //The swarm and its problem
        protected List<Particle> swarm;
        protected List<double> solution;
        public Problem problem;
        
        /* Parameters common for both PSOs */
        protected int particles;
        protected double lower_lim;
        protected double max_lim;
        protected int dim;
        protected double C1;
        protected double C2;
        protected double current_global_best;
        protected double inertia;
        protected double min_velocity;
        protected double max_velocity;
        protected const double min_inertia = 0.4;
        protected double initial_inertia;

        //Rand gen
        protected Random rand_gen;

        //Graphics related objects (Used only for 2D domains)
        public Form1 form;
        System.Drawing.Rectangle rectangle;
        System.Drawing.Graphics graphics;

        //Plot related fields
        public bool plot_results;
        public List<double> fitnesses;
        public int iterations_performed;
        public int runs;
        public bool draw;

        public PSO(int particles, double lower_lim, double max_lim, int dim, double c1, double c2, double inertia, double min_vel, double max_vel, string problem, bool init_specific, bool plot_results, bool draw)
        {
            this.particles = particles;
            rand_gen = new Random();
            swarm = new List<Particle>();
            solution = new List<double>();
            this.lower_lim = lower_lim;
            this.max_lim = max_lim;
            this.dim = dim;
            this.C1 = c1;
            this.C2 = c2;
            this.inertia = inertia;
            this.initial_inertia = inertia;
            this.max_velocity = max_vel;
            this.min_velocity = min_vel;
            this.current_global_best = double.MaxValue;
            //Initialize graphics (Only used for 2 dimensional problems)
            if (draw)
            {
                form = new Form1();
                form.Size = new System.Drawing.Size(1024, 768);
                form.Show();
                graphics = form.CreateGraphics();
                graphics.TranslateTransform((form.Width) / 2, (form.Height) / 2);
            }
            //Set the problem
            this.problem = new Problem(problem, dim);
            //Set if the results are to be plotted
            this.plot_results = plot_results;
            initialize(dim, particles, lower_lim, max_lim);

            if (this.plot_results)
            {
                this.fitnesses = new List<double>();
            }
            this.iterations_performed = 0;
            this.runs = 0;
            this.draw = draw;
        }

        //The following functions are overrided by a particular PSO
        public virtual void compare_neighbors() { }
        public virtual void update_neighborhood_particle(ref List<Particle> neighbors, ref Particle particle) { }
        public virtual void change_particle_velocity(ref Particle particle) { }

        private void initialize(int dim, int particles, double lower_lim, double max_lim)
        {
            for (int i = 0; i < particles; ++i)
            {
                List<double> temp_pos = new List<double>();
                for (int k = 0; k < dim; ++k)
                {
                    double rand_posi = (max_lim - lower_lim) * rand_gen.NextDouble() + lower_lim;
                    temp_pos.Add(rand_posi);
                }
                List<double> temp_velocity = new List<double>();
                for (int k = 0; k < dim; ++k)
                {
                    double rand_velocityi = (max_lim - lower_lim) * rand_gen.NextDouble() + lower_lim;
                    temp_velocity.Add(rand_velocityi);
                }

                Particle new_particle = new Particle(ref temp_pos, ref temp_velocity, dim);
                //Initially set the fitnesses to maximum, and use the fitness delegate to get the requested problem
                new_particle.best_local_fitness = problem.fitnessDelegate.Invoke(new_particle.position, dim);
                new_particle.best_global_fitness = problem.fitnessDelegate.Invoke(new_particle.position, dim);
                swarm.Add(new_particle);
            }
            find_first_best_global();
        }

        //The following functions are performed for all PSOs in this exercise
        protected void compare_individual_best_i(ref Particle particle)
        {
            double fitness = this.problem.fitnessDelegate.Invoke(particle.position, dim);
            if (particle.best_local_fitness > fitness)
            {
                particle.best_local_fitness = fitness;
                //Also update position
                for (int i = 0; i < this.dim; ++i)
                {
                    particle.best_local_pos[i] = particle.position[i];
                }
            }
        }
        //Update the individual positions for all particles in the swarm 
        protected void compare_all_individual()
        {
            for (int i = 0; i < swarm.Count(); ++i)
            {
                Particle temp = swarm[i];
                compare_individual_best_i(ref temp);
                swarm[i] = temp;
            }
        }
        //Requires that velocity is updated to the newest step
        protected void change_particle_position(ref Particle particle)
        {
            for (int i = 0; i < dim; ++i)
            {
                particle.position[i] = particle.position[i] + particle.velocity[i];
            }
        }

        protected void change_all_velocities()
        {
            for (int i = 0; i < swarm.Count(); ++i)
            {
                Particle temp = swarm[i];
                change_particle_velocity(ref temp);
                swarm[i] = temp;
            }
        }
        protected void change_all_positions()
        {
            for (int i = 0; i < swarm.Count(); ++i)
            {
                Particle temp = swarm[i];
                change_particle_position(ref temp);
                swarm[i] = temp;
            }
        }
        protected void print_swarm()
        {
            for (int i = 0; i < swarm.Count(); ++i)
            {
                for (int k = 0; k < dim; ++k)
                {
                    System.Console.Write(swarm[i].position[k]);
                    System.Console.Write(" ");
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine("--------------");
        }
        protected void print_fitnesses()
        {
            for (int i = 0; i < swarm.Count(); ++i)
            {

                System.Console.Write(this.problem.fitnessDelegate.Invoke(swarm[i].position, dim));
                System.Console.Write(" ");

                System.Console.WriteLine();
            }
            System.Console.WriteLine("--------------");
        }
        protected void find_first_best_global()
        {
            for (int i = 0; i < swarm.Count(); ++i)
            {
               double particle_fitness = this.problem.fitnessDelegate.Invoke(swarm[i].position, dim);
               if (this.current_global_best > particle_fitness)
                   this.current_global_best = particle_fitness;
            }
        }
        protected void draw_swarm1()
        {
            SolidBrush brush = new SolidBrush(Color.Red);
            if (dim == 1)
            {
                //Draw swarm
                graphics.Clear(Color.White);
                rectangle = new System.Drawing.Rectangle(new Point(0), new Size(6, 6));
                graphics.FillEllipse(brush, rectangle);
                graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
                brush.Color = Color.Blue;
                for (int i = 0; i < swarm.Count(); ++i)
                {
                    rectangle = new System.Drawing.Rectangle(new Point((int)(swarm[i].position[0] * 10)),
                        new Size(4, 4));

                    graphics.FillEllipse(brush, rectangle);
                    graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
                }
            }
            else if (dim == 2)
            {
                //Draw swarm
                graphics.Clear(Color.White);
                rectangle = new System.Drawing.Rectangle(new Point(0, 0), new Size(6, 6));
                graphics.FillEllipse(brush, rectangle);
                graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
                brush.Color = Color.Blue;
                for (int i = 0; i < swarm.Count(); ++i)
                {
                    rectangle = new System.Drawing.Rectangle(new Point((int)(swarm[i].position[0] * 10),
                        (int)(swarm[i].position[1] * 10)),
                        new Size(4, 4));

                    graphics.FillEllipse(brush, rectangle);
                    graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
                }
            }
            else if (dim == 3)
            {
                //Draw swarm
                graphics.Clear(Color.White);
                rectangle = new System.Drawing.Rectangle(new Point(0, 0), new Size(6, 6));
                graphics.FillEllipse(brush, rectangle);
                graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
                brush.Color = Color.Blue;
                for (int i = 0; i < swarm.Count(); ++i)
                {
                    int val = (Math.Abs((int)swarm[i].position[2]) < 1) ? (int)swarm[i].position[2] * 10 : (int)swarm[i].position[2];
                    Size size = new Size(val, val);
                    rectangle = new System.Drawing.Rectangle(new Point((int)(swarm[i].position[0] * 10),
                        (int)(swarm[i].position[1] * 10)),
                        size);

                    graphics.FillEllipse(brush, rectangle);
                    graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
                }
            }
        }
    }
}
