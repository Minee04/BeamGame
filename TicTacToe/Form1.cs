using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe.AI;
using TicTacToe.GameLogic;
using TicTacToe.Models;

namespace TicTacToe
{
    public partial class frmMain : Form
    {
        private GameEngine _gameEngine;
        private ComputerPlayer _computerPlayer;
        private bool _isAgainstComputer;
        private Dictionary<Button, CellPosition> _buttonToPosition;
        private Dictionary<CellPosition, Button> _positionToButton; 

        public frmMain()
        {
            InitializeComponent();
            InitializeGame();
        }

        /// <summary>
        /// Initializes the game engine and sets up button mappings
        /// </summary>
        private void InitializeGame()
        {
            _gameEngine = new GameEngine();
            _isAgainstComputer = false;

            // Map buttons to board positions
            _buttonToPosition = new Dictionary<Button, CellPosition>
            {
                { A1, new CellPosition(0, 0) },
                { A2, new CellPosition(0, 1) },
                { A3, new CellPosition(0, 2) },
                { B1, new CellPosition(1, 0) },
                { B2, new CellPosition(1, 1) },
                { B3, new CellPosition(1, 2) },
                { C1, new CellPosition(2, 0) },
                { C2, new CellPosition(2, 1) },
                { C3, new CellPosition(2, 2) }
            };

            _positionToButton = _buttonToPosition.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (tbxP1.Text == "Player 1" && tbxP2.Text == "Player 2")
            {
                DialogResult result = MessageBox.Show("Do you want to play against the AI?", "Game Mode", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    tbxP2.Text = "Computer";
                    SetupComputerGame();
                }
            }
        }

        /// <summary>
        /// Sets up a game against the computer
        /// </summary>
        private void SetupComputerGame()
        {
            _isAgainstComputer = true;
            _computerPlayer = new ComputerPlayer(PlayerType.O, new StrategicAI());
            tbxP1.Text = "You";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by Mine, 2022", "About");
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        /// <summary>
        /// Starts a new game by resetting the engine and UI
        /// </summary>
        private void StartNewGame()
        {
            _gameEngine.Reset();
            ResetBoardUI();
        }

        /// <summary>
        /// Resets all buttons on the UI board
        /// </summary>
        private void ResetBoardUI()
        {
            foreach (var button in _buttonToPosition.Keys)
            {
                button.Enabled = true;
                button.Text = "";
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
            Button clickedButton = (Button)sender;
            
            if (!_buttonToPosition.ContainsKey(clickedButton))
                return;

            ProcessPlayerMove(clickedButton);
        }

        /// <summary>
        /// Processes a player's move and handles game flow
        /// </summary>
        private void ProcessPlayerMove(Button button)
        {
            CellPosition position = _buttonToPosition[button];
            
            // Store current player before making the move
            PlayerType currentPlayer = _gameEngine.CurrentPlayer;
            
            // Make the move in the engine
            if (!_gameEngine.MakeMove(position.Row, position.Column))
                return;

            // Update UI with the player who just moved
            button.Text = currentPlayer == PlayerType.X ? "X" : "O";
            button.Enabled = false;

            // Check for game end
            GameResult result = _gameEngine.CheckGameState();
            if (result.State != Models.GameState.InProgress)
            {
                HandleGameEnd(result);
                return;
            }

            // Switch player and check for computer move
            _gameEngine.SwitchPlayer();

            if (_isAgainstComputer && _gameEngine.CurrentPlayer == PlayerType.O)
            {
                ProcessComputerMove();
            }
        }

        /// <summary>
        /// Processes the computer's move
        /// </summary>
        private void ProcessComputerMove()
        {
            CellPosition computerMove = _computerPlayer.GetNextMove(_gameEngine);
            
            if (computerMove != null)
            {
                // Make the move
                _gameEngine.MakeMove(computerMove.Row, computerMove.Column);

                // Update UI
                Button button = _positionToButton[computerMove];
                button.Text = "O";
                button.Enabled = false;

                // Check for game end
                GameResult result = _gameEngine.CheckGameState();
                if (result.State != Models.GameState.InProgress)
                {
                    HandleGameEnd(result);
                    return;
                }

                // Switch back to player
                _gameEngine.SwitchPlayer();
            }
        }

        /// <summary>
        /// Handles the end of a game (win or draw)
        /// </summary>
        private void HandleGameEnd(GameResult result)
        {
            DisableAllButtons();

            string message = "";
            string winner = "";

            switch (result.State)
            {
                case Models.GameState.XWins:
                    winner = tbxP1.Text;
                    XWinCount.Text = (int.Parse(XWinCount.Text) + 1).ToString();
                    message = winner == "You" ? $"{winner} Win!" : $"{winner} Wins!";
                    break;

                case Models.GameState.OWins:
                    winner = tbxP2.Text;
                    OWinCount.Text = (int.Parse(OWinCount.Text) + 1).ToString();
                    message = winner == "You" ? $"{winner} Win!" : $"{winner} Wins!";
                    break;

                case Models.GameState.Draw:
                    message = "Draw!";
                    DrawCount.Text = (int.Parse(DrawCount.Text) + 1).ToString();
                    break;
            }

            MessageBox.Show(message, result.State == Models.GameState.Draw ? "Result" : "Congrats!");
            StartNewGame();
        }

        /// <summary>
        /// Disables all buttons on the board
        /// </summary>
        private void DisableAllButtons()
        {
            foreach (var button in _buttonToPosition.Keys)
            {
                button.Enabled = false;
            }
        }
        private void btnEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Enabled && _buttonToPosition.ContainsKey(button))
            {
                button.Text = _gameEngine.CurrentPlayer == PlayerType.X ? "X" : "O";
            }
        }

        private void btnLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Enabled)
            {
                button.Text = "";
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
                SetupComputerGame();
            }
            else
            {
                _isAgainstComputer = false;
                _computerPlayer = null;
                tbxP1.Text = "Player 1";
            }
        }
        
    }
}
