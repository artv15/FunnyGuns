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
            response = $"\n<color=green>`Everyone Equal` update</color>\n\n<color=yellow>--Общие изменения--</color>\n<color=yellow>1. Админы не могут спавниться после начала ивента (начало - 1-я стадия) [BUGS MAY OCCUR, UNSTABLE]</color>\n<color=green>2. Теперь не будет объявляться о смерти ново-зашедших игроков (bugfix)</color>\n<color=green>3. Переработан стартовый инветарь у обоих комманд, так как у хаоситов явное преймущество!</color>\n<color=green>4. (server-side) Починен StackOverFlow при мутаторе `Падение с любой высоты`</color>\n<color=green>5. Добавлен магазин (.shop)</color>\n\n<color=red>GLHF!</color>\n\nNerd Stuff:\nBeta Build Identifier: {Plugin.BBI}";
            return true;
        }
    }
}
