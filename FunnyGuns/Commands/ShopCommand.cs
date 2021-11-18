using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using CommandSystem;
using MEC;

namespace FunnyGuns.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class ShopCommand : ICommand
    {
        /*
         This is shop. You can add products by adding new cases to switch (firstarg)
         Don't forget to add items to display!
         */
        //Item Code
        /*
         case "'item'":
                    try
                    {
                        if (Plugin.shopDict[sender.LogName] >= 'money')
                        {
                            if (Plugin.isRunning)
                            {
                                ahp(sender.LogName);
                                response_give = $"PurchaseMessage! Ваш баланс: {Plugin.shopDict[sender.LogName]}";
                            }
                            else
                            {
                                response_give = $"Ивент не запущен!";
                            }
                        }
                        else
                        {
                            response_give = $"Недостаточно монет! У вас {Plugin.shopDict[sender.LogName]}!";
                        }
                        break;
                    }
                    catch
                    {
                        response_give = $"Недостаточно монет! У вас 0!";
                        break;
                    }
         */
        public string Command => "shop";

        public string[] Aliases => null;

        public string Description => "Allows you to buy some boosts during event";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string response_give = "<color=yellow>[WARNING]</color> <color=orange>Мы не получили ответ от сервера, но скорее всего ваша команда выполнена!</color>";
            string firstarg;
            try
            {
                firstarg = arguments.Array[1].ToLower();
            }
            catch
            {
                firstarg = "error";
            }
            bool foundit = false;
            try
            {
                foreach (var item in Plugin.shopInventory)
                {
                    Log.Debug($"Comparing: {item.commandname.ToLower()} and {firstarg.ToLower()}. Result: {item.commandname.ToLower() == firstarg.ToLower()}");
                    if (item.commandname.ToLower() == firstarg.ToLower())
                    {
                        foundit = true;
                        if (Plugin.shopDict[sender.LogName] >= item.price)
                        {
                            Plugin.shopDict[sender.LogName] -= item.price;
                            item.onExecuted.Invoke(Plugin.playerClientDict[sender.LogName]);
                            response_give = $"<color=green>Вы успешно купили {item.name} за {item.price} монет(ы). Ваш баланс: {Plugin.shopDict[sender.LogName]}</color>";
                        }
                        else
                        {
                            response_give = $"<color=red>Вам не хватает {item.price - Plugin.shopDict[sender.LogName]} монет!</color>";
                        }
                    }
                }
                if (!foundit)
                {
                    var msg = $"<color=green>---Funny Guns > Shop---</color>\n" +
                        $"<color=green>Ваш баланс: {Plugin.shopDict[sender.LogName]}</color>\n\n<color=yellow>---Каталог---</color>";
                    foreach (var listitem in Plugin.shopInventory)
                    {
                        msg += $"<color=yellow>{listitem.name} | {listitem.price} | .shop {listitem.commandname}</color>\n";
                    }
                    msg += $"\n<color=orange>Чтобы купить товар, введите команду `.shop [имя]`, например: `.shop ahp`</color>";
                    response_give = msg;
                }
            }
            catch (Exception ex) //if not registered by killing!
            {
                if (Plugin.isRunning)
                {
                    var msg = $"<color=green>---Funny Guns > Shop---</color>\n" +
                            $"<color=green>Ваш баланс: 0</color>\n\n<color=yellow>---Каталог---</color>\n";
                    foreach (var listitem in Plugin.shopInventory)
                    {
                        msg += $"<color=yellow>{listitem.name} | {listitem.price} | .shop {listitem.commandname}</color>\n";
                    }
                    msg += $"\n<color=orange>Чтобы купить товар, введите команду `.shop [имя]`, например: `.shop ahp`</color>";
                    response_give = msg;
                }
                else
                {
                    response_give = $"<color=red>Ивент не запущен! Попроси ивентолога начать Funny Guns в следующем раунде!</color>";
                }
            }
            
            response = response_give;
            return true;
        }

        static void ahp(string logName)
        {
            /*
             This method is "trusted", it will launch anyway and will try to substract the money.
             Before this method is executed, a check was already performed and player probably (supposedly) has enough money!
             */
            /*
             It's strongly unreccomended to use `Trusted` methods!
             */
            Player pl = Plugin.playerClientDict[logName];
            Plugin.shopDict[logName] -= 10;
            pl.ArtificialHealth = 60;
        }

        static void scp500(string logName)
        {
            Player pl = Plugin.playerClientDict[logName];
            if (pl.Items.Count < 8)
            {
                Plugin.shopDict[logName] -= 20;
                pl.AddItem(ItemType.SCP500);
            }
            else
            {
                Timing.RunCoroutine(tryAddOnInventory(pl));
            }
        }

        static IEnumerator<float> tryAddOnInventory(Player pl)
        {
            while (Round.IsStarted)
            {
                if (pl.Items.Count < 8)
                {
                    pl.AddItem(ItemType.SCP500);
                    yield break;
                }
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}
