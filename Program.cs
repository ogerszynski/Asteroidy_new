using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AsteroidsGame
{
    class Program
    {
        private const int screenWidth = 80;
        private const int screenHeight = 24;
        private static Random random = new Random();

        static void Main(string[] args)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                RedirectStandardInput = false,
                UseShellExecute = true
            });

            Thread gameThread = new Thread(GameLoop);
            gameThread.Start();
            
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
//statek
        static void GameLoop()
        {
            Console.CursorVisible = false;

            Statek playerShip = new Statek(screenWidth / 2, screenHeight - 1);
            List<Asteroidy> asteroids = new List<Asteroidy>();
            List<Strzal> projectiles = new List<Strzal>();

            try
            {
                while (true)
                {
                    HandleKeyboardInput(playerShip, projectiles);
                    UpdateGameState(projectiles, asteroids);
                    try
                    {
                        RenderGameState(playerShip, asteroids, projectiles);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Console.WriteLine($"Wyjątek w RenderGameState(): {ex.Message}");
                    }

                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił nieobsłużony wyjątek: {ex.Message}");
            }
        }
//sterowanie klawiatura
        static void HandleKeyboardInput(Statek playerShip, List<Strzal> projectiles)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.LeftArrow)
                    playerShip.MoveLeft();
                else if (key.Key == ConsoleKey.RightArrow)
                    playerShip.MoveRight(screenWidth);
                else if (key.Key == ConsoleKey.Spacebar)
                    playerShip.Shoot(projectiles);
            }
        }
//pociski wyswietlanie i trafianie
        static void UpdateGameState(List<Strzal> projectiles, List<Asteroidy> asteroids)
        {
            MoveProjectiles(projectiles);
            MoveAsteroids(asteroids);
            CheckCollisions(projectiles, asteroids);
            GenerateNewAsteroids(asteroids);
        }

        static void MoveProjectiles(List<Strzal> projectiles)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Y--;

                if (projectiles[i].Y < 0)
                    projectiles.RemoveAt(i);
            }
        }
//Asteroidy tworzenie i ruch
        static void MoveAsteroids(List<Asteroidy> asteroids)
        {
            foreach (var asteroid in asteroids)
            {
                asteroid.Y++;
            }
        }

        static void GenerateNewAsteroids(List<Asteroidy> asteroids)
        {
            if (random.Next(0, 100) < 10)
            {
                int newX = random.Next(0, screenWidth);
                asteroids.Add(new Asteroidy(newX, 0));
            }
        }
//trafienia
        static void CheckCollisions(List<Strzal> projectiles, List<Asteroidy> asteroids)
        {
                        for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    if (projectiles[i].X == asteroids[j].X && projectiles[i].Y == asteroids[j].Y)
                    {
                        
                        projectiles.RemoveAt(i);
                        asteroids.RemoveAt(j);
                        break;
                    }
                }
            }
        }
//rysowanie gry - asteroidy, statek i straly
        static void RenderGameState(Statek playerShip, List<Asteroidy> asteroids, List<Strzal> projectiles)
        {
            Console.Clear();

            
            if (playerShip.X >= 0 && playerShip.X < Console.WindowWidth && playerShip.Y >= 0 && playerShip.Y < Console.WindowHeight)
            {
                Console.SetCursorPosition(playerShip.X, playerShip.Y);
                Console.Write("^");
            }

                        foreach (var asteroid in asteroids)
            {
                if (asteroid.X >= 0 && asteroid.X < Console.WindowWidth && asteroid.Y >= 0 && asteroid.Y < Console.WindowHeight)
                {
                    Console.SetCursorPosition(asteroid.X, asteroid.Y);
                    Console.Write("O");
                }
            }

            
            foreach (var projectile in projectiles)
            {
                if (projectile.X >= 0 && projectile.X < Console.WindowWidth && projectile.Y >= 0 && projectile.Y < Console.WindowHeight)
                {
                    Console.SetCursorPosition(projectile.X, projectile.Y);
                    Console.Write("|");
                }
            }
        }
    }

    class Statek
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Statek(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void MoveLeft()
        {
            if (X > 0)
                X -= 1;
        }

        public void MoveRight(int screenWidth)
        {
            if (X < screenWidth - 1)
                X += 1;
        }

        public void Shoot(List<Strzal> projectiles)
        {
            projectiles.Add(new Strzal(X, Y - 1));
        }
    }

    class Asteroidy
    {
        public int X { get; }
        public int Y { get; set; }

        public Asteroidy(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class Strzal
    {
        public int X { get; }
        public int Y { get; set; }

        public Strzal(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
