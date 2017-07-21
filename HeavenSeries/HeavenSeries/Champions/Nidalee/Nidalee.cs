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

        //Human
        public static Spell Javelin = new Spell(SpellSlot.Q, 1500f);
        public static Spell Bushwack = new Spell(SpellSlot.W, 900f);
        public static Spell Primalsurge = new Spell(SpellSlot.E, 650f);

        //Cat
        public static Spell Takedown = new Spell(SpellSlot.Q, 200f);
        public static Spell Pounce = new Spell(SpellSlot.W, 375f);
        public static Spell Swipe = new Spell(SpellSlot.E, 300f);

        //Transform
        public static Spell AspectofCougar = new Spell(SpellSlot.R);

        public static bool Cat;


        public Nidalee()
        {
            Orbwalker.Attach(Menu);

            var ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                
            }
            Menu.Add(ComboMenu);
            Menu.Attach();

            Javelin.SetSkillshot(0.50f, 70f, 1300f, true, SkillshotType.Line);
            Bushwack.SetSkillshot(0.50f, 100f, 1500f, false, SkillshotType.Circle);
            Swipe.SetSkillshot(0.50f, 375f, 1500f, false, SkillshotType.Cone);
            Pounce.SetSkillshot(0.50f, 400f, 1500f, false, SkillshotType.Cone);



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

            Cat = SpellSlot.Q.ToString() != "JavelinToss";
            if (Player.HasBuff("Takedown"))
            {
                var target = TargetSelector.GetTarget(Javelin.Range);
                if (target.IsValidTarget(Takedown.Range))
                {
                    if (Orbwalker.Mode == OrbwalkingMode.Combo && Cat)
                    {
                        Takedown.CastOnUnit(Player);
                    }
                }
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
            if (Cat)
            {
                Render.Circle(Player.Position, Javelin.Range, 30, Color.White);
            }
           
        }

        public static bool TargetHunted(Obj_AI_Base target)
        {
            return target.HasBuff("nidaleepassivehunted"); //For markings
        }


        private void Combo()
        {
            //Store boolean value of menu items

            bool useQ = Menu["combo"]["useq"].Enabled;
            var target = TargetSelector.GetTarget(Javelin.Range);

            if (Cat)
            {
                //Takedown logic
                if (target.IsValidTarget(Takedown.Range))
                {
                    //Check if takedown is ready on unit
                    if (Takedown.Ready && target.Distance(Player.ServerPosition) <= Takedown.Range + 150 * 150) //Takedown.RangeSqr not exist
                    {
                        Takedown.CastOnUnit(Player);
                    }
                }

                //Pounce logic
                if (Pounce.Ready && (target.Distance(Player.ServerPosition) <= 200*200))
                {
                    //Check to see if the unit Nidalee is pouncing on is "hunted"/marked, then use takedown if possible! 
                    //Single ampersand '&' BITWISE operator!
                    if (TargetHunted(target) & target.Distance(Player.ServerPosition) <= 750*750)
                    {
                        if (Takedown.Ready)
                        {
                            //Then use takedown then pounce for best combo
                            Takedown.CastOnUnit(Player);
                        }
                        Pounce.Cast(target.ServerPosition);
                    }
                    else if (target.Distance(Player.ServerPosition) <= 400*400)
                    {
                        if (Takedown.Ready)
                        {
                            Takedown.CastOnUnit(Player);
                        }
                        Pounce.Cast(target.ServerPosition);
                    }
                    
                }

                //Swipe logic
                if (Swipe.Ready)
                {
                    if (target.Distance(Player.ServerPosition) <= Swipe.Range) //Swipe.RangeSqr not exist
                    {
                        if (!Pounce.Ready) //NotLearned is another method we need to make. You'd assume IsReady() would account for this...
                            Swipe.Cast(target.ServerPosition);
                    }
                }
            }

            //Cougar to human check
            //force transform if q ready and no collision 
            //Maybe add a check here to see if user wants to auto transform?
            //Check if can change forms...
            if (!AspectofCougar.Ready)
            {
                return;
            }

            //OR Don't transform and stay cougar if target killable with combo
            //NEED TO IMPLEMENT COUGARDAMAGE() - A TOTAL AMOUNT OF DAMAGE IN COUGAR FORM
            /* if (target.Health <= CougarDamage(target) && target.Distance(Player.ServerPosition) <= Pounce.Range)
             {
                 return;
             }*/

            //If spear is likely to hit via prediction, then switch forms!
            var prediction = Javelin.GetPrediction(target);
            if (prediction.HitChance >= HitChance.Medium && !Pounce.Ready)
            {
                AspectofCougar.Cast();
            }

            //COUGAR to HUMAN if AA Killable and COUGAR skills on CD
            //Switch to human form if killable with aa and cougar skills not available
            /* if (!Pounce.Ready && !Swipe.Ready() && !Takedown.Ready)
             {
                 if (target.Distance(Player.ServerPosition) > Takedown.Range && target.Health <= CanKillAA(target)) //CanKillAA() method
                 {
                     //Maybe also add a check for menu. Does user want auto transform?
                     if (target.Distance(Player.ServerPosition, true) <= Math.Pow(Me.AttackRange + 50, 2))
                     {
                         if (Aspectofcougar.IsReady())
                             Aspectofcougar.Cast();
                     }
                 }
             }*/

            //Human Q logic
            if (!Cat && target.IsValidTarget(Javelin.Range))
            {
                var qTarget = TargetSelector.GetTarget(Javelin.Range, false);
                if (qTarget != null && Javelin.Ready) //if there is actually a target you can hit a spear on with no collision
                {
                    Javelin.Cast(qTarget); //Kurisu added a hitchance slider and checked to see if the chance was enough to make it worth casting
                }
            }

            //Q logic
            /* if (useQ && Javelin.Ready && Menu["combo"]["useqon" + target.ChampionName.ToLower()].Enabled && target.IsValidTarget(Javelin.Range))
             {
                 Javelin.Cast(target);
             }*/
        }
    }

}