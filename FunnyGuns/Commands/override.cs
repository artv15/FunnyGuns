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
                if (Plugin.isDevMode)
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
                                secondarg = arguments.Array[2].ToLower();
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
                                response = "Tried to override";
                                Plugin.MutatorOverride = int.Parse(secondarg);
                                var msg = "<color=green>Админ</color> выставил <color=blue>определённое значение</color> для следующего мутатора! <color=red>Следующий мутатор предопределён</color>!";
                                Exiled.API.Features.Map.Broadcast(10, msg);
                                return true;
                            }
                            break;
                        default:
                            response = "Syntax: \nfg_override damage\nfg_override players\n\n<color=yellow>Warning! Dev Mode active and not suitable for production!</color>";
                            return false;
                            break;
                    }
                }
                else
                {
                    response = "Overrides disabled, because plugin is not in dev mode!";
                    return false;
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