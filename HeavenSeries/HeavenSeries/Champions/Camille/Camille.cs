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
using System.Collections.Generic;

namespace HeavenSeries
{
    internal partial class Camille
    {
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Q = new Spell(SpellSlot.Q, 875);
        public static Spell W = new Spell(SpellSlot.W, 725);
        public static Spell E = new Spell(SpellSlot.E, 800);
        public static Spell R = new Spell(SpellSlot.R, 2200); //wiki has it at 2750? meh.

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;

        public Camille()
        {
            Q.SetSkillshot(1f, 150f, float.MaxValue, false, SkillshotType.Circle);
            R.SetSkillshot(0.5f, 260f, 850f, false, SkillshotType.Line);

            //Menus();

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

                case OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case OrbwalkingMode.Laneclear:
                    LaneClear(); //TODO
                    JungleClear();
                    break;
            }
        }

        private static void Render_OnPresent()
        {
            /*if (Champions.Nami.MenuClass.drawmenu["drawq"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.Nami.MenuClass.drawmenu["draww"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.Nami.MenuClass.drawmenu["drawe"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.Nami.MenuClass.drawmenu["drawr"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);*/

        }
        public static void Orbwalker_OnPostAttack(Object sender, PostAttackEventArgs args)
        {
            //For post attack. If none, return.
            if (IOrbwalker.Mode == OrbwalkingMode.None)
                return;

        }

        public static void Orbwalker_OnPreAttack(object sender, PreAttackEventArgs args)
        {
            
        }

        private void Combo()
        {
            //E logic
            //find suitable ally for E that IS NOT NAMI
            


        }

        private void Harass()
        {
           
        }

        private void Game_RangeAttackOnCreate(GameObject sender)
        {
            //you should check for missiles here to E, but that's not in SDK yet?
            //Console.WriteLine("sender " + sender.ToString());
            return;
        }

   

        private void JungleClear()
        {

        }

        private void LaneClear()
        {
            //TODO.
        }


    }

}