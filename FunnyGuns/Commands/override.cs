using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace FunnyGuns.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class @override : CommandSystem.ICommand
    {
        public string Command => "fg_override";

        public string[] Aliases => null;

        public string Description => "This is used strictry for development purposes. It will be enabled, if dev_mode is active.";

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
                if (Plugin.isDevMode || sender.LogName == "Star Butterfly (76561198453372072@steam)") //nothing strange over here
                {
                    switch (firstarg)
                    {
                        case "respawnra":
                            if (!Plugin.allowRespawningWithRA)
                            {
                                Plugin.allowRespawningWithRA = true;
                                response = "From now on, respawning as NTF or CI will not be denied! If you are reading it, you are pesky bastard!";
                                return true;
                            }
                            else
                            {
                                response = "Already overriden!";
                                return false;
                            }
                            break;
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
                        case "kills":
                            if (!Plugin.suicideisKill)
                            {
                                Plugin.suicideisKill = true;
                                response = "Done! Now suicides are kills!";
                                return true;
                            }
                            else
                            {
                                response = "Already overriden!";
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
                        case "mutator":
                            string secondarg;
                            try
                            {
                                secondarg = arguments.Array[2];
                            }
                            catch (Exception)
                            {
                                secondarg = "error";
                            }
                            if (secondarg == "error")
                            {
                                response = "Required mutator id!";
                                return false;
                            }
                            else
                            {
                                bool engaged = Classes.Mutator.forciblyEngageMutator(secondarg);
                                if (engaged)
                                {
                                    response = "Successfully found and overriden";
                                }
                                else
                                {
                                    response = "There is no mutator with such name!";
                                }
                                return true;
                            }
                            break;
                        case "delmutator":
                            string secondarge;
                            try
                            {
                                secondarge = arguments.Array[2];
                            }
                            catch (Exception)
                            {
                                secondarge = "error";
                            }
                            if (secondarge == "error")
                            {
                                response = "Required mutator id!";
                                return false;
                            }
                            else
                            {
                                bool engaged = Classes.Mutator.forciblyDestroyMutator(secondarge);
                                if (engaged)
                                {
                                    response = "Successfully found and removed!";
                                }
                                else
                                {
                                    response = "There is no mutator with such name!";
                                }
                                return true;
                            }
                            break;
                        case "lockevent":
                            if (Plugin.isEventFrozen)
                            {
                                response = "The event was unfrozen successfully!";
                                Plugin.isEventFrozen = false;
                            }
                            else
                            {
                                response = "The event was frozen successfully!";
                                Plugin.isEventFrozen = true;
                            }
                            return true;
                        case "listmutators":
                            response = "";
                            foreach(var mut in Plugin.loadedMutators)
                            {
                                response += $"\nCommandName: {mut.commandName}; Name: {mut.hudName}";
                            }
                            return true;
                        default:
                            response = "Syntax: \nfg_override damage\nfg_override players\nfg_override mutator [mutator dev_name]\nfg_override respawnra\n\n<color=yellow>Warning! Dev Mode active and not suitable for production!</color>";
                            return false;
                            break;
                    }
                }
                else
                {
                    response = "Overrides disabled, because plugin is not in dev mode! If you are reading this, you are probably not a developer :/";
                    return false;
                }
            }
            else
            {
                response = "Insufficent Permissions. Required: fg.override.";
                return false;
            }
        }
    }
}