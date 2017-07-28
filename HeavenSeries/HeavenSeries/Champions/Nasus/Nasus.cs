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
using Aimtec.SDK.Util;

namespace HeavenSeries
{
    internal class Nasus
    {
        public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Q = new Spell(SpellSlot.Q); //could be 150 from wiki
        public static Spell W = new Spell(SpellSlot.W, 600);
        public static Spell E = new Spell(SpellSlot.E, 650);
        public static Spell R = new Spell(SpellSlot.R);

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;

        public Nasus()
        {
            Orbwalker.Attach(Menu);

            var combomenu = new Menu("combo", "Combo")
            {
                new Menu("Q", "Q")
                {
                    new MenuBool("useq", "Use Q"),
                    new MenuSlider("minmanaq", "Minimum Mana % for Q: ", 10, 1, 100)
                },

                new Menu("W", "W")
                {
                    new MenuBool("usew", "Use W"),
                    new MenuSlider("minmanaw", "Minimum Mana % for W: ", 25, 1, 100)
                },

                new Menu("E", "E")
                {
                    new MenuBool("usee", "Use E"),
                    new MenuSlider("minmanae", "Minimum Mana % for E: ", 50, 1, 100)
                },

                new Menu("R", "R")
                {
                    new MenuBool("useq", "Use Q"),
                    new MenuBool("manar", "Keep Mana for R"),
                    new MenuSliderBool("rAOE", "Minimum enemies for R", true, 2, 1, GameObjects.EnemyHeroes.Count()),
                    //new MenuSlider("minmanar", "Minimum Mana % for R: ", 25, 1, 100)
                },
            };
            Menu.Add(combomenu);

            var harassmenu = new Menu("harass", "Mixed/Harass")
            {
                new Menu("W", "W")
                {
                    new MenuBool("usew", "Use W"),
                    new MenuSlider("minmanaw", "Minimum Mana % for W: ", 75, 0, 100)

                },

                new Menu("E", "E")
                {
                    new MenuBool("usee", "Use E"),
                    new MenuSlider("minmanae", "Minimum Mana % for E: ", 75, 1, 100)
                },
            };
            Menu.Add(harassmenu);

            var farmmenu = new Menu("farm", "Farm")
            {
                new Menu("Q", "Q")
                {
                    new MenuBool("useq", "Use Q"),
                    new MenuSlider("minmanaq", "Minimum Mana % for Q: ", 10, 1, 100)
                },
            };
            Menu.Add(farmmenu);

            Menu.Attach();

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.PreAttack += Orbwalker_OnPreAttack;
            Orbwalker.PostAttack += Orbwalker_OnPostAttack;

            Console.WriteLine("HeavenSeries - " + Player.ChampionName + " loaded.");
        }

        private void Game_OnUpdate()
        {
            //Console.WriteLine(Game.ClockTime + " | " + Player.GetSpellDamage(Player, SpellSlot.Q));
            if (Player.IsDead || MenuGUI.IsChatOpen())
                return;

            switch (Orbwalker.Mode)
            {
                case OrbwalkingMode.Lasthit:
                    if (Q.Ready)
                    {
                        useQ();
                    }
                    break;

                case OrbwalkingMode.Combo:
                    Combo();
                    break;

                case OrbwalkingMode.Mixed:
                    Mixed();
                    break;

                case OrbwalkingMode.Laneclear:
                    /*if (Q.Ready)
                    {
                        useQ();
                    }*/
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

        private static void Render_OnPresent()
        {
            //Drawings
           /* if (Menu["Draw"]["drawQstacks"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White); //CHANGE THIS!

            if (Menu["Draw"]["drawW"].Enabled)
                Render.Circle(Player.Position, W.Range, 30, Color.White);

            if (Menu["Draw"]["drawE"].Enabled)
                Render.Circle(Player.Position, E.Range, 30, Color.White);*/
        }

        private void useQ()
        {
            //Menu check
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsMinion && x.IsInRange(175) && x.IsEnemy && x.IsValidSpellTarget());

            if (minions == null)
                return;

            var lastHitMinion = minions.FirstOrDefault(x => Player.GetSpellDamage(x, SpellSlot.Q) + Player.GetAutoAttackDamage(x) >= x.Health && x.Health > 5 && x.IsMinion); //&& Player.SpellBook.GetSpell(SpellSlot.Q).SpellData.???) <--- found out stack count to determine q damage?
           
            if (lastHitMinion != null)
            {
                Q.Cast();
                IOrbwalker.ForceTarget(lastHitMinion);
            }
                
        }

        public static void Orbwalker_OnPostAttack(Object sender, PostAttackEventArgs args)
        {
            //For post attack. If none, return.
            if (IOrbwalker.Mode == OrbwalkingMode.None)
                return;
           
            

            var target = TargetSelector.GetTarget(Player.AttackRange, false);
            if (target != null && Q.Ready && IOrbwalker.Mode == OrbwalkingMode.Combo && target.HasBuffOfType(BuffType.Slow) && Player.GetSpellDamage(target, SpellSlot.Q) + Player.TotalAttackDamage + 50 >= target.Health) //he added a check for player health > q damage + auto attack + 50
            {
                Q.Cast();
                //maybe add if (!Q.Ready) check here
                DelayAction.Queue(100 + Game.Ping, Orbwalker.ResetAutoAttackTimer); //100?
            }


            //DelayAction.Queue(100 + Game.Ping, Orbwalker.ResetAutoAttackTimer);
        }

        public static void Orbwalker_OnPreAttack(object sender, PreAttackEventArgs args)
        {
            var target = TargetSelector.GetTarget(Player.AttackRange, false);
            if (Q.Ready && target != null && IOrbwalker.Mode == OrbwalkingMode.Combo && !target.HasBuffOfType(BuffType.Slow) && target.Health < Player.GetSpellDamage(target, SpellSlot.Q) + Player.TotalAttackDamage) //he added a check for player health < q damage + auto attack
            {
                Q.Cast();
            }

            


        }

        private void Mixed()
        {
            var target = TargetSelector.GetTarget(E.Range); //Since it's the longest range
            if (Q.Ready)
                useQ();



            if (target == null)
                return;

        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range);

            if (target == null)
                return;

            if (W.Ready && target.IsInRange(W.Range))
            {
                W.Cast(target);

            }

            if (Q.Ready && target.IsInRange(175))
            {
                Q.Cast();
            }

            if (E.Ready && Player.GetSpellDamage(target, SpellSlot.E) > target.Health)
            {
                var prediction = E.GetPrediction(target);
                E.Cast(prediction.CastPosition);
            }




        }
    }
}