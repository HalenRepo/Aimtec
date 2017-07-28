namespace HeavenSeries
{
    using Aimtec.SDK.Menu;

    /// <summary>
    ///     The Menu class.
    /// </summary>
    internal static class MenuClass
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the Root menu.
        /// </summary>
        public static Menu Root { get; set; }


        /// <summary>
        ///     Gets or sets the combo menu.
        /// </summary>
        public static Menu combomenu { get; set; }

        /// <summary>
        ///     Gets or sets the combohumanmenu menu.
        /// </summary>
        public static Menu combohumanmenu { get; set; }

        /// <summary>
        ///     Gets or sets the itemsmenu menu.
        /// </summary>
        public static Menu combospidermenu { get; set; }


        /// <summary>
        ///     Gets or sets the junglemenu menu.
        /// </summary>
        public static Menu junglemenu { get; set; }

        /// <summary>
        ///     Gets or sets the itemsmenu menu.
        /// </summary>
        public static Menu junglehumanmenu { get; set; }


        /// <summary>
        ///     Gets or sets the itemsmenu menu.
        /// </summary>
        public static Menu junglespidermenu { get; set; }

        public static Menu drawmenu { get; set; }

        #endregion
    }
}