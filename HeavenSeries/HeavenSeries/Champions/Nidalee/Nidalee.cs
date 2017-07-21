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

    internal class Nidalee
    {
        public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Javelin = new Spell(SpellSlot.Q, 1500f);
        public static Spell Bushwack = new Spell(SpellSlot.W, 900f);
        public static Spell Primalsurge = new Spell(SpellSlot.E, 650f);
        public static Spell Takedown = new Spell(SpellSlot.Q, 200f);
        public static Spell Pounce = new Spell(SpellSlot.W, 375f);
        public static Spell AspectofCougar = new Spell(SpellSlot.R);



        public Nidalee()
        {
            Orbwalker.Attach(Menu);

            var ComboMenu = new Menu("combo", "Combo");
            {
                //ComboMenu.Add(new MenuBool("useq", "Use Q"));
                
            }
            Menu.Add(ComboMenu);
            Menu.Attach();

            //Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.Line);



            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;

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
            }

            //TODO: Killsteal();
        }

        private static void Render_OnPresent()
        {
            //Render.Circle(Player.Position, Q.Range, 30, Color.White);
        }


        private void Combo()
        {
            //Store boolean value of menu items

            //bool useQ = Menu["combo"]["useq"].Enabled;

            //var rAOE = Menu["combo"]["rAOE"].As<MenuSliderBool>().Value;

           // var heroTarget = GetBestEnemyHeroTargetInRange(Q.Range); //Credits to Exory.

            //Q logic
           /* if (useQ && Q.Ready && Menu["combo"]["useqon" + heroTarget.ChampionName.ToLower()].Enabled && heroTarget.IsValidTarget(Q.Range))
            {
                Q.Cast(heroTarget);
            }

            //E logic - Avoid using E on already knocked up target
            if (useE && E.Ready && heroTarget.IsValidTarget(120) && !heroTarget.HasBuffOfType(BuffType.Knockup))
            {
                E.Cast();
            }

            //R logic - Use R for AOE
            if (useR && R.Ready && heroTarget.IsValidTarget(R.Range) && Player.CountEnemyHeroesInRange(R.Range) >= rAOE)
            {
                R.Cast();
            }*/
        }

        //Credits to Exory.
        public static Obj_AI_Hero GetBestEnemyHeroTargetInRange(float range)
        {
            var ts = TargetSelector.Implementation;
            var target = ts.GetTarget(range);
            /*if (target != null && target.IsValidTarget() && !Invulnerable.Check(target))
            {
                return target;
            }*/
            if (target != null && target.IsValidTarget())
            {
                return target;
            }

            var firstTarget = ts.GetOrderedTargets(range).FirstOrDefault(t => t.IsValidTarget()); //&& !Invulnerable.Check(t)
            if (firstTarget != null)
            {
                return firstTarget;
            }

            return null;
        }

    }

}