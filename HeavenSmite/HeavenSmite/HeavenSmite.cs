using System.Linq;

using Aimtec;
using Spell = Aimtec.SDK.Spell;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;

namespace HeavenSmiteReborn
{
    internal class HeavenSmite
    {
        public static Menu Menu = new Menu("HeavenSmite", "HeavenSmite", true);
        private static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();
        private static int SmiteDamages
        {
            get
            {
                int[] Dmg = new int[] { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 };

                return Dmg[Player.Level - 1];
            }
        }

        private static Spell Smite;
        private static string[] pMobs = new string[] { "SRU_Baron", "SRU_Blue", "SRU_Red", "SRU_RiftHerald" };

        public HeavenSmite()
        {
            Smite = new Spell(Player.SpellBook.Spells.FirstOrDefault(spell => spell.Name.Contains("Smite")).Slot, 500);

            Menu.Add(new MenuKeyBind("Key", "Auto Smite:", Aimtec.SDK.Util.KeyCode.H, KeybindType.Toggle));

            var Dragons = new Menu("Dragons", "Dragons");
            {
                Dragons.Add(new MenuBool("SRU_Dragon_Air", "Air Dragon?"));
                Dragons.Add(new MenuBool("SRU_Dragon_Fire", "Fire Dragon?"));
                Dragons.Add(new MenuBool("SRU_Dragon_Earth", "Earth Dragon?"));
                Dragons.Add(new MenuBool("SRU_Dragon_Water", "Water Dragon?"));
                Dragons.Add(new MenuBool("SRU_Dragon_Elder", "Elder Dragon?"));
            };
            Menu.Add(Dragons);
            var Big = new Menu("BigMobs", "Big Mobs");
            {
                Big.Add(new MenuBool("SRU_Baron", "Baron?"));
                Big.Add(new MenuBool("SRU_Blue", "Blue?"));
                Big.Add(new MenuBool("SRU_Red", "Red?"));
                Big.Add(new MenuBool("SRU_RiftHerald", "Rift Herald?"));
            }
            Menu.Add(Big);
            var Small = new Menu("SmallMobs", "Small Mobs");
            {
                Small.Add(new MenuBool("SRU_Gromp", "Gromp?"));
                Small.Add(new MenuBool("SRU_Murkwolf", "Wolves?"));
                Small.Add(new MenuBool("SRU_Krug", "Krug?"));
                Small.Add(new MenuBool("SRU_Razorbeak", "Razor?"));
                Small.Add(new MenuBool("Sru_Crab", "Crab?"));
            }
            Menu.Add(Small);
            Menu.Attach();

            Game.OnUpdate += delegate
            {
                if (Player.IsDead && !Smite.Ready)
                    return;

                if (!Menu["Key"].Enabled)
                    return;

                foreach (var Obj in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(Smite.Range) && SmiteDamages >= x.Health))
                {
                    if (Obj.UnitSkinName.StartsWith("SRU_Dragon"))
                    {
                        if (Menu["Dragons"][Obj.UnitSkinName].Enabled)
                            Smite.Cast(Obj);
                    }

                    if (pMobs.Contains(Obj.UnitSkinName))
                    {
                        if (Menu["BigMobs"][Obj.UnitSkinName].Enabled)
                            Smite.Cast(Obj);
                    }

                    if (!pMobs.Contains(Obj.UnitSkinName) && !Obj.UnitSkinName.StartsWith("SRU_Dragon"))
                    {
                        if (Menu["SmallMobs"][Obj.UnitSkinName].Enabled)
                            Smite.Cast(Obj);
                    }
                }
            };
        }
    }
}