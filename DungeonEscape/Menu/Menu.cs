using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonEscape.Menu
{
    public class MenuOption
    {
        public string OptionText { get; }
        public Action? Action { get; }
        public MenuOption[]? SubMenu { get; }

        public MenuOption(string optionText, Action? action, MenuOption[]? subMenu = null)
        {
            OptionText = optionText;
            Action = action;
            SubMenu = subMenu;


        }
    }
}