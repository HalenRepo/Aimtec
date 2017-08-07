namespace HeavenSeries
{
    using System.Linq;

    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec;
    using Aimtec.SDK.Util.Cache;

    /// <summary>
    ///     The menu class.
    /// </summary>
    internal partial class Gnar
    {
        /// <summary>
        ///     Initializes the menus.
        /// </summary>
        public void Menus()
        {
            /// <summary>
            ///     Loads the root Menu.
            /// </summary>
            Champions.Gnar.MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - Gnar", true);
            {
                Orbwalker.Attach(Champions.Gnar.MenuClass.Root);

                Champions.Gnar.MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    Champions.Gnar.MenuClass.combominimenu = new Menu("mini", "Mini");
                    {
                        Champions.Gnar.MenuClass.combominimenu.Add(new MenuBool("miniq", "Use Q"));
                    }
                    Champions.Gnar.MenuClass.combomenu.Add(Champions.Gnar.MenuClass.combominimenu);

                    Champions.Gnar.MenuClass.combomegamenu = new Menu("mega", "Mega");
                    {
                        Champions.Gnar.MenuClass.combomegamenu.Add(new MenuBool("megaq", "Use Q"));
                        Champions.Gnar.MenuClass.combomegamenu.Add(new MenuBool("megaw", "Use W"));
                        Champions.Gnar.MenuClass.combomegamenu.Add(new MenuBool("megae", "Use E", false));
                        Champions.Gnar.MenuClass.combomegamenu.Add(new MenuBool("megar", "Use R"));

                        Champions.Gnar.MenuClass.comborwhitelist = new Menu("whitelist", "R Whitelist");
                        {
                            foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                                Champions.Gnar.MenuClass.comborwhitelist.Add(new MenuBool(enemies.ChampionName.ToLower(), enemies.ChampionName));
                        }
                        Champions.Gnar.MenuClass.combomegamenu.Add(Champions.Gnar.MenuClass.comborwhitelist);
                    }
                    Champions.Gnar.MenuClass.combomenu.Add(Champions.Gnar.MenuClass.combomegamenu);
                }
                Champions.Gnar.MenuClass.Root.Add(Champions.Gnar.MenuClass.combomenu);

                Champions.Gnar.MenuClass.harassmenu = new Menu("harass", "Harass");
                {
                    Champions.Gnar.MenuClass.harassminimenu = new Menu("mini", "Mini");
                    {
                        Champions.Gnar.MenuClass.harassminimenu.Add(new MenuBool("miniq", "Use Q"));
                        Champions.Gnar.MenuClass.harassminimenu.Add(new MenuBool("minie", "Use E", false));
                    }
                    Champions.Gnar.MenuClass.harassmenu.Add(Champions.Gnar.MenuClass.harassminimenu);

                    Champions.Gnar.MenuClass.harassmegamenu = new Menu("mega", "Mega");
                    {
                        Champions.Gnar.MenuClass.harassmegamenu.Add(new MenuBool("megaq", "Use Q"));
                        Champions.Gnar.MenuClass.harassmegamenu.Add(new MenuBool("megaw", "Use W"));
                        Champions.Gnar.MenuClass.harassmegamenu.Add(new MenuBool("megae", "Use E"));
                    }
                    Champions.Gnar.MenuClass.harassmenu.Add(Champions.Gnar.MenuClass.harassmegamenu);

                    //MenuClass.drawmenu.Add(new MenuBool("drawPrediction", "Draw E Prediction"));
                }
                Champions.Gnar.MenuClass.Root.Add(Champions.Gnar.MenuClass.harassmenu);

                Champions.Gnar.MenuClass.laneclearmenu = new Menu("laneclear", "Lane Clear");
                {
                    Champions.Gnar.MenuClass.laneclearminimenu = new Menu("mini", "Mini");
                    {
                        Champions.Gnar.MenuClass.laneclearminimenu.Add(new MenuBool("miniq", "Use Q"));
                    }
                    Champions.Gnar.MenuClass.laneclearmenu.Add(Champions.Gnar.MenuClass.laneclearminimenu);

                    Champions.Gnar.MenuClass.laneclearmegamenu = new Menu("mega", "Mega");
                    {
                        Champions.Gnar.MenuClass.laneclearmegamenu.Add(new MenuBool("megaq", "Use Q"));
                    }
                    Champions.Gnar.MenuClass.laneclearmenu.Add(Champions.Gnar.MenuClass.laneclearmegamenu);

                    //MenuClass.drawmenu.Add(new MenuBool("drawPrediction", "Draw E Prediction"));
                }
                Champions.Gnar.MenuClass.Root.Add(Champions.Gnar.MenuClass.laneclearmenu);


                Champions.Gnar.MenuClass.drawmenu = new Menu("draw", "Drawings");
                {
                    Champions.Gnar.MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q", true));
                    Champions.Gnar.MenuClass.drawmenu.Add(new MenuBool("draww", "Draw W", false));
                    Champions.Gnar.MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E", false));
                    Champions.Gnar.MenuClass.drawmenu.Add(new MenuBool("drawr", "Draw R", true));
                    //MenuClass.drawmenu.Add(new MenuBool("drawPrediction", "Draw E Prediction"));
                }
                Champions.Gnar.MenuClass.Root.Add(Champions.Gnar.MenuClass.drawmenu);

            }
            Champions.Gnar.MenuClass.Root.Attach();
        }

    }
}