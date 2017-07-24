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

        public object ComboMenu { get; private set; }

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
                ComboMenu.Add(new MenuSeperator("sep1", "Use Q on: "));

                foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                    ComboMenu.Add(new MenuBool("useqon" + enemies.ChampionName.ToLower(), enemies.ChampionName));
                //ComboMenu.Add(new MenuBool("usee", "Use E "));
                ComboMenu.Add(new MenuBool("user", "Use R"));
                ComboMenu.Add(new MenuSliderBool("rAOE", "Minimum enemies for R", true, 2, 1, GameObjects.EnemyHeroes.Count()));
            }
            Menu.Add(ComboMenu);

           /* var ComboMenu = new Menu("combo", "Combo")
            {
                new Menu("Qsub", "Q")
                {
                    new MenuBool("useq", "Use Q")
                },

                new MenuBool("usew", "Use W", false),
                new MenuBool("user", "Use R", false),
            };
            Menu.Add(ComboMenu);

            ComboMenu.Attach();*/

           /* var HookMenu = new Menu("hook", "Q Settings");
            {
                foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                    HookMenu.Add(new MenuBool("useqon" + enemies.ChampionName.ToLower(), "Hook " + enemies.ChampionName));
            }
            Menu.Add(HookMenu);

            var FlayMenu = new Menu("flay", "E Settings");
            {
                FlayMenu.Add(new Menu("todo", "N/A"));

            }
            Menu.Add(FlayMenu);

            var UltMenu = new Menu("ult", "R Settings");
            {
                UltMenu.Add(new MenuSliderBool("rAOE", "Minimum enemies for R", true, 2, 1, GameObjects.EnemyHeroes.Count()));
            }
            Menu.Add(UltMenu);
                */


            var LanternMenu = new Menu("lantern", "Lantern");
            {
                foreach (var ally in GameObjects.AllyHeroes)
                {
                    LanternMenu.Add(new MenuSliderBool(ally.ChampionName.ToLower(), "Use Lantern for " + ally.ChampionName + " @ HP%", true, 30, 1, 99));
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
            if (Menu["combo"]["useq"].Enabled && Menu["combo"]["useqon" + target.ChampionName.ToLower()].Enabled && Q.Ready && target.IsInRange(Q.Range) && target.IsValidTarget())
            {
                if (prediction.HitChance >= HitChance.High)
                {
                    Q.Cast(prediction.UnitPosition);

                    //Q2 -- TODO: Add delay
                    if (target.HasBuff("threshQ"))
                    {
                        Q.CastOnUnit(Player);
                    }

                }
            }

            //W lantern
            if (!W.Ready)
                return;

            foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsInRange(W.Range) && x.IsAlly && !x.IsDead && Menu["lantern"][x.ChampionName.ToLower()].Enabled && x.HealthPercent() < Menu["lantern"][x.ChampionName.ToLower()].Value && !x.IsRecalling()))
            {
                W.Cast(Obj.Position);
            }

                //E logic -- NOT WORKING AS INTENDED
                /*  if (Menu["combo"]["usee"].Enabled && E.Ready && target.IsInRange(E.Range) && !target.HasBuff("threshQ"))
                  {
                      E.Cast(target.Position.Extend(Player.ServerPosition,Vector3.Distance(target.Position, Player.Position) + 400));
                  }*/

                if (Menu["combo"]["user"].Enabled && R.Ready && Player.CountEnemyHeroesInRange(R.Range - 30) >= Menu["combo"]["rAOE"].Value)
                R.Cast();
        }
    }

}