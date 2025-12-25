using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class frmMain : Form
    {
        // a = b (Assignment Operator), a having the same value as b.
        // a == b (Comparison Operator), a tests if a and b are equal.

        bool turn = true; // true = X turn, false = O turn
        int turnCount = 0;
        bool against_computer = false; 

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (tbxP1.Text == "Player 1" && tbxP2.Text == "Player 2")
            {

                DialogResult result = MessageBox.Show("Do you want to play against the AI?", "Restart Application", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    tbxP2.Text = "Computer";
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by Mine, 2022", "About");
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            turn = true;
            turnCount = 0;

                foreach (Control c in Controls)
                {
                try
                    {
                       Button b = (Button)c;
                       b.Enabled = true;
                       b.Text = "";
                    }
                catch { }
                } 
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit the program?", "Exit Application", MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnClick(object sender, EventArgs e)
        {

            Button b = (Button)sender;
            if (turn)
            {
                b.Text = "X";
            }
            else
            {
                b.Text = "O";
            }

            turn = !turn;
            b.Enabled = false;
            turnCount++;
            checkForWinner();

            //Check to see if playing against computer and if it's O's turn
            if ((!turn) && (against_computer))
            {
                computer_make_move();
            }
            
        }
        private void computer_make_move()
        {
            //Priority 1:  Get tic tac toe
            //Priority 2:  Block x tic tac toe
            //Priority 3:  Go for corner space
            //Priority 4:  Go for outside middle
            //Priority 5:  Go for middle space
            //Priority 6:  Pick open space

            Button move = null;

            //Look for tic tac toe opportunities
            move = look_for_win_or_block("O"); //Look for win
            if (move == null)
            {
                move = look_for_win_or_block("X"); //Look for block
                if (move == null)
                {
                    move = look_for_middle(); //Look for middle
                    if (move == null)
                    {
                        move = look_for_corner(); //Look for corner
                    }
                    if (move == null)
                    {
                        move = look_for_outside_middle(); //Look for outside middle
                        if (move == null)
                        {
                            move = look_for_open_space(); //Look for open space
                        }
                    }
                }
            }

            if(turnCount < 9)
            {
                move.PerformClick();
            }

        }

        private Button look_for_open_space()
        {
            Console.WriteLine("Looking for open space");
            Button b = null;
            foreach (Control c in Controls)
            {
                b = c as Button;
                if (b != null)
                {
                    if (b.Text == "")
                        return b;
                }
            }

            return null;
        }
            private Button look_for_middle()
        {
            Console.WriteLine("Looking for middle");
           
            if (B2.Text == "")
                return B2;
           
            return null;      
        }
            private Button look_for_corner()
        {
            Console.WriteLine("Looking for corner");
            if (A1.Text == "O")
            {
                if (A3.Text == "")
                    return A3;
                if (C3.Text == "")
                    return C3;
                if (C1.Text == "")
                    return C1;
            }

            if (A3.Text == "O")
            {
                if (A1.Text == "")
                    return A1;
                if (C3.Text == "")
                    return C3;
                if (C1.Text == "")
                    return C1;
            }

            if (C3.Text == "O")
            {
                if (A1.Text == "")
                    return A1;
                if (A3.Text == "")
                    return A3;
                if (C1.Text == "")
                    return C1;
            }

            if (C1.Text == "O")
            {
                if (A1.Text == "")
                    return A1;
                if (A3.Text == "")
                    return A3;
                if (C3.Text == "")
                    return C3;
            }
            else
            {
                if (A1.Text == "")
                    return A1;
                if (A3.Text == "")
                    return A3;
                if (C1.Text == "")
                    return C1;
                if (C3.Text == "")
                    return C3;
            }
            return null;
        }

        private Button look_for_outside_middle()
        {
                if (A2.Text == "")
                    return A2;
                if (B1.Text == "")
                    return B1;
                if (B3.Text == "")
                    return B3;
                if (C2.Text == "")
                    return C2;

            return null;
        }

        private Button look_for_win_or_block(string mark)
        {
            Console.WriteLine("Looking for win or block:  " + mark);

            //Horizontal Checks
            if ((A1.Text == mark) && (A2.Text == mark) && (A3.Text == ""))
                return A3;
            if ((A2.Text == mark) && (A3.Text == mark) && (A1.Text == ""))
                return A1;
            if ((A1.Text == mark) && (A3.Text == mark) && (A2.Text == ""))
                return A2;

            if ((B1.Text == mark) && (B2.Text == mark) && (B3.Text == ""))
                return B3;
            if ((B2.Text == mark) && (B3.Text == mark) && (B1.Text == ""))
                return B1;
            if ((B1.Text == mark) && (B3.Text == mark) && (B2.Text == ""))
                return B2;

            if ((C1.Text == mark) && (C2.Text == mark) && (C3.Text == ""))
                return C3;
            if ((C2.Text == mark) && (C3.Text == mark) && (C1.Text == ""))
                return C1;
            if ((C1.Text == mark) && (C3.Text == mark) && (C2.Text == ""))
                return C2;

            //Vertical Checks
            if ((A1.Text == mark) && (B1.Text == mark) && (C1.Text == ""))
                return C1;
            if ((B1.Text == mark) && (C1.Text == mark) && (A1.Text == ""))
                return A1;
            if ((A1.Text == mark) && (C1.Text == mark) && (B1.Text == ""))
                return B1;

            if ((A2.Text == mark) && (B2.Text == mark) && (C2.Text == ""))
                return C2;
            if ((B2.Text == mark) && (C2.Text == mark) && (A2.Text == ""))
                return A2;
            if ((A2.Text == mark) && (C2.Text == mark) && (B2.Text == ""))
                return B2;

            if ((A3.Text == mark) && (B3.Text == mark) && (C3.Text == ""))
                return C3;
            if ((B3.Text == mark) && (C3.Text == mark) && (A3.Text == ""))
                return A3;
            if ((A3.Text == mark) && (C3.Text == mark) && (B3.Text == ""))
                return B3;

            //Diagonal checks
            if ((A1.Text == mark) && (B2.Text == mark) && (C3.Text == ""))
                return C3;
            if ((B2.Text == mark) && (C3.Text == mark) && (A1.Text == ""))
                return A1;
            if ((A1.Text == mark) && (C3.Text == mark) && (B2.Text == ""))
                return B2;

            if ((A3.Text == mark) && (B2.Text == mark) && (C1.Text == ""))
                return C1;
            if ((B2.Text == mark) && (C1.Text == mark) && (A3.Text == ""))
                return A3;
            if ((A3.Text == mark) && (C1.Text == mark) && (B2.Text == ""))
                return B2;

            return null;
        }
        private void checkForWinner()
        {
            bool there_is_a_winner = false;

            //Horizontal checks
            if ((A1.Text == A2.Text) && (A2.Text == A3.Text) && (!A1.Enabled))
                there_is_a_winner = true;
            else if ((B1.Text == B2.Text) && (B2.Text == B3.Text) && (!B1.Enabled))
                    there_is_a_winner = true;
            else if ((C1.Text == C2.Text) && (C2.Text == C3.Text) && (!C1.Enabled))
                    there_is_a_winner = true;

            //Vertical checks
            if ((A1.Text == B1.Text) && (B1.Text == C1.Text) && (!A1.Enabled))
                there_is_a_winner = true;
            else if ((A2.Text == B2.Text) && (B2.Text == C2.Text) && (!B2.Enabled))
                there_is_a_winner = true;
            else if ((A3.Text == B3.Text) && (B3.Text == C3.Text) && (!C3.Enabled))
                there_is_a_winner = true;

            //Diagonal checks
            if ((A1.Text == B2.Text) && (B2.Text == C3.Text) && (!A1.Enabled))
                there_is_a_winner = true;
            else if ((A3.Text == B2.Text) && (B2.Text == C1.Text) && (!B2.Enabled))
                there_is_a_winner = true;
            
            if (there_is_a_winner)
            {
                disableButtons();

                String winner = "";
                if (turn)
                {
                    winner = tbxP2.Text;
                    OWinCount.Text = (int.Parse(OWinCount.Text) + 1).ToString();  
                }

                else
                {
                    winner = tbxP1.Text;
                    XWinCount.Text = (int.Parse(XWinCount.Text) + 1).ToString();
                }
                    
                if(winner == "You")
                {
                    MessageBox.Show(winner + " Win!", "Congrats!");
                }
                else
                {
                    MessageBox.Show(winner + " Wins!", "Congrats!");
                }

                newGameToolStripMenuItem_Click(null, null);

            }
            else
            {
                if(turnCount == 9)
                {
                    MessageBox.Show("Draw!", "Result");
                    DrawCount.Text = (int.Parse(DrawCount.Text) + 1).ToString();
                    newGameToolStripMenuItem_Click(null, null);
                }

            }
        
        }
        private void disableButtons()
        {
            foreach (Control c in Controls)
            {
                try
                {
                    Button b = (Button)c;
                    b.Enabled = false;
                    b.Text = "";

                }//end try
                catch { }
            }//end foreach
        }
        private void btnEnter(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Enabled)
            {
                if (turn)
                {
                    b.Text = "X";
                }
                else
                {
                    b.Text = "O";
                }//end if
            }
        }
        private void btnLeave(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Enabled)
            { 
                    b.Text = "";
            }
        }
        private void resetScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OWinCount.Text = "0";
            DrawCount.Text = "0";
            XWinCount.Text = "0";
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void tbxP2_TextChanged(object sender, EventArgs e)
        {
            if (tbxP2.Text.ToUpper() == "COMPUTER")
            {
                against_computer = true;

                tbxP1.Text = "You";
            }
            else
            {
                against_computer = false;
                tbxP1.Text = "Player 1";
            }
        }
        
    }
}
