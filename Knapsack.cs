using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace PSO_Final
{

    /* BRUTEFORCE THE KNAPSACK! */

    class Knapsack
    {
        double bestValue;
        bool[] bestItems;
        double[] itemValues;
        double[] itemWeights;
        double weightLimit;

        public int items = 2000;
        public Knapsack()
        {
            
            bestItems = new bool[items];
            itemValues = new double[items];
            itemWeights = new double[items];

            weightLimit = 1000;
            read_data("knapsackMedium.txt");


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
                        if (count == items)
                            break;
                        

                        string[] read_values = line.Split(' ');
                        double value = Convert.ToDouble(read_values[0]);
                        double weight = Convert.ToDouble(read_values[1]);
                        itemWeights[count] = weight;
                        itemValues[count] = value;

                        ++count;
                    }
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
        void SolveRecursive(bool[] chosen, int depth, double currentWeight, double currentValue, double remainingValue)
        {
            if (currentWeight > weightLimit) return;
            if (currentValue + remainingValue < bestValue) return;
            if (depth == chosen.Length)
            {
                bestValue = currentValue;
                System.Array.Copy(chosen, bestItems, chosen.Length);
                return;
            }
            remainingValue -= itemValues[depth];
            chosen[depth] = false;
            SolveRecursive(chosen, depth + 1, currentWeight, currentValue, remainingValue);
            chosen[depth] = true;
            currentWeight += itemWeights[depth];
            currentValue += itemValues[depth];
            SolveRecursive(chosen, depth + 1, currentWeight, currentValue, remainingValue);
        }

        public bool[] Solve()
        {
            var chosen = new bool[itemWeights.Length];
            bestItems = new bool[itemWeights.Length];
            bestValue = 0.0;
            double totalValue = 0.0;
            foreach (var v in itemValues) totalValue += v;
            SolveRecursive(chosen, 0, 0.0, 0.0, totalValue);
            return bestItems;
        }
        public void printBest()
        {
            double weight = 0.0;
            double value = 0.0;

            System.Console.WriteLine(" ");

            for (int i = 0; i < bestItems.Count(); ++i)
            {
                if (bestItems[i] == true)
                {
                    System.Console.WriteLine(itemValues[i]);
                    value += itemValues[i];
                    weight += itemWeights[i];
                }
            }
            System.Console.WriteLine(" ");
            System.Console.Write("Total bruteforce value : ");
            System.Console.Write(value);
            System.Console.WriteLine(" ");
            System.Console.Write("Total bruteforce weight : ");
            System.Console.Write(weight);

        }
    }
}
