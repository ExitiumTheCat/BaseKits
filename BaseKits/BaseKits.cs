using System;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Newtonsoft.Json.Linq;
using System.IO;

namespace BaseKits
{
    [ApiVersion(2, 1)]
    public class BaseKits : TerrariaPlugin
    {

        public override string Name => "BaseKits";
        public override Version Version => new Version(1, 1);
        public override string Author => "ExitiumTheCat";
        public override string Description => "Customizable kits commands";

        public bool GameOngoing;
        public bool AllowedToGetKits;
        public BaseKits(Main game) : base(game) { }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("tshock.tp.others", StartAndEnd, "startend"));
            Commands.ChatCommands.Add(new Command("tshock.tp.others", AllowToGetKits, "allowgetkits"));
            Commands.ChatCommands.Add(new Command(Kit, "kit"));
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        private void AllowToGetKits(CommandArgs args)
        {
            if (AllowedToGetKits)
            {
                AllowedToGetKits = false;
                args.Player.SendSuccessMessage("Players are not allowed anymore to get kits.");
            }
            else
            {
                AllowedToGetKits = true;
                args.Player.SendSuccessMessage("Players are now allowed to get kits.");
            }
        }

        private void Kit(CommandArgs args)
        {
            if (AllowedToGetKits)
            {
                if (args.Parameters.Count > 0)
                {
                    foreach (string input in args.Parameters)
                    {
                        var JsonFile = File.ReadAllText("tshock/kits.json");
                        JObject ParsedJson = JObject.Parse(JsonFile);
                        if (ParsedJson[input] != null)
                        {
                            JToken TKitItems = ParsedJson[input]["items"];
                            string[] KitItems = TKitItems.ToString().Split(' ');
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].active && Main.player[i].name == args.TPlayer.name)
                                {
                                    Player plr = Main.player[i];
                                    for (int i2 = 0; i2 < 58; i2++)
                                    {
                                        plr.inventory[i2].netDefaults(0);
                                    }
                                    for (int ii2 = 0; ii2 < 58; ii2++)
                                    {
                                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[i].Index, (float)ii2);
                                    }
                                    for (int i3 = 0; i3 < 19; i3++)
                                    {
                                        plr.armor[i3].netDefaults(0);
                                    }
                                    for (int ii3 = 0; ii3 < 19 + 58; ii3++)
                                    {
                                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[i].Index, (float)ii3);
                                    }
                                    plr.trashItem.netDefaults(0);
                                    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[i].Index, 179);
                                    for (int i4 = 0; i4 < KitItems.Length; i4++)
                                    {
                                        Commands.HandleCommand(TSPlayer.Server, "/give " + KitItems[i4] + " tsn:" + plr.name);
                                    }
                                    if (ParsedJson[input]["life"] != null)
                                    {
                                        JToken TKitLife = ParsedJson[input]["life"];
                                        plr.statLifeMax = int.Parse(TKitLife.ToString());
                                        NetMessage.SendData((int)PacketTypes.PlayerHp, -1, -1, NetworkText.Empty, TShock.Players[i].Index);
                                    }
                                    else
                                    {
                                        plr.statLifeMax = 100;
                                        NetMessage.SendData((int)PacketTypes.PlayerHp, -1, -1, NetworkText.Empty, TShock.Players[i].Index);
                                    }
                                    if (ParsedJson[input]["mana"] != null)
                                    {
                                        JToken TKitMana = ParsedJson[input]["mana"];
                                        plr.statManaMax = int.Parse(TKitMana.ToString());
                                        NetMessage.SendData((int)PacketTypes.PlayerMana, -1, -1, NetworkText.Empty, TShock.Players[i].Index);
                                    }
                                    else
                                    {
                                        plr.statManaMax = 20;
                                        NetMessage.SendData((int)PacketTypes.PlayerMana, -1, -1, NetworkText.Empty, TShock.Players[i].Index);
                                    }
                                }
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage("That kit does not exist.");
                        }
                    }
                }
                else
                {
                    args.Player.SendErrorMessage("Please input a number.");
                }
            }
            else
            {
                args.Player.SendErrorMessage("You are not allowed to get a kit currently.");
            }
        }

        private void StartAndEnd(CommandArgs args)
        {
            if (GameOngoing)
            {
                GameOngoing = false;
                TSPlayer.All.SendMessage("The game has ended!", Color.Cyan);
            }
            else
            {
                GameOngoing = true;
                TSPlayer.All.SendMessage("The game has started!", Color.Cyan);
            }
        }
        private void OnUpdate(EventArgs args)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                {
                    Player plr = Main.player[i];
                    if (GameOngoing)
                    {
                        plr.hostile = true;
                        NetMessage.SendData((int)PacketTypes.TogglePvp, -1, -1, NetworkText.Empty, TShock.Players[i].Index);
                    }
                }
            }
        }
    }
}