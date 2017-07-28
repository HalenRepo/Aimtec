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
using System.Collections.Generic;

namespace HeavenSeries
{
    internal partial class Nami
    {
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Q = new Spell(SpellSlot.Q, 875);
        public static Spell W = new Spell(SpellSlot.W, 725);
        public static Spell E = new Spell(SpellSlot.E, 800);
        public static Spell R = new Spell(SpellSlot.R, 2200); //wiki has it at 2750? meh.

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;

        public Nami()
        {
            Q.SetSkillshot(1f, 150f, float.MaxValue, false, SkillshotType.Circle);
            R.SetSkillshot(0.5f, 260f, 850f, false, SkillshotType.Line);

            Menus();

            //============ combo ==============

            Champions.Nami.MenuClass.comboqmenu.Add(new MenuSeperator("sepBubble", "Use Q (Bubble) on: "));
            foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                Champions.Nami.MenuClass.comboqmenu.Add(new MenuBool("useqon" + enemies.ChampionName.ToLower(), enemies.ChampionName));

            Champions.Nami.MenuClass.combowmenu.Add(new MenuSeperator("sepHeal", "Use W (Heal) @ HP% on: "));
            foreach (Obj_AI_Hero allies in GameObjects.AllyHeroes)
                Champions.Nami.MenuClass.combowmenu.Add(new MenuSliderBool("usewon" + allies.ChampionName.ToLower(), allies.ChampionName, true, 75, 1, 99));

            Champions.Nami.MenuClass.combowmenu.Add(new MenuSeperator("sepHeal2", "Use W (Damage) on: "));
            foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                Champions.Nami.MenuClass.combowmenu.Add(new MenuBool("usewonbounce" + enemies.ChampionName.ToLower(), enemies.ChampionName, true));

            Champions.Nami.MenuClass.comboemenu.Add(new MenuSeperator("sepE", "Use E on: "));
            foreach (Obj_AI_Hero allies in GameObjects.AllyHeroes)
                Champions.Nami.MenuClass.comboemenu.Add(new MenuBool("useeon" + allies.ChampionName.ToLower(), allies.ChampionName));

            //============= harrass ==============

            Champions.Nami.MenuClass.harasswmenu.Add(new MenuSeperator("sepW", "Use W on: "));
            foreach (Obj_AI_Hero allies in GameObjects.AllyHeroes)
                Champions.Nami.MenuClass.harasswmenu.Add(new MenuBool("useeon" + allies.ChampionName.ToLower(), allies.ChampionName));

            Champions.Nami.MenuClass.harassemenu.Add(new MenuSeperator("sepE", "Use E on: "));
            foreach (Obj_AI_Hero allies in GameObjects.AllyHeroes)
                Champions.Nami.MenuClass.harassemenu.Add(new MenuBool("useeon" + allies.ChampionName.ToLower(), allies.ChampionName));

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.PostAttack += Orbwalker_OnPostAttack;
            Orbwalker.PreAttack += Orbwalker_OnPreAttack;
            //GameObject.OnCreate += Game_RangeAttackOnCreate; //For nami E. Missle implementation. Need SDK spells support.


            Console.WriteLine("HeavenSeries - " + Player.ChampionName + " loaded.");
        }

        //for heal scaling
        private double WHeal
        {
            get
            {
                int[] heal = { 0, 65, 95, 125, 155, 185 };
                return heal[Player.SpellBook.GetSpell(SpellSlot.Q).Level] + Player.FlatMagicDamageMod * 0.3;
            }
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

                case OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case OrbwalkingMode.Laneclear:
                    LaneClear(); //TODO
                    JungleClear();
                    break;
            }
        }

        private static void Render_OnPresent()
        {
            if (Champions.Nami.MenuClass.drawmenu["drawq"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.Nami.MenuClass.drawmenu["draww"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.Nami.MenuClass.drawmenu["drawe"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.Nami.MenuClass.drawmenu["drawr"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

        }
        public static void Orbwalker_OnPostAttack(Object sender, PostAttackEventArgs args)
        {
            //For post attack. If none, return.
            if (IOrbwalker.Mode == OrbwalkingMode.None)
                return;
           
        }

        public static void Orbwalker_OnPreAttack(object sender, PreAttackEventArgs args)
        {
            switch (IOrbwalker.Mode)
            {
                //Support mode logic, credit to @Exory
                case OrbwalkingMode.Mixed:
                case OrbwalkingMode.Lasthit:
                case OrbwalkingMode.Laneclear:
                    if (GetEnemyLaneMinionsTargets().Contains(args.Target) && Champions.Nami.MenuClass.miscmenu["supportmode"].Enabled)
                    {
                        args.Cancel = GameObjects.AllyHeroes.Any(a => !a.IsMe && a.Distance(Player) < 2500);
                    }
                    break;
            }
        }

        //Credits to Exory
        public static List<Obj_AI_Minion> GetEnemyLaneMinionsTargets()
        {
            return GetEnemyLaneMinionsTargetsInRange(float.MaxValue);
        }
        public static List<Obj_AI_Minion> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(m => m.IsValidTarget(range) && m.UnitSkinName.Contains("Minion") && !m.UnitSkinName.Contains("Odin")).ToList();
        }

        private void Combo()
        {
            //E logic
            //find suitable ally for E that IS NOT NAMI
            if (Champions.Nami.MenuClass.comboemenu["usee"].Enabled)
            {
                foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsInRange(E.Range) && !x.IsMe && x.IsAlly && !x.IsDead && Champions.Nami.MenuClass.comboemenu["useeon" + x.ChampionName.ToLower()].Enabled && !Player.IsRecalling() && x.CountEnemyHeroesInRange(x.AttackRange + 50) > 1))
                {
                    if (Obj != null && E.Ready)
                    {
                        E.CastOnUnit(Obj);
                    }

                }
                //Then I guess settle for nami.
                if (E.Ready && Champions.Nami.MenuClass.comboemenu["useeon" + Player.ChampionName.ToLower()].Enabled && !Player.IsRecalling() && Player.CountEnemyHeroesInRange(Player.AttackRange + 50) > 1)
                {
                    E.CastOnUnit(Player);
                }
            }

            //Q logic
            var target = TargetSelector.GetTarget(Q.Range);
            var prediction = Q.GetPrediction(target);

            if (target == null)
                return;

            if (Q.Ready && target.IsInRange(Q.Range) && Champions.Nami.MenuClass.combomenu["comboqmenu"]["useq"].Enabled && Champions.Nami.MenuClass.comboqmenu["useqon" + target.ChampionName.ToLower()].Enabled)
            {
                if (prediction.HitChance >= HitChance.High)
                {
                    //Use CastPosition for circular skillshots
                    Q.Cast(prediction.CastPosition);
                }
            }

            //W use
            if (W.Ready)
            {
                ChooseHeal();
            }

           
        }

        private void Harass()
        {
            if (W.Ready)
            {
                ChooseHeal();
            }

            //E logic
            //find suitable ally for E that IS NOT NAMI
            if (Champions.Nami.MenuClass.harassemenu["usee"].Enabled)
            {
                foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsInRange(E.Range) && !x.IsMe && x.IsAlly && !x.IsDead && Champions.Nami.MenuClass.comboemenu["useeon" + x.ChampionName.ToLower()].Enabled && !Player.IsRecalling() && x.CountEnemyHeroesInRange(x.AttackRange + 100) > 1))
                {
                    if (Obj != null && E.Ready)
                    {
                        E.CastOnUnit(Obj);
                    }

                }
                //Then I guess settle for Nami.
                if (E.Ready && Champions.Nami.MenuClass.harassemenu["useeon" + Player.ChampionName.ToLower()].Enabled && !Player.IsRecalling() && Player.CountEnemyHeroesInRange(Player.AttackRange + 100) > 1)
                {
                    E.CastOnUnit(Player);
                }
            }
        }

        private void Game_RangeAttackOnCreate(GameObject sender)
        {
            //you should check for missiles here to E, but that's not in SDK yet?
            //Console.WriteLine("sender " + sender.ToString());
            return;
        }

        private void ChooseHeal()
        {
            //Force heal low ally
            foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsInRange(W.Range) && x.IsAlly && !x.IsDead && Champions.Nami.MenuClass.combowmenu["usewon" + x.ChampionName.ToLower()].Enabled && x.HealthPercent() < Champions.Nami.MenuClass.combowmenu["usewon" + x.ChampionName.ToLower()].Value && !x.IsRecalling()))
            {
                if (Obj != null && Obj.IsInRange(W.Range) && !Player.IsRecalling())
                {
                    W.CastOnUnit(Obj);
                    return;
                }
            }

            if (Champions.Nami.MenuClass.combowmenu["usew"].Enabled)
            {
                var target = TargetSelector.GetTarget(W.Range + (W.Range /2));

                if (target != null && Champions.Nami.MenuClass.combowmenu["usewonbounce" + target.ChampionName.ToLower()].Enabled)
                {
                    if (Player.Distance(target) > W.Range) //find a bounce! Let's bounce!
                    {
                        var bouncetarget = ObjectManager.Get<Obj_AI_Hero>()
                            .SingleOrDefault(x => x.IsEnemy && x.IsInRange(W.Range) && x.Distance(target) < W.Range);

                        if (bouncetarget != null && bouncetarget.MaxHealth - bouncetarget.Health > WHeal)
                        {
                            W.CastOnUnit(bouncetarget);
                        }
                    }
                    else
                    {
                        //then target is in range
                        W.CastOnUnit(target);
                    }
                }
            }
        }

        private void JungleClear()
        {
            
        }

        private void LaneClear()
        {
            //TODO.
        }


    }

}