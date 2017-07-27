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
    internal class Elise
    {
        public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Q = new Spell(SpellSlot.Q, 625);
        public static Spell W = new Spell(SpellSlot.W, 950);
        public static Spell E = new Spell(SpellSlot.E, 1100);

        public static Spell QS = new Spell(SpellSlot.Q, 475);
        public static Spell WS = new Spell(SpellSlot.W);
        public static Spell ES = new Spell(SpellSlot.E, 750);

        public static Spell R = new Spell(SpellSlot.R);

        private static bool HumanForm;
        private static bool SpiderForm;

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;

        public Elise()
        {

            W.SetSkillshot(0.25f, 100f, 1000, true, SkillshotType.Line);
            E.SetSkillshot(0.25f, 55f, 1600, true, SkillshotType.Line);

            Orbwalker.Attach(Menu);


            var combomenu = new Menu("combo", "Combo")
            {
                
                new Menu("humancombo", "Human")
                {
                    new MenuBool("humanq", "Use Q"),
                    new MenuBool("humanw", "Use W"),
                    new MenuBool("humane", "Use E")
                },

                new Menu("spidercombo", "Spider")
                {
                    new MenuBool("spiderq", "Use Q"),
                    new MenuBool("spiderw", "Use W"),
                    new MenuBool("spidere", "Use E"),
                },
                new MenuBool("autor", "Auto R")
            };
            Menu.Add(combomenu);

            var junglemenu = new Menu("jungle", "Jungle Clear")
            {
                new MenuBool("stayspider", "Stay as Spider when Mana below x%"),
                new Menu("humancombo", "Human")
                {
                    new MenuBool("humanq", "Use Q"),
                    new MenuBool("humanw", "Use W"),
                },

                new Menu("spidercombo", "Spider")
                {
                    new MenuBool("spiderq", "Use Q"),
                    new MenuBool("spiderw", "Use W"),
                },
                new MenuBool("autor", "Auto R")
            };
            Menu.Add(junglemenu);

            var drawmenu = new Menu("DrawMenu", "Drawings")
            {
                new MenuBool("drawq", "Draw Q", false),
                new MenuBool("draww", "Draw W", false),
                new MenuBool("drawe", "Draw E"),
                new MenuBool("drawPrediction", "Draw E Prediction")
        };
            Menu.Add(drawmenu);


            /*var ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuBool("usew", "Use W"));
                ComboMenu.Add(new MenuBool("usee", "Use E "));
                ComboMenu.Add(new MenuBool("user", "Use R"));
            }
            Menu.Add(ComboMenu);

            var JungleClear = new Menu("jungleclear", "Jungle Clear");
            {
                JungleClear.Add(new MenuBool("useq", "Use Q"));
                JungleClear.Add(new MenuBool("usew", "Use W"));
                JungleClear.Add(new MenuBool("usee", "Use E "));
            }
            Menu.Add(JungleClear);

            var UltMenu = new Menu("UltMenu", "R Settings");
            {
                UltMenu.Add(new MenuBool("autor", "Auto R"));
                UltMenu.Add(new MenuSlider("allyhealth", "Ally Health % to use R", 15, 1, 99, false));
                UltMenu.Add(new MenuSlider("enemies", "Minimum Enemies near Ally to use R", 2, 1, 5, false));
            }
            Menu.Add(UltMenu);

            var DrawMenu = new Menu("DrawMenu", "Drawings");
            {
                DrawMenu.Add(new MenuBool("drawq", "Draw Q"));
                DrawMenu.Add(new MenuBool("drawe", "Draw E "));
                DrawMenu.Add(new MenuBool("drawr", "Draw R"));
            }
            Menu.Add(DrawMenu);*/

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

            CheckForm();

            switch (Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;

                case OrbwalkingMode.Laneclear:
                    LaneClear(); //TODO
                    JungleClear();
                    break;
            }
        }

        private static void CheckForm()
        {
            if (Player.SpellBook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ")
            {
                HumanForm = true;
                SpiderForm = false;
            } else
            {
                HumanForm = false;
                SpiderForm = true;
            }
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
        }



        public static void Orbwalker_OnPostAttack(Object sender, PostAttackEventArgs args)
        {
            //For post attack. If none, return.
            if (IOrbwalker.Mode == OrbwalkingMode.None)
                return;

            //Spider W auto attack reset
            if (!WS.Ready)
                return;

            DelayAction.Queue(100 + Game.Ping, Orbwalker.ResetAutoAttackTimer);
        }

        public static void Orbwalker_OnPreAttack(object sender, PreAttackEventArgs args)
        {
            
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range);

            if (target == null)
                return;

            //Human
            if (HumanForm)
            {
                if (target.IsInRange(E.Range)) //DO MENU CHECKS HERE
                {
                    var prediction = E.GetPrediction(target);
                    //Draw prediction
                    if (Menu["DrawMenu"]["drawPrediction"].Enabled)
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



                    if (prediction.HitChance >= HitChance.High)
                        E.Cast(prediction.UnitPosition);
                }

                if (target.IsInRange(Q.Range))
                {
                    Q.Cast(target);
                }
                   
                
                if (target.IsInRange(W.Range))
                    W.Cast(target);

                //750 = Spider E range
                if (!Q.Ready && !W.Ready && !E.Ready && Player.Distance(target) <= 750)//CHECK USE R
                {
                    R.Cast();
                }

                if (!Q.Ready && !W.Ready && Player.Distance(target) <= 750)
                {
                    R.Cast();
                }
                    //R.Cast();
            }

            if (SpiderForm)
            {
                if (target.IsInRange(QS.Range)) //DO MENU CHECKS HERE
                    QS.Cast(target);

                if (target.IsInRange(Player.AttackRange))
                    WS.Cast(target);

                if (target.IsInRange(ES.Range) && Player.Distance(target) > QS.Range)
                    ES.Cast(target);

                if (Player.Distance(target) > QS.Range && !ES.Ready && R.Ready && Player.Distance(target) <= 1075) //CHECK USE R
                    R.Cast();

                if (!QS.Ready && Player.Distance(target) >= 125 && !ES.Ready && R.Ready && Player.Distance(target) <= 1075)
                    R.Cast();

                if (ES.Ready && Player.Distance(target) > QS.Range)
                    ES.Cast(target);
            }

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
                if (HumanForm)
                {
                    if (Q.Ready && minion.IsValidTarget() && minion.IsInRange(Q.Range))
                        Q.Cast(minion);

                    if (W.Ready && minion.IsValidTarget() && minion.IsInRange(W.Range))
                        W.Cast();

                    if (R.Ready && !Q.Ready && !W.Ready)
                        R.Cast();
                }

                if (!HumanForm)
                {
                    if (QS.Ready && minion.IsValidTarget() && minion.IsInRange(QS.Range))
                    {
                        QS.Cast(minion);
                    }
                        

                    if (WS.Ready && minion.IsValidTarget() && minion.IsInRange(Player.AttackRange))
                    {
                        Console.WriteLine(Game.ClockTime + " casting spider w");
                        WS.Cast();
                    }
                    if (Player.HasBuffOfType(BuffType.CombatEnchancer)) //maybe that checks for attack speed steroid?
                        {
                        Console.WriteLine("detected");
                        }

                    if (R.Ready && !QS.Ready && !WS.Ready)
                    {
                        //Console.WriteLine(Game.ClockTime + " | change!");
                        if (!WS.Ready && target.IsInRange(Player.AttackRange))
                        {
                            //maybe 3000 for 3 seconds from W buff
                            DelayAction.Queue(2500, () => R.Cast());
                        }
                        

                        R.Cast();

                    }
                        
                }
            }
        }

        private void LaneClear()
        {
            //TODO.
        }


    }

}