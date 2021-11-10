using System;
using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnyGuns
{
    class Classes
    {
        public class shopItem
        {
            public string name;
            public string commandname;
            public int price;
            public Action<Player> onExecuted;

            /// <summary>
            /// Creates a new shopItem object!
            /// </summary>
            /// <param name="name">Name of object (shown in list)</param>
            /// <param name="commandname">Command to purchase (.shop [arg])</param>
            /// <param name="price">Price of the product</param>
            /// <param name="action">Action if successful purchase. Use like (pl) => {//put here all things}</param>
            /// <returns>Returns shopItem object to be added to a shop list!</returns>
            public static shopItem initialize (string name, string commandname, int price, Action<Player> action)
            {
                shopItem shopItem = new shopItem();
                shopItem.name = name;
                shopItem.commandname = commandname;
                shopItem.price = price;
                shopItem.onExecuted = action;
                return shopItem;
            }
        }


        public class Mutator
        {
            public string hudName;
            public string commandName;
            public Action engage;
            public Action disengage;
            public Action onRespawn;

            /// <summary>
            /// Creates a new object of class mutator.
            /// </summary>
            /// <param name="plName">How plugin should be displayed to a player in active mutators?</param>
            /// <param name="commandName">How plugin will be called by plugim. It's for development, but is mandatory.</param>
            /// <param name="engage">This method will be called when mutator activates! (Required)</param>
            /// <param name="disengage">This method will be called when mutator deactivates! (Required)</param>
            /// <param name="onRespawn">This method will be called when player respawns! (Optional)</param>
            /// <returns>Returns Mutator object.</returns>
            public static Mutator initialize (string plName, string commandName, Action engage, Action disengage, Action onRespawn)
            {
                var mutator = new Mutator();
                mutator.hudName = plName;
                mutator.commandName = commandName;
                mutator.engage = engage;
                mutator.disengage = disengage;
                mutator.onRespawn = onRespawn;
                return mutator;
            }

            public static void disableAll()
            {

            }
        }
    }

    
}
