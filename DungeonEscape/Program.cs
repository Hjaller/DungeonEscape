using DungeonEscape.Menu;

namespace DungeonEscape
{
    internal class Program
    {
        static int width = 21; // Labyrintens bredde
        static int height = 11; // Labyrintens højde
        static int[,] maze;
        static (int, int) playerStart = (1, 1), playerPosition, exitPosition, keyPosition;
        static bool hasKey = false;
        static Random random = new Random();

        static void Main()
        {
            ShowMenu(MainMenuOptions);
        }

        static void GameInstructions()
        {
            Console.WriteLine("Velkommen til Dungeon Escape!");
            Console.WriteLine("Din opgave er at finde nølgen og undgå fælderne for at nå udgangen");
            Console.WriteLine("Tryk på en vilkårlig tast for at begynde");
            Console.ReadKey();
        }


        static private MenuOption[] MainMenuOptions =
{
                new MenuOption("Predefined - Dungeon Escape", null, new MenuOption[]
                {
                    new MenuOption("Show traps and keys", () => new PredefinedGame(true).StartGame()),
                    new MenuOption("Don't show traps and keys", () => new PredefinedGame(false).StartGame()),
                }),
                new MenuOption("Dynamic - Dungeon Escape", null, new MenuOption[]
                {
                    new MenuOption("Show traps and keys", () =>                     {
                        GameInstructions();
                        new DynamicGame(true).StartGame();
                    }),
                    new MenuOption("Don't show traps and keys", () =>
                    {
                        GameInstructions();
                        new DynamicGame(false).StartGame();
                    }),
                }),
                new MenuOption("Exit", ExitProgram)
            };

        static private void ShowMenu(MenuOption[] menuOptions, MenuOption[]? parentMenu = null)
        {
            int selectedIndex = 0;

            Console.Clear();
            Console.WriteLine("Use the arrow keys to navigate, and Enter to select an option:\n");

            // Initial rendering of the menu
            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {menuOptions[i].OptionText}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {menuOptions[i].OptionText}");
                }
            }

            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                int previousIndex = selectedIndex;

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex--;
                    if (selectedIndex < 0) selectedIndex = menuOptions.Length - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex++;
                    if (selectedIndex >= menuOptions.Length) selectedIndex = 0;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var selectedOption = menuOptions[selectedIndex];

                    if (selectedOption != null)
                    {
                        if (selectedOption.SubMenu != null)
                        {
                            ShowMenu(selectedOption.SubMenu, menuOptions);
                            Console.Clear();
                            Console.WriteLine("Use the arrow keys to navigate, and Enter to select an option:\n");
                            for (int i = 0; i < menuOptions.Length; i++)
                            {
                                if (i == selectedIndex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"> {menuOptions[i].OptionText}");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.WriteLine($"  {menuOptions[i].OptionText}");
                                }
                            }
                        }
                        else if (selectedOption.Action != null)
                        {
                            Console.Clear();
                            selectedOption.Action.Invoke();
                            WaitForEnter(selectedOption.Action);
                            Console.Clear();
                            Console.WriteLine("Use the arrow keys to navigate, and Enter to select an option:\n");
                            for (int i = 0; i < menuOptions.Length; i++)
                            {
                                if (i == selectedIndex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"> {menuOptions[i].OptionText}");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.WriteLine($"  {menuOptions[i].OptionText}");
                                }
                            }
                        }
                        else if (selectedOption.OptionText.StartsWith("0"))
                        {
                            return; // Return to the previous menu
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    if (parentMenu != null)
                    {
                        ShowMenu(parentMenu);
                    }
                    else
                    {
                        ExitProgram();
                    }
                    return;
                }

                // Update only the changed lines
                if (previousIndex != selectedIndex)
                {
                    Console.SetCursorPosition(0, previousIndex + 2); // +2 to account for the initial instructions
                    Console.Write($"  {menuOptions[previousIndex].OptionText}  ");

                    Console.SetCursorPosition(0, selectedIndex + 2);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"> {menuOptions[selectedIndex].OptionText}");
                    Console.ResetColor();
                }
            }
        }


        static private void WaitForEnter(Action action)
        {
            Console.WriteLine("\nPress Enter to run the task again or Esc to return to the main menu.");
            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    action.Invoke();
                    Console.WriteLine("\nPress Enter to run the task again or Esc to return to the main menu.");
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return; // Return to the main menu
                }
            }
        }

        static private void ExitProgram()
        {
            Console.WriteLine("The program is exiting. Goodbye!");
            Environment.Exit(0);
        }
    }

}
