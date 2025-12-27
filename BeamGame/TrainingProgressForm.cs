using System;
using System.Drawing;
using System.Windows.Forms;
using BeamGame.AI;

namespace BeamGame
{
    /// <summary>
    /// Progress dialog for AI training with real-time statistics
    /// </summary>
    public class TrainingProgressForm : Form
    {
        private ProgressBar progressBar;
        private Label lblGames;
        private Label lblXWins;
        private Label lblOWins;
        private Label lblDraws;
        private Label lblWinRates;
        private Label lblQTableSize;
        private Label lblExplorationRate;
        private Button btnCancel;
        
        public bool IsCancelled { get; private set; }

        public TrainingProgressForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Training Q-Learning AI";
            this.Size = new Size(500, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title Label
            Label titleLabel = new Label
            {
                Text = "AI Training in Progress...",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(450, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);

            // Progress Bar
            progressBar = new ProgressBar
            {
                Location = new Point(20, 60),
                Size = new Size(440, 30),
                Style = ProgressBarStyle.Continuous
            };
            this.Controls.Add(progressBar);

            // Games Played
            lblGames = new Label
            {
                Text = "Games Played: 0 / 10,000",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 100),
                Size = new Size(440, 25)
            };
            this.Controls.Add(lblGames);

            // Win Statistics
            lblXWins = new Label
            {
                Text = "X Wins: 0",
                Font = new Font("Segoe UI", 9),
                Location = new Point(40, 130),
                Size = new Size(200, 20)
            };
            this.Controls.Add(lblXWins);

            lblOWins = new Label
            {
                Text = "O Wins: 0",
                Font = new Font("Segoe UI", 9),
                Location = new Point(40, 155),
                Size = new Size(200, 20)
            };
            this.Controls.Add(lblOWins);

            lblDraws = new Label
            {
                Text = "Draws: 0",
                Font = new Font("Segoe UI", 9),
                Location = new Point(40, 180),
                Size = new Size(200, 20)
            };
            this.Controls.Add(lblDraws);

            // Win Rates
            lblWinRates = new Label
            {
                Text = "Win Rates - X: 0.0% | O: 0.0% | Draw: 0.0%",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(20, 210),
                Size = new Size(440, 20)
            };
            this.Controls.Add(lblWinRates);

            // Q-Table Size
            lblQTableSize = new Label
            {
                Text = "Q-Table Size: 0 states",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 235),
                Size = new Size(220, 20),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblQTableSize);

            // Exploration Rate
            lblExplorationRate = new Label
            {
                Text = "Exploration Rate: 100.0%",
                Font = new Font("Segoe UI", 9),
                Location = new Point(250, 235),
                Size = new Size(210, 20),
                ForeColor = Color.DarkGreen
            };
            this.Controls.Add(lblExplorationRate);

            // Cancel Button
            btnCancel = new Button
            {
                Text = "Cancel Training",
                Location = new Point(170, 270),
                Size = new Size(140, 30),
                DialogResult = DialogResult.Cancel
            };
            btnCancel.Click += (s, e) => { IsCancelled = true; };
            this.Controls.Add(btnCancel);

            this.CancelButton = btnCancel;
        }

        public void UpdateProgress(int currentGame, int totalGames, TrainingStats stats, int qTableSize, double explorationRate)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateProgress(currentGame, totalGames, stats, qTableSize, explorationRate)));
                return;
            }

            // Update progress bar
            progressBar.Maximum = totalGames;
            progressBar.Value = currentGame;

            // Update labels
            lblGames.Text = $"Games Played: {currentGame:N0} / {totalGames:N0}";
            lblXWins.Text = $"X Wins: {stats.XWins:N0}";
            lblOWins.Text = $"O Wins: {stats.OWins:N0}";
            lblDraws.Text = $"Draws: {stats.Draws:N0}";
            lblWinRates.Text = $"Win Rates - X: {stats.XWinRate:F1}% | O: {stats.OWinRate:F1}% | Draw: {stats.DrawRate:F1}%";
            lblQTableSize.Text = $"Q-Table Size: {qTableSize:N0} states";
            lblExplorationRate.Text = $"Exploration Rate: {explorationRate * 100:F1}%";

            // Change button to "Close" when complete
            if (currentGame >= totalGames)
            {
                btnCancel.Text = "Close";
                progressBar.Value = totalGames;
            }

            Application.DoEvents();
        }
    }
}
