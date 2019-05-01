using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO_Final
{
    class Testing
    {
        public List<double> fitnesses;
        public int iterations;

        public int c_iterations;
        private int c_neighbors;
        private int c_particles;
        private double c_lower_lim;
        private double c_max_lim;
        private double c_c1;
        private double c_c2;
        private double c_inertia;
        private double c_min_vel;
        private double c_max_vel;
        private bool c_draw;
        private double c_threshold;
        private bool c_inertia_decrease;
        public bool plot_results;

        // bool inertia_decrease, double c1, double c2, bool k_volumes)
        private double k_min_vel;
        private double k_max_vel;
        private int k_particles;
        private double k_inertia;
        private double k_min_inertia;
        private bool k_inertia_decrease;
        private double k_c1;
        private double k_c2;
        private bool k_volumes;
        private int k_iterations;
        public string problemString;

        public int runs;
        private bool default_params;
        public Testing()
        {
            //Defaults

            //Knapsack problem
            k_min_vel = -4.25;
            k_max_vel = 4.25;
            k_particles = 10;
            k_inertia = 1.0;
            k_min_inertia = 0.4;
            k_c1 = 1.9;
            k_c2 = 1.0;
            k_volumes = false;
            k_iterations = 500;

            runs = 1;
            default_params = true;
            set_clocal_defaults();

        }
        public void init_dialogue()
        {
            writel("PSO Problems");
            writel(" ");
            writel("1: Circle (1D)");
            writel("2: Circle (2D)");
            writel("3: Knapsack (value,weight)");
            writel("4: Knapsack (value,weight,volume)");
            writel("5: Knapsack (Dynamic)");
            writel("6: Knapsack (FPAS)");
            writel("");
            write("Choose a problem : ");
           
            int choice = int.Parse(Console.ReadLine());
         //   runs = enter_runs();

            if (choice == 1)
            {
                int parameter_choice = parameter_dialogue();
                if (parameter_choice == 2)
                {
                    circle_parameters();
                    default_params = false;
                }
                run_circle_problem(1);
            }
            else if (choice == 2)
            {
                int parameter_choice = parameter_dialogue();
                if (parameter_choice == 2)
                {
                    circle_parameters();
                    default_params = false;
                }
                run_circle_problem(2);
            }
            else if (choice == 3)
            {
                k_volumes = false;
                int parameter_choice = parameter_dialogue();
                if (parameter_choice == 2)
                {
                    knapsack_parameters();
                    default_params = false;
                }
                run_knapsack_problem(k_volumes);
            }
            else if (choice == 4)
            {
                k_volumes = true;
                int parameter_choice = parameter_dialogue();
                if (parameter_choice == 2)
                {
                    knapsack_parameters();
                    default_params = false;
                }
                run_knapsack_problem(k_volumes);
            }
            else if (choice == 5)
            {
                run_dynamic_knapsack_problem();
            }
            else if (choice == 6)
            {
                run_FPAS_knapsack_problem();
            }
            
        }
        private int enter_runs()
        {
            writel(" ");
            write("Enter the amount of runs : ");
            int runs = int.Parse(Console.ReadLine());

            return runs;
        }
        private int choose_method()
        {
            writel("");
            writel("Choose method");
            writel("");
            writel("1: Global best");
            writel("2: Local best");
            writel(" ");
            write("Method : ");
            int choice = int.Parse(Console.ReadLine());
            return choice;
        }
        private int parameter_dialogue()
        {
            writel(" ");
            writel("Choose an option :");
            writel("");
            writel("1: Use default parameters");
            writel("2: Enter parameters manually");
            writel(" ");
            write("Choice : ");
            int choice = int.Parse(Console.ReadLine());
            return choice;
        }
        private void writel(string str)
        {
            System.Console.WriteLine(str);
        }
        private void write(string str)
        {
            System.Console.Write(str);
        }
        private void circle_parameters()
        {
            writel(" ");
            write("Enter number of particles : ");
            this.c_particles = int.Parse(Console.ReadLine());
            writel(" ");
            write("Enter number of iterations : ");
            this.c_iterations = int.Parse(Console.ReadLine());
            writel(" ");
            write("Enter inertia : ");
            this.c_inertia = double.Parse(Console.ReadLine());
            writel(" ");
            write("Enter min velocity : ");
            this.c_min_vel = double.Parse(Console.ReadLine());
            writel(" ");
            write("Enter max velcoity : ");
            this.c_max_vel = double.Parse(Console.ReadLine());
            writel(" ");
            write("Enter threshold : ");
            this.c_threshold = double.Parse(Console.ReadLine());
            writel(" ");
            write("Enter C1 parameter : ");
            this.c_c1 = double.Parse(Console.ReadLine());
            writel(" ");
            write("Enter C2 parameter : ");
            this.c_c2 = double.Parse(Console.ReadLine());
            writel(" ");
            write("Decrease inertia ? : ");
            this.c_inertia_decrease = bool.Parse(Console.ReadLine());
            writel(" ");
            writel(" ");
        }
        private void run_circle_problem(int dim)
        {

            int method = choose_method();
            if (method == 1)
            {
                if (default_params)
                    set_cglobal_defaults();
                this.problemString = "Global PSO (Circle Problem)";
                GlobalPSO swarm = new GlobalPSO(c_neighbors, c_particles, c_lower_lim, c_max_lim, dim, c_c1, c_c2, c_inertia, c_min_vel, c_max_vel, "circle", false, plot_results, c_draw, c_inertia_decrease);
                swarm.start_swarm_task1(c_iterations, c_threshold);
                System.Threading.Thread.Sleep(1500);
                swarm.form.Close();
                copy_fitnesses(ref this.fitnesses, ref swarm.fitnesses);
                set_cglobal_defaults();

            }
            else
            {
                if (default_params)
                    set_clocal_defaults();
                this.problemString = "Local PSO (Circle Problem)";
                LocalPSO swarm = new LocalPSO(c_neighbors, c_particles, c_lower_lim, c_max_lim, dim, c_c1, c_c2, c_inertia, c_min_vel, c_max_vel, "circle", false, plot_results, c_draw, c_inertia_decrease);
                swarm.start_swarm_task1(c_iterations, c_threshold);
                System.Threading.Thread.Sleep(1500);
                swarm.form.Close();
                copy_fitnesses(ref this.fitnesses, ref swarm.fitnesses);
                set_clocal_defaults();
            }
        }
        private void run_knapsack_problem(bool volume)
        {
            this.problemString = "BPSO (value, weights)";
            if (volume)
                this.problemString = "BPSO (value, weight, volume)";

            BPSO swarm = new BPSO(2000, k_min_vel, k_max_vel, k_particles, k_inertia, k_min_inertia, k_inertia_decrease, k_c1, k_c2, volume);
            swarm.start_swarm(k_iterations);
            System.Threading.Thread.Sleep(1500);
            copy_fitnesses(ref this.fitnesses, ref swarm.fitnesses);
        }
        private void run_dynamic_knapsack_problem()
        {
            Dynamic dyn_knap = new Dynamic();
            dyn_knap.execute();
        }
        private void run_FPAS_knapsack_problem()
        {
            FPASKnapsack fpas_knap = new FPASKnapsack();
            fpas_knap.execute();
        }
        private void knapsack_parameters()
        {
            writel(" ");
            write("Enter amount of particles : ");
            k_particles = int.Parse(Console.ReadLine());
            writel(" ");
            write("Enter inertia : ");
            k_inertia = double.Parse(Console.ReadLine());
            writel(" ");
            write("Gradually decrease inertia ? (true / false) : ");
            k_inertia_decrease = bool.Parse(Console.ReadLine());
            writel(" ");
            write("Enter c1 : ");
            k_c1 = double.Parse(Console.ReadLine());
            writel(" ");
        }
        private void copy_fitnesses(ref List<double> to, ref List<double> from)
        {
            to = new List<double>(new double[from.Count()]);
            for (int i = 0; i < from.Count(); ++i)
            {
                to[i] = from[i];      
            }
        }
        void set_clocal_defaults()
        {
            c_iterations = 1000;
            c_neighbors = 3;
            c_particles = 50;
            c_lower_lim = -200;
            c_max_lim = 200;
            c_c1 = 1.4;
            c_c2 = 1.6;
            c_inertia = 1.0;
            c_min_vel = 0.1;
            c_max_vel = 2.5;
            c_threshold = 0.001;
            c_draw = true;
            c_inertia_decrease = true;
            plot_results = true;
        }
        void set_cglobal_defaults()
        {
            c_iterations = 1000;
            c_neighbors = 3;
            c_particles = 50;
            c_lower_lim = -200;
            c_max_lim = 200;
            c_c1 = 0.7;
            c_c2 = 1.6;
            c_inertia = 1.0;
            c_min_vel = 0.1;
            c_max_vel = 2.5;
            c_threshold = 0.001;
            c_draw = true;
            plot_results = true;
        }
    }
}
