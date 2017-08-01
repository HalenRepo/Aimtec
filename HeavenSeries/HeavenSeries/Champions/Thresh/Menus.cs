namespace HeavenSeries
{
    using System.Linq;

    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;

    /// <summary>
    ///     The menu class.
    /// </summary>
    internal partial class Thresh
    {
        /// <summary>
        ///     Initializes the menus.
        /// </summary>
        public void Menus()
        {
            /// <summary>
            ///     Loads the root Menu.
            /// </summary>
            /*Champions.Thresh.MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - Thresh", true);
            {
                Orbwalker.Attach(Champions.Thresh.MenuClass.Root);

                Champions.Thresh.MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    Champions.Thresh.MenuClass.combohumanmenu = new Menu("human", "Human");
                    {
                        Champions.Thresh.MenuClass.combohumanmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Thresh.MenuClass.combohumanmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Thresh.MenuClass.combohumanmenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Thresh.MenuClass.combomenu.Add(Champions.Thresh.MenuClass.combohumanmenu);

                    Champions.Thresh.MenuClass.combocatmenu = new Menu("cat", "cat");
                    {
                        Champions.Thresh.MenuClass.combocatmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Thresh.MenuClass.combocatmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Thresh.MenuClass.combocatmenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Thresh.MenuClass.combomenu.Add(Champions.Thresh.MenuClass.combocatmenu);

                    Champions.Thresh.MenuClass.combomenu.Add(new MenuBool("user", "Use R"));

                }
                Champions.Thresh.MenuClass.Root.Add(Champions.Thresh.MenuClass.combomenu);


                Champions.Thresh.MenuClass.junglemenu = new Menu("jungle", "Jungle");
                {
                    Champions.Thresh.MenuClass.junglehumanmenu = new Menu("human", "Human");
                    {
                        Champions.Thresh.MenuClass.junglehumanmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Thresh.MenuClass.junglehumanmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Thresh.MenuClass.junglehumanmenu.Add(new MenuBool("usee", "Use E"));
                        //Champions.Thresh.MenuClass.junglehumanmenu.Add(new MenuSliderBool("nidhealth", "Use E (heal) on self when <= HP%", true, 50, 1, 99));
                    }
                    Champions.Thresh.MenuClass.junglemenu.Add(Champions.Thresh.MenuClass.junglehumanmenu);

                    Champions.Thresh.MenuClass.junglecatmenu = new Menu("cat", "cat");
                    {
                        Champions.Thresh.MenuClass.junglecatmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Thresh.MenuClass.junglecatmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Thresh.MenuClass.junglecatmenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Thresh.MenuClass.junglemenu.Add(Champions.Thresh.MenuClass.junglecatmenu);

                    Champions.Thresh.MenuClass.junglemenu.Add(new MenuBool("user", "Auto R"));
                    //Champions.Thresh.MenuClass.junglemenu.Add(new MenuSliderBool("staycat", "Stay as cat when Mana below x%", true, 30, 1, 99));
                }
                Champions.Thresh.MenuClass.Root.Add(Champions.Thresh.MenuClass.junglemenu);

                Champions.Thresh.MenuClass.miscmenu = new Menu("misc", "Misc");
                {
                    Champions.Thresh.MenuClass.miscmenu.Add(new MenuBool("autoheal", "Auto Heal", true));
                    Champions.Thresh.MenuClass.miscmenu.Add(new MenuSliderBool("healhealthreq", "Heal Ally when <= HP%", true, 50, 1, 99));
                    Champions.Thresh.MenuClass.miscmenu.Add(new MenuSliderBool("healmanareq", "Heal Ally when >= MANA%", true, 50, 1, 99));
                    //Champions.Thresh.MenuClass.miscmenu.Add(new MenuBool("drawe", "Draw E", false));
                    //Champions.Thresh.MenuClass.miscmenu.Add(new MenuBool("drawPrediction", "Draw E Prediction"));
                }
                Champions.Thresh.MenuClass.Root.Add(Champions.Thresh.MenuClass.miscmenu);

                Champions.Thresh.MenuClass.drawmenu = new Menu("draw", "Drawings");
                {
                    Champions.Thresh.MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q", true));
                    Champions.Thresh.MenuClass.drawmenu.Add(new MenuBool("draww", "Draw W", false));
                    Champions.Thresh.MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E", false));
                    //Champions.Thresh.MenuClass.drawmenu.Add(new MenuBool("drawPrediction", "Draw Q Prediction"));
                }
                Champions.Thresh.MenuClass.Root.Add(Champions.Thresh.MenuClass.drawmenu);

            }
            Champions.Thresh.MenuClass.Root.Attach();*/
        }

    }
}