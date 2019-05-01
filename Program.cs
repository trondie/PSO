using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace PSO_Final
{
    static class Program
    {
        static Form2 plotForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {



            Testing testing = new Testing();
            while (true)
            {
                testing.init_dialogue();
                //create_plot(ref testing.fitnesses, testing.fitnesses.Count(), 1, testing.problemString);
                //Console.WriteLine("Press any key to continue...");
                //Console.ReadKey();
            }

            //k_min_inertia = 0.4;
            //k_c1 = 1.9;
            //k_c2 = 1.0;
            //k_volumes = false;
            //k_iterations = 500;

            //runs = 1;
            //default_params = true;
            //LocalPSO swarm = new LocalPSO(3, 50, -200, 200, 1, 1.4, 1.6, 1.0, 0.1, 6.5, "circle", false, true, false, true);
            //LocalPSO swarm2 = new LocalPSO(3, 50, -200, 200, 2, 1.4, 1.6, 1.0, 0.1, 6.5, "circle", false, true, false, true);
            //swarm.start_swarm_task1(1000, 0.001);
            //swarm2.start_swarm_task1(1000, 0.001);
            
            //create_two_plots(ref swarm.fitnesses, ref swarm2.fitnesses, swarm.fitnesses.Count(), swarm2.fitnesses.Count(), 1);
         //   create_plot(ref swarm.fitnesses, swarm.fitnesses.Count(), 1, "Circle Problem");
                //(c_neighbors, c_particles, c_lower_lim, c_max_lim, dim, c_c1, c_c2, c_inertia, c_min_vel, c_max_vel, "circle", false, plot_results, c_draw, c_inertia_decrease);
            //(int neighbors, int particles, double lower_lim, double max_lim, int dim, double c1, double c2, double inertia, double min_vel, double max_vel, string problem, bool init_specific, bool plot_results, bool draw, bool inertia_decrease)
            //            c_iterations = 1000;
            //c_neighbors = 3;
            //c_particles = 50;
            //c_lower_lim = -200;
            //c_max_lim = 200;
            //c_c1 = 1.4;
            //c_c2 = 1.6;
            //c_inertia = 1.0;
            //c_min_vel = 0.1;
            //c_max_vel = 2.5;
            //c_threshold = 0.001;
            //c_draw = true;
            //c_inertia_decrease = true;
            //plot_results = true;
        }
        public static void create_plot(ref List<double> fitnesses, int iterations, int runs, string problemString)
        {
            plotForm = new Form2(ref fitnesses, iterations, runs, problemString);
            Application.Run(plotForm);
        }
        //Quick workaround for adding support for two plots
        public static void create_two_plots(ref List<double> fitnesses, ref List<double>fitnesses2, int iterations, int iterations2, int runs)
        {
            plotForm = new Form2(ref fitnesses, ref fitnesses2, iterations, iterations2, runs);
            Application.Run(plotForm);
        }
        //Quick workaround for adding support for two plots
        public static void create_three_plots(ref List<double> fitnesses, ref List<double> fitnesses2, ref List<double> fitnesses3, int iterations, int iterations2, int iterations3, int runs)
        {
            plotForm = new Form2(ref fitnesses, ref fitnesses2, ref fitnesses3, iterations, iterations2, iterations3, runs);
            Application.Run(plotForm);
        }
    }
}
