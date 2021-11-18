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
            public Action<Player> onRespawn;
            public List<string> conflicts;

            /// <summary>
            /// Creates a new object of class mutator.
            /// </summary>
            /// <param name="plName">How plugin should be displayed to a player in active mutators?</param>
            /// <param name="commandName">How plugin will be called by plugim. It's for development, but is mandatory.</param>
            /// <param name="engage">This method will be called when mutator activates! (Required)</param>
            /// <param name="disengage">This method will be called when mutator deactivates! (Required)</param>
            /// <param name="onRespawn">This method will be called when player respawns! (Optional)</param>
            /// <returns>Returns Mutator object.</returns>
            public static Mutator initialize(string plName, string commandName, Action engage, Action disengage, Action<Player> onRespawn)
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
                foreach (var mut in Plugin.engagedMutators)
                {
                    mut.disengage.Invoke();
                }
                Plugin.engagedMutators.Clear();
            }

            /// <summary>
            /// Starts a mutator by a devName. If started successfully, returns true. In other case, returns false.
            /// </summary>
            /// <param name="devName">DevName of a mutator</param>
            /// <returns>True = success, false = not found</returns>
            public static bool forciblyEngageMutator(string devName)
            {
                foreach (var mut in Plugin.loadedMutators)
                {
                    if (mut.commandName == devName)
                    {
                        mut.engage.Invoke();
                        Plugin.engagedMutators.Add(mut);
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Stops a mutator by a devName. If started successfully, returns true. In other case, returns false.
            /// </summary>
            /// <param name="devName">DevName of a mutator</param>
            /// <returns>True = success, false = not found</returns>
            public static bool forciblyDestroyMutator(string devName)
            {
                foreach (var mut in Plugin.loadedMutators)
                {
                    if (mut.commandName == devName)
                    {
                        mut.disengage.Invoke();
                        Plugin.engagedMutators.Remove(mut);
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Checks if a mutator is running.
            /// </summary>
            /// <param name="devName">DevName of a mutator</param>
            /// <returns>True = yes; False = no</returns>
            public static bool mutatorExists(string devName)
            {
                foreach (var mut in Plugin.loadedMutators)
                {
                    if (mut.commandName == devName)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    
}
