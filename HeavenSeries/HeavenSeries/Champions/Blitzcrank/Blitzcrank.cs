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
                ComboMenu.Add(new MenuBool("usee", "Use E "));
                ComboMenu.Add(new MenuBool("user", "Use R"));
                ComboMenu.Add(new MenuSliderBool("rAOE", "Minimum enemies for R", true, 1, 1, GameObjects.EnemyHeroes.Count()));
                ComboMenu.Add(new MenuBool("userKS", "Use R to Killsteal"));

                ComboMenu.Add(new MenuSeperator("sep1", "Use Q on: "));

                foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                    ComboMenu.Add(new MenuBool("useqon" + enemies.ChampionName.ToLower(), enemies.ChampionName));
            }
            Menu.Add(ComboMenu);
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
            Render.Circle(Player.Position, Q.Range, 30, Color.White);

            //Draw circle under Q target
           /* if (Q.Ready)
            {
                Render.Circle(GetBestEnemyHeroTargetInRange(Q.Range).Position, 50, 30, Color.Blue);
            }*/
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
            if (useQ && Q.Ready && Menu["combo"]["useqon" + target.ChampionName.ToLower()].Enabled && target.IsValidTarget(Q.Range) && target != null)
            {
                var prediction = Q.GetPrediction(target);
                if (prediction.HitChance >= HitChance.High)
                {
                    Q.Cast(prediction.UnitPosition);
                }
                
            }

            //E logic - Avoid using E on already knocked up target
            if (useE && E.Ready && target.IsValidTarget(120) && !target.HasBuffOfType(BuffType.Knockup))
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