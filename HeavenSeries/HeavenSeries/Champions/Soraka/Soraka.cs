namespace HeavenSeries
{
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
    using System.Collections.Generic;

    internal class Soraka
    {
        public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Q = new Spell(SpellSlot.Q, 950);
        public static Spell W = new Spell(SpellSlot.W, 450);
        public static Spell E = new Spell(SpellSlot.E, 925);
        public static Spell R = new Spell(SpellSlot.R);

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;

        public Soraka()
        {
            Orbwalker.Attach(Menu);

            var ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuBool("usee", "Use E "));
            }
            Menu.Add(ComboMenu);
            var HealAllies = new Menu("healallies", "Auto Heal (W)");
            {
                HealAllies.Add(new MenuBool("autow", "Auto W", true));
                HealAllies.Add(new MenuSlider("autowhealth", "Keep Soraka Health above x% ", 10, 1, 99, false));
                HealAllies.Add(new MenuSeperator("sep1", "Minimum HP%: "));
                foreach (Obj_AI_Hero ally in GameObjects.AllyHeroes)
                {
                    //Don't include Soraka herself as well in Auto W
                    if (!ally.IsMe)
                        HealAllies.Add(new MenuSliderBool(ally.ChampionName.ToLower(), ally.ChampionName, true, 75, 1, 99, false));
                }
                    
            }
            Menu.Add(HealAllies);

            var UltAllies = new Menu("ultallies", "Auto R");
            {
                UltAllies.Add(new MenuBool("autor", "Auto R", true));
                UltAllies.Add(new MenuSeperator("sep1", "Minimum HP%: "));
                foreach (Obj_AI_Hero ally in GameObjects.AllyHeroes)
                    UltAllies.Add(new MenuSliderBool(ally.ChampionName.ToLower(), ally.ChampionName, true, 15, 1, 99, false));
            }
            Menu.Add(UltAllies);

            var Misc = new Menu("Misc", "Misc");
            {
                Misc.Add(new MenuBool("supportmode", "Support Mode", true));
            }
            Menu.Add(Misc);

            var Draw = new Menu("Draw", "Drawings");
            {
                Draw.Add(new MenuBool("drawQ", "Draw Q", false));
                Draw.Add(new MenuBool("drawW", "Draw W", true));
                Draw.Add(new MenuBool("drawE", "Draw E", false));
            }
            Menu.Add(Draw);

            Menu.Attach();

            Q.SetSkillshot(0.5f, 300f, 1750f, false, SkillshotType.Circle);
            E.SetSkillshot(0.5f, 70f, 1750f, false, SkillshotType.Circle);

            

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.PreAttack += Orbwalker_OnPreAttack;

            Console.WriteLine("HeavenSeries - " + Player.ChampionName + " loaded.");
        }

        private void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
                return;

            if (Menu["healallies"]["autow"].Enabled)
                AutoW();

            if (Menu["ultallies"]["autor"].Enabled)
                AutoR();

            switch (Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;

                case OrbwalkingMode.Mixed:
                    Mixed();
                    break;
            }
        }

        public static List<Obj_AI_Minion> GetEnemyLaneMinionsTargets()
        {
            return GetEnemyLaneMinionsTargetsInRange(float.MaxValue);
        }
        public static List<Obj_AI_Minion> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(m => m.IsValidTarget(range) && m.UnitSkinName.Contains("Minion") && !m.UnitSkinName.Contains("Odin")).ToList();
        }

        public static void Orbwalker_OnPreAttack(object sender, PreAttackEventArgs args)
        {
            switch (IOrbwalker.Mode)
            {
                //Support mode logic, credit to @Exory
                case OrbwalkingMode.Mixed:
                case OrbwalkingMode.Lasthit:
                case OrbwalkingMode.Laneclear:
                    if (GetEnemyLaneMinionsTargets().Contains(args.Target) && Menu["Misc"]["supportmode"].Enabled)
                    {
                        args.Cancel = GameObjects.AllyHeroes.Any(a => !a.IsMe && a.Distance(Player) < 2500);
                    }
                    break;
            }
        }

        private static void Render_OnPresent()
        {
            //Drawings
            if (Menu["Draw"]["drawQ"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Menu["Draw"]["drawW"].Enabled)
                Render.Circle(Player.Position, W.Range, 30, Color.White);

            if (Menu["Draw"]["drawE"].Enabled)
                Render.Circle(Player.Position, E.Range, 30, Color.White);
        }

        private static void AutoR()
        {
            if (!R.Ready)
                return;

            foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && x.HealthPercent() < Menu["ultallies"][x.ChampionName.ToLower()].Value && !x.IsRecalling()))
            {
                R.Cast();
            }

        }

        private static void AutoW()
        {
            
            if (!W.Ready || !Menu["healallies"]["autow"].Enabled || Player.IsDead)
                return;

            //Check Soraka minimum health to heal so she doesn't end up getting too low
            if (Player.HealthPercent() < Menu["healallies"]["autowhealth"].Value)
            {
                return;
            }

            foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsInRange(W.Range) && x.IsAlly && x.HealthPercent() < Menu["healallies"][x.ChampionName.ToLower()].Value && !x.IsRecalling() && !x.IsMe))
            {
                if (Obj.IsInRange(W.Range) && Obj != null && !Player.IsRecalling())
                {
                    W.CastOnUnit(Obj);
                }
            }
        }

        private void Mixed()
        {
            var target = TargetSelector.GetTarget(Q.Range);

            if (target == null)
                return;

            var prediction = Q.GetPrediction(target);

            if (Menu["combo"]["useq"].Enabled && Q.Ready)
            {
                if (prediction.HitChance >= HitChance.High)
                {
                    //Use CastPosition for circular skillshots
                    Q.Cast(prediction.CastPosition);
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range);

            if (target == null)
                return;

            var prediction = Q.GetPrediction(target);

            if (Menu["combo"]["useq"].Enabled && Q.Ready)
            {
                if (prediction.HitChance >= HitChance.High)
                {
                    //Use CastPosition for circular skillshots
                    Q.Cast(prediction.CastPosition);
                }
            }


            if (Menu["combo"]["usee"].Enabled && E.Ready && target.IsInRange(E.Range))
            {
                E.Cast(target);
            }
        }
    }
}