using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0) 
            {//single sudoku solver mode that uses sudoku string input
                Console.WriteLine("You are now in the single sudoku solving mode. If you want to run the experiment, run the program again but with the input text file as first string argument and experiment mode (A, B or C) as second.");
                Console.WriteLine("What solving method do you want to use?\n- Chronolgical Backtracking (type \"CBT\")\n- Iterated Local Search (type \"ILS\")");
                string method = Console.ReadLine();
                while(method != "CBT" && method != "ILS")
                {
                    Console.WriteLine("Choose a valid solving method: \"CBT\" or \"ILS\"");
                    method = Console.ReadLine();
                }
                Console.WriteLine("You can now paste your single-line sudoku string. IMPORTANT: The algorithms need sudokus that can be solved!");
                string input = Console.ReadLine();
                Sudoku s = new Sudoku(input);
                s.Print();
                Console.WriteLine("Solving sudoku...");
                Stopwatch sw = new Stopwatch();
                int time = 0;
                switch (method)
                {
                    case ("CBT"):
                        time = SudokuCBT.CBT(s, sw);
                        break;
                    case ("ILS"):
                        time = SudokuILS.ILS(s,2,9,sw);
                        break;
                    default: break;
                }         
                
                s.Print();
                Console.WriteLine($"sudoku solved in {time}ms");
            }

            if (args.Length == 1)
            {

                Console.WriteLine("Missing argument. Argument 1: input file, argument 2: experiment mode (A, B or C)");
            }

            if (args.Length == 2)
            {//experimenting mode with Chronological Backtracking
                string path = args[0];
                string[] lines = System.IO.File.ReadAllLines(path);
                string exp = args[1];
                while(exp != "A" && exp != "B" && exp != "C")
                {
                    Console.WriteLine("Choose a valid experiment: 'A', 'B' or 'C'");
                    exp = Console.ReadLine();
                }
                switch (args[1])
                {
                    case ("A"): Experiments.ExperimentA(lines);
                        break;
                    case ("B"): Experiments.ExperimentB(lines);
                        break;
                    case ("C"): Experiments.ExperimentC(lines);
                        break;
                    default: break;
                }
                for (int i = 0; i < lines.Length; i++)
                {
                    Sudoku s = new Sudoku(lines[i]);
                    s.Print();
                    Console.WriteLine("Solving sudoku...");
                    Stopwatch sw = new Stopwatch();
                    int time = SudokuCBT.CBT(s, sw);
                    s.Print();
                    Console.WriteLine($"sudoku solved in {time}ms");
                }            
            }
        }
    }

}
