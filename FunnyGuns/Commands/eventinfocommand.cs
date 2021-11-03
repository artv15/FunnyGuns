using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;

namespace FunnyGuns.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class eventinfocommand : ICommand
    {
        public string Command => "fg_event_info";

        public string[] Aliases => null;

        public string Description => "Returns funny guns event info (if funny guns event is running)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Plugin.isRunning)
            {
                response = "\n<color=green>Funny Guns</color>\n<color=red>--Ваша задача--</color>\n<color=blue>Вы должны выжить и уничтожить команду соперника (Респавна не будет!)</color>\n<color=red>--Стадии--</color>\n" +
                    "<color=blue>Всего есть 4 стадии.</color>\n<color=green>1-я стадия - обычные 'Пострелушки'.</color>\n<color=yellow>2-я стадия - добавляется первый мутатор.</color>\n<color=red>3-я стадия - добавляется второй мутатор.</color>\n<color=red>4-я стадия - отключаются все мутаторы и активируется альфа боеголовка</color>\nА теперь беги в комплекс" +
                    " пока можешь!\n\nCredits:\n--Lead Developer: Treeshold#0001 (aka Star Buttefly)--\nBeta tester: Dlorka#9909 (aka Tushkanchik)\nОбновления: .fg_updates\n\n<color=green>Thanks guys, this was a fun ride!</color>";
                return true;
            }
            else
            {
                response = "\n<color=green>Funny Guns</color>\n<color=red>--Ваша задача--</color>\n<color=blue>Вы должны выжить и уничтожить команду соперника (Респавна не будет!)</color>\n<color=red>--Стадии--</color>\n" +
                    "<color=blue>Всего есть 4 стадии.</color>\n<color=green>1-я стадия - обычные 'Пострелушки'.</color>\n<color=yellow>2-я стадия - добавляется первый мутатор.</color>\n<color=red>3-я стадия - добавляется второй мутатор.</color>\n<color=red>4-я стадия - отключаются все мутаторы и активируется альфа боеголовка</color>\n" +
                    "Если хочешь этот ивент, поори админу" +
                    ", работает в 99,9% процентов случаев!\n\nCredits:\n--Lead Developer: Treeshold#0001 (aka Star Buttefly)--\nBeta tester: Dlorka#9909 (aka Tushkanchik)\nОбновления: .fg_updates\n\n<color=green>Thanks guys, this was a fun ride!</color>";
                return true;
            }
        }
    }
}
