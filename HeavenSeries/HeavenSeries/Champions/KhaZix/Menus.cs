using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util.Cache;
using Aimtec;

namespace HeavenSeries
{


    /// <summary>
    ///     The menu class.
    /// </summary>
    internal partial class KhaZix
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
            Champions.KhaZix.MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - KhaZix", true);
            {
                Orbwalker.Attach(Champions.KhaZix.MenuClass.Root);

                //Combo menu
                Champions.KhaZix.MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    Champions.KhaZix.MenuClass.combomenu.Add(new MenuBool("useq", "Use Q"));
                    Champions.KhaZix.MenuClass.combomenu.Add(new MenuBool("usew", "Use W"));
                    Champions.KhaZix.MenuClass.combomenu.Add(new MenuBool("usee", "Use E"));
                    Champions.KhaZix.MenuClass.combomenu.Add(new MenuBool("user", "Use R"));
                    //Champions.KhaZix.MenuClass.combomenu.Add(new MenuSlider("minenemies", "Min. enemies to Auto R", 2, 1, GameObjects.EnemyHeroes.Count()));

                    Champions.KhaZix.MenuClass.assassinmenu = new Menu("assassinmenu", "Double Jump Settings");
                    {

                        Champions.KhaZix.MenuClass.assassinmenutargets = new Menu("assassintargets", "Targets");
                        {
                            foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                                Champions.KhaZix.MenuClass.assassinmenutargets.Add(new MenuSliderBool(enemies.ChampionName.ToLower(), enemies.ChampionName + " @ HP%", true, 15, 1, 100));
                        }
                        Champions.KhaZix.MenuClass.assassinmenu.Add(Champions.KhaZix.MenuClass.assassinmenutargets);

                        //Champions.KhaZix.MenuClass.assassinmenu.Add(new MenuKeyBind("assassinate", "Assassinate", Aimtec.SDK.Util.KeyCode.T, KeybindType.Toggle, true));
                        Champions.KhaZix.MenuClass.assassinmenu.Add(new MenuList("emode","E Mode: ", new[] { "To Escape", "To Next Low Enemy", "To Cursor" }, 1));

                        
                    }
                    Champions.KhaZix.MenuClass.combomenu.Add(Champions.KhaZix.MenuClass.assassinmenu);

                }
                Champions.KhaZix.MenuClass.Root.Add(Champions.KhaZix.MenuClass.combomenu);


                //Harass menu
                Champions.KhaZix.MenuClass.harassmenu = new Menu("harass", "Harass");
                {

                    //Harass -> Q
                    /*Champions.KhaZix.MenuClass.harassqmenu = new Menu("harassqmenu", "Q");
                    {
                        Champions.KhaZix.MenuClass.harassqmenu.Add(new MenuBool("useq", "Use Q", false));
                        Champions.KhaZix.MenuClass.harassqmenu.Add(new MenuSlider("useqmana", "Use Q if Mana >= x%", 30, 1, 100));
                    }
                    Champions.KhaZix.MenuClass.harassmenu.Add(Champions.KhaZix.MenuClass.harassqmenu);*/

                    //Harass -> W
                    Champions.KhaZix.MenuClass.harasswmenu = new Menu("harasswmenu", "W");
                    {
                        Champions.KhaZix.MenuClass.harasswmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.KhaZix.MenuClass.harasswmenu.Add(new MenuSlider("usewmana", "Use W if Mana >= x%", 30, 1, 100));
                    }
                    Champions.KhaZix.MenuClass.harassmenu.Add(Champions.KhaZix.MenuClass.harasswmenu);

                    //Harass -> E
                   /* Champions.KhaZix.MenuClass.harassemenu = new Menu("harassemenu", "E");
                    {
                        Champions.KhaZix.MenuClass.harassemenu.Add(new MenuBool("usee", "Use E", false));
                        Champions.KhaZix.MenuClass.harassemenu.Add(new MenuSlider("useemana", "Use E if Mana >= x%", 30, 1, 100));

                    }
                    Champions.KhaZix.MenuClass.harassmenu.Add(Champions.KhaZix.MenuClass.harassemenu);*/
                }
                Champions.KhaZix.MenuClass.Root.Add(Champions.KhaZix.MenuClass.harassmenu);

                //Jungle menu
                Champions.KhaZix.MenuClass.junglemenu = new Menu("junglemenu", "Jungle");
                {

                    //Jungle -> Q
                    Champions.KhaZix.MenuClass.jungleqmenu = new Menu("jungleqmenu", "Q");
                    {
                        Champions.KhaZix.MenuClass.jungleqmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.KhaZix.MenuClass.jungleqmenu.Add(new MenuSlider("useqmana", "Use Q if Mana >= x%", 15, 1, 100));
                    }
                    Champions.KhaZix.MenuClass.junglemenu.Add(Champions.KhaZix.MenuClass.jungleqmenu);

                    //Jungle -> W
                    Champions.KhaZix.MenuClass.junglewmenu = new Menu("junglewmenu", "W");
                    {
                        Champions.KhaZix.MenuClass.junglewmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.KhaZix.MenuClass.junglewmenu.Add(new MenuSlider("usewmana", "Use W if Mana >= x%", 30, 1, 100));
                    }
                    Champions.KhaZix.MenuClass.junglemenu.Add(Champions.KhaZix.MenuClass.junglewmenu);

                    //Jungle -> E
                    Champions.KhaZix.MenuClass.jungleemenu = new Menu("jungleemenu", "E");
                    {
                        Champions.KhaZix.MenuClass.jungleemenu.Add(new MenuBool("usee", "Use E", false));
                        Champions.KhaZix.MenuClass.jungleemenu.Add(new MenuSlider("useemana", "Use E if Mana >= x%", 75, 1, 100));
                    }
                    Champions.KhaZix.MenuClass.junglemenu.Add(Champions.KhaZix.MenuClass.jungleemenu);
                }
                Champions.KhaZix.MenuClass.Root.Add(Champions.KhaZix.MenuClass.junglemenu);


                //Misc menu - 
                /*Champions.KhaZix.MenuClass.miscmenu = new Menu("miscmenu", "Misc");
                {
                    Champions.KhaZix.MenuClass.miscoptionsmenu = new Menu("miscoptionsmenu", "Combo Options");
                    {
                        Champions.KhaZix.MenuClass.miscoptionsmenu.Add(new MenuList(
                                       "UseWWhen",
                                       "Use W: ", new[] { "Before Q", "After Q" }, 1));
                        Champions.KhaZix.MenuClass.miscoptionsmenu.Add(new MenuBool("UseETower", "Dodge Tower Shots with E", false));

                    }
                    Champions.KhaZix.MenuClass.miscmenu.Add(Champions.KhaZix.MenuClass.miscoptionsmenu);

                    Champions.KhaZix.MenuClass.killstealmenu = new Menu("killstealmenu", "Killsteal");
                    {
                        Champions.KhaZix.MenuClass.killstealmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.KhaZix.MenuClass.killstealmenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.KhaZix.MenuClass.miscmenu.Add(Champions.KhaZix.MenuClass.killstealmenu);


                }
                Champions.KhaZix.MenuClass.Root.Add(Champions.KhaZix.MenuClass.miscmenu);*/

                //Drawings
                Champions.KhaZix.MenuClass.drawmenu = new Menu("drawmenu", "Drawings");
                {

                    Champions.KhaZix.MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q", false));
                    Champions.KhaZix.MenuClass.drawmenu.Add(new MenuBool("draww", "Draw W"));
                    Champions.KhaZix.MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E"));
                }
                Champions.KhaZix.MenuClass.Root.Add(Champions.KhaZix.MenuClass.drawmenu);
            }
            Champions.KhaZix.MenuClass.Root.Attach();
        }

    }
}
