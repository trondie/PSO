using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace PSO_Final
{

    /* BINARY PARTICLE SWARM OPTIMIZATION ON THE KNAPSACK */

    class BParticle
    {
        //Represents the indices that are set to one. All other indices are thus zero.
        public List<int> position;
        public List<double> velocities;

        public List<int> best_glob_pos;
        public List<int> best_local_pos;

        public double best_global_fitness;
        public double best_local_fitness;
        public double current_fitness;
        public double acc_weight;
        public double acc_volume;
        public BParticle(int dim)
        {
            position = new List<int>(new int[dim]);
            velocities = new List<double>(new double[dim]);
            best_glob_pos = new List<int>(new int[dim]);
            best_local_pos = new List<int>(new int[dim]);
            best_local_fitness = 0.0;
            best_global_fitness = 0.0;
            acc_weight = 0.0;
            acc_volume = 0.0;
        }
    }
    class BPSO
    {
        private double min_velocity;
        private double max_velocity;
        public const int WEIGHT_LIM = 1000;
        public const int VOLUME_LIM = 250;
        private const int MIN_VOL = 1;
        private const int MAX_VOL = 5;
        public string file = "knapsackMedium.txt";
        public double track_weight;
        public List<BParticle> swarm;
        public List<double> values;
        public List<double> weights;
        public List<double> volumes;
        public int dim;
        public int num_particles;
        public Random rand_gen;

        public int lines = 2000;

        private double C1, C2, inertia, min_inertia;
        private bool inertia_decrease, k_volumes;
        private double current_global_best;
        private double current_best_value;
        private List<int> current_global_best_position;
        public List<double> fitnesses;

        public BPSO(int dim, double min_velocity, double max_velocity, int num_particles, double inertia, double min_inertia, bool inertia_decrease, double c1, double c2, bool k_volumes)
        {
            this.values = new List<double>();
            this.weights = new List<double>();
            this.swarm = new List<BParticle>();
            this.dim = dim;
            this.num_particles = num_particles;
            this.rand_gen = new Random();
            this.C1 = c1;
            this.C2 = c2;
            this.inertia = inertia;
            this.min_inertia = min_inertia;
            this.min_velocity = min_velocity;
            this.max_velocity = max_velocity;
            this.current_global_best_position = new List<int>(new int[dim]);
            this.track_weight = 0.0;
            this.inertia_decrease = inertia_decrease;
            this.k_volumes = k_volumes;
            this.fitnesses = new List<double>();
            this.current_best_value = 0.0;
            read_data(this.file);
            if (k_volumes)
            {
                this.volumes = new List<double>();
                initialize_volumes();
            }
            
            initialize_particles();
            int d = 2;
            int r = d * 2;
        }
        
        public double fitness(ref BParticle particle)
        {
            double value = 0.0;
            double weight = 0.0;
            double fitness = 0.0;

            for (int i = 0; i < dim; ++i)
            {
                if (particle.position[i] == 1)
                {
                    value += values[i];
                    weight += weights[i];                 
                }
            }
            fitness = value - (0.1 * weight);
            if (fitness > this.current_global_best)
                this.current_best_value = value;

            return fitness;
        }
        private double fitness_volumes(ref BParticle particle)
        {
            double value = 0.0;
            double weight = 0.0;
            double volumes = 0.0;
            double fitness = 0.0;
            for (int i = 0; i < dim; ++i)
            {
                if (particle.position[i] == 1)
                {
                    value += values[i];
                    weight += weights[i];
                    volumes += this.volumes[i];
                }
            }
            fitness += value - (0.1 * weight) - (0.1 * volumes);
            if (fitness > this.current_global_best)
                this.current_best_value = value;

            return fitness;
        }
        public double sigmoid(double value)
        {
            return ( 1.0 / (1.0 + Math.Exp(-value)) );
        }
        //Changes a particles velocity, and updates the position at the same time
        public void change_particle_velocity_and_position(ref BParticle particle)
        {


            for (int i = 0; i < dim; ++i)
            {
                double r1 = rand_gen.NextDouble();
                double r2 = rand_gen.NextDouble();
                double v_i = particle.velocities[i]; 
                v_i = inertia * v_i + ((C1 * r1 * (particle.best_local_pos[i] - particle.position[i]))
                    + (C2 * r2 * (particle.best_glob_pos[i] - particle.position[i])));

                if (v_i < min_velocity)
                {
                    v_i = min_velocity;
                }
                if (v_i > max_velocity)
                {
                    v_i = max_velocity;
                }
                particle.velocities[i] = v_i;
                //Updates the particle position at i
                change_particle_position(ref particle, v_i, i);
            }  
        }
        private void set_position_bit(ref BParticle particle, int index)
        {
            if (this.k_volumes)
            {
                if (particle.position[index] == 0)
                {
                    if ( ((particle.acc_weight + weights[index]) > WEIGHT_LIM) || ((particle.acc_volume + volumes[index]) > VOLUME_LIM) )
                    {
                        //Do nothing 
                    }
                    else
                    {
                        particle.position[index] = 1;
                        particle.acc_weight += weights[index];
                        particle.acc_volume += volumes[index];
                    }
                }
            }
            else
            {
                if (particle.position[index] == 0)
                {
                    if ((particle.acc_weight + weights[index]) > WEIGHT_LIM)
                    {
                        //Do nothing 
                    }
                    else
                    {
                        particle.position[index] = 1;
                        particle.acc_weight += weights[index];
                    }
                }
            }
        }
        private void unset_position_bit(ref BParticle particle, int index)
        {
            if (particle.position[index] == 1)
            {
                if (this.k_volumes)
                    particle.acc_volume -= volumes[index];
                particle.acc_weight -= weights[index];
            }
            particle.position[index] = 0;
          
        }
        //Performs the check at "bit" position i
        private void change_particle_position(ref BParticle particle, double v_i, int index)
        {
            double sigmoid_value = sigmoid(v_i);
            double rand_value = rand_gen.NextDouble();
            if (rand_value < sigmoid_value)
            {
                set_position_bit(ref particle, index);
            }
            else
            {
                unset_position_bit(ref particle, index);
            }
        }
        private void update_all_velocities_and_positions()
        {
            for (int i = 0; i < swarm.Count(); ++i)
            {
                BParticle temp_particle = swarm[i];
                change_particle_velocity_and_position(ref temp_particle);
                swarm[i] = temp_particle;
            }
        }
        protected void compare_individual_best_i(ref BParticle particle)
        {
            double fitness_i = (this.k_volumes) ? fitness_volumes(ref particle) : fitness(ref particle);
            particle.current_fitness = fitness_i;
            if (fitness_i > particle.best_local_fitness)
            {
                particle.best_local_fitness = fitness_i;
                replace_int_lists(ref particle.position, ref particle.best_local_pos);
            }
        }
        //Update the individual positions for all particles in the swarm 
        protected void compare_all_individual()
        {
            for (int i = 0; i < swarm.Count(); ++i)
            {
                BParticle temp = swarm[i];
                compare_individual_best_i(ref temp);
                swarm[i] = temp;
            }
        }
        //Compare all neighbours (In this case : all particles)
        public void compare_neighbors()
        {
            double temp_best = 0.0;
            int best_index = 0;
            //Run through particles and find the highest value
            for (int i = 0; i < swarm.Count(); ++i)
            {
                double particle_current_fitness = swarm[i].current_fitness;
                if (temp_best < particle_current_fitness)
                {
                    temp_best = particle_current_fitness;
                    best_index = i;
                }
            }
            //If the best position is better than the best global position so far, update the global best fitness and position
            if (temp_best > this.current_global_best)
            {
                replace_int_lists(ref swarm[best_index].position, ref this.current_global_best_position);
                //Set all global values in the swarm to the highest value
                for (int i = 0; i < swarm.Count(); ++i)
                {
                    swarm[i].best_global_fitness = temp_best;
                   // swarm[i].best_glob_pos.AddRange(this.current_global_best_position);
                    replace_int_lists(ref this.current_global_best_position, ref swarm[i].best_glob_pos);
                }
                this.current_global_best = temp_best;
                
            }
        }

        private void replace_int_lists(ref List<int> from, ref List<int> to)
        {
            for (int i = 0; i < dim; ++i)
            {
                 to[i] = from[i];
            }
        }
        private void read_data(string file)
        {
            string line;
            int count = 0;
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    //Read the value and weights
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (count == this.lines)
                            break;
                        ++count;

                        string[] read_values = line.Split(' ');
                        double value = Convert.ToDouble(read_values[0]);
                        double weight = Convert.ToDouble(read_values[1]);
                        this.weights.Add(weight);
                        this.values.Add(value);
                      
                    }
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            this.lines = count;
            this.dim = count;
        }
        public void initialize_particles()
        {
            for (int i = 0; i < this.num_particles; ++i)
            {
                BParticle temp_particle = new BParticle(this.dim);
                //Initialize position bit vector
                if (this.k_volumes)
                    generate_random_bitvec_volumes(ref temp_particle.position);
                else
                    generate_random_bitvec(ref temp_particle.position);
                init_particle_weight_count(ref temp_particle);
                if (this.k_volumes)
                    init_particle_volume_count(ref temp_particle);
                //Set initial velocities
                initialize_velocities(ref temp_particle);
                //Set the first best fitness for the particle
                temp_particle.best_local_fitness = (this.k_volumes) ? fitness_volumes(ref temp_particle) : fitness(ref temp_particle);
                //Set the first best local position
                temp_particle.best_local_pos = new List<int>(temp_particle.position);

                //Add particle to swarm
                swarm.Add(temp_particle);

            }
        }
        public void initialize_volumes()
        {
            for (int i = 0; i < dim; ++i)
            {
                double rand_volume = MIN_VOL + (rand_gen.NextDouble() * (MAX_VOL - MIN_VOL));
                this.volumes.Add(rand_volume);
            }
        }
        //Set random bit values until the maximum is _reached_
        //If the maximum is exceeded in an iteration, don't set the bit and return
        public void generate_random_bitvec(ref List<int> vec) 
        {
            double current_weight = 0.0;
           
            while (true)
            {
                //Generate a random index 
                int rand_value = rand_gen.Next(0, dim-1);
                //If vector already has the index set to high, continue. 
                if (vec[rand_value] == 1)
                    continue;
                current_weight += weights[rand_value];
                if (current_weight > WEIGHT_LIM)
                {
                    break;
                }
                vec[rand_value] = 1;
                
            }
        }
        public void generate_random_bitvec_volumes(ref List<int> vec)
        {
            double current_weight = 0.0;
            double current_volume = 0.0;
            while (true)
            {
                //Generate a random index 
                int rand_value = rand_gen.Next(0, dim - 1);
                //If vector already has the index set to high, continue. 
                if (vec[rand_value] == 1)
                    continue;
                current_weight += weights[rand_value];
                current_volume += volumes[rand_value];
                if ((current_weight > WEIGHT_LIM) || (current_volume > VOLUME_LIM))
                {
                    break;
                }
                vec[rand_value] = 1;

            }
        }
        private void init_particle_weight_count(ref BParticle particle)
        {
            for (int i = 0; i < particle.position.Count(); ++i)
            {
                if (particle.position[i] == 1)
                    particle.acc_weight += weights[i];
            }
        }
        private void init_particle_volume_count(ref BParticle particle)
        {
            for (int i = 0; i < particle.position.Count(); ++i)
            {
                if (particle.position[i] == 1)
                    particle.acc_volume += volumes[i];
            }
        }
        private void initialize_velocities(ref BParticle particle)
        {
            for (int i = 0; i < this.dim; ++i)
            {
                double rand_vel = min_velocity + (rand_gen.NextDouble() * (max_velocity - min_velocity));
                particle.velocities[i] = rand_vel;
            }
        }
        public void start_swarm(int iterations)
        {
            double inertia_decrease_rate = (inertia - min_inertia) / iterations;
            for (int i = 0; i < iterations; ++i)
            {
                //Update local best
                compare_all_individual();
                //Update global best
                compare_neighbors();
              //  print_best_value_weights(ref this.current_global_best_position);
                //Update velocity and position
                update_all_velocities_and_positions();
               // System.Console.WriteLine(this.current_global_best);
                write_it_status();
                this.fitnesses.Add(this.current_global_best);
                if (this.inertia_decrease)
                    this.inertia = (inertia <= min_inertia) ? inertia : (inertia - inertia_decrease_rate);

            }
            if (this.k_volumes)
                print_best_value_weights_volume(ref current_global_best_position);
            else
                print_best_value_weights(ref current_global_best_position);
            System.Console.ReadKey();

        }
        private void write_it_status()
        {
            System.Console.WriteLine(" ");
            System.Console.Write("Best Fitness : ");
            System.Console.Write(this.current_global_best);
            System.Console.Write("Value : ");
            System.Console.Write(this.current_best_value);

        }
        private void print_best_value_weights(ref List<int> position)
        {
            double acc_weight = 0.0;
            double acc_value = 0.0;
            for (int i = 0; i < dim; ++i)
            {
                if (position[i] == 1)
                {
                    System.Console.WriteLine(" ");
                    System.Console.Write("Value : ");
                    System.Console.Write(values[i]);
                    System.Console.Write("   Weight : ");
                    System.Console.Write(weights[i]);
                    acc_weight += weights[i];
                    acc_value += values[i];
                }
            }
            System.Console.WriteLine(" ");
            System.Console.Write("Total value : ");
            System.Console.Write(acc_value);
            System.Console.Write("Total weights : ");
            System.Console.Write(acc_weight);
        }
        private void print_best_value_weights_volume(ref List<int> position)
        {
            double acc_weight = 0.0;
            double acc_value = 0.0;
            double acc_volume = 0.0;
            for (int i = 0; i < dim; ++i)
            {
                if (position[i] == 1)
                {
                    System.Console.WriteLine(" ");
                    System.Console.Write("Value : ");
                    System.Console.Write(values[i]);
                    System.Console.Write("   Weight : ");
                    System.Console.Write(weights[i]);
                    System.Console.Write("   Volume : ");
                    System.Console.Write(volumes[i]);
                    acc_weight += weights[i];
                    acc_volume += volumes[i];
                    acc_value += values[i];
                }
            }
            System.Console.WriteLine(" ");
            System.Console.Write("Total value : ");
            System.Console.Write(acc_value);
            System.Console.Write("Total weights : ");
            System.Console.Write(acc_weight);
            System.Console.Write("Total volumes : ");
            System.Console.Write(acc_volume);
        }
    }
}
