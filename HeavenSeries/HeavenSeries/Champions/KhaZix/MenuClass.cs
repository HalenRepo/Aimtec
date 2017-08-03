using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeavenSeries.Champions.KhaZix
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

        public static Menu assassinmenu { get; set; }

        public static Menu assassinmenutargets { get; set; }

        public static Menu harassmenu { get; set; }

        public static Menu harassqmenu { get; set; }

        public static Menu harasswmenu { get; set; }

        public static Menu harassemenu { get; set; }

        public static Menu junglemenu { get; set; }

        public static Menu jungleqmenu { get; set; }

        public static Menu junglewmenu { get; set; }

        public static Menu jungleemenu { get; set; }

        public static Menu miscmenu { get; set; }

        public static Menu miscoptionsmenu { get; set; }

        public static Menu killstealmenu { get; set; }

        public static Menu drawmenu { get; set; }

        #endregion
    }
}
