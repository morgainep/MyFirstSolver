Instruction for using the program:
There are 2 modes. 1 is for running the ILS experiment (mode E) and 1 is for solving a single sudoku (mode S).

Mode E:

  Must be run from command prompt and should be given:
	- a txt file with the sudoku inputs as first string argument
	- the experiment mode (A, B or C) as second argument
  First build the program from visual studios in release mode.
  Then run the program from the command prompt:
  Change directory to .\\MyFirstSolver\\Solver
  Then run .\\bin\\Release\\netcoreapp3.1\\Solver.exe with 
  "simple_test_cases.txt" as string argument and "A" "B" or "C" as second.
  The program will automatically run the experiment on the sudokus in the input file 
  and write the data to a new file that shows up in the Solver folder.

Mode S:
  First choose your solving method (CBT or ILS).
  CBT is a lot more efficient than ILS.
  Write a sudoku string on a single line such as:
"3;0;0;1;0;0;0;0;0;0;0;8;0;2;0;0;3;7;0;2;0;0;0;0;0;0;0;0;6;0;0;8;0;0;0;1;0;3;0;7;5;0;0;2;0;0;0;9;2;0;3;0;7;8;9;0;2;0;0;0;1;0;0;0;0;0;0;0;5;0;0;0;7;0;1;0;0;0;0;8;0"
  in the console
  The program will print the solution.
