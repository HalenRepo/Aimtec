using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec;
using Aimtec.SDK.Util.Cache;

namespace HeavenSeries
{


    /// <summary>
    ///     The menu class.
    /// </summary>
    internal partial class Kindred
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
            Champions.Kindred.MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - Kindred", true);
            {
                Orbwalker.Attach(Champions.Kindred.MenuClass.Root);

                //Combo menu
                Champions.Kindred.MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    //Combo -> Q
                    Champions.Kindred.MenuClass.comboqmenu = new Menu("comboqmenu", "Q");
                    {
                        Champions.Kindred.MenuClass.comboqmenu.Add(new MenuBool("useq", "Use Q"));

                    }
                    Champions.Kindred.MenuClass.combomenu.Add(Champions.Kindred.MenuClass.comboqmenu);

                    //Combo -> W
                    Champions.Kindred.MenuClass.combowmenu = new Menu("combowmenu", "W");
                    {
                        Champions.Kindred.MenuClass.combowmenu.Add(new MenuBool("usew", "Use W"));
                    }
                    Champions.Kindred.MenuClass.combomenu.Add(Champions.Kindred.MenuClass.combowmenu);

                    //Combo -> E
                    Champions.Kindred.MenuClass.comboemenu = new Menu("comboemenu", "E");
                    {
                        Champions.Kindred.MenuClass.comboemenu.Add(new MenuBool("usee", "Use E"));
                        Champions.Kindred.MenuClass.comboemenu.Add(new MenuBool("UseEGapclose", "Use E for gapclose if killable"));
                    }
                    Champions.Kindred.MenuClass.combomenu.Add(Champions.Kindred.MenuClass.comboemenu);

                }
                Champions.Kindred.MenuClass.Root.Add(Champions.Kindred.MenuClass.combomenu);

                //Harass menu
                Champions.Kindred.MenuClass.harassmenu = new Menu("harass", "Harass");
                {

                    //Harass -> Q
                    Champions.Kindred.MenuClass.harassqmenu = new Menu("harassqmenu", "Q");
                    {
                        Champions.Kindred.MenuClass.harassqmenu.Add(new MenuBool("useq", "Use Q"));
                        Champions.Kindred.MenuClass.harassqmenu.Add(new MenuSlider("useqmana", "Use Q if Mana >= x%", 25, 1, 100));
                    }
                    Champions.Kindred.MenuClass.harassmenu.Add(Champions.Kindred.MenuClass.harassqmenu);

                    //Harass -> W
                    Champions.Kindred.MenuClass.harasswmenu = new Menu("harasswmenu", "W");
                    {
                        Champions.Kindred.MenuClass.harasswmenu.Add(new MenuBool("usew", "Use W"));
                        Champions.Kindred.MenuClass.harasswmenu.Add(new MenuSlider("usewmana", "Use W if Mana >= x%", 25, 1, 100));
                    }
                    Champions.Kindred.MenuClass.harassmenu.Add(Champions.Kindred.MenuClass.harasswmenu);

                    //Harass -> E
                    Champions.Kindred.MenuClass.harassemenu = new Menu("harassemenu", "E");
                    {
                        Champions.Kindred.MenuClass.harassemenu.Add(new MenuBool("usee", "Use E"));
                        Champions.Kindred.MenuClass.harassemenu.Add(new MenuSlider("useemana", "Use E if Mana >= x%", 25, 1, 100));
                    }
                    Champions.Kindred.MenuClass.harassmenu.Add(Champions.Kindred.MenuClass.harassemenu);
                }
                Champions.Kindred.MenuClass.Root.Add(Champions.Kindred.MenuClass.harassmenu);

                //Jungle menu - 
                Champions.Kindred.MenuClass.JungleClear = new Menu("junglemenu", "Jungle Clear");
                {
                    Champions.Kindred.MenuClass.JungleClearq = new Menu("Q", "Q");
                    {
                        Champions.Kindred.MenuClass.JungleClearq.Add(new MenuBool("useq", "Use Q"));
                        Champions.Kindred.MenuClass.JungleClearq.Add(new MenuSlider("useqmana", "Use Q if Mana >= x%", 25, 1, 100));

                    }
                    Champions.Kindred.MenuClass.JungleClear.Add(Champions.Kindred.MenuClass.JungleClearq);

                    Champions.Kindred.MenuClass.JungleClearw = new Menu("W", "W");
                    {
                        Champions.Kindred.MenuClass.JungleClearw.Add(new MenuBool("usew", "Use W"));
                        Champions.Kindred.MenuClass.JungleClearw.Add(new MenuSlider("usewmana", "Use W if Mana >= x%", 25, 1, 100));

                    }
                    Champions.Kindred.MenuClass.JungleClear.Add(Champions.Kindred.MenuClass.JungleClearw);

                    Champions.Kindred.MenuClass.JungleCleare = new Menu("E", "E");
                    {
                        Champions.Kindred.MenuClass.JungleCleare.Add(new MenuBool("usee", "Use E"));
                        Champions.Kindred.MenuClass.JungleCleare.Add(new MenuSlider("useemana", "Use E if Mana >= x%", 25, 1, 100));

                    }
                    Champions.Kindred.MenuClass.JungleClear.Add(Champions.Kindred.MenuClass.JungleCleare);


                }
                Champions.Kindred.MenuClass.Root.Add(Champions.Kindred.MenuClass.JungleClear);

                //R settings menu
                Champions.Kindred.MenuClass.rsettingsmenu = new Menu("rsettingsmenu", "Auto R Settings");
                {
                    Champions.Kindred.MenuClass.rsettingsmenu.Add(new MenuBool("autor", "Auto R"));
                    Champions.Kindred.MenuClass.rwhitelist = new Menu("useronmenu", "Whitelist: ");
                    foreach (Obj_AI_Hero allies in GameObjects.AllyHeroes)
                        Champions.Kindred.MenuClass.rwhitelist.Add(new MenuSliderBool(allies.ChampionName.ToLower(), "Save " + allies.ChampionName + " at %HP", true, 15, 1, 99));
                    Champions.Kindred.MenuClass.rsettingsmenu.Add(new MenuSlider("enemies", "Min. Enemies near Ally to use R", 2, 1, GameObjects.EnemyHeroes.Count(), false));
                }
                Champions.Kindred.MenuClass.rsettingsmenu.Add(Champions.Kindred.MenuClass.rwhitelist);
                Champions.Kindred.MenuClass.Root.Add(Champions.Kindred.MenuClass.rsettingsmenu);

                //Drawings
                Champions.Kindred.MenuClass.drawmenu = new Menu("drawmenu", "Drawings");
                {

                    Champions.Kindred.MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q", false));
                    Champions.Kindred.MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E", false));
                    Champions.Kindred.MenuClass.drawmenu.Add(new MenuBool("drawr", "Draw R"));
                }
                Champions.Kindred.MenuClass.Root.Add(Champions.Kindred.MenuClass.drawmenu);


            }
            Champions.Kindred.MenuClass.Root.Attach();
        }

    }
}
