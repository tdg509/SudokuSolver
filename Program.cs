using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SudokuSolver
{    
    class Board
    {
        public static Stopwatch sw = new Stopwatch();
        public static int depth = 0;
        private enum state { SOLVED, VALID, BAD };
        class Grid
        {
            private int mValue;
            private List<int> mPossibleValues;
            private int mRow;
            private int mCol;
            private int mBlock;

            public Grid(int row, int col)
            {
                this.mValue = 0;
                this.mPossibleValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                this.mRow = row;
                this.mCol = col;
                this.mBlock = 3 * (row / 3) + col / 3;
            }

            public int Value
            {
                get { return this.mValue; }
                set { this.mValue = value; this.mPossibleValues.Clear(); }
            }

            public int Row
            {
                get { return this.mRow; }
            }

            public int Col
            {
                get { return this.mCol; }
            }

            public int Block
            {
                get { return this.mBlock; }
            }

            public List<int> possibleValues
            {
                get { return this.mPossibleValues; }
            }

            public Grid(int row, int col, int value, List<int> l)
            {
                this.mValue = value;
                this.mRow = row;
                this.mCol = col;
                this.mBlock = 3 * (row / 3) + col / 3;
                this.mPossibleValues = new List<int>(l);
            }

            public void RemovePossibilty(int n)
            {
                //if (this.mPossibleValues.Contains(n))
                //{
                    this.mPossibleValues.Remove(n);
                //}
            }
        }

        private Grid[,] grids = new Grid[9, 9];

        public Board()
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    this.grids[i, j] = new Grid(i, j);
        }
                
        /*private void SetGridAndUpdateList(int x, int y, int value)
        //set the value of spesific grid and remove that value from the legal possibilities of all other
        //grids at the same row, column and block.
        {
            Grid currentGrid = grids[x, y];
            currentGrid.Value = value;
            foreach (Grid g in this.grids)
            {
                if (((g.Col == currentGrid.Col) || (g.Row == currentGrid.Row) || (g.Block == currentGrid.Block)) && (g.possibleValues.Contains(currentGrid.Value)))
                {
                    g.RemovePossibilty(currentGrid.Value);
                }
            }
        }*/

        private Board SetGridAndUpdateList(int x, int y, int value)
        //set the value of spesific grid and remove that value from the legal possibilities of all other
        //grids at the same row, column and block.
        {
            Grid currentGrid = grids[x, y];
            currentGrid.Value = value;
            foreach (Grid g in this.grids)
            {
                if (((g.Col == currentGrid.Col) || (g.Row == currentGrid.Row) || (g.Block == currentGrid.Block)) && (g.possibleValues.Contains(currentGrid.Value)))
                {
                    g.RemovePossibilty(currentGrid.Value);
                }
            }
            return this;
        }

        public Board GridsWithOnlyOnePossibility()
        //call UpdateBoard in a sequence, until no more grids to upadte
        {
            bool refreshed = false;
            while (!refreshed)
            {
                refreshed = true;
                foreach (Grid g in this.grids)
                {
                    if (g.possibleValues.Count == 1)
                    {
                        refreshed = false;
                        SetGridAndUpdateList(g.Row, g.Col, g.possibleValues[0]);
                        break;
                    }
                }
            }
            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            const string horizontalBar = "+-------+-------+-------+\n";
            const string verticalBar = "| ";
            sb.Append(horizontalBar);
            for (int i = 1; i <= 9; i++)
            {
                sb.Append(verticalBar);
                for (int j = 1; j <= 9; j++)
                {
                    sb.Append(this.grids[i - 1, j - 1].Value.ToString());
                    sb.Append(" ");
                    if (j % 3 == 0)
                    {
                        sb.Append(verticalBar);
                    }                    
                }
                sb.Append("\n");
                if (i % 3 == 0)
                {
                    sb.Append(horizontalBar);
                }
            }
            sb.Append("\n");
            return sb.ToString();
        }
        
        private bool isValid()
        //Checks if the board is in valid state - no two gris in one row, colomn or block
        //has the same values. And also - if a grid doesn't have a value and has no possible values
        {
            string[] rows = new string[9];
            string[] cols = new string[9];
            string[] blocks = new string[9];

            for (int i = 0; i < 9; i++)
            {
                rows[i] = String.Empty;
                cols[i] = String.Empty;
                blocks[i] = String.Empty;
            }
            foreach (Grid g in grids)
            {
                if (g.Value != 0)
                {
                    string s = g.Value.ToString();
                    if ((rows[g.Row].Contains(s) || (cols[g.Col].Contains(s)) || (blocks[g.Block].Contains(s))))
                    {
                        return false;
                    }
                    rows[g.Row] += s;
                    cols[g.Col] += s;
                    blocks[g.Block] += s;
                }
                else
                {
                    if (g.possibleValues.Count == 0)
                    {
                        return false;
                    }
                }
            }       
            return true;
        }

        private bool isPandigital(string s)
        {
            int[] digits = new int[10];
            int n;
            for (int i = 0; i < s.Length; i++)
            {
                n = int.Parse(s.Substring(i,1));
                digits[n]++;
                if (digits[n] > 1)          //check that the digit occurs no more than once
                    return false;
            }
            for (int i = 1; i < digits.Length; i++)
            {
                if (digits[i] != 1)         //check that the digit occurs exactly once 
                    return false;
            }
            return true;
        }

        private bool isSolved()
        //check the board to see if the solution is correct
        {
            string[] rows = new string[9];
            string[] cols = new string[9];
            string[] blocks = new string[9];

            for (int i = 0; i < 9; i++)
            {
                rows[i] = String.Empty;
                cols[i] = String.Empty;
                blocks[i] = String.Empty;
            }
            foreach (Grid g in grids)
            {
                if (g.Value != 0)
                {
                    string s = g.Value.ToString();
                    if ((rows[g.Row].Contains(s) || (cols[g.Col].Contains(s)) || (blocks[g.Block].Contains(s))))
                    {
                        return false;
                    }
                    rows[g.Row] += s;
                    cols[g.Col] += s;
                    blocks[g.Block] += s;
                }
            }
            for (int i = 0; i < 9; i++)
            {
                if (!isPandigital(rows[i])) return false;
                if (!isPandigital(cols[i])) return false;
                if (!isPandigital(blocks[i])) return false;
            }
            return true;
        }

        private state State
        {
            get
            {
                if (this.isSolved()) return state.SOLVED;
                if (this.isValid()) return state.VALID;
                else return state.BAD;
            }
        }

        private Grid minList()  //what if the list's length is 1 or 0?
        {
            int m = 10;
            int row = 0;
            int col = 0;
            Grid minGrid = new Grid(-1, -1);
            /*for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write("{0} {1} - {2}|", i, j, grids[i, j].possibleValues.Count);
                }
                Console.WriteLine("\n-----------------------------------------------------------------------");
            }*/
            foreach (Grid g in this.grids)
            {              
                //col = g.Col;
                if ((g.possibleValues.Count < m) && (g.possibleValues.Count > 0))
                {
                    m = g.possibleValues.Count;
                    row = g.Row;
                    col = g.Col; 
                    if (m == 2) break;  //if m=2 we can't find smaller list, so return the answer
                }
            }
            if (m <= 1)
            {                
                return minGrid;
            }
            minGrid = new Grid(row, col, 0, grids[row, col].possibleValues);  
            return minGrid;
        }

        private static Board Copy(Board b)
        {
            Board newBoard = new Board();
            foreach (Grid g in b.grids)
            {
                if (g.Value != 0)
                {
                    newBoard.SetGridAndUpdateList(g.Row, g.Col, g.Value);
                }
            }
            return newBoard;
        }

        public void Read(string puzzle)
        {
            string s;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    s = puzzle.Substring(i * 9 + j, 1);
                    if (s.Equals("."))
                    {
                        s = "0";
                    }
                    if (!s.Equals("0"))
                    {
                        this.SetGridAndUpdateList(i, j, int.Parse(s));
                    }
                }
            }   
        }

        /*public Board Solve(Board b)
        {
            //b = b.GridsWithOnlyOnePossibility();
            if (b.State == state.SOLVED)
            {
                Console.WriteLine("DONE!\n" + b.ToString());
                Console.WriteLine(depth);
            }
            else
            {
                Grid g = minList();
                if ((g.Col == -1) || (b.State==state.BAD))
                {
                    //Console.WriteLine("Stuck!");
                    //Console.WriteLine(b.ToString());
                    return b;
                }
                foreach (int possibleValue in g.possibleValues)     //state is VALID
                {
                    Board newBoard = Board.Copy(b);//.SetGridAndUpdateList(g.Row, g.Col, possibleValue));  //Board.Copy(b);
                    newBoard.SetGridAndUpdateList(g.Row, g.Col, possibleValue);
                    newBoard = newBoard.GridsWithOnlyOnePossibility();//////////
                    newBoard.Solve(newBoard);
                }
            }
            depth++;
            return b;
        }*/
        public bool Solve(Board b)
        {
            depth++;
            b.GridsWithOnlyOnePossibility();
            if (b.State == state.SOLVED)
            {
                Console.WriteLine("DONE!\n" + b.ToString());
                sw.Stop();
                Console.WriteLine("depth - {0}",depth);
                Console.WriteLine("time - {0}mSEC", sw.ElapsedMilliseconds);
                Environment.Exit(0);
                return true;
            }
            else
            {
                Grid g = minList();
                if ((g.Col == -1) || (b.State == state.BAD))
                {
                    return false;
                }
                foreach (int possibleValue in g.possibleValues)     //state is VALID
                {
                    Board newBoard = Board.Copy(b);//.SetGridAndUpdateList(g.Row, g.Col, possibleValue));  //Board.Copy(b);
                    newBoard.SetGridAndUpdateList(g.Row, g.Col, possibleValue);
                    newBoard.GridsWithOnlyOnePossibility();//////////
                    if (newBoard.Solve(newBoard) == true) break;
                    //b.SetGridAndUpdateList(g.Row, g.Col, possibleValue);
                    //b.GridsWithOnlyOnePossibility();//////////
                    //b.Solve(b);
                }
            }
            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Stopwatch sw = new Stopwatch();
           
            //string sudoku = "610900000950300000070400200009000070405000806020000500007008050000009023000002048";//websudoku evil Depth 9
            //string sudoku = "000000090087000000040810000000537000200000005009004003620403000000002040000080309";//sudokuconquest expert Depth - 505
            //string sudoku = "000002000000070001700300090800700000020890600013006000090050824000008910000000000";
            //string sudoku = "850002400720000009004000000000107002305000900040000000000080070017000000000036040";//Inkala1 - Depth - 95 (was 900)
            //string sudoku = "005300000800000020070010500400005300010070006003200080060500009004000030000009700";//Inkala2 - Depth - 86
            //string sudoku = "000006000059000008200008000045000000003000000006003054000325006000000000000000000";//Norvig's "hard1" - more than one solution
            //string sudoku = "080000040000469000400000007005904600070608030008502100900000005000781000060000010";//1-17?
            //string sudoku = "000008003009020000010600000003054001050703060400290500000001050000030900200500000";//5-24?
            //string sudoku = "000005080000601043000000000010500000000106000300000005530000061000000004000000000";//Norvig hard - no solution
            //string sudoku = "000000010400000000020000000000050407008000300001090000300400200050100000000806000";//Vaste's #1. Depth - 959 (https://projecteuler.net/thread=96;page=2)
            //string sudoku = "000000010400000000020000000000050604008000300001090000300400200050100000000807000";//Vaste's #2. Depth - 1771
            //string sudoku = "000000012000035000000600070700000300000400800100000000000120000080000040050000600";//Vaste's #3. Depth - 6089
            //string sudoku = "000000012003600000000007000410020000000500300700000600280000040000300500000000000";//Vaste's #4. Depth - 1578
            //string sudoku = "000000012008030000000000040120500000000004700060000000507000300000620000000100000";//Vaste's #5. Depth - 4702 (2623,9409,4703mS)
            //string sudoku = "000000012040050000000009000070600400000100000000000050000087500601000300200000000";//Vaste's #6. Depth - 516
            //string sudoku = "000000012050400000000000030700600400001000000000080000920000800000510700000003000";//Vaste's #7. Depth - 5533
            //string sudoku = "000000012300000060000040000900000500000001070020000000000350400001400800060000000";//Vaste's #8. Depth - 11997 (was 33940)
            string sudoku = "000000012400090000000000050070200000600000400000108000018000000000030700502000000";//Vaste's #9. Depth - 2097
            //string sudoku = "000000012500008000000700000600120000700000450000030000030000800000500700020000000";//Vaste's #10. Depth - 269
            //string sudoku = "000100038200005000000000000050000400400030000000700006001000050000060200060004000";//Vaste's #11. Depth - 725422 (583063) - 823 seconds
         
            Board.sw.Start();
            Board b = new Board();
            b.Read(sudoku);
            Console.WriteLine("before:");
            Console.WriteLine(b.ToString());
            b.Solve(b);
            //sw.Stop();
            //Console.WriteLine("Recursion depth - {0}", Board.depth);
            //Console.WriteLine("time - {0}mSEC", sw.ElapsedMilliseconds);
            //Console.ReadKey();
        }
    }
}