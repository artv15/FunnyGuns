using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace FunnyGuns.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class eventcontrol : ICommand
    {
        public string Command => "fg_event";

        public string[] Aliases => null;

        public string Description => "Starts or stops event, type without any argument to see possible subcommands!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string message;
            bool success;
            string firstarg;
            try
            {
                firstarg = arguments.Array[1];
            }
            catch (Exception ex)
            {
                firstarg = "error";
            }
            if (sender.CheckPermission("fg.event"))
            {
                switch (firstarg)
                {
                    case "start":
                        if (Round.IsStarted)
                        {
                            if (Plugin.isRunning)
                            {
                                message = "Event is already running, stop the event or wait until it ends!";
                                success = false;
                            }
                            else
                            {
                                if (!Exiled.API.Features.Warhead.SitePanel.blastDoor.isClosed)
                                {
                                    message = "Success! Started event!";
                                    success = true;
                                    EventHandler.StartEvent();
                                }
                                else
                                {
                                    message = "Unable to start event if blast doors are closed! Restart round and try again!";
                                    success = false;
                                }
                            }
                        }
                        else
                        {
                            message = "Please, start the round before starting event!";
                            success = false;
                        }
                        break;
                    case "stage":
                        message = "Setting stage is no longer supported, due to stage change breaking game controller coroutine!";
                        success = false;
                        break;
                    case "stop":
                        if (!Plugin.isRunning)
                        {
                            message = "Event is not running!";
                            success = false;
                        }
                        else
                        {
                            message = "Success! Stopped event!";
                            success = true;
                            EventHandler.StopEvent();
                        }
                        break;
                    default:
                        message = "Unknown subcommand! Check syntax and try again!\n\nSubcommands:\nfg_event start - Starts event, controlled by plugin itself\nfg_event stop - Stops event.\n";
                        success = false;
                        break;
                }
            }
            else
            {
                message = "Insufficent Permissions. Required: fg.event. Contact owner or local system administator if you beleive this is a mistake!\n\nIf you are an owner/local system administator, edit permissions.yml in Exiled config folder to grant permission to a certain group, " +
                    "then type reload all in RA to apply permission changes!";
                success = false;
            }
            response = message;
            return success;
        }
    }
}
