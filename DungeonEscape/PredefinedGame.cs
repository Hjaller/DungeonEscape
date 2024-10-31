using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonEscape
{
    internal class PredefinedGame
    {

        private int[,] _maze =
        {
            //1 wall, 2 trap, 3 key, 4 exit 
            { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 2, 3, 0, 0, 0, 1, 1 },
            { 1, 0, 1, 0, 0, 2, 0, 1, 1 },
            { 1, 0, 2, 0, 2, 3, 0, 0, 1 },
            { 1, 0, 0, 0, 2, 2, 2, 0, 1 },
            { 1, 2, 2, 2, 1, 1, 2, 4, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        }; // 2D array representing the maze
        private (int, int) _playerStart = (1, 1), _playerPosition;
        private bool _hasKey = false; // Indicates whether the player has the key
        private bool _showKeysAndTraps;
        public PredefinedGame(bool showKeysAndTraps = false)
        {
            _showKeysAndTraps = showKeysAndTraps; // Set the preference based on input
        }
        public void StartGame()
        {
            
            _playerPosition = _playerStart; // Set the player's starting position
            PrintMaze(); // Print the maze

            while (true)
            {
                string message = ""; // Initialize an empty message

                // Get user input
                ConsoleKey move = Console.ReadKey().Key;
                bool moved = MovePlayer(move); // Move the player based on input

                // Check for actions based on player's position
                if (_maze[_playerPosition.Item1, _playerPosition.Item2] == 3)
                {
                    message = "You have found the key!"; // Message when the key is found
                    _hasKey = true;
                }
                else if (_maze[_playerPosition.Item1, _playerPosition.Item2] == 2)
                {
                    message = "You fell into a trap! Start over."; // Message when falling into a trap
                    _playerPosition = _playerStart; // Reset player's position
                    _hasKey = false; // Reset key status
                }
                else if (_maze[_playerPosition.Item1, _playerPosition.Item2] == 4)
                {
                    if (!_hasKey)
                    {
                        message = "You need the key to exit!"; // Message when trying to exit without key

                    } else
                    {
                        message = "Congratulations! You have escaped the maze!"; // Message when exiting
                        PrintMaze(message); // Print maze with exit message
                        break; // End the game
                    }

                }
                else if (!moved) // If movement failed
                {
                    message = "There's a wall there!";
                }

                // Print the maze and any message
                PrintMaze(message);
            }
        }

        private void PrintMaze(string message = "")
        {
            Console.Clear(); // Clear the console
            // Print the maze
            for (int i = 0; i < _maze.GetLength(0); i++)
            {
                for (int j = 0; j < _maze.GetLength(1); j++)
                {
                    if (i == _playerPosition.Item1 && j == _playerPosition.Item2)
                        Console.Write("P "); // Player
                    else if (_maze[i, j] == 4)
                        Console.Write("E "); // Exit
                    else if (_showKeysAndTraps)
                    {
                        if (_maze[i, j] == 3 && !_hasKey)
                            Console.Write("K "); // Key
                        else if (_maze[i, j] == 2)
                            Console.Write("X "); // Trap
                        else
                            Console.Write(_maze[i, j] == 1 ? "# " : ". ");
                    }
                    else
                    {
                        Console.Write(_maze[i, j] == 1 ? "# " : ". "); // Only show walls and open paths
                    }
                }
                Console.WriteLine();
            }

            // Display a message if provided
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine(message);
            }
        }

        private bool MovePlayer(ConsoleKey key)
        {
            int newX = _playerPosition.Item1, newY = _playerPosition.Item2;

            // Update position based on key pressed
            switch (key)
            {
                case ConsoleKey.W: newX--; break; // Move up
                case ConsoleKey.S: newX++; break; // Move down
                case ConsoleKey.A: newY--; break; // Move left
                case ConsoleKey.D: newY++; break; // Move right
                default: return false; // Invalid key
            }

            // Check if the new position is valid
            if (_maze[newX, newY] == 0 || _maze[newX, newY] == 2 || _maze[newX, newY] == 3 || _maze[newX, newY] == 4)
            {
                _playerPosition = (newX, newY); // Update player position
                return true; // Move was successful
            }

            return false; // Move failed (blocked by wall)
        }
    }
}
