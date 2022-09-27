using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Solver
{
    class SudokuILS
    {
        /// <summary>
        /// Complements each grid such that it is filled with each number between 1-9
        /// </summary>
        /// <param name="s"></param>
        public static void InitialFill(Sudoku s)
        {
            //fill up grid by grid
            for(int i = 0; i < 9; i++)
            {
                //first assume that all numbers are missing
                List<int> missing = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
                Box gridLeftTop = s.GetFirstIndexInGrid(i);

                int x = gridLeftTop.X;
                int y = gridLeftTop.Y;

                //check each box in grid to see what fixed values we have
                for (int j = y; j < y + 3; j++)
                {
                    for (int k = x; k < x + 3; k++)
                    {
                        if (s.IsFixed[j, k])
                            //each fixed number must be removed from missing numbers list
                            missing.Remove((int)s.SudokuArray[j, k].Value);
                        else
                            //if a number is not fixed, it is swapable, so update
                            s.SwapeableBoxes[i].Add(s.SudokuArray[j, k]);
                    }
                }

                //list of missing numbers is shuffled randomly
                shuffleList(missing);
                int c = 0;

                for (int j = y; j < y + 3; j++)
                {
                    for (int k = x; k < x + 3; k++)
                        if (!s.IsFixed[j, k])
                        {
                            //fill up empty spot with next number from missing list
                            s.SudokuArray[j, k].Value = missing[c];
                            c++;
                        }
                }
            }

            //the swapeableBoxes for each grid is now up to date, so we can now calculate which possible swaps there are
            s.UpdatePossibleSwaps();
        }

        /// <summary>
        /// Performs iterated local search on a sudoku given an S value (length of random walk),
        /// p value (failed iterations before random walk), and a stopwatch
        /// </summary>
        /// <param name="sudoku"></param>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="sw"></param>
        /// <returns></returns>
        public static int ILS(Sudoku sudoku, int s, int p, Stopwatch sw)
        {
            InitialFill(sudoku);
            sw.Start();
            Random r = new Random();
            int score = Evaluate(sudoku);
            int counter = 0; //to keep track how many iterations the score has not been improved
            int itt = 0;
            while (score > 0)
            {
                itt++;

                //if score has not improved for p iterations, perform random walk
                if (counter == p)
                {        
                    counter = 0;
                    randomWalk(s, sudoku);
                    score = Evaluate(sudoku);
                    itt += s;
                }
                int currentScore = score;
                Tuple<Box, Box> bestSwap = null;

                int gridNumber = r.Next(0, 9); //randomly choose 1 of 9 grids
                foreach (Tuple<Box, Box> swap in sudoku.PossibleSwaps[gridNumber])
                {
                    performSwap(swap); //make swap
                    int interScore = Evaluate(sudoku);
                    if (interScore <= score)
                    {//only remember swaps that are equal to or improve the best score up until now (including original score)
                        score = interScore;
                        bestSwap = swap;
                    }
                    performSwap(swap); //swap back
                }
                if (bestSwap != null)
                {
                    performSwap(bestSwap);
                }
                if (currentScore == score)
                {
                    counter++;
                }
                else
                    counter = 0; //score has been improved, reset counter
                
            }
            sw.Stop();
            return itt;
        }

        /// <summary>
        /// performs random walk in a sudoku state of length s
        /// </summary>
        /// <param name="k"></param>
        /// <param name="s"></param>
        static void randomWalk(int s, Sudoku sudoku)
        {
            Random r = new Random();
            int i = 0;
            while (i < s)
            {
                int gridNumber = r.Next(0, 9);
                int swapIndex = r.Next(sudoku.PossibleSwaps[gridNumber].Count());
                Tuple<Box, Box> swap = sudoku.PossibleSwaps[gridNumber][swapIndex];
                performSwap(swap);
                i++;
            }
        }

        /// <summary>
        /// swaps the values of two boxes
        /// </summary>
        /// <param name="swap"></param>
        static void performSwap(Tuple <Box, Box> swap)
        {
            int buffer = (int)swap.Item1.Value;
            swap.Item1.Value = swap.Item2.Value;
            swap.Item2.Value = buffer;
        }

        /// <summary>
        /// Returns int score of sudoku state from total of missing numbers in each column and row
        /// </summary
        /// <param name="s"></param>
        /// <returns></returns>
        public static int Evaluate(Sudoku s)
        {
            int score = 0;
            bool[] seen = new bool[9];

            //check each 9 columns and rows for missing values
            for (int i = 0; i < 9; i++)
            {
                resetArray(seen);

                int index = 0;
                //check each number in the row and mark the numbers you see
                for(int j = 0; j < 9; j++)
                {
                   index = (int)s.SudokuArray[i, j].Value - 1;
                   seen[index] = true;
                }

                //if the nth element of seen is false, it means number n + 1 was missing
                score += seen.Where(x => x == false ).Count();
                resetArray(seen);

                //check each number in the column and mark the numbers you see
                for (int j = 0; j < 9; j++)
                {
                    index = (int)s.SudokuArray[j, i].Value - 1;
                    seen[index] = true;
                }
                score += seen.Where(x => x == false).Count();
            }
            return score;
        }

        /// <summary>
        /// quick helper method that resets all elements in a bool array to false
        /// </summary>
        /// <param name="a"></param>
        static void resetArray(bool[] a)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] = false;
        }

        /// <summary>
        /// shuffles a list in random order according to the Fisher-Yates technique
        /// </summary>
        /// <param name="l"></param>
        static void shuffleList(List<int> l)
        {
            Random r = new Random();
            int n = l.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                int value = l[k];
                l[k] = l[n];
                l[n] = value;
            }
        }
    }
}
