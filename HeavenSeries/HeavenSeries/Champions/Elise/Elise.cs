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
    internal partial class Elise
    {
        //public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
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

            Menus();
            MenuClass.combomenu.Add(new MenuSeperator("sepElise", "Use Human E (Cocoon) on: "));
            foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                MenuClass.combomenu.Add(new MenuBool("useeon" + enemies.ChampionName.ToLower(), enemies.ChampionName));

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
                if (target.IsInRange(E.Range) && MenuClass.combohumanmenu["humane"].Enabled && MenuClass.combomenu["useeon" + target.ChampionName.ToLower()].Enabled) //DO MENU CHECKS HERE
                {
                    var prediction = E.GetPrediction(target);
                    //Draw prediction
                    if (MenuClass.drawmenu["drawPrediction"].Enabled)
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

                if (target.IsInRange(Q.Range) && MenuClass.combohumanmenu["humanq"].Enabled)
                {
                    Q.Cast(target);
                }
                   
                
                if (target.IsInRange(W.Range) && MenuClass.combohumanmenu["humanw"].Enabled)
                    W.Cast(target);

                //750 = Spider E range
                if (!Q.Ready && !W.Ready && Player.Distance(target) <= 750 && MenuClass.combomenu["autor"].Enabled)//CHECK USE R
                {
                    R.Cast();
                }

                if (!Q.Ready && !W.Ready && Player.Distance(target) <= 750 && MenuClass.combomenu["autor"].Enabled)
                {
                    R.Cast();
                }
                   
            }

            if (SpiderForm)
            {
                if (target.IsInRange(QS.Range) && MenuClass.combospidermenu["spiderq"].Enabled) //DO MENU CHECKS HERE
                    QS.Cast(target);

                if (target.IsInRange(Player.AttackRange) && MenuClass.combospidermenu["spiderw"].Enabled)
                    WS.Cast(target);

                if (target.IsInRange(ES.Range) && Player.Distance(target) > QS.Range && MenuClass.combospidermenu["spidere"].Enabled)
                    ES.Cast(target);

                if (Player.Distance(target) > QS.Range && MenuClass.combomenu["autor"].Enabled && !ES.Ready && R.Ready && Player.Distance(target) <= 1075) //CHECK USE R
                    R.Cast();

                if (!QS.Ready && Player.Distance(target) >= 125 && MenuClass.combomenu["autor"].Enabled && !ES.Ready && R.Ready && Player.Distance(target) <= 1075)
                    R.Cast();

                if (ES.Ready && Player.Distance(target) > QS.Range && MenuClass.combospidermenu["spidere"].Enabled)
                    ES.Cast(target);

                if (MenuClass.combomenu["autor"].Enabled && !QS.Ready && WS.Ready)
                {
                    R.Cast();
                }

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
                    if (Q.Ready && MenuClass.junglehumanmenu["humanq"].Enabled && minion.IsValidTarget() && minion.IsInRange(Q.Range))
                    {
                        if (minion.Health <= Player.GetAutoAttackDamage(minion))
                        {
                            return;
                        }
                        Q.Cast(minion);
                    }
                        

                    if (W.Ready && MenuClass.junglehumanmenu["humanw"].Enabled && minion.IsValidTarget() && minion.IsInRange(W.Range))
                    {
                        if (minion.Health <= Player.GetAutoAttackDamage(minion) || minion.Health <= 25)
                        {
                            return;
                        }
                        W.Cast(minion);
                    }
                        

                    if (R.Ready && !Q.Ready && !W.Ready && MenuClass.junglemenu["autor"].Enabled)
                        R.Cast();
                }

                if (!HumanForm)
                {
                    if (QS.Ready && MenuClass.junglespidermenu["spiderq"].Enabled && minion.IsValidTarget() && minion.IsInRange(QS.Range))
                    {
                        QS.Cast(minion);
                    }
                        

                    if (WS.Ready && MenuClass.junglespidermenu["spiderw"].Enabled && minion.IsValidTarget() && minion.IsInRange(Player.AttackRange))
                    {
                        //Console.WriteLine(Game.ClockTime + " casting spider w");
                        WS.Cast();
                    }

                    if (R.Ready && !WS.Ready)
                    {
                        if (!WS.Ready && target.IsInRange(Player.AttackRange) && Player.HasBuffOfType(BuffType.Haste) && MenuClass.junglespidermenu["spiderw"].Enabled)
                        {
                            //Delay transform if W attack speed steriod was used
                            DelayAction.Queue(Game.Ping + 2500, () => R.Cast());
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