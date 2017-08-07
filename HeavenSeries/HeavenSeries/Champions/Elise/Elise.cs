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
            

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
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

            /*var buffs = ObjectManager.GetLocalPlayer().Buffs.Select(buff => buff.Name).ToList();
            Console.WriteLine(string.Join(Environment.NewLine, buffs));*/

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

        public void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (!sender.IsMe)
                return;
            //if changing from spider to human
            if (args.SpellData.Name.ToLower() == "eliserspider")
            {
                //if player still has attack speed steriod from spider W
                if (Player.HasBuff("EliseSpiderW"))
                {
                    Console.WriteLine("stop");
                    return;
                }
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
            if (HumanForm)
            {
                if (E.Ready && Champions.Elise.MenuClass.combohumanmenu["humane"].Enabled)
                {
                    foreach (Obj_AI_Hero target in GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsValidTarget(E.Range)))
                    {
                        var prediction = E.GetPrediction(target);
                        if (E.Ready && Champions.Elise.MenuClass.combomenuwhitelist["useeon" + target.ChampionName.ToLower()].Enabled && prediction.HitChance >= HitChance.High)
                        {
                            E.Cast(prediction.UnitPosition);
                        }
                    }
                }

                if (Q.Ready && Champions.Elise.MenuClass.combohumanmenu["humanq"].Enabled)
                {
                    foreach (Obj_AI_Hero target in GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsValidTarget(Q.Range)))
                    {
                        Q.Cast(target);
                    }
                }

                if (W.Ready && Champions.Elise.MenuClass.combohumanmenu["humanw"].Enabled)
                {
                    foreach (Obj_AI_Hero target in GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsValidTarget(Player.AttackRange)))
                    {
                        W.Cast(target);
                    }
                }

                if (!Q.Ready && !W.Ready && !E.Ready && R.Ready && Champions.Elise.MenuClass.combomenu["autor"].Enabled)
                {
                    R.Cast();
                }
            }

            if (SpiderForm)
            {
                if (WS.Ready && Champions.Elise.MenuClass.combospidermenu["spiderw"].Enabled)
                {
                    foreach (Obj_AI_Hero target in GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsValidTarget(QS.Range)))
                    {
                        WS.Cast();
                    }
                }

                if (QS.Ready && Champions.Elise.MenuClass.combospidermenu["spiderq"].Enabled)
                {
                    foreach (Obj_AI_Hero target in GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsValidTarget(QS.Range)))
                    {
                        QS.Cast(target);
                    }
                }

                if (ES.Ready && Champions.Elise.MenuClass.combospidermenu["spidere"].Enabled)
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range)))
                    {
                        if (Player.Distance(target) <= ES.Range && Player.Distance(target) > QS.Range &&
                        Champions.Elise.MenuClass.combospidermenu["spidere"].Enabled && ES.Ready)
                        {
                            E.Cast(target);
                        }
                        if (Player.Distance(target) <= ES.Range && Player.Distance(target) > QS.Range &&
                        Champions.Elise.MenuClass.combospidermenu["spidere"].Enabled && ES.Ready && Player.CountAllyHeroesInRange(E.Range) == 1 && target.HealthPercent() < 5)
                        {
                            E.Cast(target);
                        }
                    }
                }

                if (!QS.Ready && !WS.Ready && R.Ready && Champions.Elise.MenuClass.combomenu["autor"].Enabled)
                {
                    R.Cast();
                }
            }

        }

        private void JungleClear()
        {
            //Get jungle minions
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(Player.AttackRange) && x.IsEnemy && x.IsValidSpellTarget() && !x.UnitSkinName.ToLower().Contains("minion"));
            var target = minions.FirstOrDefault(x => x.IsInRange(Player.AttackRange + 100));
            
            if (minions == null)
                return;
            foreach (var minion in minions)
            {
                if (HumanForm)
                {
                    if (Q.Ready && Champions.Elise.MenuClass.junglehumanmenu["humanq"].Enabled && Champions.Elise.MenuClass.junglehumanmenu["humanq"].Value < Player.ManaPercent())
                    {
                        Q.Cast(minion);
                    }

                    if (W.Ready && Champions.Elise.MenuClass.junglehumanmenu["humanw"].Enabled && Champions.Elise.MenuClass.junglehumanmenu["humanw"].Value < Player.ManaPercent())
                    {
                        W.Cast(minion);
                    }

                    //If q isn't ready, out of mana for q, or q is not enabled in menu for jungle AND if w isn't ready, out of mana for w, or w is not enabled in menu for jungle = SWITCH
                    if (Champions.Elise.MenuClass.junglemenu["autor"].Enabled && (!Q.Ready || Champions.Elise.MenuClass.junglehumanmenu["humanq"].Value >= Player.ManaPercent() || !Champions.Elise.MenuClass.junglehumanmenu["humanq"].Enabled) && (!W.Ready || Champions.Elise.MenuClass.junglehumanmenu["humanw"].Value >= Player.ManaPercent() || !Champions.Elise.MenuClass.junglehumanmenu["humanw"].Enabled) )
                    {
                        R.Cast();
                    }
                }

                if (SpiderForm)
                {
                    if (WS.Ready && Champions.Elise.MenuClass.junglespidermenu["spiderw"].Enabled)
                    {
                        WS.Cast();
                    }

                    if (Q.Ready && Champions.Elise.MenuClass.junglespidermenu["spiderq"].Enabled)
                    {
                        QS.Cast(minion);
                    }

                    //if q isn't ready, or not enabled in menu AND if w isn't ready, or not enabled in menu THEN switch
                    if (Champions.Elise.MenuClass.junglemenu["autor"].Enabled && (!QS.Ready || !Champions.Elise.MenuClass.junglespidermenu["spiderq"].Enabled) && (!WS.Ready || !Champions.Elise.MenuClass.junglespidermenu["spiderw"].Enabled))
                    {
                        R.Cast();
                    }
                }

                if (Champions.Elise.MenuClass.junglemenu["junglesteal"].Enabled)
                {
                    if (minion.UnitSkinName.Contains("SRU_Dragon") || minion.UnitSkinName.Contains("SRU_Baron") || minion.UnitSkinName.Contains("SRU_Red") 
                        || minion.UnitSkinName.Contains("SRU_Blue"))
                    {
                        if (SpiderForm)
                        {
                            if (QS.Ready && Player.GetSpellDamage(minion, SpellSlot.Q) >= minion.Health)
                            {
                                QS.Cast(minion);
                            }
                        }

                        if (HumanForm)
                        {
                            if (Q.Ready && Player.GetSpellDamage(minion, SpellSlot.Q) >= minion.Health)
                            {
                                Q.Cast(minion);
                            }
                        }
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