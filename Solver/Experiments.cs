using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Solver
{
    class Experiments
    {
        /// <summary>
        /// Writes data of experiment to .csv file with timestamp and experiment type in file name
        /// </summary>
        /// <param name="data"></param>
        /// <param name="experiment"></param>
        static void writeToFile(string data, string experiment)
        {
            var path = $"data{experiment}{DateTime.Now.ToString("HH-mm-ss")}.csv";
            using (var sw = new StreamWriter(path))
            {
                sw.Write(data);
            }
        }

        /// <summary>
        /// Experiments with solving using Iterated Local Search
        /// by only varying the S parameter (length of random walk)
        /// </summary>
        /// <param name="sudokus"></param>
        static public void ExperimentA(string[] sudokus)
        {
            //we will be collecting all data in 1 string and then write that string to an output file
            string data = "length random walk;threshold random walk;iterations;sudoku id;runtime\n";

            for (int h = 0; h < sudokus.Length; h++)
            {
                string line = sudokus[h];
                for (int i = 1; i < 11; i++) //varying S value
                {
                    for (int j = 1; j < 41; j++) //number of runs
                    {
                        Sudoku s = new Sudoku(line);
                        SudokuILS.InitialFill(s);
                        Stopwatch sw = new Stopwatch();
                        int itt = SudokuILS.ILS(s, i, 9, sw);
                        data += $"{i};9;{itt};{h + 1};{sw.ElapsedMilliseconds}\n";
                        Console.WriteLine($"sudoku number: {h + 1}, s value: {i}, run: {j}, iterations: {itt} runtime: {sw.ElapsedMilliseconds}ms");
                    }
                }
            }

            writeToFile(data, "A");
        }

        /// <summary>
        /// Experiments with solving using Iterated Local Search
        /// by only varying the P parameter (number of stagnating iterations before random walk)
        /// </summary>
        /// <param name="sudokus"></param>
        static public void ExperimentB(string[] sudokus)
        {
            //we will be collecting all data in 1 string and then write that string to an output file
            string data = "length random walk;threshold random walk;iterations;sudoku id;runtime\n";

            //experiment B: varying with P parameter
            for (int h = 0; h < sudokus.Length; h++)
            {
                string line = sudokus[h];
                for (int i = 7; i < 16; i++) //varying P value
                {
                    for (int j = 1; j < 41; j++) //number of runs
                    {
                        Sudoku s = new Sudoku(line);
                        SudokuILS.InitialFill(s);
                        Stopwatch sw = new Stopwatch();
                        int itt = SudokuILS.ILS(s, 1, i, sw);
                        data += $"1;{i};{itt};{h + 1};{sw.ElapsedMilliseconds}\n";
                        Console.WriteLine($"sudoku number: {h + 1}, p value: {i}, run: {j}, iterations: {itt} runtime: {sw.ElapsedMilliseconds}ms");
                    }
                }
            }

            writeToFile(data, "B");
        }

        /// <summary>
        /// Experiments with solving using Iterated Local Search
        /// by varying the S parameter (length of random walk)
        ///and the P parameter (number of stagnating iterations before random walk)
        /// </summary>
        static public void ExperimentC(string[] sudokus)
        {
            //we will be collecting all data in 1 string and then write that string to an output file
            string data = "length random walk;threshold random walk;iterations;sudoku id;runtime\n";

            //experiment C: varying with combinations of P and S values
            for (int h = 0; h < sudokus.Length; h++)
            {
                string line = sudokus[h];
                for (int i = 7; i < 16; i += 2) //varying P value
                {
                    for (int j = 1; j < 11; j += 2) //varying S value
                    {
                        for (int k = 1; k < 41; k++) //number of runs
                        {
                            Sudoku s = new Sudoku(line);
                            SudokuILS.InitialFill(s);
                            Stopwatch sw = new Stopwatch();
                            int itt = SudokuILS.ILS(s, j, i, sw);
                            data += $"{j};{i};{itt};{h + 1};{sw.ElapsedMilliseconds}\n";
                            Console.WriteLine($"sudoku number: {h + 1}, p value: {i}, s value: {j}, run: {k}, iterations: {itt} runtime: {sw.ElapsedMilliseconds}ms");
                        }
                    }
                }
            }
            writeToFile(data, "C");
        }
    }
}
