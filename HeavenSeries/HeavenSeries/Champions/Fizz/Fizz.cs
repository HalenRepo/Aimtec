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
    internal partial class Fizz
    {
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        //CHECK ALL THESE VALUES
        public static Spell Q = new Spell(SpellSlot.Q, 550);
        public static Spell W = new Spell(SpellSlot.W, Player.AttackRange);
        public static Spell E = new Spell(SpellSlot.E, 400);
        public static Spell R = new Spell(SpellSlot.R, 1300);

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;
        public Fizz()
        {
            E.SetSkillshot(0.25f, 400, 1300, false, SkillshotType.Circle);
            R.SetSkillshot(0.25f, 80, 1200, false, SkillshotType.Line);

            Menus();

            foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                Champions.Fizz.MenuClass.comboronmenu.Add(new MenuBool("useron" + enemies.ChampionName.ToLower(), enemies.ChampionName));

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.PostAttack += Orbwalker_OnPostAttack;
            Orbwalker.PreAttack += Orbwalker_OnPreAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;

            Console.WriteLine("HeavenSeries - " + Player.ChampionName + " loaded.");
        }

        private void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            var target = TargetSelector.GetTarget(R.Range);
            KillSteal(target);

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

        private void KillSteal(Obj_AI_Hero target)
        {
            if (Q.Ready && target.IsInRange(Q.Range) && Champions.Fizz.MenuClass.killstealmenu["useq"].Enabled)
            {
                if (Q.Ready && Player.GetSpellDamage(target, SpellSlot.Q) >= target.Health)
                {
                    Q.Cast(target);
                }

                if (E.Ready && target.IsInRange(E.Range*2) && Champions.Fizz.MenuClass.killstealmenu["usee"].Enabled)
                {
                    if (E.Ready && Player.GetSpellDamage(target, SpellSlot.E) >= target.Health)
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        private static void Render_OnPresent()
        {
            if (Champions.Fizz.MenuClass.drawmenu["drawq"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.Fizz.MenuClass.drawmenu["draww"].Enabled)
                Render.Circle(Player.Position, W.Range, 30, Color.White);

            if (Champions.Fizz.MenuClass.drawmenu["drawe"].Enabled)
                Render.Circle(Player.Position, E.Range, 30, Color.White);

            if (Champions.Fizz.MenuClass.drawmenu["drawr"].Enabled)
                Render.Circle(Player.Position, R.Range, 30, Color.White);

            if (LastHarassPos != null && Champions.Fizz.MenuClass.harassemenu["emode"].Value == 0)
            {
                Render.Circle((Vector3)LastHarassPos, 100, 30, Color.Red);
            }

        }
        public static void Orbwalker_OnPostAttack(Object sender, PostAttackEventArgs args)
        {
            //For post attack. If none, return.
            if (IOrbwalker.Mode == OrbwalkingMode.None)
                return;

        }

        public static void Orbwalker_OnPreAttack(object sender, PreAttackEventArgs args)
        {
            if (IOrbwalker.Mode == OrbwalkingMode.None)
                return;
        }

        public void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (sender is Obj_AI_Turret && args.Target.IsMe && E.Ready && Champions.Fizz.MenuClass.miscoptionsmenu["UseETower"].Enabled)
            {
                E.Cast(Game.CursorPos);
            }

            if (!sender.IsMe)
            {
                return;
            }

            if (args.SpellData.Name == "FizzQ")
            {
                if (IOrbwalker.Mode == OrbwalkingMode.Combo)
                {
                    DelayAction.Queue((int)(sender.SpellBook.CastEndTime - Game.ClockTime) + Game.Ping / 2 + 250, () => W.Cast());
                }

                if (IOrbwalker.Mode == OrbwalkingMode.Mixed && E.Ready)
                {
                    //THEN HARASS. SET LAST POSITION BEFORE CASTING.
                    LastHarassPos = Player.ServerPosition;
                }
            }

            if (args.SpellData.Name == "FizzETwo" || args.SpellData.Name == "FizzEBuffer")
            {
                //Get rid of last pos since we are landing now. For next harass.
                LastHarassPos = null;
            }

        }

        private static Vector3? LastHarassPos { get; set; }

        //For fizz E

        private static float DamageToUnit(Obj_AI_Hero target)
        {
            var damage = 0d;
            if (Q.Ready)
                damage += Player.GetSpellDamage(target, SpellSlot.Q);

            //We might want to check if this is actually more than 0
            if (W.Ready)
                damage += Player.GetSpellDamage(target, SpellSlot.W);

            if (E.Ready)
                damage += Player.GetSpellDamage(target, SpellSlot.E);

            if (R.Ready)
                damage += Player.GetSpellDamage(target, SpellSlot.R);

            damage += Player.GetAutoAttackDamage(target); //add auto attack

            return (float)damage;
        }

        public static bool CanKillWithUltCombo(Obj_AI_Hero target)
        {
            return Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.W) + Player.GetSpellDamage(target, SpellSlot.R) >
                   target.Health;
        }

        //Extend the fizz ult for better accuracy
        public static void CastRSmart(Obj_AI_Hero target)
        {
            var castPosition = R.GetPrediction(target).UnitPosition;
            castPosition = Player.ServerPosition.Extend(castPosition, R.Range);

            R.Cast(castPosition);
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(R.Range);

            if (target == null || !target.IsValidTarget())
                return;

            if (Champions.Fizz.MenuClass.comboemenu["UseEGapclose"].Enabled && CanKillWithUltCombo(target) && Q.Ready && W.Ready &&
                 E.Ready && R.Ready && (Player.Distance(target) < Q.Range + E.Range * 2))
            {
                CastRSmart(target);

                E.Cast(Player.ServerPosition.Extend(target.ServerPosition, E.Range - 1));

                W.Cast();
                Q.Cast(target);
            }
            else
            {
                if (R.Ready && Champions.Fizz.MenuClass.combormenu["user"].Enabled && Champions.Fizz.MenuClass.comboronmenu["useron" + target.ChampionName.ToLower()].Enabled)
                {
                    if (Player.GetSpellDamage(target, SpellSlot.R) > target.Health)
                    {
                        CastRSmart(target);
                    }

                    if (DamageToUnit(target) > target.Health)
                    {
                        CastRSmart(target);
                    }

                    if ((Q.Ready || E.Ready))
                    {
                        CastRSmart(target);
                    }

                    if (target.IsInRange(Player.AttackRange))
                    {
                        CastRSmart(target);
                    }
                }

                // Use W Before Q
                if (W.Ready && Champions.Fizz.MenuClass.combowmenu["usew"].Enabled && Champions.Fizz.MenuClass.miscoptionsmenu["UseWWhen"].Value == 0 &&
                    (Q.Ready || target.IsInRange(Player.AttackRange)))
                {
                    W.Cast();
                }

                if (Q.Ready && Champions.Fizz.MenuClass.comboqmenu["useq"].Enabled)
                {
                    if (target.IsInRange(Q.Range))
                        Q.Cast(target);
                }

                if (E.Ready && Champions.Fizz.MenuClass.comboemenu["usee"].Enabled)
                {
                    if (target.IsInRange(E.Range))
                    {
                        E.Cast(target.ServerPosition);
                    }
                        
                }
            }

        }

        private void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range);

            if (target == null || !target.IsValidTarget())
                return;

            // Use W Before Q
            if (W.Ready && Champions.Fizz.MenuClass.harasswmenu["usew"].Enabled && Champions.Fizz.MenuClass.harasswmenu["usewmana"].Value <= Player.ManaPercent() && Champions.Fizz.MenuClass.miscoptionsmenu["UseWWhen"].Value == 0 &&
                (Q.Ready || target.IsInRange(Player.AttackRange)))
            {
                W.Cast();
            }

            if (Q.Ready && Champions.Fizz.MenuClass.harassqmenu["useq"].Enabled && Champions.Fizz.MenuClass.harassqmenu["useqmana"].Value <= Player.ManaPercent())
            {

                //if you want safe harass and E isn't ready... Don't harass.
                if (Champions.Fizz.MenuClass.harassemenu["emode"].Value == 0 && !E.Ready)
                    return;

                Q.Cast(target);
            }

            //Jump to last position
            if (Champions.Fizz.MenuClass.harassemenu["emode"].Value == 0 && LastHarassPos != null && E.Ready && Champions.Fizz.MenuClass.harassemenu["useemana"].Value <= Player.ManaPercent())
            {
                E.Cast((Vector3)LastHarassPos);
                
            }

            //Land on target with E
            if (E.Ready && Champions.Fizz.MenuClass.harassemenu["usee"].Enabled && Champions.Fizz.MenuClass.harassemenu["emode"].Value == 1 && Champions.Fizz.MenuClass.harassemenu["useemana"].Value <= Player.ManaPercent())
            {
                E.Cast(target.ServerPosition);
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

