using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Solver
{
    class Sudoku
    {
        Box[,] sudokuArray; //sudoku is represented as 2d array of Boxes
        bool[,] isFixed;
        Box[] grids; //grids are represented as upper left box in grid
        List<Box>[] swapeableBoxes; //one list of non-fixed boxes for each grid
        List<Tuple<Box, Box>>[] possibleSwaps; //needed to access all possible swaps of grid without recalculating
        Tuple<int, int>[] gridIndices;
        public Box[,] SudokuArray { get => sudokuArray; set => sudokuArray = value; }
        public bool[,] IsFixed { get => isFixed; set => isFixed = value; }
        public List<Box>[] SwapeableBoxes { get => swapeableBoxes; set => swapeableBoxes = value; }
        public List<Tuple<Box, Box>>[] PossibleSwaps { get => possibleSwaps; set => possibleSwaps = value; }
        internal Box[] Grids { get => grids; set => grids = value; }

        public Sudoku(string input)
        {

            inputToSudoku(input);

            //each of the 9 subgrids is represented by the index of the upper left box of the grid
            this.grids = new Box[9] {sudokuArray[0,0], sudokuArray[3,0], sudokuArray[6,0], 
                sudokuArray[0,3], sudokuArray[3,3], sudokuArray[6,3], 
                sudokuArray[0,6], sudokuArray[3,6], sudokuArray[6,6] };

            intializeGrids();

            //initialize member arrays and lists so we can add to them externally
            this.swapeableBoxes = new List<Box>[9];
            this.possibleSwaps = new List<Tuple<Box, Box>>[9];

            for(int i = 0; i < 9; i++)
            {
                this.possibleSwaps[i] = new List<Tuple<Box, Box>>();
                this.swapeableBoxes[i] = new List<Box>();
            }          
        }

        /// <summary>
        /// Assigns one of 9 subgrids to every box in sudoku
        /// Initialises neighbours of all boxes
        /// </summary>
        void intializeGrids()
        {
            for (int j = 0; j < 9; j++)
            {
                int x = this.grids[j].X;
                int y = this.grids[j].Y;
                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        this.sudokuArray[x + k, y + l].Grid = j;
                        this.sudokuArray[x + k, y + l].InitNeighbours(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Splits input string and converts to 2d Box array
        /// Also keeps track of fixed boxes
        /// </summary>
        /// <param name="input"></param>
        void inputToSudoku(string input)
        {
            if(input[1] == ';')
            {
                string[] split = input.Split(';');
                int[] intInputs = Array.ConvertAll<string, int>(split, int.Parse); ;
                this.SudokuArray = new Box[9, 9];
                this.IsFixed = new bool[9, 9];
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        int val = intInputs[i * 9 + j];
                        this.SudokuArray[j, i] = new Box(j, i, val);
                        if (val > 0)
                            this.IsFixed[j, i] = true;
                    }
                }
            }
            else
            {
                this.SudokuArray = new Box[9, 9];
                this.IsFixed = new bool[9, 9];
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        int val = 0;
                        if (input[i * 9 + j] != '.')
                        {
                            val = input[i * 9 + j] - '0';
                            this.IsFixed[j, i] = true;
                        }
                            
                        Console.WriteLine(val.ToString());
                        this.SudokuArray[j, i] = new Box(j, i, val);
                            
                    }
                }
            }       
        }

        /// <summary>
        /// pretty printer for the sudoku
        /// </summary>
        public void Print()
        {
            for (int i = 0; i < 9; i++)
            {
                if (i == 3 || i == 6)
                {
                    Console.WriteLine("---------------------");
                }
                for (int j = 0; j < 9; j++)
                {
                    if (j == 3 || j == 6)
                        Console.Write("| ");
                    Console.Write($"{sudokuArray[j,i].Value} ");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        /// <summary>
        /// Gets first index (left upper box) in a given subgrid
        /// </summary>
        /// <param name="gridNumber"></param>
        /// <returns></returns>
        public Box GetFirstIndexInGrid(int gridNumber)
        {
            return this.grids[gridNumber];
        }

        /// <summary>
        /// Stores all possible swaps (all unique pairs of swapable boxes) for each of 9 grids after swapable boxes have been stored
        /// </summary>
        public void UpdatePossibleSwaps()
        {
            for(int i = 0; i < 9; i++)
            {
                for(int j = 0; j < swapeableBoxes[i].Count; j++)
                {
                    for(int k = j + 1; k < swapeableBoxes[i].Count; k++)
                    {
                        possibleSwaps[i].Add(new Tuple<Box, Box>(swapeableBoxes[i][j], swapeableBoxes[i][k]));
                    }
                }
            }
        }
    }

    class Box
    {
        int x, y, grid;
        int value;
        List<int> domain;
        Dictionary<int, Box> complementDomain;
        HashSet<Tuple<int,int>> neighbours;
        Tuple<int, int> gridIndex;
        
        public Box(int xcor, int ycor, int v)
        {
            //to quickly access the position of a box in the sudoku and its value
            this.x = xcor;
            this.y = ycor;
            this.value = v;
            domain = new List<int>();
            for (int i = 1; i <= 9; i++)
                domain.Add(i);
            complementDomain = new Dictionary<int, Box>();
            this.neighbours = new HashSet<Tuple<int, int>>();    
        }

        /// <summary>
        /// Adds all relevant neighbour boxes to a box's neighbours hash set
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        public void InitNeighbours(int gridX, int gridY)
        {
            for (int i = 0; i < 9; i++)
            {
                this.neighbours.Add(new Tuple<int, int>(i, this.y));
                this.neighbours.Add(new Tuple<int, int>(this.x, i));
            }
            for(int j = gridY; j < gridY + 3; j++)
            {
                for (int k = gridX; k < gridX + 3; k++)
                    this.neighbours.Add(new Tuple<int, int>(k, j));
            }
        }

        public bool NoNextValue { get => domain.IndexOf(this.value) == domain.Count - 1; }
        public int X { get => x; }
        public int Y { get => y; }
        public int Value { get => value; set => this.value = value; }
        public int Grid { get => grid; set => grid = value; }
        internal Dictionary<int, Box> ComplementDomain { get => complementDomain; set => complementDomain = value; }
        public List<int> Domain { get => domain; set => domain = value; }
        public Tuple<int, int> GridIndex { get => gridIndex; set => gridIndex = value; }
        public HashSet<Tuple<int, int>> Neighbours { get => neighbours; set => neighbours = value; }
    }
}
