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
    internal class RekSai
    {
        public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        //comment is wiki
        public static Spell Q = new Spell(SpellSlot.Q, 300); //325
        public static Spell W = new Spell(SpellSlot.W, 200); //160
        public static Spell E = new Spell(SpellSlot.E, 250); 

        public static Spell Qb = new Spell(SpellSlot.Q, 1450); //1650
        public static Spell Wb = new Spell(SpellSlot.W, 200); //160
        public static Spell Eb = new Spell(SpellSlot.E, 700); 

        public static Spell R = new Spell(SpellSlot.R, 1500); //wiki



        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;

        public RekSai()
        {
            Qb.SetSkillshot(0.5f, 60, 1950, true, SkillshotType.Line);
            Eb.SetSkillshot(0, 60, 1600, false, SkillshotType.Line);

            Orbwalker.Attach(Menu);


            var combomenu = new Menu("combo", "Combo")
            {
                new Menu("unborrowcombo", "Unburrowed")
                {
                    new MenuBool("unburrowedq", "Use Q"),
                    new MenuBool("unburrowedw", "Use W"),
                    new MenuBool("unburrowede", "Use E")
                },

                new Menu("burrowedcombo", "Burrowed")
                {
                    new MenuBool("burrowedq", "Use Q"),
                    new MenuBool("burrowedw", "Use W"),
                    new MenuBool("burrowede", "Use E")
                },
                new MenuBool("autow", "Auto (Un)Burrow in Combo"),
                new MenuBool("autor", "Auto R KS")
            };
            Menu.Add(combomenu);

            var junglemenu = new Menu("jungle", "Jungle Clear")
            {
                new Menu("unborrowcombo", "Unburrowed")
                {
                    new MenuBool("unburrowedq", "Use Q"),
                    new MenuBool("unburrowedw", "Use W"),
                    new MenuBool("unburrowede", "Use E")
                },

                new Menu("burrowedcombo", "Burrowed")
                {
                    new MenuBool("burrowedq", "Use Q"),
                    new MenuBool("burrowedw", "Use W"),
                    new MenuBool("burrowede", "Use E")
                },
                new MenuBool("autor", "Auto R")
            };
            Menu.Add(junglemenu);

            var miscmenu = new Menu("miscmenu", "Misc")
            {
                new MenuBool("burrownothing", "Auto Burrow When Idle"),
            };
            Menu.Add(miscmenu);

            var drawmenu = new Menu("DrawMenu", "Drawings")
            {
                new MenuBool("drawq", "Draw Q"),
                new MenuBool("draww", "Draw W"),
                new MenuBool("drawe", "Draw E"),
                new MenuBool("drawr", "Draw R"),
            };
            Menu.Add(drawmenu);

            Menu.Attach();

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.PostAttack += Orbwalker_OnPostAttack;
            Orbwalker.PreAttack += Orbwalker_OnPreAttack;


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
                case OrbwalkingMode.None:
                    BurrowNothing();
                    break;

                case OrbwalkingMode.Combo:
                    Combo();
                    break;

                case OrbwalkingMode.Laneclear:
                    LaneClear(); //TODO
                    JungleClear();
                    break;

                case OrbwalkingMode.Mixed:
                    Mixed();
                    break;
            }
        }

        //Burrow when idle/doing nothing
        private static void BurrowNothing()
        {
            if (IsBurrowed() || Player.IsRecalling() || Player.IsDead)
                return;
        }

        private static bool IsBurrowed()
        {
            return Player.HasBuff("ReksaiW");
        }

        private static void Render_OnPresent()
        {
            //Drawings
            if (Menu["DrawMenu"]["drawq"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Menu["DrawMenu"]["draww"].Enabled)
                Render.Circle(Player.Position, W.Range, 30, Color.White);

            if (Menu["DrawMenu"]["drawe"].Enabled)
                Render.Circle(Player.Position, E.Range, 30, Color.White);

            if (Menu["DrawMenu"]["drawr"].Enabled)
                Render.Circle(Player.Position, R.Range, 30, Color.White);
        }



        public static void Orbwalker_OnPostAttack(Object sender, PostAttackEventArgs args)
        {
    
        }

        public static void Orbwalker_OnPreAttack(object sender, PreAttackEventArgs args)
        {

        }

        private void Combo()
        {
            
            var reksaifury = Equals(Player.Mana, Player.MaxMana); //Determines if RekSai has max fury

            if (IsBurrowed())
            {
                //NOTE - he has used different vars for diff target selector variables, one for knockup, one for just burrowed q... not sure if necessary
                //E start combo
                var target = TargetSelector.GetTarget(Eb.Range + Wb.Range); //knockup range

                if (target == null)
                    return;

                if (target.IsInRange(Player.AttackRange))
                {
                    return;
                }

                if (Eb.Ready && target.IsValidTarget(Eb.Range + Wb.Range))
                {
                    var prediction = Eb.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.High)
                        Eb.Cast(prediction.CastPosition.Extend(Player.ServerPosition, -50));
                }

                if (Qb.Ready && target.IsValidTarget(Qb.Range))
                {
                    var prediction = Qb.GetPrediction(target);
                    if (prediction.HitChance <= HitChance.High)
                        Qb.Cast(prediction.UnitPosition);
                }

                //knockup
                if (W.Ready && target.IsValidTarget(Wb.Range) && !Qb.Ready && !target.HasBuffOfType(BuffType.Knockup))
                    W.CastOnUnit(Player);
            }

            if (!IsBurrowed())
            {
                var target = TargetSelector.GetTarget(Q.Range);

                if (target == null)
                    return;

                if (Q.Ready && target.IsValidTarget(Q.Range))
                    Q.CastOnUnit(Player); //Q.Cast(target);

                if (E.Ready && target.IsValidTarget(E.Range))
                {
                    if (reksaifury && !Player.HasBuff("RekSaiQ")) //?
                    {
                        E.Cast(target);
                    }
                    E.Cast(target);
                }

                //CHECK MENU FOR AUTO W USE IN COMBO
            
                    target = TargetSelector.GetTarget(Qb.Range);

                    if (target == null)
                        return;

                    if (!Q.Ready && target.IsValidTarget(E.Range) && target.IsValidTarget(Qb.Range) && !target.HasBuff("RekSaiKnockupImmune")) // && !Player.HasBuff("RekSaiQ")?
                    {
                        W.CastOnUnit(Player);
                    }
                
                    

            }

            /*   var target = TargetSelector.GetTarget(Qb.Range);

               if (target == null)
                   return;

               var prediction = Qb.GetPrediction(target);*/


        }

        private void JungleClear()
        {
            //Get jungle minions
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(Player.AttackRange) && x.IsEnemy && x.IsValidSpellTarget() && !x.UnitSkinName.ToLower().Contains("minion"));
            var target = minions.FirstOrDefault(x => x.IsInRange(Player.AttackRange));

            if (minions == null)
                return;
            foreach (var minion in minions)
            {
               
            }
        }

        private void Mixed()
        {
            var target = TargetSelector.GetTarget(Qb.Range);

            if (target == null)
                return;

            if (!IsBurrowed() && Qb.Ready && target.IsValidTarget(Qb.Range) && Wb.Ready)
            {
                Wb.CastOnUnit(Player);
            }

            if (IsBurrowed() && Qb.Ready && target.IsValidTarget(Qb.Range))
            {
                var prediction = Qb.GetPrediction(target);
                if (prediction.HitChance >= HitChance.High)
                {
                    Qb.Cast(prediction.UnitPosition);
                }
            }
                
        }

        private void LaneClear()
        {
            //TODO.
        }


    }

}