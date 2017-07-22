namespace HeavenSeries
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Collections.Generic;

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

        public static bool Cat = false;


        public Nidalee()
        {
            Orbwalker.Attach(Menu);

            var ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuSliderBool("comboW", "Use Human W (trap) to Combo with % Minimum Mana", true, 50, 0, 99));
            }
            Menu.Add(ComboMenu);
            var JungleMenu = new Menu("jungle", "Jungle");
            {
                JungleMenu.Add(new MenuSliderBool("jungleclearQ", "Use Cougar Q (spear) to Jungle with % Minimum Mana", true, 10, 0, 99));
                JungleMenu.Add(new MenuSliderBool("jungleclearW", "Use Human W (trap) to Jungle with % Minimum Mana", true, 10, 0, 99));
            }
            Menu.Add(JungleMenu);
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

            //Credits to Dycaste for isMelee suggestion
            Cat = ObjectManager.GetLocalPlayer().IsMelee;
            
            if (Player.HasBuff("Takedown"))
            {
                var target = TargetSelector.GetTarget(Javelin.Range);
                if (target != null && target.IsValidTarget(Takedown.Range))
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

                case OrbwalkingMode.Mixed:
                    Mixed();
                    break;

                case OrbwalkingMode.Laneclear:
                    Laneclear();
                    break;
            }

            //TODO: Killsteal();
        }

        private static void Render_OnPresent()
        {
            if (!Cat)
            {
                Render.Circle(Player.Position, Javelin.Range, 30, Color.White);
            }
           
        }

        //For hunted markings
        public static bool TargetHunted(Obj_AI_Base target)
        {
            return target.HasBuff("nidaleepassivehunted"); //For markings
        }


        private void Combo()
        {
            //Store boolean value of menu items

            //bool useQ = Menu["combo"]["useq"].Enabled;
            var target = TargetSelector.GetTarget(Javelin.Range);
            var prediction = Javelin.GetPrediction(target);
            var manaPercent = (int)((Player.Mana / Player.MaxMana) * 100);
            //Human Q logic
            if (target != null && !Cat && Javelin.Ready && target.IsValidTarget(Javelin.Range))
            {
               
                if (prediction.HitChance >= HitChance.High)
                {
                    Javelin.Cast(prediction.CastPosition);
                }

            }

            if (target != null && !Cat && Bushwack.Ready && Menu["combo"]["comboW"].Enabled && manaPercent >= Menu["combo"]["comboW"].Value)
            {
                Bushwack.Cast(target);
            }

            if (target != null && Pounce.Ready && (target.DistanceSqr(Player.Position)) > 200*200)
            {
                if (TargetHunted(target) & target.DistanceSqr(Player.Position) <= 750*750)
                {
                    if (Takedown.Ready)
                    {
                        //Then use takedown then pounce for best combo
                        Takedown.CastOnUnit(Player);

                    }
                    Pounce.Cast(target.ServerPosition);
                }
                else if (target.DistanceSqr(Player.Position) <= 400*400)
                {
                    if (Takedown.Ready)
                    {
                        Takedown.CastOnUnit(Player);
                    }
                    Pounce.Cast(target.ServerPosition);
                }
            }

            if (target != null && Cat && Swipe.Ready && target.Distance(Player.ServerPosition) <= Swipe.Range)
            {
                Swipe.Cast(target.ServerPosition);
            }


            //Switch to cat form
            if (target != null && Cat && target.IsValidTarget(Javelin.Range) && TargetHunted(target))
            {
                if (AspectofCougar.Ready)
                    AspectofCougar.Cast();
            }

            //If spear is likely to hit via prediction, then switch forms!

            if (prediction.HitChance >= HitChance.Medium && !Pounce.Ready)
            {
                if (AspectofCougar.Ready)
                    AspectofCougar.Cast();
            }

            //Switch to human for aa
            if (target != null && Cat && target.IsValidTarget(Javelin.Range) && !Takedown.Ready && !Pounce.Ready)
            {
                if (AspectofCougar.Ready)
                    AspectofCougar.Cast();
            }

        }

        private void Mixed()
        {
            var target = TargetSelector.GetTarget(Javelin.Range);
            //Human Q logic
            if (target != null && Javelin.Ready && target.IsValidTarget(Javelin.Range))
            {
                var prediction = Javelin.GetPrediction(target);
                if (prediction.HitChance >= HitChance.High)
                {
                    Javelin.Cast(prediction.CastPosition);
                }

            }
        }

        private void Laneclear()
        {
            //Get mana %
            var manaPercent = (int)((Player.Mana / Player.MaxMana) * 100);

            //All creeps and jungle minus plants
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsInRange(Javelin.Range) && x.IsEnemy && x.Name != "PlantSatchel" && x.Name != "PlantVision" && x.Name != "PlantHealth");
            var target = minions.FirstOrDefault(x => x.IsInRange(Player.AttackRange));

            //Switch to human for aa
            if (target != null && Cat && !Takedown.Ready && !Pounce.Ready && !Javelin.Ready && !Swipe.Ready)
            {
                
                if (AspectofCougar.Ready)
                {
                    AspectofCougar.Cast();
                }
                    
            }

            if (Menu["jungle"]["jungleclearQ"].Enabled && manaPercent >= Menu["jungle"]["jungleclearQ"].Value)
            {
                if (Javelin.Ready && target != null)
                {
                    Javelin.Cast(target);
                }


            } else
            {
                if (target != null && !Cat && target.IsValidTarget(Javelin.Range))
                {
                    if (AspectofCougar.Ready)
                        AspectofCougar.Cast();
                }
            }

            if (target != null && !Cat && Bushwack.Ready && Menu["jungle"]["jungleclearW"].Enabled && manaPercent >= Menu["jungle"]["jungleclearW"].Value)
            {
                Bushwack.Cast(target);
            }

            //Switch to cat form
            if (target != null && !Cat && target.IsValidTarget(Javelin.Range))
            {
                if (AspectofCougar.Ready)
                {
                    AspectofCougar.Cast();
                }
            }

            if (Cat)
            {
                if (Takedown.Ready && target != null)
                {
                    Takedown.CastOnUnit(Player);
                }

                if (Pounce.Ready && target != null)
                {
                    Pounce.Cast(target.ServerPosition);
                }

                if (Swipe.Ready && target != null)
                {
                    Swipe.Cast(target.ServerPosition);
                }
            }

            
        }
    }

}