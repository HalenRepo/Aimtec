using System;
using System.Drawing;
using System.Linq;

using Aimtec;
using Aimtec.SDK.Damage;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.TargetSelector;
using Aimtec.SDK.Util.Cache;

using Spell = Aimtec.SDK.Spell;
using Aimtec.SDK.Prediction.Skillshots;
using Aimtec.SDK.Util;

namespace HeavenSeries
{
    internal class Thresh
    {
        public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Q = new Spell(SpellSlot.Q, 1080);
        public static Spell W = new Spell(SpellSlot.W, 950);
        public static Spell E = new Spell(SpellSlot.E, 500);
        public static Spell R = new Spell(SpellSlot.R, 400);

        public Thresh()
        {
            Orbwalker.Attach(Menu);

            var ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuBool("usee", "Use E "));
                ComboMenu.Add(new MenuBool("user", "Use R"));
            }
            Menu.Add(ComboMenu);
            var LanternMenu = new Menu("lantern", "Lantern");
            {
                foreach (var ally in GameObjects.AllyHeroes)
                {
                    LanternMenu.Add(new MenuBool(ally.ChampionName.ToLower(), "Use Lantern for: " + ally.ChampionName));
                }
            }
            Menu.Add(LanternMenu);
            Menu.Attach();

            Q.SetSkillshot(0.4f, 60f, 1400f, true, SkillshotType.Line);
            W.SetSkillshot(0.5f, 50f, 2200f, false, SkillshotType.Circle);

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;

            Console.WriteLine("HeavenSeries - " + Player.ChampionName + " loaded.");
        }

        private void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            switch (Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
            }
        }

        private static void Render_OnPresent()
        {
            //Basic Q range indicator
            Render.Circle(Player.Position, Q.Range, 30, Color.White);
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range);

            if (target == null)
                return;
            
            var prediction = Q.GetPrediction(target);

            //Q logic
            if (Menu["combo"]["useq"].Enabled && Q.Ready)
            {
                if (prediction.HitChance >= HitChance.High)
                {
                    Q.Cast(prediction.UnitPosition);

                    //Q2 logic
                    if (target.HasBuff("threshQ"))
                    {
                        Q.CastOnUnit(Player);
                    }

                }
            }

            //E logic
            if (Menu["combo"]["usee"].Enabled && E.Ready)
            {
                E.Cast(target.Position.Extend(Player.ServerPosition,Vector3.Distance(target.Position, Player.Position) + 400));
            }
        }
    }

}