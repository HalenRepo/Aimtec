
using System.Linq;

using Aimtec;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;

namespace HeavenSmiteReborn
{
    internal class HeavenSmite
    {
        public static Menu Menu = new Menu("HeavenSmite", "HeavenSmite", true);
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public HeavenSmite()
        {
            var SmiteMenu = new Menu("smite", "Auto Smite");
            {
                SmiteMenu.Add(new MenuBool("autosmite", "Auto Smite Enabled"));
                SmiteMenu.Add(new MenuBool("smiteepic", "Smite Epic"));
                SmiteMenu.Add(new MenuBool("smiteblue", "Smite Blue"));
                SmiteMenu.Add(new MenuBool("smitered", "Smite Red"));
                SmiteMenu.Add(new MenuBool("smitegromp", "Smite Gromp"));
                SmiteMenu.Add(new MenuBool("smitewolf", "Smite Wolf"));
                SmiteMenu.Add(new MenuBool("smiteraptor", "Smite Raptor"));
                SmiteMenu.Add(new MenuBool("smitegolem", "Smite Golem"));
                SmiteMenu.Add(new MenuBool("smitecrab", "Smite Scuttle Crab"));
            }
            Menu.Add(SmiteMenu);
            Menu.Attach();



            Game.OnUpdate += Game_OnUpdate;
        }

        public int[] SmiteDamages = new[] { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 };
        public int SmiteDamage
        {
            get { return SmiteDamages[Player.Level - 1]; }
        }

        private void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            /*SpellSlot smiteSlot = Player.SpellBook.Spells
               .FirstOrDefault(spell => spell.Name.Equals("summonerSmite", StringComparison.OrdinalIgnoreCase)).Slot;*/

            SpellSlot smiteSlot = Player.SpellBook.Spells.FirstOrDefault(spell => spell.Name.Contains("Smite")).Slot;



            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(500) && x.Name != "PlantSatchel" && x.Name != "PlantVision" && x.Name != "PlantHealth" && x.IsEnemy && SmiteDamage >= x.Health && x.IsValidTarget()).OrderBy(x => x.Health);
            var target = minions.FirstOrDefault(x => x.IsInRange(500));

            /*bool smiteBaron = Menu["smite"]["smitebaron"].Enabled;
            bool smiteDragon = Menu["smite"]["smitedragon"].Enabled;
            bool smiteRed = Menu["smite"]["smitered"].Enabled; //SRU_Red
            bool smiteBlue = Menu["smite"]["smiteblue"].Enabled; //SRU_Blue
            bool smiteGromp = Menu["smite"]["smitegromp"].Enabled; //SRU_Gromp
            bool smiteWolf = Menu["smite"]["smitewolf"].Enabled; //SRU_MurkWolf
            bool smiteRaptor = Menu["smite"]["smiteraptor"].Enabled; //SRU_Razorbeak
            bool smiteGolem = Menu["smite"]["smitegolem"].Enabled;*/ //SRU_Krug



            if (target != null)
            {

                //Don't smite minions... lol
                if (target.UnitSkinName.ToString().Contains("Minion") || target.UnitSkinName.ToString().Contains("Plant") || target.UnitSkinName.ToString().Contains("Mini"))
                {

                    return;
                }
                else
                {
                    bool shouldSmite = true;
                    switch (target.UnitSkinName)
                    {

                        case "SRU_Red":
                            if (!Menu["smite"]["smitered"].Enabled)
                            {
                                shouldSmite = false;
                                return;
                            }
                            break;

                        case "SRU_Blue":
                            if (!Menu["smite"]["smiteblue"].Enabled)
                            {
                                shouldSmite = false;
                                return;
                            }
                            break;

                        case "SRU_Gromp":
                            if (!Menu["smite"]["smitegromp"].Enabled)
                            {
                                shouldSmite = false;
                                return;
                            }
                            break;

                        case "SRU_MurkWolf":
                            if (!Menu["smite"]["smitewolf"].Enabled)
                            {
                                shouldSmite = false;
                                return;
                            }
                            break;

                        case "SRU_Razorbeak":
                            if (!Menu["smite"]["smiteraptor"].Enabled)
                            {
                                shouldSmite = false;
                                return;
                            }
                            break;

                        case "SRU_Krug":
                            if (!Menu["smite"]["smitegolem"].Enabled)
                            {
                                shouldSmite = false;
                                return;
                            }
                            break;

                        case "SRU_Crab":
                            if (!Menu["smite"]["smitecrab"].Enabled)
                            {
                                shouldSmite = false;
                                return;
                            }
                            break;

                        default:
                            if (!Menu["smite"]["smiteepic"].Enabled)
                            {
                                shouldSmite = false;
                                return;
                            }
                            break;

                    }


                    // Console.WriteLine("Smiting - " + target.UnitSkinName);
                    if (shouldSmite == true)
                        Player.SpellBook.CastSpell(smiteSlot, target);
                }


            }



        }
    }

}