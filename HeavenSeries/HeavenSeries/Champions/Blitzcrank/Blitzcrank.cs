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

namespace HeavenSeries
{
    internal class Blitzcrank
    {
        public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Q = new Spell(SpellSlot.Q, 1000f);
        public static Spell W = new Spell(SpellSlot.W);
        public static Spell E = new Spell(SpellSlot.E, 150f);
        public static Spell R = new Spell(SpellSlot.R, 550f);

        public Blitzcrank()
        {
            Orbwalker.Attach(Menu);

            var ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuSlider("predictionSlider", "Prediction (4 is the highest): ", 4, 1, 5));
                ComboMenu.Add(new MenuBool("usee", "Use E "));
                ComboMenu.Add(new MenuBool("user", "Use R"));
                ComboMenu.Add(new MenuSliderBool("rAOE", "Minimum enemies for R", true, 1, 1, GameObjects.EnemyHeroes.Count()));
                ComboMenu.Add(new MenuBool("userKS", "Use R to Killsteal"));

                ComboMenu.Add(new MenuSeperator("sep1", "Use Q on: "));

                foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                    ComboMenu.Add(new MenuBool("useqon" + enemies.ChampionName.ToLower(), enemies.ChampionName));
            }
            Menu.Add(ComboMenu);

            var DrawMenu = new Menu("draw", "Drawings");
            {
                DrawMenu.Add(new MenuBool("drawQ", "Draw Q"));
                DrawMenu.Add(new MenuBool("drawPrediction", "Draw Prediction"));
            }
            Menu.Add(DrawMenu);

            Menu.Attach();

            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.Line);
            R.SetSkillshot(0.25f, 600f, float.MaxValue, false, SkillshotType.Circle);



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

            Killsteal();
        }

        private static void Render_OnPresent()
        {
            //Basic Q range indicator
            if (Menu["draw"]["drawQ"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);
        }


        private void Combo()
        {
            //Store boolean value of menu items
            bool useQ = Menu["combo"]["useq"].Enabled;
            bool useE = Menu["combo"]["usee"].Enabled;
            bool useR = Menu["combo"]["user"].Enabled;
            var rAOE = Menu["combo"]["rAOE"].As<MenuSliderBool>().Value;

            var target = TargetSelector.GetTarget(Q.Range);

            //Q logic
            if (target != null && useQ && Q.Ready && Menu["combo"]["useqon" + target.ChampionName.ToLower()].Enabled && target.IsValidTarget(Q.Range))
            {
                var prediction = Q.GetPrediction(target);
                //Draw prediction
                if (Menu["draw"]["drawPrediction"].Enabled)
                {
                    Render.WorldToScreen(Player.Position, out Vector2 playerScreenPos);
                    Color lineColour;
                    switch (prediction.HitChance)
                    {
                        case HitChance.Collision:
                            lineColour = Color.Red;
                            break;

                        case HitChance.Impossible:
                            lineColour = Color.Orange;
                            break;

                        case HitChance.Medium:
                            lineColour = Color.Orange;
                            break;

                        case HitChance.High:
                            lineColour = Color.LightGreen;
                            break;

                        default:
                            lineColour = Color.Red;
                            return;
                    }
                    
                    Render.WorldToScreen(prediction.UnitPosition, out Vector2 predictionSreenPos);
                    Render.Line(playerScreenPos, predictionSreenPos, lineColour);

                }
                //If prediction high chance -> Q

                HitChance slider = HitChance.High;
                switch (Menu["combo"]["predictionSlider"].Value)
                {
                    case 1:
                        slider = HitChance.Impossible;
                        break;

                    case 2:
                        slider = HitChance.Low;
                        break;

                    case 3:
                        slider = HitChance.Medium;
                        break;

                    case 4:
                        slider = HitChance.High;
                        break;
                }
                //if (prediction.HitChance >= HitChance.High)
                if (prediction.HitChance >= slider)
                {
                    Q.Cast(prediction.UnitPosition);
                }
                
            }

            //E logic - Avoid using E on already knocked up target
            if (useE && E.Ready && target.IsValidTarget(E.Range) && !target.HasBuffOfType(BuffType.Knockup))
            {
                E.Cast();
            }

            //R logic - Use R for AOE
            if (useR && R.Ready && target.IsValidTarget(R.Range) && Player.CountEnemyHeroesInRange(R.Range) >= rAOE)
            {
                R.Cast();
            }
        }

        private static void Killsteal()
        {
            var ks = Menu["combo"]["userKS"].Enabled;
            var target = TargetSelector.GetTarget(Q.Range, false);

            if (ks && R.Ready && target.IsValidTarget(R.Range) && Player.GetSpellDamage(target, SpellSlot.R) >= target.Health)
            {
                R.Cast();
            }
        }
    }

}