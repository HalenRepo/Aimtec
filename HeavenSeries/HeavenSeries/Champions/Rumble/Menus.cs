using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util.Cache;
using System.Linq;

namespace HeavenSeries
{


    /// <summary>
    ///     The menu class.
    /// </summary>
    internal partial class Rumble
    {
        /// <summary>
        ///     Initializes the menus.
        /// </summary>
        public void Menus()
        {
            /// <summary>
            ///     Loads the root Menu.
            /// </summary>
            /// 
            Champions.Rumble.MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - Rumble", true);
            {
                Orbwalker.Attach(Champions.Rumble.MenuClass.Root);

                //Combo menu
                Champions.Rumble.MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    //Combo -> Q
                    Champions.Rumble.MenuClass.comboqmenu = new Menu("comboqmenu", "Q");
                    {
                        Champions.Rumble.MenuClass.comboqmenu.Add(new MenuBool("useq", "Use Q"));

                    }
                    Champions.Rumble.MenuClass.combomenu.Add(Champions.Rumble.MenuClass.comboqmenu);

                    //Combo -> W
                    Champions.Rumble.MenuClass.combowmenu = new Menu("combowmenu", "W");
                    {
                        Champions.Rumble.MenuClass.combowmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Rumble.MenuClass.combowmenu.Add(new MenuSlider("health", "When <= HP%", 80, 1, 100));
                        Champions.Rumble.MenuClass.combowmenu.Add(new MenuSlider("enemies", "When x enemies are nearby", 1, 1, GameObjects.EnemyHeroes.Count()));
                    }
                    Champions.Rumble.MenuClass.combomenu.Add(Champions.Rumble.MenuClass.combowmenu);

                    //Combo -> E
                    Champions.Rumble.MenuClass.comboemenu = new Menu("comboemenu", "E");
                    {
                        Champions.Rumble.MenuClass.comboemenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Rumble.MenuClass.combomenu.Add(Champions.Rumble.MenuClass.comboemenu);

                    Champions.Rumble.MenuClass.combormenu = new Menu("combormenu", "R");
                    {
                        Champions.Rumble.MenuClass.combormenu.Add(new MenuBool("user", "Use R"));
                        Champions.Rumble.MenuClass.combormenu.Add(new MenuSlider("minenemies", "R minimum enemies: ", 2, 1, GameObjects.EnemyHeroes.Count()));
                        Champions.Rumble.MenuClass.comboronmenu = new Menu("useronmenu", "Use R on: ");
                        {
                            foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                                Champions.Rumble.MenuClass.comboronmenu.Add(new MenuBool(enemies.ChampionName.ToLower(), enemies.ChampionName));
                        }

                    }
                    Champions.Rumble.MenuClass.combomenu.Add(Champions.Rumble.MenuClass.combormenu);
                    Champions.Rumble.MenuClass.combormenu.Add(Champions.Rumble.MenuClass.comboronmenu);
                }
                Champions.Rumble.MenuClass.Root.Add(Champions.Rumble.MenuClass.combomenu);


                //Harass menu
                Champions.Rumble.MenuClass.harassmenu = new Menu("harass", "Harass");
                {
                    //Harass -> Q
                    Champions.Rumble.MenuClass.harassqmenu = new Menu("harassqmenu", "Q");
                    {
                        Champions.Rumble.MenuClass.harassqmenu.Add(new MenuBool("useq", "Use Q"));
                    }
                    Champions.Rumble.MenuClass.harassmenu.Add(Champions.Rumble.MenuClass.harassqmenu);

                    //Harass -> W
                    Champions.Rumble.MenuClass.harasswmenu = new Menu("harasswmenu", "W");
                    {
                        Champions.Rumble.MenuClass.harasswmenu.Add(new MenuBool("usew", "Use W"));
                    }
                    Champions.Rumble.MenuClass.harassmenu.Add(Champions.Rumble.MenuClass.harasswmenu);

                    //Harass -> E
                    Champions.Rumble.MenuClass.harassemenu = new Menu("harassemenu", "E");
                    {
                        Champions.Rumble.MenuClass.harassemenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Rumble.MenuClass.harassmenu.Add(Champions.Rumble.MenuClass.harassemenu);
                }
                Champions.Rumble.MenuClass.Root.Add(Champions.Rumble.MenuClass.harassmenu);

                //Last hit menu
                Champions.Rumble.MenuClass.lasthitmenu = new Menu("lasthitmenu", "Last Hit");
                {
                    Champions.Rumble.MenuClass.lasthitmenu.Add(new MenuBool("usee", "Use E"));
                }
                Champions.Rumble.MenuClass.Root.Add(Champions.Rumble.MenuClass.lasthitmenu);

                //Heat menu - 
                Champions.Rumble.MenuClass.heatmenu = new Menu("heatmenu", "Heat Settings");
                {
                    Champions.Rumble.MenuClass.heatmenu.Add(new MenuSliderBool("autoheat", "Maintain >= x% Heat: ", true, 50, 25, 100));

                    Champions.Rumble.MenuClass.heatspellsmenu = new Menu("heatspells", "Spells");
                    {
                        Champions.Rumble.MenuClass.heatspellsmenu.Add(new MenuBool("heatq", "Use Q", true));
                        Champions.Rumble.MenuClass.heatspellsmenu.Add(new MenuBool("heatw", "Use W", true));
                    }
                    Champions.Rumble.MenuClass.heatmenu.Add(Champions.Rumble.MenuClass.heatspellsmenu);
                }
                Champions.Rumble.MenuClass.Root.Add(Champions.Rumble.MenuClass.heatmenu);

                //Drawings
                Champions.Rumble.MenuClass.drawmenu = new Menu("drawmenu", "Drawings");
                {
                    Champions.Rumble.MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q", false));
                    Champions.Rumble.MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E"));
                    Champions.Rumble.MenuClass.drawmenu.Add(new MenuBool("drawr", "Draw R"));
                }
                Champions.Rumble.MenuClass.Root.Add(Champions.Rumble.MenuClass.drawmenu);


            }
            Champions.Rumble.MenuClass.Root.Attach();
        }

    }
}