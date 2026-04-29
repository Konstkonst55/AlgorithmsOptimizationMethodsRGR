using System;
using System.IO;

namespace AlgorithmsOptimizationMethodsRGR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string filePath = "../../../input_inf_solutions.txt";
                LinearProgram lp = new LinearProgram(filePath);
                DualSimplexSolver solver = new DualSimplexSolver(lp);
                solver.Solve();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
            }
        }
    }
}
