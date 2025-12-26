using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        private QLearningAI _qLearningAI;
        private bool _isAgainstComputer;
        private bool _useQLearning;
        private Dictionary<Button, CellPosition> _buttonToPosition;
        private Dictionary<CellPosition, Button> _positionToButton;
        private const string QTablePath = "qtable.dat"; 

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
            _useQLearning = false;

            // Try to load existing Q-table
            _qLearningAI = new QLearningAI(
                learningRate: 0.3,
                discountFactor: 0.95,
                explorationRate: 0.0,  // NO exploration during gameplay - play optimally!
                explorationDecay: 0.999
            );

            try
            {
                _qLearningAI.LoadQTable(QTablePath);
            }
            catch
            {
                // Q-table doesn't exist yet, will be created through training
            }

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

            // Initialize UI state
            UpdateAIStatus();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Form is loaded, game mode can be selected via radio buttons
            UpdateAIStatus();
        }

        private void tbxP2_TextChanged(object sender, EventArgs e)
        {
            // Keep this for compatibility but main switching is via radio buttons
        }

        /// <summary>
        /// Handles game mode changes from radio buttons
        /// </summary>
        private void GameModeChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked)
                return;

            // Start a new game when mode changes
            StartNewGame();

            if (rb.Name == "rbPlayerVsPlayer")
            {
                _isAgainstComputer = false;
                _useQLearning = false;
                _computerPlayer = null;
                tbxP1.Text = "Player 1";
                tbxP2.Text = "Player 2";
            }
            else if (rb.Name == "rbStrategicAI")
            {
                _isAgainstComputer = true;
                _useQLearning = false;
                _computerPlayer = new ComputerPlayer(PlayerType.O, new StrategicAI());
                tbxP1.Text = "You";
                tbxP2.Text = "Strategic AI";
            }
            else if (rb.Name == "rbQLearningAI")
            {
                _isAgainstComputer = true;
                _useQLearning = true;
                _computerPlayer = new ComputerPlayer(PlayerType.O, _qLearningAI);
                _qLearningAI.StartNewGame();
                tbxP1.Text = "You";
                tbxP2.Text = "Q-Learning AI";
            }

            UpdateAIStatus();
        }

        /// <summary>
        /// Updates the AI status display with training information
        /// </summary>
        private void UpdateAIStatus()
        {
            int qTableSize = _qLearningAI.QTableSize;

            if (qTableSize == 0)
            {
                lblAIStatusValue.Text = "❌ Not trained\n\nTrain the AI using:\nSettings → Train AI";
                lblAIStatusValue.ForeColor = Color.DarkRed;
            }
            else if (qTableSize < 100)
            {
                lblAIStatusValue.Text = $"⚠️ Beginner\n\nStates Learned: {qTableSize:N0}\n\nTrain more for better play";
                lblAIStatusValue.ForeColor = Color.DarkOrange;
            }
            else if (qTableSize < 500)
            {
                lblAIStatusValue.Text = $"📚 Learning\n\nStates Learned: {qTableSize:N0}\n\nMaking progress!";
                lblAIStatusValue.ForeColor = Color.DarkGoldenrod;
            }
            else if (qTableSize < 2000)
            {
                lblAIStatusValue.Text = $"🎯 Competent\n\nStates Learned: {qTableSize:N0}\n\nPlays well";
                lblAIStatusValue.ForeColor = Color.DarkBlue;
            }
            else
            {
                lblAIStatusValue.Text = $"🏆 Expert\n\nStates Learned: {qTableSize:N0}\n\nNear-optimal play!";
                lblAIStatusValue.ForeColor = Color.DarkGreen;
            }
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
            // If using Q-Learning, let it learn from this game
            if (_useQLearning && _isAgainstComputer)
            {
                _qLearningAI.LearnFromGame(result);
                _qLearningAI.SaveQTable(QTablePath);
                UpdateAIStatus(); // Refresh status display
            }

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
            
            // Start new game and reset Q-Learning history
            if (_useQLearning)
            {
                _qLearningAI.StartNewGame();
            }
            
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

        private void trainAIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Train Q-Learning AI?\n\nThis will play 10,000 games for training.\nIt may take 5-10 seconds.\n\nContinue?",
                "Train AI",
                MessageBoxButtons.YesNo
            );

            if (result == DialogResult.Yes)
            {
                TrainQLearningAI();
            }
        }

        private void resetAIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Reset Q-Learning AI?\n\nThis will delete all learned knowledge.\nThe AI will need to be retrained.\n\nContinue?",
                "Reset AI",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                _qLearningAI.ResetKnowledge();
                
                // Delete qtable.dat file if it exists
                if (System.IO.File.Exists(QTablePath))
                {
                    System.IO.File.Delete(QTablePath);
                }
                
                UpdateAIStatus();
                MessageBox.Show(
                    "AI knowledge has been reset.\nThe AI is now untrained.",
                    "Reset Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        /// <summary>
        /// Trains the Q-Learning AI by playing games against itself
        /// </summary>
        private void TrainQLearningAI()
        {
            const int totalGames = 10000;

            // Create a training AI with higher exploration
            var trainingAI = new QLearningAI(
                learningRate: 0.3,
                discountFactor: 0.95,
                explorationRate: 1.0,  // Start with full exploration
                explorationDecay: 0.998
            );

            // Load existing knowledge if available
            try
            {
                trainingAI.LoadQTable(QTablePath);
            }
            catch { }

            var trainer = new AITrainer(trainingAI, new QLearningAI());

            // Create and show progress form
            using (var progressForm = new TrainingProgressForm())
            {
                progressForm.Show();
                double currentExplorationRate = trainingAI.ExplorationRate;
                progressForm.UpdateProgress(0, totalGames, new TrainingStats(), trainingAI.QTableSize, currentExplorationRate);

                // Train with progress updates every 100 games
                var stats = trainer.TrainAI(totalGames, (games, currentStats) =>
                {
                    currentExplorationRate = trainingAI.ExplorationRate;
                    progressForm.UpdateProgress(games, totalGames, currentStats, trainingAI.QTableSize, currentExplorationRate);
                    
                    if (progressForm.IsCancelled)
                    {
                        // Allow early stopping
                        throw new OperationCanceledException();
                    }
                });

                // Final update
                progressForm.UpdateProgress(totalGames, totalGames, stats, trainingAI.QTableSize, trainingAI.ExplorationRate);

                // Save the trained Q-table
                trainingAI.SaveQTable(QTablePath);

                // Reload the Q-table into the game AI
                _qLearningAI.LoadQTable(QTablePath);

                // Update AI status display
                UpdateAIStatus();

                MessageBox.Show(
                    $"Training Complete!\n\n{stats}\n\nQ-Table Size: {trainingAI.QTableSize:N0} states\nExploration Rate: {trainingAI.ExplorationRate * 100:F1}%",
                    "Training Results",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        
    }
}
