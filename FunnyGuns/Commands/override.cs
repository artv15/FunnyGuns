using CommandSystem;
using System;
using Exiled.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Permissions.Extensions;

namespace FunnyGuns.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class @override : CommandSystem.ICommand
    {
        public string Command => "fg_override";

        public string[] Aliases => null;

        public string Description => "This command was intended for development, but you can override player checks or damage checks here.";

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
            if (sender.CheckPermission("fg.override"))
            {
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
                        response = "Syntax: \nfg_override damage\nfg_override players\n\n<color=yellow>Warning! This command may break everything! I have warned you!</color>";
                        return false;
                        break;
                }
            }
            else
            {
                response = "Insufficent Permissions. Required: fg.override. Contact owner or local system administator if you beleive this is a mistake!\n\nIf you are an owner/local system administator, edit permissions.yml in Exiled config folder to grant permission to a certain group, " +
                    "then type reload all in RA to apply permission changes!";
                return false;
            }
            
        }
    }
}
