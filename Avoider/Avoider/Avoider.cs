using System.Linq;

using Aimtec;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using System.Drawing;
using Aimtec.SDK.Util.Cache;
using System;
using System.Collections.Generic;
using Aimtec.SDK.Orbwalking;

namespace Avoider
{
    internal class Avoider
    {
        public static Menu Menu = new Menu("Avoider", "Avoider", true);
        private static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static List<GameObject> trapsList = new List<GameObject>();

        public static bool avoiding;

        public static float FizzRadius;

        public Avoider()
        {
            Menu.Add(new MenuKeyBind("Key", "Auto Avoid", Aimtec.SDK.Util.KeyCode.N, KeybindType.Toggle));

            var Draw = new Menu("Draw", "Drawings");
            {
                Draw.Add(new MenuBool("Object", "Traps"));
                Draw.Add(new MenuBool("Pathing", "Pathing", false));
            }
            Menu.Add(Draw);
            Menu.Attach();
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.Implementation.PreAttack += Orbwalker_OnPreAttack;
            GameObject.OnCreate += OnGameObjectCreated;
            GameObject.OnDestroy += OnGameObjectDestroyed;

            Obj_AI_Base.OnIssueOrder += OnIssueOrder;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private void OnGameObjectCreated(GameObject sender)
        {
            /*if (sender.Name.ToLower().Contains("trap"))
                Console.WriteLine(Game.ClockTime + "- " + sender.Name + " | isAlly: " + sender.IsAlly + " | Team" + sender.Team);*/

            /*if (sender.IsAlly)
            {
                return;
            }*/

            //Console.WriteLine(Game.ClockTime + " " + sender.Name);

            //Supports: Caitlyn trap, Jinx trap, Nidalee trap, Teemo trap
            if (sender.Name == "caitlyn_Base_yordleTrap_idle_red.troy" || sender.Name == "Cupcake Trap" 
                || sender.Name == "Noxious Trap" /*|| sender.Name == "Fizz_Base_R_Ring_Green.troy"*/ || sender.Name == "Ziggs_Base_E_placedMine.troy")
            {
                trapsList.Add(sender);
            }
        }

        private void OnGameObjectDestroyed(GameObject sender)
        {
            if (sender.IsAlly)
                return;

            if (sender.Name == "caitlyn_Base_yordleTrap_idle_red.troy" || sender.Name == "Cupcake Trap"
                || sender.Name == "Noxious Trap" /*|| sender.Name == "Fizz_Base_R_Ring_Green.troy"*/ || sender.Name == "Ziggs_Base_E_placedMine.troy")
            {
                trapsList.Remove(sender);
            }
        }


        private static void OnIssueOrder(Obj_AI_Base sender, Obj_AI_BaseIssueOrderEventArgs args)
        {
            if (!sender.IsMe || !Menu["Key"].Enabled)
                return;

            //Re-check the position for a trap! Occurs with mostly traps close together.
            if (args.OrderType == OrderType.MoveTo)
            {
                if (trapsList == null)
                    return;

                var movePos = args.Position.To2D();

                for (var i = 0; i < trapsList.Count; i++)
                {
                    if (movePos.Distance(trapsList[i]) < Player.BoundingRadius)
                    {
                        args.ProcessEvent = false;
                    }
                }
            }
        }


        public void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            //To get Fizz R distance which determines radius
           /* if (sender.UnitSkinName == "Fizz")
            {
                if (args.SpellSlot == SpellSlot.R)
                {
                    if (args.End.Distance(args.Start) > 910)
                        FizzRadius = 450;
                    

                    if (args.End.Distance(args.Start) >= 455 && args.End.Distance(args.Start) <= 910)
                        FizzRadius = 320;

                    if (args.End.Distance(args.Start) >= 455 && args.End.Distance(args.Start) <= 910)
                        FizzRadius = 200;
                }
            }*/
        }

            public static void Orbwalker_OnPreAttack(object sender, PreAttackEventArgs args)
        {
            if (Orbwalker.Implementation.Mode == OrbwalkingMode.None)
                return;

            //Fix stuttering when attempting to auto attack whilst avoiding.
            if (avoiding)
                args.Cancel = true;

            avoiding = false;
        }

        private void Game_OnUpdate()
        {
            if (Player.IsDead || !Menu["Key"].Enabled)
                return;

            if (Player.HasBuffOfType(BuffType.Stun) || Player.HasBuffOfType(BuffType.Snare))
            {
                return;
            }

            for (var i = 0; i < trapsList.Count; i++)
            {
                if (Player.Distance(trapsList[i]) > 1000)
                    continue;

                if (Menu["Object"].Enabled)
                    Render.Circle(trapsList[i].Position, 65, 30, Color.Red);

                if (Player.Distance(trapsList[i]) < 200)
                {
                    if (trapsList[i].Name == "caitlyn_Base_yordleTrap_idle_red.troy" || trapsList[i].Name == "Noxious Trap")
                    {
                        Avoid(trapsList[i].Position, 200, trapsList[i]);
                    }

                    if (trapsList[i].Name == "Cupcake Trap" || trapsList[i].Name == "Ziggs_Base_E_placedMine.troy")
                    {
                        Avoid(trapsList[i].Position, 220, trapsList[i]);
                    }

                    /*if (trapsList[i].Name  == "Fizz_Base_R_Ring_Green.troy")
                    {
                        Avoid(trapsList[i].Position, 220, trapsList[i]);
                    }*/
                }
            }
        }

        public static List<Vector3> Pathing(float radius, Vector3 position, GameObject trap)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= 360; i++)
            {
                var angle = i * 2 * Math.PI / 360; //angle = i * 2 * Math.PI / 360;

                if (trap.Name == "Cupcake Trap" || trap.Name == "Ziggs_Base_E_placedMine.troy")
                    angle = i * Math.PI / 360;

                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z + radius * (float)Math.Sin(angle));

                points.Add(point);
            }
            return points;
        }

        private static void Avoid(Vector3 position, float range, GameObject trap)
        {
            avoiding = true;
            var nextPoints = Pathing(100, Player.Position, trap);
            var getPoint = nextPoints.Where(x => x.Distance(position) > range).OrderBy(y => y.Distance(Game.CursorPos)).FirstOrDefault();

            if (getPoint != null)
            {
                if (Menu["Pathing"].Enabled)
                    Render.Circle(getPoint, 30, 30, Color.LightBlue);

                Orbwalker.Implementation.Move(getPoint);
            }
        }
    }
}