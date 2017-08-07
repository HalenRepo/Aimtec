using System;
using System.Drawing;
using Aimtec;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.TargetSelector;
using Aimtec.SDK.Util.Cache;

using Spell = Aimtec.SDK.Spell;
using Aimtec.SDK.Prediction.Skillshots;
using System.Collections.Generic;
using System.Linq;
using Aimtec.SDK.Damage;

namespace HeavenSeries
{
    internal partial class Gnar
    {
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Q = new Spell(SpellSlot.Q, 1100);
        public static Spell W = new Spell(SpellSlot.W);
        public static Spell E = new Spell(SpellSlot.E, 475);

        public static Spell Qmega = new Spell(SpellSlot.Q, 1100);
        public static Spell Wmega = new Spell(SpellSlot.W, 525);
        public static Spell Emega = new Spell(SpellSlot.E, 475);

        public static Spell R = new Spell(SpellSlot.R, 420);

        public static bool mini;

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;
        public Gnar()
        {
            Q.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.Line);
            E.SetSkillshot(0.5f, 150, float.MaxValue, false, SkillshotType.Circle);
            // Mega
            Qmega.SetSkillshot(0.25f, 80, 1200, true, SkillshotType.Line);
            Wmega.SetSkillshot(0.25f, 80, float.MaxValue, false, SkillshotType.Line);
            Emega.SetSkillshot(0.5f, 150, float.MaxValue, false, SkillshotType.Circle);

            R.SetSkillshot(0.25f, 500, 1200, false, SkillshotType.Circle);

            Menus();

            /*foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                Champions.Gnar.MenuClass.comboronmenu.Add(new MenuBool("useron" + enemies.ChampionName.ToLower(), enemies.ChampionName));*/

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

            CheckForm();

            var target = TargetSelector.GetTarget(Q.Range);

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

        private void CheckForm()
        {

            /*var buffs = ObjectManager.GetLocalPlayer().Buffs.Select(buff => buff.Name).ToList();
Console.WriteLine(string.Join(Environment.NewLine, buffs));*/

            if (Player.SpellBook.GetSpell(SpellSlot.Q).Name.ToLower() == "gnarq")
            {
                mini = true;
            }
            else
            {
                mini = false;
            }
        }

        private static void Render_OnPresent()
        {
            if (Champions.Gnar.MenuClass.drawmenu["drawq"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.Gnar.MenuClass.drawmenu["draww"].Enabled)
                Render.Circle(Player.Position, W.Range, 30, Color.White);

            if (Champions.Gnar.MenuClass.drawmenu["drawe"].Enabled)
                Render.Circle(Player.Position, E.Range, 30, Color.White);

            if (Champions.Gnar.MenuClass.drawmenu["drawr"].Enabled)
                Render.Circle(Player.Position, R.Range, 30, Color.White);

            //Render.Circle(Player.Position, 700, 30, Color.Red);

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
            if (!sender.IsMe)
            {
                return;
            }
        }

        public static List<Obj_AI_Minion> GetEnemyLaneMinionsTargets()
        {
            return GetEnemyLaneMinionsTargetsInRange(float.MaxValue);
        }

        /// <summary>
        ///     Gets the valid lane minions targets in the game inside a determined range.
        /// </summary>
        public static List<Obj_AI_Minion> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(m => m.IsValidTarget(range) && m.UnitSkinName.Contains("Minion") && !m.UnitSkinName.Contains("Odin")).ToList();
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range);

            if (target == null || !target.IsValidTarget())
                return;

            if (mini)
            {
                if (Q.Ready && Champions.Gnar.MenuClass.combominimenu["miniq"].Enabled)
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        Q.Cast(prediction.UnitPosition);
                    }
                }
            }
            else //mega
            {
                //R
                if (target != null && R.Ready && Champions.Gnar.MenuClass.comborwhitelist[target.ChampionName.ToLower()].Enabled && Champions.Gnar.MenuClass.combomegamenu["megar"].Enabled && !target.HasBuffOfType(BuffType.Stun))
                {
                    var prediction = R.GetPrediction(target); //We're not actually going to use this though really. Slows things down.
                    var maxAngle = 180f;
                    var step = maxAngle / 24f;
                    var currentAngle = 0f;
                    var currentStep = 0f;
                    var direction = (Player.ServerPosition - prediction.UnitPosition).Normalized();

                    //Go through angles
                    while (true)
                    {
                        //If no good angle then stop trying to find one
                        if (currentStep > maxAngle && currentAngle < 0)
                            break;

                        //Go through angles...
                        if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                        {
                            currentAngle = (currentStep) * (float)Math.PI / 180;
                            currentStep += step;
                        }
                        else if (currentAngle > 0)
                            currentAngle = -currentAngle;
                        Vector3 checkPoint;
                        //Check direct line
                        if (currentStep == 0)
                        {
                            currentStep = step;
                            checkPoint = prediction.UnitPosition + 500 * direction; 
                        }
                        //Check via angles
                        else
                        {
                            checkPoint = prediction.UnitPosition + 500 * direction.To2D().Rotated(currentAngle).To3D();
                            Render.Circle(Player.Position + 500 * (checkPoint - prediction.UnitPosition).Normalized(), 50, 30, Color.Red);
                        }

                        //Is wall or building?
                        if (NavMesh.WorldToCell(checkPoint).Flags.HasFlag(NavCellFlags.Wall | NavCellFlags.Building))
                        {
                            //Cast ult in that direction
                            if (prediction.HitChance >= HitChance.High)
                            {
                                Render.Circle(Player.Position + 500 * (checkPoint - prediction.UnitPosition).Normalized(), 50, 30, Color.LightGreen);
                                R.Cast(Player.Position + 500 * (checkPoint - prediction.UnitPosition).Normalized());
                            }
                            
                            break;
                        }
                    }
                }

                //W
                if (Wmega.Ready && Champions.Gnar.MenuClass.combomegamenu["megaw"].Enabled)
                {
                    if (target != null || target.HasBuffOfType(BuffType.Stun))
                    {
                        var prediction = Wmega.GetPrediction(target);
                        if (prediction.HitChance >= HitChance.High)
                        {
                            Wmega.Cast(prediction.CastPosition);
                        }
                    }
                }

                //E
                if (Emega.Ready && Champions.Gnar.MenuClass.combomegamenu["megae"].Enabled)
                {
                    if (target != null)
                    {
                        var prediction = Emega.GetPrediction(target);
                        if (prediction.HitChance >= HitChance.High)
                        {
                            Emega.Cast(prediction.CastPosition);
                        }
                    }
                }

                //Q
                if (Qmega.Ready && Champions.Gnar.MenuClass.combomegamenu["megaq"].Enabled)
                {
                    if (target != null)
                    {
                        var prediction = Qmega.GetPrediction(target);
                        if (prediction.HitChance >= HitChance.High)
                        {
                            Qmega.Cast(prediction.UnitPosition);
                        }
                    }
                }
            }
        }

        private void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range);

            if (target == null || !target.IsValidTarget())
                return;

            if (mini)
            {
                if (Q.Ready && Champions.Gnar.MenuClass.harassminimenu["miniq"].Enabled)
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        Q.Cast(prediction.UnitPosition);
                    }
                }
            }
            else //mega
            {
                //W
                if (Wmega.Ready && Champions.Gnar.MenuClass.harassmegamenu["megaw"].Enabled)
                {
                    if (target != null || target.HasBuffOfType(BuffType.Stun))
                    {
                        var prediction = Wmega.GetPrediction(target);
                        if (prediction.HitChance >= HitChance.High)
                        {
                            Wmega.Cast(prediction.CastPosition);
                        }
                    }
                }

                //E
                if (Emega.Ready && Champions.Gnar.MenuClass.harassmegamenu["megae"].Enabled)
                {
                    if (target != null)
                    {
                        var prediction = Emega.GetPrediction(target);
                        if (prediction.HitChance >= HitChance.High)
                        {
                            Emega.Cast(prediction.CastPosition);
                        }
                    }
                }

                //Q
                if (Qmega.Ready && Champions.Gnar.MenuClass.harassmegamenu["megaq"].Enabled)
                {
                    if (target != null)
                    {
                        var prediction = Qmega.GetPrediction(target);
                        if (prediction.HitChance >= HitChance.High)
                        {
                            Qmega.Cast(prediction.UnitPosition);
                        }
                    }
                }
            }
        }


        private void JungleClear()
        {
            foreach (var minion in GetEnemyLaneMinionsTargetsInRange(Q.Range))
            {
                if (minion.IsValidTarget(Q.Range) && minion != null)
                {
                    Q.Cast(minion);
                }
            }
        }

        private void LaneClear()
        {
            //TODO.
        }
    }
}

