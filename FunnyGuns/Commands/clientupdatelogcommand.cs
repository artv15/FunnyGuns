using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;

namespace FunnyGuns.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class clientupdatelogcommand : ICommand
    {
        public string Command => "fg_updates";

        public string[] Aliases => null;

        public string Description => "Funny Guns update log (stupid)!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"\n<color=green>--Funny Guns > Updates</color>\n" +
                $"<color=yellow>{Plugin.updateName}</color> <color=green>update</color>!\n" +
                $"{Plugin.updateDesc}\n" +
                $"Nerd Stuff:\nBuild Identifier: {Plugin.BBI}";
            return true;
        }
    }
}
