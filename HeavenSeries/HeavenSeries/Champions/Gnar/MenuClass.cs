using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeavenSeries.Champions.Gnar
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
        public static Menu comboqmenu { get; set; }

        public static Menu comboemenu { get; set; }

        /// <summary>
        ///     Gets or sets the itemsmenu menu.
        /// </summary>
        public static Menu combowmenu { get; set; }

        public static Menu combormenu { get; set; }

        public static Menu comboronmenu { get; set; }

        public static Menu harassmenu { get; set; }

        public static Menu harassqmenu { get; set; }

        public static Menu harasswmenu { get; set; }

        public static Menu harassemenu { get; set; }

        public static Menu miscmenu { get; set; }

        public static Menu miscoptionsmenu { get; set; }

        public static Menu killstealmenu { get; set; }

        public static Menu drawmenu { get; set; }

        #endregion
    }
}
