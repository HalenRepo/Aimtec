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
    internal partial class Nami
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
            Champions.Nami.MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - Nami", true);
            {
                Orbwalker.Attach(Champions.Nami.MenuClass.Root);

                //Combo menu
                Champions.Nami.MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    //Combo -> Q
                    Champions.Nami.MenuClass.comboqmenu = new Menu("comboqmenu", "Q");
                    {
                        Champions.Nami.MenuClass.comboqmenu.Add(new MenuBool("useq", "Use Q"));

                    }
                    Champions.Nami.MenuClass.combomenu.Add(Champions.Nami.MenuClass.comboqmenu);

                    //Combo -> W
                    Champions.Nami.MenuClass.combowmenu = new Menu("combowmenu", "W");
                    {
                        Champions.Nami.MenuClass.combowmenu.Add(new MenuBool("usew", "Use W"));
                    }
                    Champions.Nami.MenuClass.combomenu.Add(Champions.Nami.MenuClass.combowmenu);

                    //Combo -> E
                    Champions.Nami.MenuClass.comboemenu = new Menu("comboemenu", "E");
                    {
                        Champions.Nami.MenuClass.comboemenu.Add(new MenuBool("usee", "Use E"));
                    }
                    Champions.Nami.MenuClass.combomenu.Add(Champions.Nami.MenuClass.comboemenu);

                }
                Champions.Nami.MenuClass.Root.Add(Champions.Nami.MenuClass.combomenu);
                

                //Harass menu
                Champions.Nami.MenuClass.harassmenu = new Menu("harass", "Harass");
                {
                    //Harass -> W
                    Champions.Nami.MenuClass.harasswmenu = new Menu("harasswmenu", "W");
                    {
                        Champions.Nami.MenuClass.harasswmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Nami.MenuClass.harasswmenu.Add(new MenuSlider("usewmana", "Use W if Mana >= x%", 25, 1, 100));
                    }
                    Champions.Nami.MenuClass.harassmenu.Add(Champions.Nami.MenuClass.harasswmenu);

                    //Harass -> E
                    Champions.Nami.MenuClass.harassemenu = new Menu("harassemenu", "E");
                    {
                        Champions.Nami.MenuClass.harassemenu.Add(new MenuBool("usee", "Use E"));
                        Champions.Nami.MenuClass.harassemenu.Add(new MenuSlider("useemana", "Use E if Mana >= x%", 25, 1, 100));
                    }
                    Champions.Nami.MenuClass.harassmenu.Add(Champions.Nami.MenuClass.harassemenu);
                }
                Champions.Nami.MenuClass.Root.Add(Champions.Nami.MenuClass.harassmenu);

                //Misc menu - Support mode (Credits to @Exory)
                Champions.Nami.MenuClass.miscmenu = new Menu("miscmenu", "Misc");
                {
                    //Misc -> Support Mode
                    Champions.Nami.MenuClass.miscmenu.Add(new MenuBool("autoheal", "Auto Heal"));
                    Champions.Nami.MenuClass.miscmenu.Add(new MenuBool("autoqcc", "Auto Q CC'd Enemy"));
                    Champions.Nami.MenuClass.miscmenu.Add(new MenuBool("antigapq", "Auto Q Gapcloser"));
                    Champions.Nami.MenuClass.miscmenu.Add(new MenuBool("supportmode", "Support Mode"));
                }
                Champions.Nami.MenuClass.Root.Add(Champions.Nami.MenuClass.miscmenu);

                //Drawings
                Champions.Nami.MenuClass.drawmenu = new Menu("drawmenu", "Drawings");
                {
                    
                    Champions.Nami.MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q"));
                    Champions.Nami.MenuClass.drawmenu.Add(new MenuBool("draww", "Draw W"));
                    Champions.Nami.MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E", false));
                    Champions.Nami.MenuClass.drawmenu.Add(new MenuBool("drawr", "Draw R", false));
                }
                Champions.Nami.MenuClass.Root.Add(Champions.Nami.MenuClass.drawmenu);


            }
            Champions.Nami.MenuClass.Root.Attach();
        }

    }
}
