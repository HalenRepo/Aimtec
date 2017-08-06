using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;

namespace HeavenSeries
{


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
            /// 
            Champions.Gnar.MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - Gnar", true);
            {
                Orbwalker.Attach(Champions.Gnar.MenuClass.Root);

                //Combo menu
                Champions.Gnar.MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    //Combo -> Q
                    Champions.Gnar.MenuClass.comboqmenu = new Menu("comboqmenu", "Q");
                    {
                        Champions.Gnar.MenuClass.comboqmenu.Add(new MenuBool("useq", "Use Q"));

                    }
                    Champions.Gnar.MenuClass.combomenu.Add(Champions.Gnar.MenuClass.comboqmenu);

                    //Combo -> W
                    Champions.Gnar.MenuClass.combowmenu = new Menu("combowmenu", "W");
                    {
                        Champions.Gnar.MenuClass.combowmenu.Add(new MenuBool("usew", "Use W"));
                    }
                    Champions.Gnar.MenuClass.combomenu.Add(Champions.Gnar.MenuClass.combowmenu);

                    //Combo -> E
                    Champions.Gnar.MenuClass.comboemenu = new Menu("comboemenu", "E");
                    {
                        Champions.Gnar.MenuClass.comboemenu.Add(new MenuBool("usee", "Use E"));
                        Champions.Gnar.MenuClass.comboemenu.Add(new MenuBool("UseEGapclose", "Use E for gapclose if killable"));
                    }
                    Champions.Gnar.MenuClass.combomenu.Add(Champions.Gnar.MenuClass.comboemenu);

                    Champions.Gnar.MenuClass.combormenu = new Menu("combormenu", "R");
                    {
                        Champions.Gnar.MenuClass.combormenu.Add(new MenuBool("user", "Use R"));
                        Champions.Gnar.MenuClass.comboronmenu = new Menu("useronmenu", "Use R on: ");

                    }
                    Champions.Gnar.MenuClass.combomenu.Add(Champions.Gnar.MenuClass.combormenu);
                    Champions.Gnar.MenuClass.combormenu.Add(Champions.Gnar.MenuClass.comboronmenu);



                }
                Champions.Gnar.MenuClass.Root.Add(Champions.Gnar.MenuClass.combomenu);


                //Harass menu
                Champions.Gnar.MenuClass.harassmenu = new Menu("harass", "Harass");
                {

                    //Harass -> Q
                    Champions.Gnar.MenuClass.harassqmenu = new Menu("harassqmenu", "Q");
                    {
                        Champions.Gnar.MenuClass.harassqmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Gnar.MenuClass.harassqmenu.Add(new MenuSlider("useqmana", "Use Q if Mana >= x%", 30, 1, 100));
                    }
                    Champions.Gnar.MenuClass.harassmenu.Add(Champions.Gnar.MenuClass.harassqmenu);

                    //Harass -> W
                    Champions.Gnar.MenuClass.harasswmenu = new Menu("harasswmenu", "W");
                    {
                        Champions.Gnar.MenuClass.harasswmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Gnar.MenuClass.harasswmenu.Add(new MenuSlider("usewmana", "Use W if Mana >= x%", 30, 1, 100));
                    }
                    Champions.Gnar.MenuClass.harassmenu.Add(Champions.Gnar.MenuClass.harasswmenu);

                    //Harass -> E
                    Champions.Gnar.MenuClass.harassemenu = new Menu("harassemenu", "E");
                    {
                        Champions.Gnar.MenuClass.harassemenu.Add(new MenuBool("usee", "Use E"));
                        Champions.Gnar.MenuClass.harassemenu.Add(new MenuSlider("useemana", "Use E if Mana >= x%", 30, 1, 100));
                        Champions.Gnar.MenuClass.harassemenu.Add(new MenuList(
                                        "emode",
                                        "E Mode: ", new[] { "To Escape", "On Enemy" }, 0));
                    }
                    Champions.Gnar.MenuClass.harassmenu.Add(Champions.Gnar.MenuClass.harassemenu);
                }
                Champions.Gnar.MenuClass.Root.Add(Champions.Gnar.MenuClass.harassmenu);

                //Misc menu - 
                Champions.Gnar.MenuClass.miscmenu = new Menu("miscmenu", "Misc");
                {
                    Champions.Gnar.MenuClass.miscoptionsmenu = new Menu("miscoptionsmenu", "Combo Options");
                    {
                        Champions.Gnar.MenuClass.miscoptionsmenu.Add(new MenuList(
                                       "UseWWhen",
                                       "Use W: ", new[] { "Before Q", "After Q" }, 1));
                        Champions.Gnar.MenuClass.miscoptionsmenu.Add(new MenuBool("UseETower", "Dodge Tower Shots with E", false));

                    }
                    Champions.Gnar.MenuClass.miscmenu.Add(Champions.Gnar.MenuClass.miscoptionsmenu);

                    Champions.Gnar.MenuClass.killstealmenu = new Menu("killstealmenu", "Killsteal");
                    {
                        Champions.Gnar.MenuClass.killstealmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Gnar.MenuClass.killstealmenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Gnar.MenuClass.miscmenu.Add(Champions.Gnar.MenuClass.killstealmenu);


                }
                Champions.Gnar.MenuClass.Root.Add(Champions.Gnar.MenuClass.miscmenu);

                //Drawings
                Champions.Gnar.MenuClass.drawmenu = new Menu("drawmenu", "Drawings");
                {

                    Champions.Gnar.MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q"));
                    Champions.Gnar.MenuClass.drawmenu.Add(new MenuBool("draww", "Draw W"));
                    Champions.Gnar.MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E", false));
                    Champions.Gnar.MenuClass.drawmenu.Add(new MenuBool("drawr", "Draw R", false));
                }
                Champions.Gnar.MenuClass.Root.Add(Champions.Gnar.MenuClass.drawmenu);


            }
            Champions.Gnar.MenuClass.Root.Attach();
        }

    }
}
