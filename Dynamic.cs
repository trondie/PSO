using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PSO_Final
{

    class VW_Tuple
    {
        private double value;
        private double weight;
        public VW_Tuple(double value, double weight)
        {
            this.value = value;
            this.weight = weight;
        }
        public double Weight { get { return weight; } }
        public double Value { get { return value; } }
        public VW_Tuple accumulate_tuple(double v_j, double w_j)
        {
            return new VW_Tuple(value + v_j, weight + w_j);
        }
    }
    class Dynamic
    {
        public const int WEIGHT_LIM = 1000;
        public string file = "knapsackMedium.txt";
        public List<double> values;
        public List<double> weights;
        public List<double> volumes;
        public int lines = 2000;

        public Dynamic()
        {
            this.values = new List<double>();
            this.weights = new List<double>();
            read_data(file);
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
        }
        public void execute()
        {
            List<VW_Tuple> tuples = new List<VW_Tuple>();
            List<VW_Tuple> tuples_increment = new List<VW_Tuple>();
            tuples.Add(new VW_Tuple(0, 0));
            tuples.Add(vw(0));
            int add = 0;
            int remove = 0;
            List<VW_Tuple> temp_tuples = new List<VW_Tuple>();
            List<int> indices = new List<int>();

        
            for (int j = 2; j < lines; ++j)
            {
                System.Console.WriteLine("ITER : " + j + "Size : " + tuples.Count());
                temp_tuples.Clear();
                /* NOTE : THIS IS WHAT MAKES THE ALGORITHM PSEUDO POLYNOMIAL!
                 * The tuple list progressively grows as new tuples are added. At first sight, the complexity is clearly O(NW), however,
                 * it will :
                 *       Grow as a function of the length in bits from its input! O(n2^(length in bits of number W))
                 *       Hence, "pseudopolynomial" time.
                 * */
                foreach (VW_Tuple tuple in tuples)
                {
                    if ( (tuple.Weight + weights[j]) <= WEIGHT_LIM) 
                    {
                        temp_tuples.Add(tuple.accumulate_tuple(values[j], weights[j]));
                        add++;
                    }
                }
                tuples = tuples.Concat(temp_tuples).ToList();
                check_dominance(ref tuples, ref remove);
            }
            print_tuple(find_best_tuple(ref tuples));
        }
        public VW_Tuple vw(int index)
        {
            return new VW_Tuple(values[index], weights[index]);
        }
        private void check_dominance(ref List<VW_Tuple> tuples, ref int remove)
        {
            int count = tuples.Count();
          
            tuples.Sort(delegate(VW_Tuple t1, VW_Tuple t2) { return t2.Value.CompareTo(t1.Value); });
            for (int i = 0; i < count; ++i)
            {
                VW_Tuple temp = tuples[i];
                for (int j = 0; j < count; ++j)
                {
                    if (temp.Equals(tuples[j]))
                        continue;
                    if (check_dominant_pair(temp, tuples[j]))
                    {
                        tuples.Remove(tuples[j]);
                        --count;
                        --j;
                    }
                }
            }
        }
        private bool check_dominant_pair(VW_Tuple t, VW_Tuple ts)
        {
            if ((t.Value >= ts.Value) && (t.Weight <= ts.Weight))
            {
                return true;
            }
            return false;
        }
        private VW_Tuple find_best_tuple(ref List<VW_Tuple> tuples)
        {
            VW_Tuple best = tuples[0];
            foreach (VW_Tuple tuple in tuples)
            {
                if (tuple.Value > best.Value)
                    best = tuple;
            }
            return best;
        }
        private void print_tuple(VW_Tuple tuple)
        {
            System.Console.WriteLine("Value : " + tuple.Value + " Weight : " + tuple.Weight);
        }

    }
}
