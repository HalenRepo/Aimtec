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
    internal partial class KhaZix
    {
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        //CHECK ALL THESE VALUES
        public static Spell Q = new Spell(SpellSlot.Q, 325);
        public static Spell W = new Spell(SpellSlot.W, 1000);
        public static Spell E = new Spell(SpellSlot.E, 700);
        public static Spell R = new Spell(SpellSlot.R);

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;

        protected bool EvolvedQ, EvolvedW, EvolvedE;
        protected Obj_AI_Base KhaETrail, KhaELand;
        protected bool isMidAir;

        public KhaZix()
        {
            W.SetSkillshot(0.225f, 80f, 825f, true, SkillshotType.Line);
            E.SetSkillshot(0.25f, 100f, 1000f, false, SkillshotType.Circle);

            Menus();

            /*foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                Champions.KhaZix.MenuClass.comboronmenu.Add(new MenuBool("useron" + enemies.ChampionName.ToLower(), enemies.ChampionName));*/

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.PostAttack += Orbwalker_OnPostAttack;
            Orbwalker.PreAttack += Orbwalker_OnPreAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            GameObject.OnCreate += OnGameObjectCreated;
            GameObject.OnDestroy += OnGameObjectDestroyed;

            Console.WriteLine("HeavenSeries - " + Player.ChampionName + " loaded.");
        }

        private void OnGameObjectCreated(GameObject sender)
        {
            if (sender.Name == "Khazix_Base_E_WeaponTrails.troy")
            {
                //flying
                KhaETrail = sender as Obj_AI_Base;
                isMidAir = true;
            }
            if (sender.Name == "Khazix_Base_E_Land.troy")
            {
                KhaELand = sender as Obj_AI_Base;
                isMidAir = false;
            }
        }

        private void OnGameObjectDestroyed(GameObject sender)
        {
            if (sender.Name == "Khazix_Base_E_WeaponTrails.troy")
            {
                KhaETrail = null;
                isMidAir = false;
            }
            if (sender.Name == "Khazix_Base_E_Land.troy")
            {
                KhaELand = null;
                isMidAir = false;
            }
        }

        private void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            EvolvedSpells();

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

        private void EvolvedSpells()
        {
            if (!EvolvedQ && Player.HasBuff("khazixqevo"))
            {
                Q.Range = 375;
                EvolvedQ = true;
            }
            if (!EvolvedW && Player.HasBuff("khazixwevo"))
            {
                W.Width = 103f;
                EvolvedW = true;
            }

            if (!EvolvedE && Player.HasBuff("khazixeevo"))
            {
                E.Range = 900;
                EvolvedE = true;
            }
        }

        /* 
        private bool IsIsolated(Obj_AI_Base enemy)
        {
            return
                !ObjectManager.Get<Obj_AI_Base>()
                    .Any(
                        x =>
                            (x.NetworkId != enemy.NetworkId) && (x.Team == enemy.Team) && (x.Distance(enemy) <= 450) &&
                            ((x.Type == GameObjectType.obj_AI_Hero) || (x.Type == GameObjectType.obj_AI_Minion) ||
                             (x.Type == GameObjectType.obj_AI_Turret)));
        }*/

        private static void Render_OnPresent()
        {
            if (Champions.KhaZix.MenuClass.drawmenu["drawq"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.KhaZix.MenuClass.drawmenu["draww"].Enabled)
                Render.Circle(Player.Position, W.Range, 30, Color.White);

            if (Champions.KhaZix.MenuClass.drawmenu["drawe"].Enabled)
                Render.Circle(Player.Position, E.Range, 30, Color.White);
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

        }

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

        private void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range);
            if (target == null || !target.IsValidTarget())
                return;

            if (Champions.KhaZix.MenuClass.assassinmenutargets[target.ChampionName.ToLower()].Enabled)
            {
                //Console.WriteLine(Game.ClockTime + " assassin combo");
                //Look for reset!
                var JumpPoint1 = GetDoubleJumpPoint(target, true);
                E.Cast(JumpPoint1.To2D());
                Q.Cast(target);

                if (Champions.KhaZix.MenuClass.combomenu["usew"].Enabled && W.Ready)
                {
                    var prediction = W.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.High)
                        W.Cast(prediction.UnitPosition);
                }

                DelayAction.Queue(Game.Ping + 300, () =>
                {
                    if (E.Ready && !isMidAir)
                    {
                        //2nd jump
                        var Jumppoint2 = GetDoubleJumpPoint(target, false);
                        if (Jumppoint2 != null && Jumppoint2.X != 0 && Jumppoint2.Y != 0)
                        {
                            E.Cast(Jumppoint2.To2D());
                        }
                            
                    }
                });
            }
            else
            {
                //Console.WriteLine(Game.ClockTime + " standard combo");
                //STANDARD COMBO
                /*if (Champions.KhaZix.MenuClass.combomenu["user"].Enabled && Player.CountEnemyHeroesInRange(E.Range) >= Champions.KhaZix.MenuClass.combomenu["minenemies"].Value)
                {
                    R.Cast();
                }*/

                if (Champions.KhaZix.MenuClass.combomenu["usee"].Enabled && E.Ready && !target.IsInRange(Q.Range) && !isMidAir)
                {

                    if (target.IsUnderEnemyTurret() /*  && MENU CHECK FOR DONT E UNDER TURRET*/)
                        return;

                    E.Cast(target.Position);
                }

                if (Champions.KhaZix.MenuClass.combomenu["useq"].Enabled && Q.Ready && !isMidAir)
                {
                    Q.CastOnUnit(target);
                }

                /* DO HYDRA MID AIR? here*/

                if (Champions.KhaZix.MenuClass.combomenu["usew"].Enabled && W.Ready)
                {
                    var prediction = W.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.High)
                        W.Cast(prediction.UnitPosition);
                }

                /* DO ITEM SUPPORT HERE*/
            }
        }

        //Credits to @Seph for some of this logic
        Vector3 GetDoubleJumpPoint(Obj_AI_Hero Qtarget, bool firstjump)
        {
            if (firstjump == true)
            {
                return Qtarget.ServerPosition;
            }

            Obj_AI_Turret closestTower = ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsAlly).OrderBy(tur => tur.Distance(Player.Position)).First();

            //Jump back to friendly closest turret
            if (Player.ServerPosition.PointUnderEnemyTurret())
            {
                return Player.ServerPosition.Extend(closestTower.ServerPosition, E.Range);
            }

            //To safety
            if (Champions.KhaZix.MenuClass.assassinmenu["emode"].Value == 0)
            {
                return Player.ServerPosition.Extend(closestTower.ServerPosition, E.Range);
            }

            //To cursor
            if (Champions.KhaZix.MenuClass.assassinmenu["emode"].Value == 2)
            {
                return Game.CursorPos;
            }

            Vector3 Position = new Vector3();
            var jumptarget = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && Champions.KhaZix.MenuClass.assassinmenutargets[x.ChampionName.ToLower()].Enabled 
            && x.HealthPercent() <= Champions.KhaZix.MenuClass.assassinmenutargets[x.ChampionName.ToLower()].Value && x.IsValidTarget() && x.IsValidSpellTarget() 
            && x.IsEnemy && !x.IsDead && !x.IsInvulnerable && x != Qtarget).FirstOrDefault(x => x.IsInRange(E.Range));

            if (jumptarget != null)
            {
                Position = jumptarget.ServerPosition;
            }

            if (Champions.KhaZix.MenuClass.assassinmenu["emode"].Value == 1)
            {
                return Position;
            }

            //Then to safety because no other targets
            if (jumptarget == null)
            {
                return Player.ServerPosition.Extend(closestTower.ServerPosition, E.Range);
            }
            
            return Position;
        }


        private void Harass()
        {
            var target = TargetSelector.GetTarget(W.Range);
            if (target == null || !target.IsValidTarget())
                return;

            if (Champions.KhaZix.MenuClass.harasswmenu["usew"].Enabled && W.Ready)
            {
                var prediction = W.GetPrediction(target);
                if (prediction.HitChance >= HitChance.High)
                    W.Cast(prediction.UnitPosition);
            }
        }


        private void JungleClear()
        {

            var minionse = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(E.Range) && x.IsEnemy && x.IsValidSpellTarget() && !x.UnitSkinName.ToLower().Contains("minion"));
            var targete = minionse.FirstOrDefault(x => x.IsInRange(E.Range));
            if (Champions.KhaZix.MenuClass.jungleemenu["usee"].Enabled && Player.ManaPercent() >= Champions.KhaZix.MenuClass.jungleemenu["useemana"].Value)
            {
                if (targete != null && E.Ready)
                    E.Cast(targete.Position);
            }

            var minionsq = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(Q.Range) && x.IsEnemy && x.IsValidSpellTarget() && !x.UnitSkinName.ToLower().Contains("minion"));
            var targetq = minionsq.OrderBy(x => x.MaxHealth).FirstOrDefault(); //was OrderByDescending. Changed to OrderBy to Q small minions first, leaving the large minion to last for isolation

            /*USE ITEM FOR JUNGLE CLEAR E.G. TIAMAT HERE*/

            if (targetq != null && Q.Ready && targetq.IsValidTarget() && Champions.KhaZix.MenuClass.jungleqmenu["useq"].Enabled 
                && Player.ManaPercent() >= Champions.KhaZix.MenuClass.jungleqmenu["useqmana"].Value)
            {
                Q.CastOnUnit(targetq);
            }

            var minionsw = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(E.Range) && x.IsEnemy && x.IsValidSpellTarget() && !x.UnitSkinName.ToLower().Contains("minion"));
            var targetw = minionsw.OrderByDescending(x => x.MaxHealth).FirstOrDefault();

            if (targetw != null && W.Ready)
            {
                W.Cast(targetw.Position);
            }
        }

        private void LaneClear()
        {
            //TODO.
        }
    }
}

