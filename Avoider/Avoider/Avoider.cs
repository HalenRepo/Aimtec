using System.Linq;

using Aimtec;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using System.Drawing;
using Aimtec.SDK.Util.Cache;
using System;
using System.Collections.Generic;

namespace Avoider
{
    internal class Avoider
    {
        public static Menu Menu = new Menu("Avoider", "Avoider", true);
        private static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public Avoider()
        {
            Menu.Add(new MenuKeyBind("Key", "Auto Avoid", Aimtec.SDK.Util.KeyCode.N, KeybindType.Toggle));

            var Draw = new Menu("Draw", "Drawings");
            {
                Draw.Add(new MenuBool("Object", "Traps"));
                Draw.Add(new MenuBool("Pathing", "Pathing"));
            }
            Menu.Add(Draw);
            Menu.Attach();
            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate()
        {
            if (Player.IsDead || !Menu["Key"].Enabled)
                return;

            foreach (var obj in GameObjects.AllGameObjects.Where(obj => obj.Team != Player.Team && obj.IsValid))
            {
                if (Player.Distance(obj) > 1000)
                    continue;

                //supports caitlyn trap, teemo trap. WILL NOT AVOID JINX TRAP!
                if (obj.Name.ToLower().Contains("cupcake trap") || obj.Name.ToLower().Contains("noxious trap"))
                {
                    if (Player.Distance(obj) < 200)
                        Avoid(obj.Position, 200);

                    if (Menu["Object"].Enabled)
                        Render.Circle(obj.Position, 100, 30, Color.Red);
                }
            }
        }

        public static List<Vector3> Pathing(float radius, Vector3 position)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= 360; i++)
            {
                var angle = i * 2 * Math.PI / 360;
                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z + radius * (float)Math.Sin(angle));

                points.Add(point);
            }
            return points;
        }

        private static void Avoid(Vector3 position, float range)
        {
            var nextPoints = Pathing(100, Player.Position);
            var getPoint = nextPoints.Where(x => x.Distance(position) > range).OrderBy(y => y.Distance(Game.CursorPos)).FirstOrDefault();

            if (getPoint != null)
            {
                if (Menu["Pathing"].Enabled)
                    Render.Circle(getPoint, 30, 30, Color.LightBlue);

                Player.IssueOrder(OrderType.MoveTo, getPoint);
            }
        }
    }
}