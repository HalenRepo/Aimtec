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
using Aimtec.SDK.Util.ThirdParty;

namespace HeavenSeries
{
    internal partial class Rumble
    {
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        public static Spell Q = new Spell(SpellSlot.Q, 575);
        public static Spell W = new Spell(SpellSlot.W);
        public static Spell E = new Spell(SpellSlot.E, 865);
        public static Spell R = new Spell(SpellSlot.R, 1700);

        public static IOrbwalker IOrbwalker = Orbwalker.Implementation;



        public Rumble()
        {
            E.SetSkillshot(0.4f, 90, 2000, true, SkillshotType.Line);
            R.SetSkillshot(0.4f, 200, 4800, false, SkillshotType.Line);

            Menus();


            Game.OnUpdate += Game_OnUpdate;
            Render.OnPresent += Render_OnPresent;
            Orbwalker.PostAttack += Orbwalker_OnPostAttack;
            Orbwalker.PreAttack += Orbwalker_OnPreAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;

            Console.WriteLine("HeavenSeries - " + Player.ChampionName + " loaded.");
        }

        private void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            if (Champions.Rumble.MenuClass.heatmenu["autoheat"].Enabled)
            {
                Heat();
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

                case OrbwalkingMode.Lasthit:
                    LastHit();
                    break;
            }
        }

        private void KillSteal(Obj_AI_Hero target)
        {

        }

        private void Heat()
        {
            if (Player.Mana < Champions.Rumble.MenuClass.heatmenu["autoheat"].Value && !Player.IsRecalling())
            {
                if (Champions.Rumble.MenuClass.heatspellsmenu["heatq"].Enabled && Q.Ready)
                    Q.Cast();

                if (Champions.Rumble.MenuClass.heatspellsmenu["heatw"].Enabled && W.Ready)
                    W.Cast();
            }
        }

        private static void Render_OnPresent()
        {
            if (Champions.Rumble.MenuClass.drawmenu["drawq"].Enabled)
                Render.Circle(Player.Position, Q.Range, 30, Color.White);

            if (Champions.Rumble.MenuClass.drawmenu["drawe"].Enabled)
                Render.Circle(Player.Position, E.Range, 30, Color.White);

            if (Champions.Rumble.MenuClass.drawmenu["drawr"].Enabled)
                Render.Circle(Player.Position, R.Range, 30, Color.White);

        }
        public static void Orbwalker_OnPostAttack(Object sender, PostAttackEventArgs args)
        {
            //For post attack. If none, return.
            if (IOrbwalker.Mode == OrbwalkingMode.None)
                return;

        }

        public static void Orbwalker_OnPreAttack(object sender, PreAttackEventArgs args)
        {
            if (IOrbwalker.Mode == OrbwalkingMode.None)
                return;
        }

        public void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
        }


        private void Combo()
        {
            var skillstarget = TargetSelector.GetTarget(E.Range);

            //R logic
            if (Champions.Rumble.MenuClass.combormenu["user"].Enabled && R.Ready)
            {
                var heroes = GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsValidSpellTarget(R.Range));
                var heroPositions = heroes as Obj_AI_Hero[] ?? heroes.ToArray();
                var positions = heroPositions.Select(x => x.ServerPosition).ToList();

                var locations = new List<Vector3>();

                UltResult maxhit = null;

                locations.AddRange(positions);

                var max = positions.Count;

                for (var i = 0; i < max; i++)
                {
                    for (var j = 0; j < max; j++)
                    {
                        if (positions[j] != positions[i])
                        {
                            locations.Add((positions[j] + positions[i]) / 5);
                        }
                    }
                }

                var results = new HashSet<UltResult>();

                //Go through all targets within R range + length of rumble ult
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidSpellTarget(R.Range) && !x.IsDead && Champions.Rumble.MenuClass.comboronmenu[x.ChampionName.ToLower()].Enabled))
                {
                    foreach (var p in locations)
                    {
                        var rect = new Rectangle(target.Position, p, R.Width);

                        var count = heroPositions.Count(m => rect.Contains(m.Position));

                        results.Add(new UltResult(count, p, target));
                    }
                }
                //best endpoint
                maxhit = results.MaxBy(x => x.NumberOfChampionsHit);

                //Then ult
                if (maxhit != null && maxhit.GetTarget != null && maxhit.NumberOfChampionsHit >= Champions.Rumble.MenuClass.combormenu["minenemies"].Value - 1) //-1 because initial target.
                {
                    /*Render.Circle(maxhit.CastPosition, 30, 30, Color.Blue);
                    Render.Circle(maxhit.GetTarget.Position, 30, 30, Color.Red);
                    Render.Line(maxhit.CastPosition.ToScreenPosition(), maxhit.GetTarget.ServerPosition.ToScreenPosition(), Color.White);*/
                    R.Cast(maxhit.GetTarget.Position, maxhit.CastPosition);
                }
            }

            //Q logic
            if (Champions.Rumble.MenuClass.comboqmenu["useq"].Enabled && Q.Ready && skillstarget.IsValidTarget(Q.Range) /*&& Player.IsFacing(skillstarget)*/ && Player.Mana < 80)
            {
                Q.Cast();
            }

            //E logic
            var prediction = E.GetPrediction(skillstarget);
            if (Champions.Rumble.MenuClass.comboemenu["usee"].Enabled && E.Ready && skillstarget.IsValidTarget(E.Range) && (Player.Mana < 80))
            {
                if (prediction.HitChance >= HitChance.High)
                {
                    E.Cast(prediction.UnitPosition);
                }
            }

            //W logic
            if (Champions.Rumble.MenuClass.combowmenu["usew"].Enabled && W.Ready && Player.CountEnemyHeroesInRange(1000) > Champions.Rumble.MenuClass.combowmenu["enemies"].Value && Player.HealthPercent() <= Champions.Rumble.MenuClass.combowmenu["health"].Value)
                W.Cast();
        }

        //Credits to @eox, and possibly others.
        #region polygons
        /*Polygon*/
        public abstract class Polygon
        {
            public List<Vector3> Points = new List<Vector3>();

            public List<IntPoint> ClipperPoints
            {
                get
                {
                    return Points.Select(p => new IntPoint(p.X, p.Z)).ToList();
                }
            }

            public bool Contains(Vector3 point)
            {
                var p = new IntPoint(point.X, point.Z);
                var inpolygon = Clipper.PointInPolygon(p, ClipperPoints);
                return inpolygon == 1;
            }
        }
        public class Rectangle : Polygon
        {
            public Rectangle(Vector3 startPosition, Vector3 endPosition, float width)
            {
                var direction = (startPosition - endPosition).Normalized();
                var perpendicular = Perpendicular(direction);

                var leftBottom = startPosition + width * perpendicular;
                var leftTop = startPosition - width * perpendicular;

                var rightBottom = endPosition - width * perpendicular;
                var rightLeft = endPosition + width * perpendicular;

                Points.Add(leftBottom);
                Points.Add(leftTop);
                Points.Add(rightBottom);
                Points.Add(rightLeft);
            }

            public Vector3 Perpendicular(Vector3 v)
            {
                return new Vector3(-v.Z, v.Y, v.X);
            }
        }

        //R location
        //added target to preserve value
        public class UltResult
        {
            public UltResult(int hit, Vector3 cp, Obj_AI_Hero target)
            {
                NumberOfChampionsHit = hit;
                CastPosition = cp;
                GetTarget = target;
            }

            public int NumberOfChampionsHit;
            public Vector3 CastPosition;
            public Obj_AI_Hero GetTarget;
        } 
        #endregion

        private void Harass()
        {
            var skillstarget = TargetSelector.GetTarget(E.Range);

            //Q logic
            if (Champions.Rumble.MenuClass.harassqmenu["useq"].Enabled && Q.Ready && skillstarget.IsValidTarget(Q.Range) /*&& Player.IsFacing(skillstarget)*/ && Player.Mana < 80)
            {
                Q.Cast();
            }

            //E logic
            var prediction = E.GetPrediction(skillstarget);
            if (Champions.Rumble.MenuClass.harassemenu["usee"].Enabled && E.Ready && skillstarget.IsValidTarget(E.Range) && (Player.Mana < 80))
            {
                if (prediction.HitChance >= HitChance.High)
                {
                    E.Cast(prediction.UnitPosition);
                }
            }

            //W logic
            if (Champions.Rumble.MenuClass.harasswmenu["usew"].Enabled && W.Ready && Player.CountEnemyHeroesInRange(1000) > Champions.Rumble.MenuClass.combowmenu["enemies"].Value && Player.HealthPercent() <= Champions.Rumble.MenuClass.combowmenu["health"].Value)
                W.Cast();
        }


        private void JungleClear()
        {

        }

        private void LaneClear()
        {
            //TODO.
        }

        private void LastHit()
        {
            foreach (var minion in GetEnemyLaneMinionsTargetsInRange(E.Range))
            {
                if (minion.IsValidTarget(E.Range) && minion != null)
                {
                    if (minion.Health <= Player.GetSpellDamage(minion, SpellSlot.E))
                    {
                        E.Cast(minion);
                    }
                }
            }
        }

        //Credits to @Exory
        public static List<Obj_AI_Minion> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(m => m.IsValidTarget(range)).ToList();
        }
    }
}

