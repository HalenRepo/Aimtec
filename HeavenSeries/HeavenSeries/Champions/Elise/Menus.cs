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
    internal partial class Elise
    {
        /// <summary>
        ///     Initializes the menus.
        /// </summary>
        public void Menus()
        {
            /// <summary>
            ///     Loads the root Menu.
            /// </summary>
            Champions.Elise.MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - Elise", true);
            {
                Orbwalker.Attach(Champions.Elise.MenuClass.Root);

                Champions.Elise.MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    Champions.Elise.MenuClass.combohumanmenu = new Menu("human", "Human");
                    {
                        Champions.Elise.MenuClass.combohumanmenu.Add(new MenuBool("humanq", "Use Q"));
                        Champions.Elise.MenuClass.combohumanmenu.Add(new MenuBool("humanw", "Use W"));
                        Champions.Elise.MenuClass.combohumanmenu.Add(new MenuBool("humane", "Use E"));
                    }
                    Champions.Elise.MenuClass.combomenu.Add(Champions.Elise.MenuClass.combohumanmenu);

                    Champions.Elise.MenuClass.combospidermenu = new Menu("spider", "Spider");
                    {
                        Champions.Elise.MenuClass.combospidermenu.Add(new MenuBool("spiderq", "Use Q"));
                        Champions.Elise.MenuClass.combospidermenu.Add(new MenuBool("spiderw", "Use W"));
                        Champions.Elise.MenuClass.combospidermenu.Add(new MenuBool("spidere", "Use E"));
                    }
                    Champions.Elise.MenuClass.combomenu.Add(Champions.Elise.MenuClass.combospidermenu);

                    Champions.Elise.MenuClass.combomenuwhitelist = new Menu("whitelist", "E Whitelist");
                    {
                        foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                            Champions.Elise.MenuClass.combomenuwhitelist.Add(new MenuBool("useeon" + enemies.ChampionName.ToLower(), enemies.ChampionName));
                    }
                    Champions.Elise.MenuClass.combomenu.Add(Champions.Elise.MenuClass.combomenuwhitelist);

                    Champions.Elise.MenuClass.combomenu.Add(new MenuBool("autor", "Auto R"));

                }
                Champions.Elise.MenuClass.Root.Add(Champions.Elise.MenuClass.combomenu);

                /// <summary>
                ///     Sets the menu for the drawings.
                /// </summary>
                Champions.Elise.MenuClass.junglemenu = new Menu("jungle", "Jungle");
                {
                    Champions.Elise.MenuClass.junglehumanmenu = new Menu("human", "Human");
                    {
                        Champions.Elise.MenuClass.junglehumanmenu.Add(new MenuSliderBool("humanq", "Use Human Q if mana % >=", true, 25, 1, 100));
                        Champions.Elise.MenuClass.junglehumanmenu.Add(new MenuSliderBool("humanw", "Use Human W if mana % >=", true, 25, 1, 100));
                    }
                    Champions.Elise.MenuClass.junglemenu.Add(Champions.Elise.MenuClass.junglehumanmenu);

                    Champions.Elise.MenuClass.junglespidermenu = new Menu("spider", "Spider");
                    {
                        Champions.Elise.MenuClass.junglespidermenu.Add(new MenuBool("spiderq", "Use Q"));
                        Champions.Elise.MenuClass.junglespidermenu.Add(new MenuBool("spiderw", "Use W"));
                    }
                    Champions.Elise.MenuClass.junglemenu.Add(Champions.Elise.MenuClass.junglespidermenu);

                    Champions.Elise.MenuClass.junglemenu.Add(new MenuBool("autor", "Auto R"));
                    Champions.Elise.MenuClass.junglemenu.Add(new MenuBool("junglesteal", "Jungle Steal"));
                    //MenuClass.junglemenu.Add(new MenuSliderBool("stayspider", "Stay as Spider when Mana below x%", true, 30, 1, 99));
                }
                Champions.Elise.MenuClass.Root.Add(Champions.Elise.MenuClass.junglemenu);

                Champions.Elise.MenuClass.drawmenu = new Menu("draw", "Drawings");
                {
                    Champions.Elise.MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q", false));
                    Champions.Elise.MenuClass.drawmenu.Add(new MenuBool("draww", "Draw W", false));
                    Champions.Elise.MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E"));
                    //MenuClass.drawmenu.Add(new MenuBool("drawPrediction", "Draw E Prediction"));
                }
                Champions.Elise.MenuClass.Root.Add(Champions.Elise.MenuClass.drawmenu);

            }
            Champions.Elise.MenuClass.Root.Attach();
        }

    }
}