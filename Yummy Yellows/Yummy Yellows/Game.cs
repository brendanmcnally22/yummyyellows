
// Include the namespaces (code libraries) you need below.
using System;
using System.Collections.Generic;
using System.Numerics;

// The namespace your code is in.
namespace MohawkGame2D
{
    /// <summary>
    ///     Your game code goes inside this class!
    /// </summary>
    public class Game
    {
        private class Collectible
        {
            public Vector2 Position;
            public bool Moves;
            public float Speed = 1.0f;
        }

        private List<Collectible> collectibles = new List<Collectible>();
        private System.Random random = new System.Random();
        private int playerScore = 0;
        private bool showScoreboard = false;
        private Vector2 followerPosition = new Vector2(100, 100);
        private float followerSpeed = 1.0f;
        private Vector2 playerPosition = new Vector2(200, 200);
        private Color[] playerColors = { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Magenta };
        private int currentColorIndex = 0;
        private Color playerColor = Color.Red;
        private float speed = 2.0f;
        private bool isDashing = false;
        private float dashTimer = 0;
        private bool isShielded = false;
        private float shieldTimer = 0;
        private bool slowMotion = false;
        private float slowMotionTimer = 0;
        private Texture2D playerTexture;
        private Texture2D aiTexture;
        private enum GameState { MainMenu, Playing }
        private GameState currentState = GameState.MainMenu;

        Music[] music = new Music[1];
        public void Setup()
            
        {
            Graphics.LoadTexture("C:\\Users\\brend\\Downloads\\YummyMenu.png");
            music[0] = Audio.LoadMusic("C:\\Users\\brend\\Downloads\\CantTouchThis.mp3");
            Window.SetSize(400, 400);
            Window.SetTitle("YUMMY YELLOWS");
            SpawnCollectibles();
            Audio.Play(music[0]);
        }

        public void Update()
        {
            Window.ClearBackground(Color.OffWhite);

            if (currentState == GameState.MainMenu)
            {
                drawMainMenu();
                HandleMenuInput();
            }
            else if (currentState == GameState.Playing)
            {
               
                HandleInput();
                UpdateFollower();
                UpdateCollectibles();
                CheckCollisions();
                UpdateTimers();
                DrawPlayer();
                DrawCollectibles();
                DrawFollower();
            }
        }
        void SpawnCollectibles()
        {
            for (int i = 0; i < 20; i++)
            {
                collectibles.Add(new Collectible
                {
                    Position = new Vector2(random.Next(20, 380), random.Next(20, 380)),
                    Moves = random.Next(2) == 0
                });
            }
        }

        void UpdateCollectibles()
        {
            foreach (var collectible in collectibles)
            {
                if (collectible.Moves)
                {
                    collectible.Position.X += random.Next(-1, 2);
                    collectible.Position.Y += random.Next(-1, 2);
                }
            }
        }

        void HandleInput() // Controller Inputs and stuff 
        {

            {
                float moveX = Input.GetControllerAxis(0, ControllerAxis.LeftX); // move player
                float moveY = Input.GetControllerAxis(0, ControllerAxis.LeftY);

                float currentSpeed = isDashing ? speed * 2 : speed;
                if (slowMotion) currentSpeed *= 0.5f;

                playerPosition.X += moveX * currentSpeed;
                playerPosition.Y += moveY * currentSpeed;

                if (Input.IsControllerButtonPressed(0, ControllerButton.RightFaceRight))
                {
                    currentColorIndex = (currentColorIndex + 1) % playerColors.Length;
                    playerColor = playerColors[currentColorIndex];
                }

                if (Input.IsControllerButtonPressed(0, ControllerButton.LeftTrigger1))
                {
                    isDashing = true;
                    dashTimer = 0.5f;
                }

                if (Input.IsControllerButtonPressed(0, ControllerButton.RightTrigger1))
                {
                    slowMotion = true;
                    slowMotionTimer = 1.0f;
                }

                if (Input.IsControllerButtonDown(0, ControllerButton.LeftTrigger2))
                {
                    (showScoreboard) = true;
                    DrawScoreboard();
                }

            }
        }
        void CheckCollisions()
        {
            if (Math.Abs(playerPosition.X - followerPosition.X) < 20 && Math.Abs(playerPosition.Y - followerPosition.Y) < 20)
            {
                Console.WriteLine("Follower caught you!");
            }
            {
                for (int i = collectibles.Count - 1; i >= 0; i--)
                {
                    if (Math.Abs(playerPosition.X - collectibles[i].Position.X) < 15 &&
                        Math.Abs(playerPosition.Y - collectibles[i].Position.Y) < 15)
                    {
                        collectibles.RemoveAt(i);
                        playerScore++;
                        SpawnNewCollectible();
                    }
                }
            }
        }
        void SpawnNewCollectible()
        {
            collectibles.Add(new Collectible
            {
                Position = new Vector2(random.Next(20, 380), random.Next(20, 380)),
                Moves = random.Next(2) == 0
            });
        }

        void UpdateFollower()
        {




            Vector2 direction = new Vector2(playerPosition.X - followerPosition.X, playerPosition.Y - followerPosition.Y);
            float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            if (length > 0)
            {
                direction.X /= length;
                direction.Y /= length;
                followerPosition.X += direction.X * followerSpeed;
                followerPosition.Y += direction.Y * followerSpeed;
            }
        }

        void DrawFollower()
        {
            Draw.FillColor = Color.Red;
            Draw.Rectangle(followerPosition.X, followerPosition.Y, 20, 20);
        }
        void DrawCollectibles()
        {
            DrawFollower();
            {
                DrawFollower();
                {
                    Draw.FillColor = Color.Yellow;
                    foreach (var collectible in collectibles)
                    {
                        Draw.Rectangle(collectible.Position.X, collectible.Position.Y, 15, 15);
                    }
                }
            }
        }
        void DrawPlayer()
        {
            if (showScoreboard)
            {
                Draw.FillColor = Color.Black;

            }
            {
                Draw.FillColor = playerColor;
                Draw.Rectangle(playerPosition.X, playerPosition.Y, 20, 20);
            }

        }

        void UpdateTimers()
        {
            UpdateFollower();
            {
                if (isDashing) // doing dash
                {
                    dashTimer -= 0.016f;
                    if (dashTimer <= 0) isDashing = false;
                }
                if (slowMotion)
                {
                    slowMotionTimer -= 0.016f;
                    if (slowMotionTimer <= 0) slowMotion = false;
                }
                if (isShielded)
                {
                    shieldTimer -= 0.016f;
                    if (shieldTimer <= 0) isShielded = false;
                }
            }
        }
        void DrawScoreboard()
        {
            Draw.FillColor = Color.Black;
            Text.Draw("Score: " + playerScore, 10, 10);
        }
        void drawMainMenu()
        {

            Texture2D backgroundTexture = Graphics.LoadTexture("C:\\Users\\brend\\OneDrive\\Documents\\YummyMenu.png");
            if (backgroundTexture.Width == 0 || backgroundTexture.Height == 0)
            {
                Console.WriteLine("Failed to load texture.");
            }
            else
            {
                Graphics.Draw(backgroundTexture, 0, 0);
            }

        }
        void HandleMenuInput()
        {

            if (Input.IsControllerButtonPressed(0, ControllerButton.MiddleLeft))
            {
                currentState = GameState.Playing;
            }
        }

    }
}

