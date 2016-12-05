using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tic_Tac_Toe
{
    public partial class Form1 : Form
    {
        int SquareSize;
        int X;
        int Y;
        string Game;
        Random R;
        System.Collections.ArrayList Memory;



        public Form1()
        {
            InitializeComponent();
            this.Text = "Tic Tac Toe";
            Memory = new System.Collections.ArrayList();
            LoadMemory();
            //MessageBox.Show(Memory.Count.ToString(), "Here");
            R = new Random();
            // Specify size of each of the nine squares
            SquareSize = 65;
            // Specify top left of Tic Tac Toe grid
            X = 40;
            Y = 30;
            Game = "         ";
            DrawView();
        }



        private void SaveMemory()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("memory.txt");
            foreach (string p in Memory)
                sw.WriteLine(p);
            sw.Close();
        }



        private void LoadMemory()
        {
            System.IO.StreamReader sr = new System.IO.StreamReader("memory.txt");
            string line = sr.ReadLine();
            while (line != null)
            {
                Memory.Add(line);
                line = sr.ReadLine();
            }
            sr.Close();
        }



        private void ComputerMove()
        {
            int Move = -1;
            string Outcome = "L";

            // Move left to right, top to bottom
            for (int i = 0; i < 9; i++)
                if (Game[i] == ' ')
                {
                    string Copy = Game.Substring(0, i) + 'O' + Game.Substring(i + 1);
                    bool InMemory = false;
                    foreach (string s in Memory)
                    {
                        if (Copy == s.Substring(0, 9)) // We found considered position in memory
                        {
                            if (Move == -1) // first open move considered
                            {
                                Move = i;
                                Outcome = s.Substring(10);
                            }
                            else // keep only if better than current best move
                            {
                                if ((Outcome == "L") && (s.Substring(10) != "L"))
                                {
                                    Move = i;
                                    Outcome = s.Substring(10);
                                }
                            }
                            InMemory = true;
                        }
                    }
                    // What if move being considered not in memory?
                    if (InMemory == false) 
                    {
                        if (Move == -1) // first possible move
                        {
                            Move = i;
                            Outcome = "U";
                        }
                        else // keep only if better than current best move
                        {
                            if ((Outcome == "T") || (Outcome == "L"))
                            {
                                Move = i;
                                Outcome = "U";
                            }
                        }
                    }
                } // end if (Game[i] == ' ')

            if (Outcome=="L")
            {
                // Add new losing positions to memory
                for (int k = 0; k < 9; k++)
                {
                    if (Game[k] == 'X')
                    {
                        string Loser = Game.Substring(0, k) + ' ' + Game.Substring(k + 1) + " L";
                        Memory.Add(Loser);
                    }
                }
            }

            // Implement move determined above
            Game = Game.Substring(0, Move) + 'O' + Game.Substring(Move + 1);

            // Is game over
            if (Winning(Game, 'O') == true)
            {
                DrawView();
                DialogResult dialogResult = MessageBox.Show("I beat you human! Do you want to play again?", "Game Over!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Game = "         ";
                }
                else if (dialogResult == DialogResult.No)
                {
                    this.Close();
                }
            }
        } // end method ComputerMove



        private bool Winning(string GameState, char Who)
        {
            if ((GameState[0] == Who) && (GameState[1] == Who) && (GameState[2] == Who)) return true;
            if ((GameState[3] == Who) && (GameState[4] == Who) && (GameState[5] == Who)) return true;
            if ((GameState[6] == Who) && (GameState[7] == Who) && (GameState[8] == Who)) return true;

            if ((GameState[0] == Who) && (GameState[3] == Who) && (GameState[6] == Who)) return true;
            if ((GameState[1] == Who) && (GameState[4] == Who) && (GameState[7] == Who)) return true;
            if ((GameState[2] == Who) && (GameState[5] == Who) && (GameState[8] == Who)) return true;

            if ((GameState[0] == Who) && (GameState[4] == Who) && (GameState[8] == Who)) return true;
            if ((GameState[2] == Who) && (GameState[4] == Who) && (GameState[6] == Who)) return true;

            return false;
        }



        private void DrawView()
        {
            // create bitmap and graphics object
            Bitmap GameView = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(GameView);
            // white background
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, GameView.Width, GameView.Height);
            // black grid
            Pen GridPen = new Pen(Color.Black, 3);
            g.DrawLine(GridPen, X, Y + SquareSize, X + 3 * SquareSize, Y + SquareSize);
            g.DrawLine(GridPen, X, Y + 2 * SquareSize, X + 3 * SquareSize, Y + 2 * SquareSize);
            g.DrawLine(GridPen, X + SquareSize, Y, X + SquareSize, Y + 3 * SquareSize);
            g.DrawLine(GridPen, X + 2 * SquareSize, Y, X + 2 * SquareSize, Y + 3 * SquareSize);
            // draw Xs and Os
            Font font = new Font("Sans Serif", 40); // Courier New, New Times Roman
            for (int i = 0; i < 9; i++)
            {
                char c = Game[i];
                if (c != ' ')
                {
                    float cx = X + 6 + (i % 3) * SquareSize;
                    float cy = Y + 4 + (i / 3) * SquareSize;
                    g.DrawString(c.ToString(), font, new SolidBrush(Color.Black), new PointF(cx, cy));
                }
            }
            // display
            pictureBox1.Image = GameView;
        } // end method DrawView



        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // What square was clicked in?
            if ((e.X > X) && (e.X < X + 3 * SquareSize) && (e.Y > Y) && (e.Y < Y + 3 * SquareSize))
            {
                int Column = (e.X - X) / SquareSize;
                int Row = (e.Y - Y) / SquareSize;
                int i = Row * 3 + Column;
                Game = Game.Substring(0, i) + 'X' + Game.Substring(i + 1);
                if (Winning(Game, 'X') == true)
                {
                    // Add new losing positions to memory
                    for (int k = 0; k < 9; k++)
                    {
                        if (Game[k] == 'X')
                        {
                            string Loser = Game.Substring(0, k) + ' ' + Game.Substring(k + 1) + " L";
                            Memory.Add(Loser);
                        }
                    }
                    // Show game after move
                    DrawView(); // show player winning game state
                    // Do you want to play again?
                    DialogResult dialogResult = MessageBox.Show("You won human! Do you want to play again?", "Game Over!", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Game = "         ";
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        SaveMemory();
                        this.Close();
                    }
                }
                else
                {
                    // Which squares are open
                    System.Collections.ArrayList OpenMoves = new System.Collections.ArrayList();
                    for (int j = 0; j < 9; j++)
                        if (Game[j] == ' ')
                            OpenMoves.Add(j);
                    if (OpenMoves.Count == 0)
                    {
                        // Add new tie game positions to memory
                        for (int k = 0; k < 9; k++)
                        {
                            if (Game[k] == 'X')
                            {
                                string Loser = Game.Substring(0, k) + ' ' + Game.Substring(k + 1) + " T";
                                Memory.Add(Loser);
                            }
                        }
                        DrawView(); // show player winning game state
                        DialogResult dialogResult = MessageBox.Show("Tie game! Do you want to play again?", "Game Over!", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            Game = "         ";
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            this.Close();
                        }
                    }
                    else
                        ComputerMove();
                }
                DrawView();
            }
        } // end method pictureBox1_MouseDown

    }
}