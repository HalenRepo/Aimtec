namespace HeavenSeries
{
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

    internal class Kindred
    {
        public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static int erangemodifier = 0;

        public static Spell Q = new Spell(SpellSlot.Q, 340);
        public static Spell W = new Spell(SpellSlot.W, 500);
        public static Spell E = new Spell(SpellSlot.E, 565); //E increases in range with passive now. //kindredmarkofthekindredstackcounter
        public static Spell R = new Spell(SpellSlot.R, 500);

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;
        

        public Kindred()
        {
            Orbwalker.Attach(Menu);

            var ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuBool("usew", "Use W"));
                ComboMenu.Add(new MenuBool("usee", "Use E "));
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
                //UltMenu.Add(new MenuSlider("allyhealth", "Ally Health % to use R", 15, 1, 99, false));
                UltMenu.Add(new MenuSlider("enemies", "Min. Enemies near Ally to use R", 2, 1, GameObjects.EnemyHeroes.Count(), false));

                UltMenu.Add(new MenuSeperator("sepKindred", "Auto R Settings"));

                foreach (Obj_AI_Hero allies in GameObjects.AllyHeroes)
                    UltMenu.Add(new MenuSliderBool("useron" + allies.ChampionName.ToLower(), "Save " + allies.ChampionName + " at %HP", true, 15, 1, 99));
            }
            Menu.Add(UltMenu);

            var DrawMenu = new Menu("DrawMenu", "Drawings");
            {
                DrawMenu.Add(new MenuBool("drawq", "Draw Q", false));
                DrawMenu.Add(new MenuBool("drawe", "Draw E", false));
                DrawMenu.Add(new MenuBool("drawr", "Draw R"));
            }
            Menu.Add(DrawMenu);

            Menu.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Render.OnPresent += Render_OnPresent;
            Orbwalker.PostAttack += Orbwalker_OnPostAttack;
            Orbwalker.PreAttack += Orbwalker_OnPreAttack;
            

            Console.WriteLine("HeavenSeries - " + Player.ChampionName + " loaded.");
        }

        private static readonly string[] Jungleminions =
        {
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab",
            "SRU_Baron", "SRU_Dragon_Air","SRU_Dragon_Fire","SRU_Dragon_Earth","SRU_Dragon_Water","SRU_Dragon_Elder", "SRU_RiftHerald", "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp"
        };

        private void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            //Count kindred stacks to adjust E range
            //kindredmarkofthekindredstackcounter
            int stackCount = Player.GetBuffCount("kindredmarkofthekindredstackcounter");

            if (stackCount < 4)
            {
                erangemodifier = 0;
            }
            else if (stackCount >= 4 && stackCount < 7)
            {
                erangemodifier = 75;
            }
            else if (stackCount >= 7 && stackCount < 10)
            {
                erangemodifier = 100;
            }
            else if (stackCount >= 10 && stackCount < 13)
            {
                erangemodifier = 125;
            }
            else if (stackCount >= 13 && stackCount < 16)
            {
                erangemodifier = 150;
            }
            else if (stackCount >= 16 && stackCount < 19)
            {
                erangemodifier = 175;
            }
            else if (stackCount >= 19 && stackCount < 22)
            {
                erangemodifier = 200;
            }
            else if (stackCount >= 22 && stackCount < 25)
            {
                erangemodifier = 250;
            }
            else if (stackCount > 25)
            {
                erangemodifier = 250;
            }

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

            if (Menu["UltMenu"]["autor"].Enabled && R.Ready)
                AutoR();
        }

        private static void Render_OnPresent()
        {
            //Drawings
            if (Menu["DrawMenu"]["drawq"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Menu["DrawMenu"]["drawr"].Enabled)
                Render.Circle(Player.Position, R.Range, 30, Color.White);

            if (Menu["DrawMenu"]["drawe"].Enabled)
                Render.Circle(Player.Position, E.Range + erangemodifier, 30, Color.White);
        }

        

        public static void Orbwalker_OnPostAttack(Object sender, PostAttackEventArgs args)
        {
            //For post attack. If none, return.
            if (IOrbwalker.Mode == OrbwalkingMode.None)
                return;

            var dashPosition = Player.Position.Extend(Game.CursorPos, 350);

            if (!Q.Ready) // || !target.IsValidTarget(Player.AttackRange
                return;

            //First Q logic - just Q to mouse
            Q.Cast(dashPosition);
            DelayAction.Queue(100 + Game.Ping, Orbwalker.ResetAutoAttackTimer);
        }

        public static void Orbwalker_OnPreAttack(object sender, PreAttackEventArgs args)
        {
            //kindredhittracker is the name of the target for kindred passive hunt!
            if (IOrbwalker.Mode == OrbwalkingMode.Laneclear)
            {
                var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(Player.AttackRange) && x.IsEnemy && x.IsValidSpellTarget() && !x.UnitSkinName.ToLower().Contains("minion"));
                if (minions == null)
                    return;

                foreach (var minion in minions)
                {
                    if (minion.HasBuff("kindredecharge")) //Focus E'd jungle camp
                    {
                        IOrbwalker.ForceTarget(minion);
                    }
                }
 
            }

            //Focus kindredcharge target
            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && x.IsValidTarget() && x.Distance(Player) < Player.AttackRange && x.IsVisible)) 
            {
                if (!target.HasBuff("kindredecharge") || target == null)
                    return;

                Orbwalker.ForceTarget(target);
            }
        }

        private static void AutoR()
        {
            var target = TargetSelector.GetTarget(W.Range);
            if (!R.Ready && target != null)
                return;

            foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValid && x.IsAlly && x.IsVisible && x.Distance(Player) < R.Range &&
            x.CountEnemyHeroesInRange(R.Range) <= Menu["UltMenu"]["enemies"].Value && Menu["UltMenu"]["useron" + x.ChampionName.ToLower()].Enabled && x.HealthPercent() < Menu["UltMenu"]["useron" + x.ChampionName.ToLower()].Value))
            {    

                if (target.Distance(Obj) > 550)
                    return;

                if (Player.Distance(Obj) < R.Range)
                    R.Cast();
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range);

            if (target == null)
                return;

            if (W.Ready)
            {
                //TODO GRAB CDR of Q. If Q = 4, RECAST W. Add to below with ||
                if (Q.Ready || target.HealthPercent() < 15 || Player.SpellBook.GetSpell(SpellSlot.Q).Cooldown == 4)
                {
                    if (Menu["combo"]["usew"].Enabled)
                    W.Cast();
                }
                    
            }

            if (E.Ready && target.IsValidTarget(E.Range + erangemodifier) && Menu["combo"]["usee"].Enabled)
                E.Cast(target);

            var dashPosition = Player.Position.Extend(Game.CursorPos, 320);
            
            //logic with auto attack resets
            if (Player.Distance(target) > Player.AttackRange && Player.Distance(target) < Q.Range)
            {
                Q.Cast(dashPosition);
                if (target.Distance(Player) < Player.AttackRange)
                {
                    Player.IssueOrder(OrderType.AutoAttack, target);
                }
                DelayAction.Queue(Game.Ping, IOrbwalker.ResetAutoAttackTimer);
            }

            if (!R.Ready)
                return;
        }

        private void JungleClear()
        {
            //Get jungle minions
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(Player.AttackRange) && x.IsEnemy && x.IsValidSpellTarget() && !x.UnitSkinName.ToLower().Contains("minion"));
            //var target = minions.FirstOrDefault(x => x.IsInRange(Player.AttackRange));
            var dashPosition = Player.Position.Extend(Game.CursorPos, Q.Range);
            if (minions == null)
                return;
            foreach (var minion in minions)
            {
                if (W.Ready && Menu["jungleclear"]["usew"].Enabled)
                    W.Cast();

                if (Q.Ready && Menu["jungleclear"]["useq"].Enabled)
                    Q.Cast(dashPosition);

                if (E.Ready && Menu["jungleclear"]["usee"].Enabled)
                    E.Cast(minion);
            }
        }

        private void LaneClear()
        {
            //TODO.
        }


    }

}