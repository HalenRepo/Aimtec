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

    internal class Thresh
    {
        public static Menu Menu = new Menu("HeavenSeries", "HeavenSeries - " + Player.ChampionName, true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Q = new Spell(SpellSlot.Q, 1080);
        public static Spell W = new Spell(SpellSlot.W, 950);
        public static Spell E = new Spell(SpellSlot.E, 500);
        public static Spell R = new Spell(SpellSlot.R, 400);

        public static int elastattempt;
        //public static int elastattemptin;

        public Thresh()
        {
            Orbwalker.Attach(Menu);

            var ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuBool("usee", "Use E "));
                ComboMenu.Add(new MenuBool("user", "Use R"));
                //ComboMenu.Add(new MenuSliderBool("rAOE", "Minimum enemies for R", true, 1, 1, GameObjects.EnemyHeroes.Count()));
                //ComboMenu.Add(new MenuBool("userKS", "Use R to Killsteal"));

                //ComboMenu.Add(new MenuSeperator("sep1", "Use Q on: "));

                /*foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                    ComboMenu.Add(new MenuBool("useqon" + enemies.ChampionName.ToLower(), enemies.ChampionName));*/
            }
            Menu.Add(ComboMenu);
            var LanternMenu = new Menu("lantern", "Lantern");
            {
                foreach (var ally in GameObjects.AllyHeroes)
                {
                    LanternMenu.Add(new MenuBool(ally.ChampionName.ToLower(), "Use Lantern for: " + ally.ChampionName));
                }
            }
            Menu.Add(LanternMenu);
            Menu.Attach();

            Q.SetSkillshot(0.4f, 60f, 1400f, true, SkillshotType.Line);
            W.SetSkillshot(0.5f, 50f, 2200f, false, SkillshotType.Circle);

            

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

            
        }

        private static void Render_OnPresent()
        {
            //Basic Q range indicator
            Render.Circle(Player.Position, Q.Range, 30, Color.White);

            //Draw circle under Q target
            /* if (Q.Ready)
             {
                 Render.Circle(GetBestEnemyHeroTargetInRange(Q.Range).Position, 50, 30, Color.Blue);
             }*/
        }

        public static int lastbuff { get; set; }

        public static int lastq { get; set; }

        public static int eattempt { get; set; }

        private void Combo()
        {
            //Store boolean value of menu items
            bool useQ = Menu["combo"]["useq"].Enabled;
            bool useE = Menu["combo"]["usee"].Enabled;
            bool useR = Menu["combo"]["user"].Enabled;

            var target = TargetSelector.GetTarget(Q.Range);
            
            ///var rAOE = Menu["combo"]["rAOE"].As<MenuSliderBool>().Value;

            

            if (target == null)
            {
                return;
            }

            var prediction = Q.GetPrediction(target);

            //Q logic
            if (useQ && Q.Ready)
            {

                
                if (prediction.HitChance >= HitChance.Impossible)
                {
                   // Console.WriteLine(target.Name + " |GOOD| " + prediction.HitChance);
                    Q.Cast(prediction.UnitPosition);

                    //Q2 logic
                    if (target != null && target.HasBuff("threshQ"))
                    {
                        Q.CastOnUnit(Player); // Add a delay?

                    }

                } else
                {
                        //Console.WriteLine(target.Name + " |BAD| " + prediction.HitChance);
                }

                

            }

            if (E.Ready)
            checkE();

            if (W.Ready)
            {
                //checkW();
            }


            /*if (target != null && !target.HasBuff("threshQ") && E.Ready && target.IsValidTarget(E.Range))
            {
                E.Cast(target.Position.Extend(Player.ServerPosition, Vector3.Distance(target.Position, Player.Position) + 400));
            }*/

        }

        /*private void checkW()
        {
                if (Player.ManaPercent < Config.Item("manalant").GetValue<Slider>().Value)
                    return;
                // Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (!W.Ready) return;

            var bestAllies = GameObjects.AllyHeroes.Where(t => !t.IsMe && t.IsValidTarget(E.Range, true)).OrderBy(o => o.Health);
            foreach (var ally in bestAllies.Where(a => Menu["lantern"][a.ChampionName.ToLower()].As<MenuBool>().Enabled))
            {
            }

                foreach (var hero in ObjectManager.GetLocalPlayer.(950).Where(hero => !hero.IsDead &&
                                                                                hero.HealthPercent <=
                                                                                Config.Item("hpsettings" +
                                                                                            hero.ChampionName)
                                                                                    .GetValue<Slider>()
                                                                                    .Value
                                                                                && hero.Distance(Player) <= 900))
                    if (Config.Item("healop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (hero.Distance(Player) <= 900)
                        {
                            W.Cast(hero.Position);
                        }
                    }
        }*/

        private void checkE()
        {
           int elastattemptin = 0;
        //Console.WriteLine("elastattempt: " + elastattemptin);
            var target2 = TargetSelector.GetTarget(Q.Range);
            if (E.Ready)
            {
                Random rnd = new Random();
                int delay = rnd.Next(150, 300);
                if (Game.TickCount - elastattemptin < delay)
                {
                    return;
                }
            }
            
            if (E.Ready && target2 != null && target2.IsValidTarget(E.Range))
            {
                if (target2.HasBuff("threshQ"))
                {
                    return;
                }

                    E.Cast(target2.Position.Extend(Player.ServerPosition, Vector3.Distance(target2.Position, Player.Position) + 400));

                elastattemptin = Environment.TickCount;
            }
            
        }
    }

}