namespace HeavenSeries
{
    using System.Linq;

    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;

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
            MenuClass.Root = new Menu("HeavenSeries", "HeavenSeries - Elise", true);
            {
                Orbwalker.Attach(MenuClass.Root);

                MenuClass.combomenu = new Menu("combo", "Combo");
                {
                    MenuClass.combohumanmenu = new Menu("human", "Human");
                    {
                        MenuClass.combohumanmenu.Add(new MenuBool("humanq", "Use Q"));
                        MenuClass.combohumanmenu.Add(new MenuBool("humanw", "Use W"));
                        MenuClass.combohumanmenu.Add(new MenuBool("humane", "Use E"));
                    }
                    MenuClass.combomenu.Add(MenuClass.combohumanmenu);

                    MenuClass.combospidermenu = new Menu("spider", "Spider");
                    {
                        MenuClass.combospidermenu.Add(new MenuBool("spiderq", "Use Q"));
                        MenuClass.combospidermenu.Add(new MenuBool("spiderw", "Use W"));
                        MenuClass.combospidermenu.Add(new MenuBool("spidere", "Use E"));
                    }
                    MenuClass.combomenu.Add(MenuClass.combospidermenu);

                    MenuClass.combomenu.Add(new MenuBool("autor", "Auto R"));

                }
                MenuClass.Root.Add(MenuClass.combomenu);

                /// <summary>
                ///     Sets the menu for the drawings.
                /// </summary>
                MenuClass.junglemenu = new Menu("jungle", "Jungle");
                {
                    MenuClass.junglehumanmenu = new Menu("human", "Human");
                    {
                        MenuClass.junglehumanmenu.Add(new MenuBool("humanq", "Use Q"));
                        MenuClass.junglehumanmenu.Add(new MenuBool("humanw", "Use W"));
                    }
                    MenuClass.junglemenu.Add(MenuClass.junglehumanmenu);

                    MenuClass.junglespidermenu = new Menu("spider", "Spider");
                    {
                        MenuClass.junglespidermenu.Add(new MenuBool("spiderq", "Use Q"));
                        MenuClass.junglespidermenu.Add(new MenuBool("spiderw", "Use W"));
                    }
                    MenuClass.junglemenu.Add(MenuClass.junglespidermenu);

                    MenuClass.junglemenu.Add(new MenuBool("autor", "Auto R"));
                    //MenuClass.junglemenu.Add(new MenuSliderBool("stayspider", "Stay as Spider when Mana below x%", true, 30, 1, 99));
                }
                MenuClass.Root.Add(MenuClass.junglemenu);

                MenuClass.drawmenu = new Menu("draw", "Drawings");
                {
                    MenuClass.drawmenu.Add(new MenuBool("drawq", "Draw Q", false));
                    MenuClass.drawmenu.Add(new MenuBool("draww", "Draw W", false));
                    MenuClass.drawmenu.Add(new MenuBool("drawe", "Draw E"));
                    MenuClass.drawmenu.Add(new MenuBool("drawPrediction", "Draw E Prediction"));
                }
                MenuClass.Root.Add(MenuClass.drawmenu);

            }
            MenuClass.Root.Attach();
        }

    }
}