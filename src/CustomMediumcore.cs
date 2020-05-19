using System;
using System.Collections.Generic;
using System.IO;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;

namespace CustomMediumcore
{
    [ApiVersion(2, 1)]
    public class CustomMediumcore : TerrariaPlugin
    {
        /// <summary>
        /// Gets the author(s) of this plugin
        /// </summary>
        public override string Author => "Miffyli";

        /// <summary>
        /// Gets the description of this plugin.
        /// A short, one lined description that tells people what your plugin does.
        /// </summary>
        public override string Description => "A plugin for customizing what items are dropped on death (drops resources, nothing else)";

        /// <summary>
        /// Gets the name of this plugin.
        /// </summary>
        public override string Name => "CustomMediumcore";

        /// <summary>
        /// Gets the version of this plugin.
        /// </summary>
        public override Version Version => new Version(1, 4, 0, 0);

        /// <summary>
        /// Name of the file which contains Item IDs that should be dropped on death, one per line
        /// </summary>
        public const string ItemIDFile = "drop_item_ids.txt";

        // ID's of the resources/droppable items
        public int[] DropIDs = new int[] { 2,3,9,11,12,13,14,19,20,21,22,23,
                                          26,27,29,30,31,38,56,57,59,60,61,68,
                                          69,70,71,72,73,74,75,85,86,93,94,105,
                                          116,117,118,126,129,130,131,132,133,134,
                                          135,137,138,139,140,141,142,143,144,145,146,147,
                                          149,150,154,169,170,171,172,173,174,175,176,177,178,179,
                                          180,181,182,183,184,192,195,209,210,214,225,236,254,255,259,260,
                                          275,276,307,308,310,309,311,312,313,314,315,316,317,318,319,320,
                                          321,330,362,364,365,366,369,370,391,392,408,409,412,413,414,415,416,
                                          417,418,419,420,421,424,480,501,502,520,521,522,526,257,528,540,547,548,549,
                                          575,577,586,587,593,594,595,604,605,606,607,608,609,610,619,620,621,622,
                                          623,624,625,631,632,633,634,635,699,700,701,702,703,704,705,706,717,718,719,
                                          720,721,722,751,752,762,763,766,883,911,947,965,967,1006,1104,1105,1106,1124,
                                          1125,1126,1127,1175,1176,1177,1184,1191,1198,1225,1291,1329,1330,1332,1508,2503,2504,
                                          880, 331 };

        public Item[] DropItems;

        /// <summary>
        /// Initializes a new instance of the CustomMediumcore class.
        /// This is where you set the plugin's order and perfrom other constructor logic
        /// </summary>
        public CustomMediumcore(Main game) : base(game)
        {
            // Read item IDs that should be dropped, and turn them into items
            string[] idStrings = System.IO.File.ReadAllLines(ItemIDFile);
            var ItemList = new List<Item>();
            foreach (string idString in idStrings)
            {
                int itemId = int.Parse(idString);
                ItemList.Add(TShock.Utils.GetItemById(itemId));
            }
            DropItems = ItemList.ToArray();
            Console.WriteLine("[CustomMediumcore] Loaded {0} items for dropping on death", idStrings.Length);
        }

        /// <summary>
        /// Handles plugin initialization. 
        /// Fired when the server is started and the plugin is being loaded.
        /// You may register hooks, perform loading procedures etc here.
        /// </summary>
        public override void Initialize()
        {
            ServerApi.Hooks.NetGetData.Register(this, onGetData);
        }

       
        /// <summary>
        /// Handles plugin disposal logic.
        /// *Supposed* to fire when the server shuts down.
        /// You should deregister hooks and free all resources here.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NetGetData.Deregister(this, onGetData);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Checks incoming packets and manually hooks
        /// to packets of interest
        /// </summary>
        public void onGetData(GetDataEventArgs args)
        {
            if (args.MsgID == PacketTypes.PlayerDeathV2)
            {
                // No need for fancy readers here, just peek next char
                // with existing readers like the filthy hijacker we are
                int PlayerID = args.Msg.reader.PeekChar();
                OnPlayerDeath(PlayerID);
            }
        }

        void OnPlayerDeath(int PlayerID)
        {
            TSPlayer player = TShock.Players[PlayerID];
            Item emptyItem = new Item();
            // Go over the inventory
            for (int i = 0; i < 58; i++)
            {
                foreach (Item dropitem in DropItems)
                {
                    if (player.TPlayer.inventory[i].IsTheSameAs(dropitem))
                    {
                        // Spawn item
                        int itemIndex = Item.NewItem((int)player.X, (int)player.Y, player.TPlayer.width, player.TPlayer.height, dropitem.netID, player.TPlayer.inventory[i].stack, true, 0, true);
                        NetMessage.SendData((int)PacketTypes.ItemDrop, player.Index, -1, Terraria.Localization.NetworkText.FromFormattable(""), itemIndex);
                        // Empty slot
                        player.TPlayer.inventory[i] = emptyItem;
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1,           -1, Terraria.Localization.NetworkText.FromLiteral(player.TPlayer.inventory[i].Name), player.Index, i, player.TPlayer.inventory[i].prefix);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, Terraria.Localization.NetworkText.FromLiteral(player.TPlayer.inventory[i].Name), player.Index, i, player.TPlayer.inventory[i].prefix); 
                    }
                }
            }
        }
    }
}