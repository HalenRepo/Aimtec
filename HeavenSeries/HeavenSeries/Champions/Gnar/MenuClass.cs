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

        public static Menu combominimenu { get; set; }

        public static Menu combomegamenu { get; set; }

        public static Menu comborwhitelist { get; set; }

        public static Menu harassmenu { get; set; }

        public static Menu harassminimenu { get; set; }

        public static Menu harassmegamenu { get; set; }

        public static Menu laneclearmenu { get; set; }

        public static Menu laneclearminimenu { get; set; }

        public static Menu laneclearmegamenu { get; set; }

        public static Menu drawmenu { get; set; }

        #endregion
    }
}
