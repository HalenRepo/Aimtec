namespace HeavenSeries
{
    using System.Linq;

    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;

    /// <summary>
    ///     The menu class.
    /// </summary>
    internal partial class Nidalee
    {
        /// <summary>
        ///     Initializes the menus.
        /// </summary>
        public void Menus()
        {
            /// <summary>
            ///     Loads the root Menu.
            /// </summary>
            Champions.Nidalee.MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - Nidalee", true);
            {
                Orbwalker.Attach(Champions.Nidalee.MenuClass.Root);

                Champions.Nidalee.MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    Champions.Nidalee.MenuClass.combohumanmenu = new Menu("human", "Human");
                    {
                        Champions.Nidalee.MenuClass.combohumanmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Nidalee.MenuClass.combohumanmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Nidalee.MenuClass.combohumanmenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Nidalee.MenuClass.combomenu.Add(Champions.Nidalee.MenuClass.combohumanmenu);

                    Champions.Nidalee.MenuClass.combocatmenu = new Menu("cat", "Cougar");
                    {
                        Champions.Nidalee.MenuClass.combocatmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Nidalee.MenuClass.combocatmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Nidalee.MenuClass.combocatmenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Nidalee.MenuClass.combomenu.Add(Champions.Nidalee.MenuClass.combocatmenu);

                    Champions.Nidalee.MenuClass.combomenu.Add(new MenuBool("user", "Use R"));

                }
                Champions.Nidalee.MenuClass.Root.Add(Champions.Nidalee.MenuClass.combomenu);


                Champions.Nidalee.MenuClass.junglemenu = new Menu("jungle", "Jungle");
                {
                    Champions.Nidalee.MenuClass.junglehumanmenu = new Menu("human", "Human");
                    {
                        Champions.Nidalee.MenuClass.junglehumanmenu.Add(new MenuSliderBool("useq", "Use Q when MANA% >=", true, 25, 1, 100));
                        Champions.Nidalee.MenuClass.junglehumanmenu.Add(new MenuSliderBool("usew", "Use W when MANA% >=", true, 25, 1, 100));
                        Champions.Nidalee.MenuClass.junglehumanmenu.Add(new MenuSliderBool("usee", "Use E when MANA% >=", true, 25, 1, 100));
                        //Champions.Nidalee.MenuClass.junglehumanmenu.Add(new MenuSliderBool("nidhealth", "Use E (heal) on self when <= HP%", true, 50, 1, 99));
                    }
                    Champions.Nidalee.MenuClass.junglemenu.Add(Champions.Nidalee.MenuClass.junglehumanmenu);

                    Champions.Nidalee.MenuClass.junglecatmenu = new Menu("cat", "Cougar");
                    {
                        Champions.Nidalee.MenuClass.junglecatmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Nidalee.MenuClass.junglecatmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Nidalee.MenuClass.junglecatmenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Nidalee.MenuClass.junglemenu.Add(Champions.Nidalee.MenuClass.junglecatmenu);

                    Champions.Nidalee.MenuClass.junglemenu.Add(new MenuBool("user", "Auto R"));
                    //Champions.Nidalee.MenuClass.junglemenu.Add(new MenuSliderBool("staycat", "Stay as cat when Mana below x%", true, 30, 1, 99));
                }
                Champions.Nidalee.MenuClass.Root.Add(Champions.Nidalee.MenuClass.junglemenu);

                Champions.Nidalee.MenuClass.miscmenu = new Menu("misc", "Misc");
                {
                    Champions.Nidalee.MenuClass.miscmenu.Add(new MenuBool("autoheal", "Auto Heal", true));
                    Champions.Nidalee.MenuClass.miscmenu.Add(new MenuSliderBool("healhealthreq", "Heal Ally when <= HP%", true, 50, 1, 99));
                    Champions.Nidalee.MenuClass.miscmenu.Add(new MenuSliderBool("healmanareq", "Heal Ally when >= MANA%", true, 50, 1, 99));
                    //Champions.Nidalee.MenuClass.miscmenu.Add(new MenuBool("drawe", "Draw E", false));
                    //Champions.Nidalee.MenuClass.miscmenu.Add(new MenuBool("drawPrediction", "Draw E Prediction"));
                }
                Champions.Nidalee.MenuClass.Root.Add(Champions.Nidalee.MenuClass.miscmenu);

                Champions.Nidalee.MenuClass.drawmenu = new Menu("draw", "Drawings");
                {
                    Champions.Nidalee.MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q", true));
                    Champions.Nidalee.MenuClass.drawmenu.Add(new MenuBool("draww", "Draw W", false));
                    Champions.Nidalee.MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E", false));
                    //Champions.Nidalee.MenuClass.drawmenu.Add(new MenuBool("drawPrediction", "Draw Q Prediction"));
                }
                Champions.Nidalee.MenuClass.Root.Add(Champions.Nidalee.MenuClass.drawmenu);

            }
            Champions.Nidalee.MenuClass.Root.Attach();
        }

    }
}