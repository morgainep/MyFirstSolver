using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Solver
{
    class SudokuCBT
    {
        /// <summary>
        /// Performs Chronological Backtracking with Forward Checking on a sudoku
        /// </summary>
        /// <param name="sudoku"></param>
        /// <param name="sw"></param>
        public static int CBT(Sudoku sudoku, Stopwatch sw)
        {
            sw.Start();
            nodeConsistency(sudoku);
            
            for(int i = 0; i < 9; i++) //iteratively walk past each box from left to right, top to bottom
            {
                for(int j = 0; j < 9; j++)
                {
                    Box currentBox = sudoku.SudokuArray[j, i];
                    if (sudoku.IsFixed[j, i])
                        continue;

                    assignNextValue(currentBox);

                    while (domainChange(sudoku, currentBox)) //domain change returns true if a domain has become empty
                        currentBox = backtrack(sudoku, currentBox); //backtrack to last box that has an untried value in domain

                    j = currentBox.X;
                    i = currentBox.Y;
                }
            }
            sw.Stop();
            return (int)sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Assigns next value in domain to a box
        /// or first value is the box is empty
        /// </summary>
        /// <param name="box"></param>
        static void assignNextValue(Box box)
        {
            if (box.Value == 0)
                box.Value = box.Domain[0];
            else
                box.Value = box.Domain[box.Domain.IndexOf(box.Value) + 1];               
        }

        /// <summary>
        /// Finds the last box before the given box that
        /// is not a fixed box
        /// </summary>
        /// <param name="sudoku"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        static Box findPreviousBox(Sudoku sudoku, Box box)
        {
            if (box.X != 0)
                box = sudoku.SudokuArray[box.X - 1, box.Y];
            else
                box = sudoku.SudokuArray[8, box.Y - 1];
            while(sudoku.IsFixed[box.X, box.Y] && box != sudoku.SudokuArray[0,0])
            {
                if (box.X != 0)
                    box = sudoku.SudokuArray[box.X - 1, box.Y];
                else
                    box = sudoku.SudokuArray[8, box.Y - 1];
            }
            return box;
        }

        /// <summary>
        /// Goes back to the last box that still has an untried possible value in domain
        /// </summary>
        /// <param name="sudoku"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        static Box backtrack(Sudoku sudoku, Box box)
        {
            domainRepair(sudoku, box); //value of box is added again to relevant domains
            while (box.NoNextValue) //check if box is at last value of domain
            {
                box.Value = 0;
                box = findPreviousBox(sudoku, box);
                domainRepair(sudoku, box);               
            }
            assignNextValue(box); //assign next value to last box that still has a value left in domain
            return box;
        }

        /// <summary>
        /// Removes the values of all fixed numbers from the neighbouring domains
        /// Used only at the initialisation
        /// </summary>
        /// <param name="sudoku"></param>
        static void nodeConsistency(Sudoku sudoku)
        {
            for(int i = 0; i < 9; i++)
            {
                for(int j = 0; j < 9; j++)
                {
                    if (sudoku.IsFixed[i,j]) //only domains of neighbours of fixed boxes need to be changed
                        domainChange(sudoku, sudoku.SudokuArray[i, j]);                   
                }
            }
        }

        /// <summary>
        /// Puts value back in relevant domains
        /// and removes from relevant complement domains
        /// when the assignment of a value to a box is undone
        /// </summary>
        /// <param name="s"></param>
        /// <param name="b"></param>
        static void domainRepair(Sudoku s, Box b)
        {
            
            for (int i = 0; i < b.Neighbours.Count; i++)
            {
                int x = b.Neighbours.ElementAt(i).Item1;
                int y = b.Neighbours.ElementAt(i).Item2;
                if (s.SudokuArray[x, y].Value == 0 && s.SudokuArray[x, y].ComplementDomain[b.Value] == b)
                {
                    s.SudokuArray[x, y].ComplementDomain.Remove(b.Value);
                    s.SudokuArray[x, y].Domain.Add(b.Value);
                    s.SudokuArray[x, y].Domain.Sort();
                }
            }         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static bool domainChange(Sudoku s, Box b)
        {
            bool existsEmptyDomain = false;
            for (int i = 0; i < b.Neighbours.Count; i++)
            {
                int x = b.Neighbours.ElementAt(i).Item1;
                int y = b.Neighbours.ElementAt(i).Item2;
                if (s.SudokuArray[x, y].Value == 0 && s.SudokuArray[x, y].Domain.Contains(b.Value))
                {
                    s.SudokuArray[x, y].Domain.Remove(b.Value);
                    s.SudokuArray[x, y].ComplementDomain.Add(b.Value, b);
                }
                if (s.SudokuArray[x, y].Domain.Count == 0)
                    existsEmptyDomain = true;
            }
            return existsEmptyDomain;
        }
    }
}
