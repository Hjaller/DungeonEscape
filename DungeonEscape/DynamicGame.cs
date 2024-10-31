using System;
using System.Collections.Generic;

namespace DungeonEscape
{
    internal class DynamicGame
    {
        private int _width = 21; // The width of the maze
        private int _height = 11; // The height of the maze
        private int[,] _maze; // 2D array representing the maze
        private (int, int) _playerStart = (1, 1), _playerPosition, _exitPosition, _keyPosition;
        private bool _hasKey = false; // Indicates whether the player has the key
        private Random _random = new Random(); // Random number generator
        private bool _showKeysAndTraps;

        public DynamicGame(bool showKeysAndTraps = false)
        {
            _showKeysAndTraps = showKeysAndTraps; // Set the preference based on input
        }

        /// <summary>
        /// Starts the game by generating the maze, placing the key, and handling user input.
        /// </summary>
        public void StartGame()
        {
            GenerateMaze(); // Create the maze structure
            _playerPosition = _playerStart; // Set the player's starting position
            _exitPosition = (_height - 2, _width - 2); // Define the exit position

            // Randomly place the key in the maze
            do
            {
                _keyPosition = (_random.Next(1, _height - 2), _random.Next(1, _width - 2));
            } while (_maze[_keyPosition.Item1, _keyPosition.Item2] != 0 ||
                     _keyPosition == _exitPosition ||
                     _keyPosition == _playerStart ||
                     !PathExists(_playerStart, _keyPosition));

            PlaceTraps(); // Add traps to the maze

            PrintMaze(); // Print the maze

            while (true)
            {
                string message = ""; // Initialize an empty message

                // Get user input
                ConsoleKey move = Console.ReadKey().Key;
                bool moved = MovePlayer(move); // Move the player based on input

                // Check for actions based on player's position
                if (_playerPosition == _keyPosition)
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
                else if (_playerPosition == _exitPosition && _hasKey)
                {
                    message = "Congratulations! You have escaped the maze!"; // Message when exiting
                    PrintMaze(message); // Print maze with exit message
                    break; // End the game
                }
                else if (!moved) // If movement failed
                {
                    message = "There's a wall there!";
                }

                // Print the maze and any message
                PrintMaze(message);
            }
        }

        /// <summary>
        /// Generates a maze using a randomized depth-first search algorithm.
        /// </summary>
        private void GenerateMaze()
        {
            _maze = new int[_height, _width]; // Initialize the maze array
            for (int x = 0; x < _height; x++)
                for (int y = 0; y < _width; y++)
                    _maze[x, y] = 1; // Set all cells to walls

            Stack<(int, int)> path = new Stack<(int, int)>(); // Stack to hold the path
            path.Push(_playerStart); // Push the starting position onto the stack
            _maze[1, 1] = 0; // Mark the starting position as a path

            while (path.Count > 0)
            {
                var (currentX, currentY) = path.Peek(); // Get the current position
                var directions = new List<(int, int)> { (2, 0), (-2, 0), (0, 2), (0, -2) }; // Possible directions
                bool moved = false;

                directions.Shuffle(); // Shuffle directions for randomness
                foreach (var (deltaX, deltaY) in directions)
                {
                    int newX = currentX + deltaX, newY = currentY + deltaY; // Calculate new position
                    // Check if the new position is valid and a wall
                    if (newX > 0 && newX < _height - 1 && newY > 0 && newY < _width - 1 && _maze[newX, newY] == 1)
                    {
                        _maze[newX, newY] = 0; // Mark the new position as a path
                        _maze[currentX + deltaX / 2, currentY + deltaY / 2] = 0; // Create a passage
                        path.Push((newX, newY)); // Push the new position onto the stack
                        moved = true;
                        break;
                    }
                }

                if (!moved) // If no movement was made, backtrack
                    path.Pop();
            }
        }

        /// <summary>
        /// Places traps randomly in the maze.
        /// </summary>
        private void PlaceTraps()
        {
            int trapCount = (_width * _height) / 10; // Calculate the number of traps

            for (int i = 0; i < trapCount; i++)
            {
                int x, y;
                do
                {
                    x = _random.Next(1, _height - 1);
                    y = _random.Next(1, _width - 1);
                } while (_maze[x, y] != 0 ||
                         (x == _playerStart.Item1 && y == _playerStart.Item2) ||
                         (x == _exitPosition.Item1 && y == _exitPosition.Item2) ||
                         (x == _keyPosition.Item1 && y == _keyPosition.Item2));

                _maze[x, y] = 2; // 2 represents a trap
                // Check if paths still exist after placing the trap
                if (!PathExists(_playerPosition, _keyPosition) || !PathExists(_keyPosition, _exitPosition))
                {
                    _maze[x, y] = 0; // Remove the trap if it blocks the path
                    i--; // Decrement counter to place another trap
                }
            }
        }

        /// <summary>
        /// Checks if a path exists between two points in the maze.
        /// </summary>
        /// <param name="start">The starting position.</param>
        /// <param name="end">The ending position.</param>
        /// <returns>True if a path exists, otherwise false.</returns>
        private bool PathExists((int, int) start, (int, int) end)
        {
            bool[,] visited = new bool[_height, _width]; // 2D array to track visited cells
            return DepthFirstSearch(start.Item1, start.Item2, end, visited); // Perform depth-first search
        }

        /// <summary>
        /// Performs a depth-first search to find a path to the end position.
        /// </summary>
        /// <param name="x">Current x-coordinate.</param>
        /// <param name="y">Current y-coordinate.</param>
        /// <param name="end">The end position.</param>
        /// <param name="visited">Array tracking visited positions.</param>
        /// <returns>True if a path to the end exists, otherwise false.</returns>
        private bool DepthFirstSearch(int x, int y, (int, int) end, bool[,] visited)
        {
            if ((x, y) == end) return true; // Found the end

            // Check if the position is out of bounds or a wall/trap or already visited
            if (x < 0 || x >= _height || y < 0 || y >= _width || _maze[x, y] == 1 || _maze[x, y] == 2 || visited[x, y])
                return false;

            visited[x, y] = true; // Mark the position as visited

            // Recursively check adjacent cells
            return DepthFirstSearch(x + 1, y, end, visited) ||
                   DepthFirstSearch(x - 1, y, end, visited) ||
                   DepthFirstSearch(x, y + 1, end, visited) ||
                   DepthFirstSearch(x, y - 1, end, visited);
        }

        /// <summary>
        /// Prints the maze to the console, displaying the player, exit, and key positions.
        /// </summary>
        /// <param name="message">Optional message to display.</param>
        /// <summary>
        /// Prints the maze to the console, displaying the player, exit, and optionally the key and traps.
        /// </summary>
        /// <param name="message">Optional message to display.</param>
        private void PrintMaze(string message = "")
        {
            Console.Clear(); // Clear the console
                             // Print the maze
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (i == _playerPosition.Item1 && j == _playerPosition.Item2)
                        Console.Write("P "); // Player
                    else if (i == _exitPosition.Item1 && j == _exitPosition.Item2)
                        Console.Write("E "); // Exit
                    else if (_showKeysAndTraps && i == _keyPosition.Item1 && j == _keyPosition.Item2 && !_hasKey)
                        Console.Write("K "); // Key
                    else if (_showKeysAndTraps && _maze[i, j] == 2)
                        Console.Write("X "); // Trap
                    else
                        Console.Write(_maze[i, j] == 1 ? "# " : ". ");
                }
                Console.WriteLine();
            }

            // Display a message if provided
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine(message);
            }
        }


        /// <summary>
        /// Moves the player based on the input key.
        /// </summary>
        /// <param name="key">The key input from the user.</param>
        /// <returns>True if the player moved, otherwise false.</returns>
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
            if (_maze[newX, newY] == 0 || _maze[newX, newY] == 2)
            {
                _playerPosition = (newX, newY); // Update player position
                return true; // Move was successful
            }

            return false; // Move failed (blocked by wall)
        }
    }

    /// <summary>
    /// Extension methods for list manipulations.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Shuffles the elements of a list in place.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = new Random().Next(n--);
                (list[n], list[k]) = (list[k], list[n]); // Swap elements
            }
        }
    }
}
