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
            response = $"\n<color=green>New begginings update</color>\n<color=green>В этом обновлении в основном был оптимизирован код и добавились новые механики</color>\n\n<color=green>1. Починен модуль анти-абуза, раньше он выдавал false-positive'ы.</color>\n" +
                $"<color=green>2. Добавлено 2 новых мутатора: Move or Die и отключение тесла ворот</color>\n\n" +
                $"<color=blue>Теперь обновления для серверов</color>\n" +
                $"<color=green>1. Так как теперь мутатор - это класс, мутаторы подгружаются во время ожидания игроков. Если у вас ошибки при случайном выборе мутатора, проверьте, загружены ли мутаторы!</color>\n" +
                $"<color=green>2. Предметы в магазине - тоже класс. Они также пишутся в консоли сервера при инициальзации.</color>\n\n" +
                $"<color=red>GLHF!</color>\n\n" +
                $"Build Identifier: {Plugin.BBI}";
            return true;
        }
    }
}
