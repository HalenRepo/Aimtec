namespace Last_Hit
{
    using System.Drawing;
    using System.Linq;

    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Util.Cache;

    internal class Last_Hit_Marker
    {
        public static Menu Menu = new Menu("LastHit", "Last Hit Marker by Halen", true);
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();
        public static Color Color;


        public Last_Hit_Marker()
        {
            var settingsMenu = new Menu("settingsmenu", "Settings");
            {
                settingsMenu.Add(new MenuSlider("rangeslider", "Range", 1500, 250, 2500, false));
            }
            Menu.Add(settingsMenu);

            var DrawMenu = new Menu("DrawMenu", "Drawings");
            {
                DrawMenu.Add(new MenuBool("drawminion", "Draw Last Hit Marker", true));
                DrawMenu.Add(new MenuSlider("circlesize", "Circle Size", 60, 1, 150));
                DrawMenu.Add(new MenuList("color", "Color: ", new[] { "Orange", "Red", "Blue", "Light Green" }, 0));

            }
            Menu.Add(DrawMenu);

            Menu.Attach();

            Render.OnPresent += Render_OnPresent;
        }

        private static void Render_OnPresent()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen() || !Menu["DrawMenu"]["drawminion"].Enabled)
            {
                return;
            }

            switch(Menu["DrawMenu"]["color"].Value)
            {
                case 0:
                    Color = Color.Orange;
                    break;

                case 1:
                    Color = Color.Red;
                    break;

                case 2:
                    Color = Color.Blue;
                    break;

                case 3:
                    Color = Color.LightGreen;
                    break;

                default:
                    Color = Color.Orange;
                    break;
            }

            foreach (var minion in GameObjects.EnemyMinions.Where(x => x.UnitSkinName.Contains("Minion") && x.IsValidTarget() && x.IsInRange(Menu["settingsmenu"]["rangeslider"].Value) && x.Health <= Player.GetAutoAttackDamage(x) && !x.UnitSkinName.Contains("Odin")).ToList())
            {
                Render.Circle(minion.ServerPosition, Menu["DrawMenu"]["circlesize"].Value, 30, Color);
            }
        }
    }
}