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
    internal partial class Fizz
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
            Champions.Fizz.MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - Fizz", true);
            {
                Orbwalker.Attach(Champions.Fizz.MenuClass.Root);

                //Combo menu
                Champions.Fizz.MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    //Combo -> Q
                    Champions.Fizz.MenuClass.comboqmenu = new Menu("comboqmenu", "Q");
                    {
                        Champions.Fizz.MenuClass.comboqmenu.Add(new MenuBool("useq", "Use Q"));

                    }
                    Champions.Fizz.MenuClass.combomenu.Add(Champions.Fizz.MenuClass.comboqmenu);

                    //Combo -> W
                    Champions.Fizz.MenuClass.combowmenu = new Menu("combowmenu", "W");
                    {
                        Champions.Fizz.MenuClass.combowmenu.Add(new MenuBool("usew", "Use W"));
                    }
                    Champions.Fizz.MenuClass.combomenu.Add(Champions.Fizz.MenuClass.combowmenu);

                    //Combo -> E
                    Champions.Fizz.MenuClass.comboemenu = new Menu("comboemenu", "E");
                    {
                        Champions.Fizz.MenuClass.comboemenu.Add(new MenuBool("usee", "Use E"));
                        Champions.Fizz.MenuClass.comboemenu.Add(new MenuBool("UseEGapclose", "Use E for gapclose if killable"));
                    }
                    Champions.Fizz.MenuClass.combomenu.Add(Champions.Fizz.MenuClass.comboemenu);

                    Champions.Fizz.MenuClass.combormenu = new Menu("combormenu", "R");
                    {
                        Champions.Fizz.MenuClass.combormenu.Add(new MenuBool("user", "Use R"));
                        Champions.Fizz.MenuClass.comboronmenu = new Menu("useronmenu", "Use R on: ");
                        
                    } 
                    Champions.Fizz.MenuClass.combomenu.Add(Champions.Fizz.MenuClass.combormenu);
                    Champions.Fizz.MenuClass.combormenu.Add(Champions.Fizz.MenuClass.comboronmenu);

                    

                }
                Champions.Fizz.MenuClass.Root.Add(Champions.Fizz.MenuClass.combomenu);


                //Harass menu
                Champions.Fizz.MenuClass.harassmenu = new Menu("harass", "Harass");
                {

                    //Harass -> Q
                    Champions.Fizz.MenuClass.harassqmenu = new Menu("harassqmenu", "Q");
                    {
                        Champions.Fizz.MenuClass.harassqmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Fizz.MenuClass.harassqmenu.Add(new MenuSlider("useqmana", "Use Q if Mana >= x%", 30, 1, 100));
                    }
                    Champions.Fizz.MenuClass.harassmenu.Add(Champions.Fizz.MenuClass.harassqmenu);

                    //Harass -> W
                    Champions.Fizz.MenuClass.harasswmenu = new Menu("harasswmenu", "W");
                    {
                        Champions.Fizz.MenuClass.harasswmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Fizz.MenuClass.harasswmenu.Add(new MenuSlider("usewmana", "Use W if Mana >= x%", 30, 1, 100));
                    }
                    Champions.Fizz.MenuClass.harassmenu.Add(Champions.Fizz.MenuClass.harasswmenu);

                    //Harass -> E
                    Champions.Fizz.MenuClass.harassemenu = new Menu("harassemenu", "E");
                    {
                        Champions.Fizz.MenuClass.harassemenu.Add(new MenuBool("usee", "Use E"));
                        Champions.Fizz.MenuClass.harassemenu.Add(new MenuSlider("useemana", "Use E if Mana >= x%", 30, 1, 100));
                        Champions.Fizz.MenuClass.harassemenu.Add(new MenuList(
                                        "emode",
                                        "E Mode: ", new[] { "To Escape", "On Enemy" }, 0));
                    }
                    Champions.Fizz.MenuClass.harassmenu.Add(Champions.Fizz.MenuClass.harassemenu);
                }
                Champions.Fizz.MenuClass.Root.Add(Champions.Fizz.MenuClass.harassmenu);

                //Misc menu - 
                Champions.Fizz.MenuClass.miscmenu = new Menu("miscmenu", "Misc");
                {
                    Champions.Fizz.MenuClass.miscoptionsmenu = new Menu("miscoptionsmenu", "Combo Options");
                    {
                        Champions.Fizz.MenuClass.miscoptionsmenu.Add(new MenuList(
                                       "UseWWhen",
                                       "Use W: ", new[] { "Before Q", "After Q" }, 1));
                        Champions.Fizz.MenuClass.miscoptionsmenu.Add(new MenuBool("UseETower", "Dodge Tower Shots with E", false));

                    }
                    Champions.Fizz.MenuClass.miscmenu.Add(Champions.Fizz.MenuClass.miscoptionsmenu);

                    Champions.Fizz.MenuClass.killstealmenu = new Menu("killstealmenu", "Killsteal");
                    {
                        Champions.Fizz.MenuClass.killstealmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Fizz.MenuClass.killstealmenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Fizz.MenuClass.miscmenu.Add(Champions.Fizz.MenuClass.killstealmenu);


                }
                Champions.Fizz.MenuClass.Root.Add(Champions.Fizz.MenuClass.miscmenu);

                //Drawings
                Champions.Fizz.MenuClass.drawmenu = new Menu("drawmenu", "Drawings");
                {

                    Champions.Fizz.MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q"));
                    Champions.Fizz.MenuClass.drawmenu.Add(new MenuBool("draww", "Draw W"));
                    Champions.Fizz.MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E", false));
                    Champions.Fizz.MenuClass.drawmenu.Add(new MenuBool("drawr", "Draw R", false));
                }
                Champions.Fizz.MenuClass.Root.Add(Champions.Fizz.MenuClass.drawmenu);


            }
            Champions.Fizz.MenuClass.Root.Attach();
        }

    }
}
