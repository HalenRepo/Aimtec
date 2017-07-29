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
    internal partial class Nidalee
    {
        //public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        //cat
        public static Spell catQ = new Spell(SpellSlot.Q, 200); //takedown
        public static Spell catW = new Spell(SpellSlot.W, 375); //pounce
        public static Spell catE = new Spell(SpellSlot.E, 275); //swipe

        //human
        public static Spell Q = new Spell(SpellSlot.Q, 1500); //javelin
        public static Spell W = new Spell(SpellSlot.W, 900); //bushwack trap
        public static Spell E = new Spell(SpellSlot.E, 650); //primalsurge heal

        public static Spell R = new Spell(SpellSlot.R);

        private static bool cougarForm;

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;

        public Nidalee()
        {
            Q.SetSkillshot(0.125f, 40f, 1300f, true, SkillshotType.Line); //spear
            W.SetSkillshot(0.50f, 100f, 1500f, false, SkillshotType.Circle); //bushwack trap
            catE.SetSkillshot(0.50f, 375f, 1500f, false, SkillshotType.Cone); //swipe
            catW.SetSkillshot(0.50f, 400f, 1500f, false, SkillshotType.Cone); //pounce

            Menus();

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            SpellBook.OnCastSpell += OnCastSpell;
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

            var target = TargetSelector.GetTarget(1200); //1200?

            TrackCooldowns();
            PrimalSurge();

            


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

        private static float CalculateCd(float time)
        {
            return time + (time * Player.PercentCooldownMod);
        }

        private static void GetCooldowns(SpellBookCastSpellEventArgs args)
        {
            if (cougarForm)
            {
                if (args.Slot == SpellSlot.Q)
                    CQtimer = Game.ClockTime + CalculateCd(5);
                if (args.Slot == SpellSlot.W)
                    CWtimer = Game.ClockTime + CalculateCd(5);
                if (args.Slot == SpellSlot.E)
                    CEtimer = Game.ClockTime + CalculateCd(5);
            }
            else
            {
                if (args.Slot == SpellSlot.Q)
                    HQtimer = Game.ClockTime + CalculateCd(HumanQcd[Player.SpellBook.GetSpell(SpellSlot.Q).Level - 1]);
                if (args.Slot == SpellSlot.W)
                    HWtimer = Game.ClockTime + CalculateCd(HumanWcd[Player.SpellBook.GetSpell(SpellSlot.W).Level - 1]);
                if (args.Slot == SpellSlot.E)
                    HEtimer = Game.ClockTime + CalculateCd(HumanEcd[Player.SpellBook.GetSpell(SpellSlot.E).Level - 1]);
            }
        }

        public void OnCastSpell(Obj_AI_Base sender, SpellBookCastSpellEventArgs args)
        {
            if (sender.IsMe)
                GetCooldowns(args);
        }

        private static readonly float[] HumanQcd = { 6, 6, 6, 6, 6 };
        private static readonly float[] HumanWcd = { 13, 12, 11, 10, 9 };
        private static readonly float[] HumanEcd = { 12, 12, 12, 12, 12 };

        private static float CQtimer, CWtimer, CEtimer;
        private static float HQtimer, HWtimer, HEtimer;
        private static float CQ, CW, CE;
        private static float HQ, HW, HE;

        //keep track of cooldons
        private static void TrackCooldowns()
        {
            if (Player.IsDead)
                return;

            CQ = ((CQtimer - Game.ClockTime) > 0) ? (CQtimer - Game.ClockTime) : 0;
            CW = ((CWtimer - Game.ClockTime) > 0) ? (CWtimer - Game.ClockTime) : 0;
            CE = ((CEtimer - Game.ClockTime) > 0) ? (CEtimer - Game.ClockTime) : 0;
            HQ = ((HQtimer - Game.ClockTime) > 0) ? (HQtimer - Game.ClockTime) : 0;
            HW = ((HWtimer - Game.ClockTime) > 0) ? (HWtimer - Game.ClockTime) : 0;
            HE = ((HEtimer - Game.ClockTime) > 0) ? (HEtimer - Game.ClockTime) : 0;
        }

        private static readonly string[] Jungleminions =
        {
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab",
            "SRU_Baron", "SRU_Dragon_Air","SRU_Dragon_Fire","SRU_Dragon_Earth","SRU_Dragon_Water","SRU_Dragon_Elder", "SRU_RiftHerald", "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp"
        };

        private static bool CheckForm()
        {
            if (Player.SpellBook.GetSpell(SpellSlot.Q).Name.ToLower().Contains("javelin"))
            {
                cougarForm = false;
            }
            else
            {
                cougarForm = true;
            }
            return cougarForm;
        }

        private static void PrimalSurge()
        {
            //maybe add heal menu check here
            if ((HE != 0 || !E.Ready) || Player.IsRecalling())
            {
                return;
            }
            //foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsInRange(W.Range) && !x.IsMe &&x.IsAlly && !x.IsDead && Menu["healallies"][x.ChampionName.ToLower()].Enabled && x.HealthPercent() < Menu["healallies"][x.ChampionName.ToLower()].Value && !x.IsRecalling()))
            var target = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsInRange(W.Range + 100) && x.IsAlly && !x.IsDead && !x.IsRecalling()).OrderByDescending(xe => xe.FlatPhysicalDamageMod).FirstOrDefault();
            if (target == null)
            {
                return;
            }
                

            //added force heal if you have blue regardless of mana -- TODO Add champ whitelist check for heal
            if (!cougarForm && (Player.ManaPercent() >= Champions.Nidalee.MenuClass.miscmenu["healmanareq"].Value || Player.HasBuff("crestoftheancientgolem")) && target.HealthPercent() <= Champions.Nidalee.MenuClass.miscmenu["healhealthreq"].Value)
            {
                E.CastOnUnit(target);
            }
        }


        private static bool TargetHunted(Obj_AI_Base target)
        {
            return target.HasBuff("nidaleepassivehunted"); //marked
        }

        private static void Render_OnPresent()
        {
            if (Champions.Nidalee.MenuClass.drawmenu["drawq"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.Nidalee.MenuClass.drawmenu["draww"].Enabled)
                Render.Circle(Player.Position, W.Range, 30, Color.White);

            if (Champions.Nidalee.MenuClass.drawmenu["drawe"].Enabled)
                Render.Circle(Player.Position, E.Range, 30, Color.White);
        }



        public static void Orbwalker_OnPostAttack(Object sender, PostAttackEventArgs args)
        {
            
        }

        public static void Orbwalker_OnPreAttack(Object sender, PreAttackEventArgs args)
        {

        }

        private static bool CanKillAA(Obj_AI_Base target)
        {
            var damage = 0d;
            if (target.IsValidTarget(Player.AttackRange + 30))
                damage = Player.GetAutoAttackDamage(target);

            return target.Health <= (float)damage * 5;
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(1200); //1200?

            if (target == null)
                return;

            #region COUGAR COMBO
            //COUGAR COMBO
            if (cougarForm && target.IsValidTarget(Q.Range))
            {
                //Check if Cougar Q ready
                if (CQ == 0 && Champions.Nidalee.MenuClass.combocatmenu["useq"].Enabled && target.IsInRange(catQ.Range))
                {
                    catQ.CastOnUnit(Player);
                }

                //Check if pounce is ready
                if((CW == 0 || catW.Ready) && Champions.Nidalee.MenuClass.combocatmenu["usew"].Enabled && (target.Distance(Player.ServerPosition) <= 750 || CougarDamage(target) >= target.Health))
                {
                    //Check if pounce target is marked
                    if (TargetHunted(target) & target.Distance(Player.ServerPosition) <= 750*750)
                    {
                        catW.Cast(target.ServerPosition);
                    }
                    else if (target.Distance(Player.ServerPosition) <= 400*400)
                    {
                        catW.Cast(target.ServerPosition);
                    }
                }

                //Check if swipe is ready
                if ((CE == 0 || catE.Ready) && Champions.Nidalee.MenuClass.combocatmenu["usee"].Enabled)
                {
                    if (target.IsInRange(catE.Range))
                    {
                        if (!catW.Ready || Player.SpellBook.CanUseSpell(catW.Slot) == false) //i.e. not learned
                        {
                            catE.Cast(target.ServerPosition);
                        }
                    }
                }

                //FORCE TRANSFORM if Q ready + no collision
                if ((HQ == 0 && Champions.Nidalee.MenuClass.combomenu["user"].Enabled))
                {

                    if (!R.Ready)
                        return;

                    //or stay cougar if we can kill with spells
                    if (target.Health <= CougarDamage(target) && target.IsInRange(catW.Range))
                    {
                        return;
                    }

                    var prediction = Q.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        R.Cast();
                    }
                }

                //Switch to human form if can kill in aa && cougar skills not available
                if ((CW != 0 || !catW.Ready) && (CE != 0 || !catE.Ready) && (CQ != 0 || !catQ.Ready))
                {
                    if (target.Distance(Player) > catQ.Range && CanKillAA(target))
                    {
                        if (Champions.Nidalee.MenuClass.combomenu["user"].Enabled && target.Distance(Player) <= Math.Pow(Player.AttackRange + 50, 2))
                        {
                            if (R.Ready)
                                R.Cast();
                        }
                    }
                }
            }
            #endregion

            #region Human combo
            //HUMAN COMBO

            if (!cougarForm && target.IsValidTarget(Q.Range))
            {
                var qtarget = TargetSelector.GetTarget(Q.Range);
                if ((HQ == 0 || Q.Ready) && Champions.Nidalee.MenuClass.combohumanmenu["useq"].Enabled)
                {
                    var prediction = Q.GetPrediction(qtarget);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        Q.Cast(prediction.UnitPosition);
                    }
                }
            }


            if (!cougarForm && target.IsValidTarget(Q.Range))
            {
                //Switch to cat if target marked or can kill
                if (R.Ready && Champions.Nidalee.MenuClass.combomenu["user"].Enabled && (TargetHunted(target) || target.Health <= CougarDamage(target) && !Q.Ready))
                {
                    if ((CW == 0 || catW.Ready) && (CQ == 0 || CE == 0))
                    {
                        if (TargetHunted(target) && target.Distance(Player.ServerPosition) <= 750*750)
                        {
                            R.Cast();
                        }

                        if (target.Health <= CougarDamage(target) && target.Distance(Player.ServerPosition) <= 350*350)
                        {
                            R.Cast();
                        }
                    } 
                }
                //Check bushwack and cast
                if ((HW == 0 || W.Ready) && Champions.Nidalee.MenuClass.combohumanmenu["usew"].Enabled && target.IsInRange(W.Range))
                {
                    var prediction = W.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        W.Cast(prediction.CastPosition);
                    }
                }
            }
            #endregion



        }

        private static float CougarDamage(Obj_AI_Base target)
        {
            var damage = 0d;
            if (catQ.Ready)
                damage += Player.GetSpellDamage(target, SpellSlot.Q);

            if (catW.Ready)
                damage += Player.GetSpellDamage(target, SpellSlot.W);

            if (catE.Ready)
                damage += Player.GetSpellDamage(target, SpellSlot.E);

            return (float) damage;
        }

        private void JungleClear()
        {
            //different logic for small parts of the camp compared to big
            var small = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.Name.Contains("Mini") && !x.Name.StartsWith("Minion") && x.IsValidTarget(700));

            var big = ObjectManager.Get<Obj_AI_Minion>()
                .FirstOrDefault(x => !x.Name.Contains("Mini") && !x.Name.StartsWith("Minion") && Jungleminions.Any(name => x.Name.StartsWith(name)) && x.IsValidTarget(900));

            var m = big ?? small;

            if (m == null)
            {
                return;
            }

            if (cougarForm)
            {
                if (m.IsInRange(catE.Range) && CE == 0)
                {
                    if (Champions.Nidalee.MenuClass.junglecatmenu["usee"].Enabled && (!catW.Ready || Player.SpellBook.CanUseSpell(catW.Slot) == false)) //i.e. not learned
                    {
                        catE.Cast(m.ServerPosition);
                    }
                }

                if (TargetHunted(m) & m.Distance(Player.ServerPosition) <= 750*750 && (CW == 0 || catW.Ready))
                {
                    if (Champions.Nidalee.MenuClass.junglecatmenu["usew"].Enabled)
                    {
                        catW.Cast(m.ServerPosition);
                    }
                        
                }
                else if (m.Distance(Player.ServerPosition) <= 400*400 && (CW == 0 || W.Ready))
                {
                    if (Champions.Nidalee.MenuClass.junglecatmenu["usew"].Enabled)
                    {
                        catW.Cast(m.ServerPosition);
                    }
                        
                }

                if (m.IsInRange(catQ.Range) && CQ == 0)
                {
                    if (Champions.Nidalee.MenuClass.junglecatmenu["useq"].Enabled)
                        catQ.CastOnUnit(Player);
                }

                if ((CW != 0 || !catW.Ready || Player.SpellBook.CanUseSpell(catW.Slot) == false) &&
                    (CQ != 0 || Player.SpellBook.CanUseSpell(catQ.Slot) == false) && (CE != 0 || Player.SpellBook.CanUseSpell(E.Slot) == false))
                {
                    if ((HQ == 0 || HE == 0 && Player.HealthPercent() <= Champions.Nidalee.MenuClass.miscmenu["healhealthreq"].Value && 
                        Champions.Nidalee.MenuClass.junglehumanmenu["usee"].Enabled) && R.Ready && Champions.Nidalee.MenuClass.junglemenu["user"].Enabled)
                    {
                        if (Player.ManaPercent() >= Champions.Nidalee.MenuClass.miscmenu["healmanareq"].Value)
                        {
                            R.Cast();
                        }
                    }
                }
            }
            // NOT COUGAR FORM
            else
            {
                if (/*CHECK FOR MANA REQ FOR JUNGLE Q*/ HQ == 0 || Player.HasBuff("crestoftheancientgolem") && HQ == 0)
                {
                    if (Champions.Nidalee.MenuClass.junglehumanmenu["useq"].Enabled)
                    {
                        var prediction = Q.GetPrediction(m);
                        if (prediction.HitChance >= HitChance.Low)
                            Q.Cast(m.ServerPosition);
                    }
                }

                if (m.IsInRange(W.Range))
                {
                    if (/*CHECK FOR MANA REQ FOR JUNGLE W*/
                         HW == 0 || Player.HasBuff("crestoftheancientgolem") && HQ == 0)
                    {
                        if (Champions.Nidalee.MenuClass.junglehumanmenu["usew"].Enabled)
                            W.Cast(m.ServerPosition);
                    }
                }

                if (Champions.Nidalee.MenuClass.junglemenu["user"].Enabled && R.Ready)
                {
                    var poutput = Q.GetPrediction(m);
                    if ((HQ != 0 || poutput.HitChance == HitChance.Collision) || Player.HasBuff("crestoftheancientgolem") && HQ == 0 /*|| CHECK FOR MANA REQ FOR JUNGLE Q*/ )
                    {
                        if (CQ == 0 && CE == 0 && (CW == 0 || catW.Ready))
                        {
                            if (TargetHunted(m) & m.Distance(Player.ServerPosition) <= 750 * 750)
                                R.Cast();
                            else if (m.Distance(Player.ServerPosition) <= 450 * 450)
                                R.Cast();
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