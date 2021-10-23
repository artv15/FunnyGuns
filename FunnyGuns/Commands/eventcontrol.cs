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

        public string Description => "Controls event start/stop. Type help for... help..?";

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
                    case "help":
                        message = "fg_event start - Starts event, controlled by plugin itself\nfg_event stop - Stops event.\nfg_event help - This command.";
                        success = true;
                        break;
                    case "start":
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
                        break;
                    case "stage":
                        message = "Event is not running!";
                        success = false;
                        if (Plugin.isRunning)
                        {
                            if (arguments.Array.Length == 3)
                            {
                                Plugin.stage = int.Parse(arguments.Array[2]);
                                message = "Can't resolve number in 2nd argument!";
                                success = false;
                            }
                        }
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
                        message = "fg_event start - Starts event, controlled by plugin itself\nfg_event stop - Stops event.\nfg_event help - This command.";
                        success = true;
                        break;
                }
            }
            else
            {
                message = "Insufficent Permissions. Required: fg.event. Contact owner or local system administator if you beleive this is a mistake!";
                success = false;
            }
            response = message;
            return success;
        }
    }
}
