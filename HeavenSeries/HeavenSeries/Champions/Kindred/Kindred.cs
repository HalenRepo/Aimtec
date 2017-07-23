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

        public static Spell Q = new Spell(SpellSlot.Q, 340);
        public static Spell W = new Spell(SpellSlot.W, 500);
        public static Spell E = new Spell(SpellSlot.E, 500);
        public static Spell R = new Spell(SpellSlot.R, 500);

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;

        public Kindred()
        {
            Orbwalker.Attach(Menu);

            var JungleClear = new Menu("jungleclear", "Jungle Clear");
            {
                JungleClear.Add(new MenuBool("useq", "Use Q"));
                JungleClear.Add(new MenuBool("usew", "Use W"));
                JungleClear.Add(new MenuBool("usee", "Use E "));
                JungleClear.Add(new MenuBool("user", "Use R"));
            }
            Menu.Add(JungleClear);

            var ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuBool("usew", "Use W"));
                ComboMenu.Add(new MenuBool("usee", "Use E "));
                ComboMenu.Add(new MenuBool("user", "Use R"));
            }
            Menu.Add(ComboMenu);

            var UltMenu = new Menu("UltMenu", "R Settings");
            {
                UltMenu.Add(new MenuSliderBool("allyhealth", "Ally Health % to use R", true, 15, 1, 99, false));
                UltMenu.Add(new MenuSliderBool("enemies", "Minimum Enemies near Ally to use R", true, 2, 1, 5, false));
            }
            Menu.Add(UltMenu);

            var DrawMenu = new Menu("DrawMenu", "Drawings");
            {
                DrawMenu.Add(new MenuBool("drawq", "Draw Q"));
                DrawMenu.Add(new MenuBool("drawe", "Draw E "));
                DrawMenu.Add(new MenuBool("drawr", "Draw R"));
            }
            Menu.Add(DrawMenu);

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
                case OrbwalkingMode.Combo:
                    Combo();
                    break;

                case OrbwalkingMode.Laneclear:
                    LaneClear(); //TODO
                    JungleClear();
                    break;
            }


        }

        private static void Render_OnPresent()
        {
            //Drawings
            if (Menu["DrawMenu"]["drawq"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Menu["DrawMenu"]["drawr"].Enabled)
                Render.Circle(Player.Position, R.Range, 30, Color.White);

            if (Menu["DrawMenu"]["drawe"].Enabled)
                Render.Circle(Player.Position, E.Range, 30, Color.White);
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
            //Focus kindredcharge target
            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && x.IsValidTarget() && x.Distance(Player) < Player.AttackRange && x.IsVisible)) 
            {
                if (!target.HasBuff("kindredcharge") || target == null)
                    return;

                Orbwalker.ForceTarget(target);
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
                if (Q.Ready || target.HealthPercent() < 15)
                {
                    W.Cast();
                }
                    
            }

            if (E.Ready && target.IsValidTarget(E.Range))
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

            foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValid && x.IsAlly && x.IsVisible && x.Distance(Player) < R.Range &&
            x.CountEnemyHeroesInRange(R.Range) <= Menu["UltMenu"]["enemies"].Value && x.HealthPercent() < Menu["UltMenu"]["allyhealth"].Value))
            {
                if (!target.IsFacing(Obj))
                    return;
              
                if (target.Distance(Obj) > 550)
                    return;

                if (Menu["combo"]["user"].Enabled && Player.Distance(Obj) < R.Range)
                    R.Cast();
            }
        }

        private void JungleClear()
        {
            //Get jungle minions
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(Player.AttackRange) && x.IsEnemy && x.Name != "PlantSatchel" && x.Name != "PlantVision" && x.Name != "PlantHealth" && !x.UnitSkinName.ToLower().Contains("minion"));
            var target = minions.FirstOrDefault(x => x.IsInRange(Player.AttackRange));
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