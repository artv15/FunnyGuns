using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnyGuns.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class @override : CommandSystem.ICommand
    {
        public string Command => "fg_override";

        public string[] Aliases => null;

        public string Description => "Overrides damages from guns to all damage types or player amount control. Lasts until round is restarted.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string firstarg;
            try
            {
                firstarg = arguments.Array[1].ToLower();
            }
            catch (Exception ex)
            {
                firstarg = "error";
            }
            switch (firstarg)
            {
                case "damage":
                    try
                    {
                        if (!Plugin.isOverriden)
                        {
                            Plugin.isOverriden = true;
                            response = "Successfully executed command! All damage types are now going to be punished!";
                            return true;
                        }
                        else
                        {
                            response = "Already overriden!";
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        response = $"Failed to override damage types, error: {ex.Message}";
                        return false;
                    }
                    break;
                case "players":
                    if (!Plugin.isPlayerOverriden)
                    {
                        Plugin.isPlayerOverriden = true;
                        response = "Now plugin will try to avoid stopping event depending on player count!";
                        return true;
                    }
                    else
                    {
                        response = "Already overriden!";
                        return false;
                    }
                    break;
                default:
                    response = "Syntax: \nfg_override damage\nfg_override players";
                    return false;
                    break;
            }
            
        }
    }
}
