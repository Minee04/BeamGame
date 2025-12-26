using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BeamGame.GameLogic;
using BeamGame.Models;
using BeamGame.AI;

namespace BeamGame
{
    public enum GameMode
    {
        TwoPlayer,
        VsAI
    }

    public partial class frmMain : Form
    {
        private GameEngine _gameEngine;
        private Timer _gameTimer;
        
        private PlayerAction _player1CurrentAction;
        private PlayerAction _player2CurrentAction;
        
        private int _player1Wins;
        private int _player2Wins;
        
        private Panel _gamePanel;
        private Label _statsLabel;
        private Label _instructionsLabel;
        private Label _player1ScoreLabel;
        private Label _player2ScoreLabel;
        private Button _btnStart;
        private Button _btnReset;
        private Button _btnTrainAI;
        
        // AI mode
        private GameMode _currentGameMode;
        private ComputerPlayer _aiPlayer;
        private RadioButton _rbTwoPlayer;
        private RadioButton _rbVsAI;
        
        // AI Training
        private Label _aiStatusLabel;
        private ProgressBar _trainingProgress;
        private int _aiGamesPlayed;
        private int _aiWins;
        private const string AI_DATA_FILE = "ai_qtable.dat";
        
        // Visual constants
        private const int BEAM_WIDTH = 600;
        private const int BEAM_HEIGHT = 20;
        private const int BALL_RADIUS = 15;

        public frmMain()
        {
            InitializeComponent();
            InitializeGame();
            SetupCustomUI();
        }

        private void InitializeGame()
        {
            _gameEngine = new GameEngine();
            
            // Game timer for physics updates
            _gameTimer = new Timer();
            _gameTimer.Interval = 50; // 20 FPS
            _gameTimer.Tick += GameTimer_Tick;
            
            _player1CurrentAction = PlayerAction.None;
            _player2CurrentAction = PlayerAction.None;
            
            _player1Wins = 0;
            _player2Wins = 0;
            
            _currentGameMode = GameMode.TwoPlayer;
            _aiPlayer = new ComputerPlayer(Player.Player2);
            
            // Load AI data if exists
            LoadAIData();
        }

        private void SetupCustomUI()
        {
            this.Text = "Balance Beam Battle - 2 Player Game";
            this.ClientSize = new Size(900, 820);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;
            this.KeyDown += FrmMain_KeyDown;
            this.KeyUp += FrmMain_KeyUp;
            
            // Title panel
            Panel titlePanel = new Panel();
            titlePanel.Location = new Point(10, 10);
            titlePanel.Size = new Size(860, 100);
            titlePanel.BackColor = Color.FromArgb(230, 240, 255);
            titlePanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(titlePanel);
            
            Label titleLabel = new Label();
            titleLabel.Text = "⚖️ BALANCE BEAM BATTLE ⚖️";
            titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
            titleLabel.Location = new Point(10, 5);
            titleLabel.Size = new Size(840, 35);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titlePanel.Controls.Add(titleLabel);
            
            _instructionsLabel = new Label();
            _instructionsLabel.Text = "🎯 GOAL: Push your opponent off the beam or stay on longer!\n" +
                                     "🔴 Player 1: A/D = Move, W = Jump  |  🔵 Player 2: ← → = Move, ↑ = Jump";
            _instructionsLabel.Font = new Font("Arial", 9);
            _instructionsLabel.Location = new Point(10, 40);
            _instructionsLabel.Size = new Size(840, 50);
            _instructionsLabel.TextAlign = ContentAlignment.TopCenter;
            titlePanel.Controls.Add(_instructionsLabel);
            
            // Game Mode Selection Panel
            Panel gameModePanel = new Panel();
            gameModePanel.Location = new Point(10, 120);
            gameModePanel.Size = new Size(860, 50);
            gameModePanel.BackColor = Color.FromArgb(255, 250, 240);
            gameModePanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(gameModePanel);
            
            Label modeLabel = new Label();
            modeLabel.Text = "🎮 GAME MODE:";
            modeLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            modeLabel.Location = new Point(20, 15);
            modeLabel.Size = new Size(120, 20);
            gameModePanel.Controls.Add(modeLabel);
            
            _rbTwoPlayer = new RadioButton();
            _rbTwoPlayer.Text = "👥 1v1 (Two Players)";
            _rbTwoPlayer.Font = new Font("Arial", 9);
            _rbTwoPlayer.Location = new Point(200, 12);
            _rbTwoPlayer.Size = new Size(200, 25);
            _rbTwoPlayer.Checked = true;
            _rbTwoPlayer.CheckedChanged += GameMode_Changed;
            gameModePanel.Controls.Add(_rbTwoPlayer);
            
            _rbVsAI = new RadioButton();
            _rbVsAI.Text = "🤖 vs AI (Q-Learning)";
            _rbVsAI.Font = new Font("Arial", 9);
            _rbVsAI.Location = new Point(450, 12);
            _rbVsAI.Size = new Size(200, 25);
            _rbVsAI.CheckedChanged += GameMode_Changed;
            gameModePanel.Controls.Add(_rbVsAI);
            
            // Score display
            Panel scorePanel = new Panel();
            scorePanel.Location = new Point(10, 180);
            scorePanel.Size = new Size(860, 60);
            scorePanel.BackColor = Color.FromArgb(245, 245, 245);
            scorePanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(scorePanel);
            
            _player1ScoreLabel = new Label();
            _player1ScoreLabel.Text = "🔴 Player 1\nWins: 0";
            _player1ScoreLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            _player1ScoreLabel.ForeColor = Color.FromArgb(200, 50, 50);
            _player1ScoreLabel.Location = new Point(50, 10);
            _player1ScoreLabel.Size = new Size(200, 40);
            _player1ScoreLabel.TextAlign = ContentAlignment.MiddleLeft;
            scorePanel.Controls.Add(_player1ScoreLabel);
            
            Label vsLabel = new Label();
            vsLabel.Text = "VS";
            vsLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            vsLabel.Location = new Point(380, 15);
            vsLabel.Size = new Size(100, 30);
            vsLabel.TextAlign = ContentAlignment.MiddleCenter;
            scorePanel.Controls.Add(vsLabel);
            
            _player2ScoreLabel = new Label();
            _player2ScoreLabel.Text = "🔵 Player 2\nWins: 0";
            _player2ScoreLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            _player2ScoreLabel.ForeColor = Color.FromArgb(50, 50, 200);
            _player2ScoreLabel.Location = new Point(610, 10);
            _player2ScoreLabel.Size = new Size(200, 40);
            _player2ScoreLabel.TextAlign = ContentAlignment.MiddleRight;
            scorePanel.Controls.Add(_player2ScoreLabel);
            
            // Game panel
            _gamePanel = new Panel();
            _gamePanel.Location = new Point(10, 250);
            _gamePanel.Size = new Size(860, 380);
            _gamePanel.BackColor = Color.FromArgb(245, 250, 255);
            _gamePanel.BorderStyle = BorderStyle.FixedSingle;
            _gamePanel.Paint += GamePanel_Paint;
            this.Controls.Add(_gamePanel);
            
            // Stats panel
            _statsLabel = new Label();
            _statsLabel.Location = new Point(10, 640);
            _statsLabel.Size = new Size(860, 50);
            _statsLabel.Font = new Font("Consolas", 9);
            _statsLabel.BackColor = Color.FromArgb(240, 240, 240);
            _statsLabel.BorderStyle = BorderStyle.FixedSingle;
            _statsLabel.TextAlign = ContentAlignment.MiddleCenter;
            _statsLabel.Text = "Press START GAME to begin!";
            this.Controls.Add(_statsLabel);
            
            // Control buttons
            _btnStart = new Button();
            _btnStart.Text = "▶️ START";
            _btnStart.Location = new Point(30, 700);
            _btnStart.Size = new Size(160, 45);
            _btnStart.Font = new Font("Arial", 10, FontStyle.Bold);
            _btnStart.Click += BtnStart_Click;
            this.Controls.Add(_btnStart);
            
            _btnReset = new Button();
            _btnReset.Text = "🔄 RESET";
            _btnReset.Location = new Point(220, 700);
            _btnReset.Size = new Size(160, 45);
            _btnReset.Font = new Font("Arial", 10, FontStyle.Bold);
            _btnReset.Click += BtnReset_Click;
            this.Controls.Add(_btnReset);
            
            // AI Training button
            _btnTrainAI = new Button();
            _btnTrainAI.Text = "🎓 TRAIN AI";
            _btnTrainAI.Location = new Point(410, 700);
            _btnTrainAI.Size = new Size(160, 45);
            _btnTrainAI.Font = new Font("Arial", 10, FontStyle.Bold);
            _btnTrainAI.Click += BtnTrainAI_Click;
            this.Controls.Add(_btnTrainAI);
            
            // Reset AI button
            Button btnResetAI = new Button();
            btnResetAI.Text = "🗑️ RESET AI";
            btnResetAI.Location = new Point(600, 700);
            btnResetAI.Size = new Size(160, 45);
            btnResetAI.Font = new Font("Arial", 10, FontStyle.Bold);
            btnResetAI.Click += BtnResetAI_Click;
            btnResetAI.BackColor = Color.FromArgb(255, 240, 240);
            this.Controls.Add(btnResetAI);
            
            // AI Status Panel
            Panel aiStatusPanel = new Panel();
            aiStatusPanel.Location = new Point(10, 755);
            aiStatusPanel.Size = new Size(860, 55);
            aiStatusPanel.BackColor = Color.FromArgb(250, 245, 255);
            aiStatusPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(aiStatusPanel);
            
            Label aiTitleLabel = new Label();
            aiTitleLabel.Text = "🤖 AI STATUS:";
            aiTitleLabel.Font = new Font("Arial", 9, FontStyle.Bold);
            aiTitleLabel.Location = new Point(10, 5);
            aiTitleLabel.Size = new Size(100, 20);
            aiStatusPanel.Controls.Add(aiTitleLabel);
            
            _aiStatusLabel = new Label();
            _aiStatusLabel.Font = new Font("Consolas", 8);
            _aiStatusLabel.Location = new Point(10, 25);
            _aiStatusLabel.Size = new Size(840, 25);
            _aiStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            aiStatusPanel.Controls.Add(_aiStatusLabel);
            
            UpdateScoreDisplay();
            UpdateAIStatus();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            _gameEngine.Reset();
            _gamePanel.Invalidate();
        }

        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            int centerX = _gamePanel.Width / 2;
            int beamY = _gamePanel.Height / 2 + 30;
            
            // Draw ground line
            using (Pen groundPen = new Pen(Color.FromArgb(100, 100, 100), 3))
            {
                g.DrawLine(groundPen, 0, beamY + 80, _gamePanel.Width, beamY + 80);
            }
            
            // Draw pivot (triangle)
            int pivotSize = 35;
            Point[] pivotPoints = new Point[]
            {
                new Point(centerX, beamY - 12),
                new Point(centerX - pivotSize/2, beamY + 25),
                new Point(centerX + pivotSize/2, beamY + 25)
            };
            g.FillPolygon(Brushes.DarkGray, pivotPoints);
            g.DrawPolygon(Pens.Black, pivotPoints);
            
            // Draw beam (rotated)
            g.TranslateTransform(centerX, beamY);
            float beamAngle = (float)_gameEngine.Board.Beam.Angle;
            g.RotateTransform(beamAngle);
            
            Rectangle beamRect = new Rectangle(-BEAM_WIDTH/2, -BEAM_HEIGHT/2, BEAM_WIDTH, BEAM_HEIGHT);
            using (LinearGradientBrush beamBrush = new LinearGradientBrush(
                beamRect, Color.SaddleBrown, Color.Peru, LinearGradientMode.Vertical))
            {
                g.FillRectangle(beamBrush, beamRect);
            }
            g.DrawRectangle(Pens.Black, beamRect);
            
            g.ResetTransform();
            
            // Draw both players
            DrawPlayer(g, _gameEngine.Board.Player1Ball, centerX, beamY, beamAngle, Color.FromArgb(220, 50, 50), "P1");
            DrawPlayer(g, _gameEngine.Board.Player2Ball, centerX, beamY, beamAngle, Color.FromArgb(50, 50, 220), "P2");
            
            // Draw danger zones (edges)
            using (Pen dangerPen = new Pen(Color.Red, 2))
            {
                dangerPen.DashStyle = DashStyle.Dash;
                g.DrawLine(dangerPen, 30, beamY - 100, 30, beamY + 100);
                g.DrawLine(dangerPen, _gamePanel.Width - 30, beamY - 100, _gamePanel.Width - 30, beamY + 100);
            }
        }

        private void DrawPlayer(Graphics g, BallState ball, int centerX, int beamY, float beamAngle, Color color, string label)
        {
            if (ball.HasFallen)
            {
                // Draw fallen player below
                double radians = beamAngle * Math.PI / 180.0;
                float fallBallX = (float)(ball.Position * (BEAM_WIDTH / 2.0));
                float fallX = centerX + fallBallX * (float)Math.Cos(radians);
                float fallY = beamY + 120;
                
                using (SolidBrush fadedBrush = new SolidBrush(Color.FromArgb(100, color)))
                {
                    g.FillEllipse(fadedBrush, fallX - BALL_RADIUS, fallY - BALL_RADIUS, BALL_RADIUS * 2, BALL_RADIUS * 2);
                }
                using (Pen fadedPen = new Pen(Color.FromArgb(100, Color.Black), 2))
                {
                    g.DrawEllipse(fadedPen, fallX - BALL_RADIUS, fallY - BALL_RADIUS, BALL_RADIUS * 2, BALL_RADIUS * 2);
                }
                
                // Draw X
                using (Pen xPen = new Pen(Color.Red, 3))
                {
                    g.DrawLine(xPen, fallX - 10, fallY - 10, fallX + 10, fallY + 10);
                    g.DrawLine(xPen, fallX + 10, fallY - 10, fallX - 10, fallY + 10);
                }
                return;
            }
            
            // Calculate position on beam
            double radians2 = beamAngle * Math.PI / 180.0;
            float ballXOnBeam = (float)(ball.Position * (BEAM_WIDTH / 2.0));
            float ballWorldX = centerX + ballXOnBeam * (float)Math.Cos(radians2);
            float ballWorldY = beamY + ballXOnBeam * (float)Math.Sin(radians2) - BALL_RADIUS - BEAM_HEIGHT/2;
            
            // Add vertical position for jumping
            ballWorldY -= (float)(ball.VerticalPosition * 100);
            
            // Draw ball
            using (SolidBrush ballBrush = new SolidBrush(color))
            {
                g.FillEllipse(ballBrush, 
                    ballWorldX - BALL_RADIUS, 
                    ballWorldY - BALL_RADIUS, 
                    BALL_RADIUS * 2, 
                    BALL_RADIUS * 2);
            }
            using (Pen ballPen = new Pen(Color.Black, 2))
            {
                g.DrawEllipse(ballPen, 
                    ballWorldX - BALL_RADIUS, 
                    ballWorldY - BALL_RADIUS, 
                    BALL_RADIUS * 2, 
                    BALL_RADIUS * 2);
            }
            
            // Draw label
            using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(label, labelFont);
                g.DrawString(label, labelFont, Brushes.White, 
                    ballWorldX - textSize.Width/2, 
                    ballWorldY - textSize.Height/2);
            }
            
            // Draw jump indicator
            if (!ball.IsOnBeam && ball.VerticalPosition > 0.05)
            {
                using (Pen jumpPen = new Pen(Color.Yellow, 2))
                {
                    jumpPen.DashStyle = DashStyle.Dash;
                    g.DrawEllipse(jumpPen, 
                        ballWorldX - BALL_RADIUS - 3, 
                        ballWorldY - BALL_RADIUS - 3, 
                        (BALL_RADIUS + 3) * 2, 
                        (BALL_RADIUS + 3) * 2);
                }
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            var result = _gameEngine.CheckGameState();
            
            if (result.State == Models.GameState.InProgress)
            {
                // In AI mode, get AI action for Player 2
                if (_currentGameMode == GameMode.VsAI)
                {
                    _player2CurrentAction = _aiPlayer.DetermineMove(_gameEngine);
                }
                
                _gameEngine.Step(_player1CurrentAction, _player2CurrentAction);
                _gamePanel.Invalidate();
                UpdateStats();
            }
            else
            {
                // Game ended
                _gameTimer.Stop();
                HandleGameEnd(result);
            }
        }

        private void HandleGameEnd(GameResult result)
        {
            string message = "";
            
            switch (result.State)
            {
                case Models.GameState.Player1Wins:
                    _player1Wins++;
                    message = "🔴 Player 1 Wins!\n\nPlayer 2 fell off the beam!";
                    break;
                    
                case Models.GameState.Player2Wins:
                    _player2Wins++;
                    message = "🔵 Player 2 Wins!\n\nPlayer 1 fell off the beam!";
                    break;
                    
                case Models.GameState.BothFell:
                    message = "😱 Both Players Fell!\n\nIt's a draw!";
                    break;
                    
                case Models.GameState.TimeExpired:
                    message = "⏰ Time's Up!\n\nBoth players survived - It's a draw!";
                    break;
            }
            
            UpdateScoreDisplay();
            _gamePanel.Invalidate();
            
            MessageBox.Show($"{message}\n\nGame Time: {result.GameTime:F2}s", 
                "Round Over!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            _btnStart.Text = "▶️ PLAY AGAIN";
        }

        private void UpdateStats()
        {
            var p1 = _gameEngine.Board.Player1Ball;
            var p2 = _gameEngine.Board.Player2Ball;
            
            string modeText = _currentGameMode == GameMode.VsAI ? "🤖 vs AI Mode" : "👥 1v1 Mode";
            
            _statsLabel.Text = $"{modeText}  |  Time: {_gameEngine.GameTime:F1}s  |  " +
                              $"🔴 P1 Pos: {p1.Position:F2}  |  " +
                              $"🔵 P2 Pos: {p2.Position:F2}  |  " +
                              $"Beam Angle: {_gameEngine.Board.Beam.Angle:F1}°";
        }

        private void UpdateScoreDisplay()
        {
            _player1ScoreLabel.Text = $"🔴 Player 1\nWins: {_player1Wins}";
            _player2ScoreLabel.Text = $"🔵 Player 2\nWins: {_player2Wins}";
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            _gameEngine.Reset();
            _player1CurrentAction = PlayerAction.None;
            _player2CurrentAction = PlayerAction.None;
            _gameTimer.Start();
            _btnStart.Text = "▶️ PLAYING...";
            _gamePanel.Invalidate();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            _player1Wins = 0;
            _player2Wins = 0;
            UpdateScoreDisplay();
            _gameEngine.Reset();
            _gameTimer.Stop();
            _btnStart.Text = "▶️ START GAME";
            _statsLabel.Text = "Press START GAME to begin!";
            _gamePanel.Invalidate();
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            // Keys are handled in ProcessCmdKey for key press
            // This event is kept for compatibility but ProcessCmdKey handles the logic
        }

        private void FrmMain_KeyUp(object sender, KeyEventArgs e)
        {
            // Player 1 - release controls
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.D || e.KeyCode == Keys.W)
            {
                _player1CurrentAction = PlayerAction.None;
            }
            
            // Player 2 - release controls (only in 1v1 mode)
            if (_currentGameMode == GameMode.TwoPlayer)
            {
                if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up)
                {
                    _player2CurrentAction = PlayerAction.None;
                }
            }
        }
        
        private void GameMode_Changed(object sender, EventArgs e)
        {
            if (_rbTwoPlayer.Checked)
            {
                _currentGameMode = GameMode.TwoPlayer;
                _instructionsLabel.Text = "🎯 GOAL: Push your opponent off the beam or stay on longer!\n" +
                                         "🔴 Player 1: A/D = Move, W = Jump  |  🔵 Player 2: ← → = Move, ↑ = Jump";
                _player2ScoreLabel.Text = "🔵 Player 2\nWins: " + _player2Wins;
            }
            else
            {
                _currentGameMode = GameMode.VsAI;
                _instructionsLabel.Text = "🎯 GOAL: Push the AI opponent off the beam or stay on longer!\n" +
                                         "🔴 You: A/D = Move, W = Jump  |  🤖 AI: Q-Learning (Reinforcement Learning)";
                _player2ScoreLabel.Text = "🤖 AI\nWins: " + _player2Wins;
            }
            
            // Stop game if running when mode changes
            if (_gameTimer.Enabled)
            {
                _gameTimer.Stop();
                _gameEngine.Reset();
                _btnStart.Text = "▶️ START GAME";
                _statsLabel.Text = "Press START GAME to begin!";
                _gamePanel.Invalidate();
            }
        }
        
        // Override to prevent arrow keys from navigating form controls
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle game controls directly here to prevent form navigation
            if (keyData == Keys.A)
            {
                _player1CurrentAction = PlayerAction.MoveLeft;
                return true;
            }
            else if (keyData == Keys.D)
            {
                _player1CurrentAction = PlayerAction.MoveRight;
                return true;
            }
            else if (keyData == Keys.W)
            {
                _player1CurrentAction = PlayerAction.Jump;
                return true;
            }
            
            // Player 2 controls (only in 1v1 mode)
            if (_currentGameMode == GameMode.TwoPlayer)
            {
                if (keyData == Keys.Left)
                {
                    _player2CurrentAction = PlayerAction.MoveLeft;
                    return true;
                }
                else if (keyData == Keys.Right)
                {
                    _player2CurrentAction = PlayerAction.MoveRight;
                    return true;
                }
                else if (keyData == Keys.Up)
                {
                    _player2CurrentAction = PlayerAction.Jump;
                    return true;
                }
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _gameTimer?.Stop();
            SaveAIData();
        }
        
        private void LoadAIData()
        {
            try
            {
                if (System.IO.File.Exists(AI_DATA_FILE))
                {
                    _aiPlayer.LoadQTable(AI_DATA_FILE);
                    
                    // Try to load training stats
                    string statsFile = "ai_stats.dat";
                    if (System.IO.File.Exists(statsFile))
                    {
                        string[] lines = System.IO.File.ReadAllLines(statsFile);
                        if (lines.Length >= 2)
                        {
                            int.TryParse(lines[0], out _aiGamesPlayed);
                            int.TryParse(lines[1], out _aiWins);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load AI data: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void SaveAIData()
        {
            try
            {
                _aiPlayer.SaveQTable(AI_DATA_FILE);
                
                // Save training stats
                string statsFile = "ai_stats.dat";
                System.IO.File.WriteAllLines(statsFile, new[]
                {
                    _aiGamesPlayed.ToString(),
                    _aiWins.ToString()
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save AI data: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void UpdateAIStatus()
        {
            // Get AI statistics
            var aiStats = _aiPlayer.GetAIStatistics();
            int qTableSize = aiStats.Item1;
            double explorationRate = aiStats.Item2;
            
            // Calculate AI level based on games played
            string level = "Beginner";
            if (_aiGamesPlayed >= 50000) level = "Master";
            else if (_aiGamesPlayed >= 20000) level = "Expert";
            else if (_aiGamesPlayed >= 10000) level = "Advanced";
            else if (_aiGamesPlayed >= 5000) level = "Intermediate";
            else if (_aiGamesPlayed >= 1000) level = "Novice";
            
            double winRate = _aiGamesPlayed > 0 ? (_aiWins * 100.0 / _aiGamesPlayed) : 0;
            
            _aiStatusLabel.Text = $"Level: {level}  |  Games Trained: {_aiGamesPlayed:N0}  |  Win Rate: {winRate:F1}%  |  " +
                                 $"States Learned: {qTableSize:N0}  |  Exploration: {explorationRate:P0}";
        }
                private void BtnResetAI_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "This will delete all AI training data and reset the AI to beginner level.\n\n" +
                "Are you sure you want to reset the AI?",
                "Reset AI Knowledge",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
                
            if (result == DialogResult.Yes)
            {
                try
                {
                    // Delete AI files
                    if (System.IO.File.Exists(AI_DATA_FILE))
                    {
                        System.IO.File.Delete(AI_DATA_FILE);
                    }
                    
                    string statsFile = "ai_stats.dat";
                    if (System.IO.File.Exists(statsFile))
                    {
                        System.IO.File.Delete(statsFile);
                    }
                    
                    // Reset AI player
                    _aiPlayer = new ComputerPlayer(Player.Player2);
                    _aiGamesPlayed = 0;
                    _aiWins = 0;
                    
                    UpdateAIStatus();
                    
                    MessageBox.Show(
                        "AI knowledge has been reset successfully!\n\n" +
                        "The AI is now at beginner level and needs training.",
                        "Reset Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to reset AI: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        
        private void BtnTrainAI_Click(object sender, EventArgs e)
        {
            // Create training form
            Form trainingForm = new Form();
            trainingForm.Text = "AI Training in Progress";
            trainingForm.Size = new Size(500, 250);
            trainingForm.StartPosition = FormStartPosition.CenterParent;
            trainingForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            trainingForm.MaximizeBox = false;
            trainingForm.MinimizeBox = false;
            
            Label infoLabel = new Label();
            infoLabel.Text = "Training AI using Q-Learning (Reinforcement Learning)\nThe AI plays against itself to learn winning strategies.";
            infoLabel.Location = new Point(20, 20);
            infoLabel.Size = new Size(450, 40);
            infoLabel.Font = new Font("Arial", 9);
            trainingForm.Controls.Add(infoLabel);
            
            Label gamesLabel = new Label();
            gamesLabel.Text = "Number of training games:";
            gamesLabel.Location = new Point(20, 70);
            gamesLabel.Size = new Size(200, 20);
            trainingForm.Controls.Add(gamesLabel);
            
            NumericUpDown numGames = new NumericUpDown();
            numGames.Location = new Point(220, 68);
            numGames.Size = new Size(120, 25);
            numGames.Minimum = 100;
            numGames.Maximum = 100000;
            numGames.Value = 5000;
            numGames.Increment = 500;
            trainingForm.Controls.Add(numGames);
            
            ProgressBar progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 110);
            progressBar.Size = new Size(450, 25);
            trainingForm.Controls.Add(progressBar);
            
            Label progressLabel = new Label();
            progressLabel.Location = new Point(20, 140);
            progressLabel.Size = new Size(450, 20);
            progressLabel.Font = new Font("Consolas", 8);
            trainingForm.Controls.Add(progressLabel);
            
            Button startBtn = new Button();
            startBtn.Text = "Start Training";
            startBtn.Location = new Point(150, 170);
            startBtn.Size = new Size(120, 35);
            trainingForm.Controls.Add(startBtn);
            
            Button cancelBtn = new Button();
            cancelBtn.Text = "Cancel";
            cancelBtn.Location = new Point(280, 170);
            cancelBtn.Size = new Size(120, 35);
            cancelBtn.DialogResult = DialogResult.Cancel;
            trainingForm.Controls.Add(cancelBtn);
            
            startBtn.Click += (s, ev) =>
            {
                startBtn.Enabled = false;
                numGames.Enabled = false;
                
                int gamesToTrain = (int)numGames.Value;
                var trainer = new AITrainer(_aiPlayer.GetQLearningAI());
                
                // Train in background
                System.Threading.Tasks.Task.Run(() =>
                {
                    var stats = trainer.TrainAI(gamesToTrain, (gamesCompleted, trainingStats) =>
                    {
                        // Update UI on main thread
                        trainingForm.Invoke((Action)(() =>
                        {
                            progressBar.Value = (int)((gamesCompleted * 100.0) / gamesToTrain);
                            progressLabel.Text = $"Progress: {gamesCompleted}/{gamesToTrain} games  |  Win Rate: {trainingStats.WinRate:F1}%";
                        }));
                    });
                    
                    // Update stats
                    _aiGamesPlayed += stats.TotalGames;
                    _aiWins += stats.Wins;
                    
                    // Save after training
                    SaveAIData();
                    
                    // Close form on main thread
                    trainingForm.Invoke((Action)(() =>
                    {
                        UpdateAIStatus();
                        MessageBox.Show($"Training Complete!\n\nGames: {stats.TotalGames:N0}\nWins: {stats.Wins} ({stats.WinRate:F1}%)\nAvg Game Time: {stats.AverageGameTime:F2}s",
                            "Training Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        trainingForm.Close();
                    }));
                });
            };
            
            trainingForm.ShowDialog();
        }
    }
}
