using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace CustomItems {
    [ApiVersion(2, 1)]
    public class CustomItems : TerrariaPlugin {
        public override string Name => "CustomItems";
        public override string Author => "Interverse, updated by Comdar + RenderBr + Maxthegreat99";
        public override string Description => "Allows you to spawn custom items";
        public override Version Version => new Version(1, 3, 2);

        public CustomItems(Main game) : base(game) {
        }

        public override void Initialize() {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args) {
            Commands.ChatCommands.Add(new Command("customitem", CustomItem, "customitem", "citem") {
                HelpText = "/customitem <id/itemname> <parameters> <#> ... \nParameters: hexcolor (hc), damage (d), knockback (kb), useanimation (ua), " +
                "usetime (ut), shoot (s), shootspeed (ss), width (w), height (h), scale (sc), ammo (a), useammo (uam), notammo (na), stack (st)."
            });

            Commands.ChatCommands.Add(new Command("customitem.give", GiveCustomItem, "givecustomitem", "gcitem") {
                HelpText = "/givecustomitem <name> <id/itemname> <parameters> <#> ... \nParameters: hexcolor (hc), damage (d), knockback (kb), useanimation (ua), " +
                "usetime (ut), shoot (s), shootspeed (ss), width (w), height (h), scale (sc), ammo (a), useammo (uam), notammo (na), stack (st)."
            });
        }

        private void CustomItem(CommandArgs args) {
            List<string> parameters = args.Parameters;
            int num = parameters.Count();

            if (num == 0) {
                args.Player.SendErrorMessage("Invalid Syntax. /customitem <id/itemname> <parameters> <#> ... \nParameters: hexcolor (hc), damage (d), knockback (kb), useanimation (ua), " +
                "usetime (ut), shoot (s), shootspeed (ss), width (w), height (h), scale (sc), ammo (a), useammo (uam), notammo (na), stack (st).");
                return;
            }

            List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[0]);
            if (items.Count == 0) {
                args.Player.SendErrorMessage($"No item found by the name of {args.Parameters[0]}.");
                return;
            }
            Item item = items[0];

            TSPlayer player = new TSPlayer(args.Player.Index);

            Item targetItem = ModifyAndGiveCustomItem(parameters, player, item, 1);

            args.Player.SendSuccessMessage("You were sucessfully given {0} custom {1}!", targetItem.stack, targetItem.HoverName);
        }

        private void GiveCustomItem(CommandArgs args) {
            List<string> parameters = args.Parameters;
            int num = parameters.Count();

            if (num == 0) {
                args.Player.SendErrorMessage("Invalid Syntax. /givecustomitem <name> <id/itemname> <parameters> <#> ... \nParameters: hexcolor (hc), damage (d), knockback (kb), useanimation (ua), " +
                "usetime (ut), shoot (s), shootspeed (ss), width (w), height (h), scale (sc), ammo (a), useammo (uam), notammo (na), stack (st).");
                return;
            }

            List<TSPlayer> players = TSPlayer.FindByNameOrID(args.Parameters[0]); 
            if (players.Count != 1) {
                args.Player.SendErrorMessage("Failed to find player of: " + args.Parameters[0]);
                return;
            }

            if (num == 1) {
                args.Player.SendErrorMessage("Failed to provide arguments to item.");
                return;
            }

            List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
            if (items.Count == 0) {
                args.Player.SendErrorMessage($"No item found by the name of {args.Parameters[1]}.");
                return;
            }
            Item item = items[0];

            TSPlayer player = new TSPlayer(players[0].Index);

            Item targetItem = ModifyAndGiveCustomItem(parameters, player, item, 2);

            player.SendSuccessMessage("{0} gave you {1} custom {2}!",args.Player.Name , targetItem.stack, targetItem.HoverName );
            args.Player.SendSuccessMessage("Sucessfully gave {0} {1} custom {2}!", player.Name, targetItem.stack, targetItem.HoverName);
        }
        private Item ModifyAndGiveCustomItem(List<string> parameters, TSPlayer target, Item item , int skip )
        {
            Item _targetItem = TShock.Utils.GetItemById(item.type);
            var pairedInputs = SplitIntoPairs<string>(parameters.Skip(skip).ToArray());

            foreach (var pair in pairedInputs)
            {
                string param = pair[0];
                string arg = pair[1];
                switch (param)
                {
                    case "hexcolor":
                    case "hc":
                        _targetItem.color = new Color(int.Parse(arg.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                            int.Parse(arg.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                            int.Parse(arg.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                        break;
                    case "damage":
                    case "d":
                        _targetItem.damage = int.Parse(arg);
                        break;
                    case "knockback":
                    case "kb":
                        _targetItem.knockBack = int.Parse(arg);
                        break;
                    case "useanimation":
                    case "ua":
                        _targetItem.useAnimation = int.Parse(arg);
                        break;
                    case "usetime":
                    case "ut":
                        _targetItem.useTime = int.Parse(arg);
                        break;
                    case "shoot":
                    case "s":
                        _targetItem.shoot = int.Parse(arg);
                        break;
                    case "shootspeed":
                    case "ss":
                        _targetItem.shootSpeed = int.Parse(arg);
                        break;
                    case "width":
                    case "w":
                        _targetItem.width = int.Parse(arg);
                        break;
                    case "height":
                    case "h":
                        _targetItem.height = int.Parse(arg);
                        break;
                    case "scale":
                    case "sc":
                        _targetItem.scale = int.Parse(arg);
                        break;
                    case "ammo":
                    case "a":
                        _targetItem.ammo = int.Parse(arg);
                        break;
                    case "useammo":
                    case "uam":
                        _targetItem.useAmmo = int.Parse(arg);
                        break;
                    case "notammo":
                    case "na":
                        _targetItem.notAmmo = Boolean.Parse(arg);
                        break;
                    case "stack":
                    case "st":
                        _targetItem.stack = int.Parse(arg);
                        break;
                }
            }
            int itemIndex = Item.NewItem(Projectile.GetNoneSource(), (int)target.X, (int)target.Y, _targetItem.width, _targetItem.height, item.type, _targetItem.stack, pfix: 0, noGrabDelay: true);
            Item targetItem = Main.item[itemIndex];
            targetItem.playerIndexTheItemIsReservedFor = target.Index;

            targetItem.damage = _targetItem.damage;
            targetItem.knockBack = _targetItem.knockBack;
            targetItem.useAnimation = _targetItem.useAnimation;
            targetItem.useTime = _targetItem.useTime;
            targetItem.shoot = _targetItem.shoot;
            targetItem.shootSpeed = _targetItem.shootSpeed;
            targetItem.scale = _targetItem.scale;
            targetItem.ammo = _targetItem.ammo;
            targetItem.useAmmo = _targetItem.useAmmo;
            targetItem.notAmmo = _targetItem.notAmmo;

            TSPlayer.All.SendData(PacketTypes.UpdateItemDrop, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.ItemOwner, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.TweakItem, null, itemIndex, 255, 63);

            return targetItem;
        }
        public static T[][] SplitIntoPairs<T>(T[] input) {
            T[][] split = new T[input.Length / 2][];

            for (int x = 0; x < input.Length / 2; x++) {
                split[x] = new[] { input[x * 2], input[x * 2 + 1] };
            }

            return split;
        }
    }
}
