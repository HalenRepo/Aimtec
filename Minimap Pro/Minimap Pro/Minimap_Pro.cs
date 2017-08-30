using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using Aimtec;
using Aimtec.SDK.Extensions;

using Aimtec.SDK.Util.Cache;

using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;

namespace MiniMap_Pro
{
    internal class Minimap_Pro
    {
        private static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();
        public static Menu Menu = new Menu("Minimap Pro", "Minimap Pro", true);

        private static readonly List<ChampionTracker> _lastPositions = new List<ChampionTracker>();

        public Minimap_Pro()
        {
            #region Menu
            var turrethealth = new Menu("TurretHealth", "Turret Health")
            {
                new MenuBool("Toggle", "Enabled"),

                new Menu("Allies", "Allies")
                {
                    new MenuBool("turrets", "Turrets"),
                    new MenuBool("inhibs", "Inhibitors")
                },

                new Menu("Enemies", "Enemies")
                {
                    new MenuBool("turrets", "Turrets"),
                    new MenuBool("inhibs", "Inhibitors")
                },
            };
            Menu.Add(turrethealth);

            var waypoints = new Menu("Waypoints", "Waypoints")
            {
                new MenuBool("Toggle", "Enabled"),
            };
            Menu.Add(waypoints);

            //Submenu of waypoints
            var waypointssub = new Menu("whitelist", "Whitelist");
            {
                foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                    waypointssub.Add(new MenuBool(enemies.ChampionName.ToLower(), enemies.ChampionName));
            }
            waypoints.Add(waypointssub);

            //Submenu2 of waypoints
            var waypointssub2 = new MenuList("color", "Color: ", new[] { "Yellow", "White", "Red", "Orange" }, 1);
            waypoints.Add(waypointssub2);

            var jungletimer = new Menu("JungleTimer", "Jungle Timers")
            {
                new MenuBool("Toggle", "Enabled"),
                new MenuBool("teamtoggle", "Ally Side Jungle"),
                new MenuBool("enemytoggle", "Enemy Side Jungle"),
                new MenuList("color","Color: ", new[] { "Yellow", "White", "Green" }, 0)

            };
            Menu.Add(jungletimer);

            var championtracker = new Menu("ChampionTracker", "Champion Tracker")
            {
                new MenuBool("Toggle", "Enabled"),
            };
            Menu.Add(championtracker);

            //Submenu of championtracker
            var championtrackersub = new Menu("whitelist", "Whitelist");
            {
                foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                    championtrackersub.Add(new MenuBool(enemies.ChampionName.ToLower(), enemies.ChampionName));
            }
            championtracker.Add(championtrackersub);

            Menu.Attach();
            #endregion

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += OnGameObjectCreated;
            GameObject.OnDestroy += OnGameObjectDestroyed;
            //Obj_AI_Base.OnTeleport += OnTeleport;

            //For champion tracker
            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                var enemychamp = new ChampionTracker(enemy) { LastPosition = enemy.Orientation };
                _lastPositions.Add(enemychamp);
            }

        }

        private void Game_OnUpdate()
        {

        }

        /*private void OnTeleport(Obj_AI_Base sender, Obj_AI_BaseTeleportEventArgs args)
        {

        }*/

        private void OnGameObjectCreated(GameObject sender)
        {
            if (!sender.IsValid || sender.Type != GameObjectType.obj_AI_Minion || sender.Team != GameObjectTeam.Neutral)
            {
                return;
            }

            //Console.WriteLine(Game.ClockTime + " Name: " + sender.Name.ToLower());

            foreach (var camp in jgcamps)
            {
                var mob = camp.Mobs.FirstOrDefault(m => m.Name.ToLower().Equals(sender.Name.ToLower()));

                if (mob != null)
                {
                    mob.Dead = false;
                    camp.Dead = false;
                }
            }
        }

        private void OnGameObjectDestroyed(GameObject sender)
        {
            if (!sender.IsValid || sender.Type != GameObjectType.obj_AI_Minion || sender.Team != GameObjectTeam.Neutral)
            {
                return;
            }

            foreach (var camp in jgcamps.ToArray())
            {
                var mob =
                    camp.Mobs.FirstOrDefault(m => m.Name.ToLower().Equals(sender.Name.ToLower()));
                if (mob != null)
                {
                    if (mob.Name.ToLower().Contains("Herald"))
                    {
                        if (Game.ClockTime + camp.RespawnTime > 20 * 60 || GameObjects.AllGameObjects.Any(j => j.Name.ToLower().Contains("baron")))
                        {
                            jgcamps.Remove(camp);
                        }
                    }

                    mob.Dead = true;
                    camp.Dead = camp.Mobs.All(m => m.Dead);
                    if (camp.Dead)
                    {
                        camp.Dead = true;
                        camp.NextRespawnTime = (int)Game.ClockTime + camp.RespawnTime - 3;
                        //Console.WriteLine(Game.ClockTime + " | " + camp.NextRespawnTime);
                    }
                }
            }
        }

        private static void Render_OnPresent()
        {

            #region Turret health
            //Turret health
            if (Menu["TurretHealth"]["Toggle"].Enabled)
            {
                foreach (var turret in ObjectManager.Get<Obj_AI_Turret>().Where(t => t != null && t.IsValid && !t.IsDead && t.HealthPercent() < 100))
                {
                    if (!Menu["TurretHealth"]["Allies"]["turrets"].Enabled)
                    {
                        if (turret.Team == Player.Team)
                            continue;
                    }

                    if (!Menu["TurretHealth"]["Enemies"]["turrets"].Enabled)
                    {
                        if (turret.Team != Player.Team)
                            continue;
                    }
                    //var pos = turret.TurretPosition;
                    //Console.WriteLine(pos);
                    var pos = turret.ServerPosition.ToMiniMapPosition();
                    int health = (int)turret.HealthPercent();
                    Render.Text(pos, Color.FromArgb(200, 255, 255, 255), health + "%", RenderTextFlags.VerticalTop);
                }

                //Inhib health
                foreach (var inhibitor in ObjectManager.Get<Obj_BarracksDampener>().Where(t => t != null && t.IsValid && !t.IsDead && t.HealthPercent() < 100))
                {
                    if (!Menu["TurretHealth"]["Allies"]["inhibs"].Enabled)
                    {
                        if (inhibitor.Team == Player.Team)
                            continue;
                    }

                    if (!Menu["TurretHealth"]["Enemies"]["inhibs"].Enabled)
                    {
                        if (inhibitor.Team != Player.Team)
                            continue;
                    }

                    var pos = inhibitor.ServerPosition.ToMiniMapPosition();
                    int health = (int)inhibitor.HealthPercent();
                    Render.Text(pos, Color.FromArgb(200, 255, 255, 255), health + "%", RenderTextFlags.VerticalTop);
                }
            }     
            #endregion

            #region Jungle timers
            //Jungle timers
            if (Menu["JungleTimer"]["Toggle"].Enabled)
            {
                Color selectedColor = Color.FromArgb(200, 255, 255, 0); //default yellow
                switch (Menu["JungleTimer"]["color"].Value)
                {
                    case 0:
                        selectedColor = Color.FromArgb(200, 255, 255, 0); //yellow
                        break;

                    case 1:
                        selectedColor = Color.FromArgb(200, 255, 255, 255); //white
                        break;

                    case 2:
                        selectedColor = Color.FromArgb(200, 143, 245, 87); //Light green
                        break;
                }

                //For the beginning of the game. 112 being 1:52, the latest gromp spawn. 150 for dragon.
                if (Game.ClockTime <= 150) //Latest dragon spawn early game
                {
                    foreach (var camp in jgcamps.Where(x => x.SpawnTime <= 150)) // <= 150 so that baron/herald timers don't show, but first dragon will
                    {
                        var timer = camp.SpawnTime - (int)Game.ClockTime;
                        TimeSpan t = TimeSpan.FromSeconds(timer);
                        string text = string.Format("{1:D1}:{2:D2}",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);

                        if (timer <= 0)
                        {
                            camp.Dead = false;
                            continue;
                        }

                        if (camp.Team == Player.Team && !Menu["JungleTimer"]["teamtoggle"].Enabled)
                        {
                            continue;
                        }

                        if (camp.Team != Player.Team && camp.Team != GameObjectTeam.Neutral && !Menu["JungleTimer"]["enemytoggle"].Enabled)
                        {
                            continue;
                        }

                        Render.Text(camp.MinimapPosition, selectedColor, text, RenderTextFlags.HorizontalCenter);
                    }
                }
                else //for later on in the game
                {

                    foreach (var camp in jgcamps.Where(x => x.Dead))
                    {
                        var timer = camp.NextRespawnTime - (int)Game.ClockTime;
                        TimeSpan t = TimeSpan.FromSeconds(timer);
                        string text = string.Format("{1:D1}:{2:D2}",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);

                        if (timer <= 0)
                        {
                            camp.Dead = false;
                            continue;
                        }

                        Render.Text(camp.MinimapPosition, selectedColor, text, RenderTextFlags.HorizontalCenter);

                    }
                }
            }
            #endregion

            #region Champ tracker
            //Champion tracker
            if (Menu["ChampionTracker"]["Toggle"].Enabled)
            {
                var index = 0;

                foreach (var champtrack in _lastPositions)
                {
                    index++;
                    Color trackColor = Color.Red;

                    if (!Menu["ChampionTracker"]["whitelist"][champtrack.Champ.ChampionName.ToLower()].Enabled)
                    {
                        continue;
                    }

                    if (champtrack.Champ.IsDead && !champtrack.LastPosition.Equals(Vector3.Zero) && champtrack.LastPosition.Distance(champtrack.Champ.Position) > 500)
                    {
                        champtrack.Teleported = false;
                        champtrack.LastSeen = Game.ClockTime;
                    }
                    champtrack.LastPosition = champtrack.Champ.Position;
                    if (champtrack.Champ.IsVisible)
                    {
                        champtrack.Teleported = false;
                        if (!champtrack.Champ.IsDead)
                        {
                            champtrack.LastSeen = Game.ClockTime;
                        }
                    }
                    if (!champtrack.Champ.IsVisible && !champtrack.Champ.IsDead)
                    {
                        var pos = champtrack.Teleported ? champtrack.Champ.Orientation : champtrack.LastPosition;
                        Render.WorldToMinimap(pos, out var mpPos);
                        Render.WorldToScreen(pos, out var mPos);

                        //Check if you want to draw ss circle [unimplemented] here
                        if (!champtrack.LastSeen.Equals(0f) && Game.ClockTime - champtrack.LastSeen > 3f)
                        {
                            var radius = Math.Abs((Game.ClockTime - champtrack.LastSeen - 1) * champtrack.Champ.MoveSpeed * 0.9f) / 100;

                            //Select champion color
                            switch (index)
                            {
                                case 0:
                                    trackColor = Color.Red;
                                    break;

                                case 1:
                                    trackColor = Color.Orange;
                                    break;

                                case 3:
                                    trackColor = Color.LightGreen;
                                    break;

                                case 4:
                                    trackColor = Color.Blue;
                                    break;

                                case 5:
                                    trackColor = Color.HotPink;
                                    break;
                            }

                            if (radius <= 800)
                            {
                                //you can draw the circle to screen here

                                //draw to minimap
                                Vector2 le;
                                Render.WorldToMinimap(pos, out le);
                                Render.Text(le, trackColor, champtrack.Champ.ChampionName, RenderTextFlags.HorizontalCenter);
                                Draw2DCircle(le, radius, trackColor);
                            }
                        }
                    }
                }
            }
            #endregion

            #region Waypoints
            if (Menu["Waypoints"]["Toggle"].Enabled)
            {
                foreach (var enemy in GameObjects.EnemyHeroes)
                {
                    if (!Menu["Waypoints"]["whitelist"][enemy.ChampionName.ToLower()].Enabled)
                    {
                        continue;
                    }

                    var waypointcolor = Color.White; //Default white.
                    // "Yellow", "White", "Red", "Orange" 
                    switch (Menu["Waypoints"]["color"].Value)
                    {
                        case 0:
                            waypointcolor = Color.Yellow;
                            break;

                        case 1:
                            waypointcolor = Color.White;
                            break;

                        case 2:
                            waypointcolor = Color.Red;
                            break;

                        case 3:
                            waypointcolor = Color.Orange;
                            break;
                    }

                    if (enemy.IsVisible && !enemy.IsDead && enemy.Path.LastOrDefault() != null)
                    {
                        if (enemy.Path.LastOrDefault() != enemy.Position)
                        {
                            Render.Line(enemy.Position.ToScreenPosition(), enemy.Path.LastOrDefault().ToScreenPosition(), waypointcolor);
                            Render.Circle(enemy.Path.LastOrDefault(), 25, 30, waypointcolor);
                        }
                    }
                }
            }
            #endregion
        }

        #region ChampionTracker constructor
        internal class ChampionTracker
        {
            public ChampionTracker(Obj_AI_Hero champ)
            {
                Champ = champ;
                LastPosition = Vector3.Zero;
            }

            public Obj_AI_Hero Champ { get; private set; }
            public bool IsTeleporting { get; set; } //unimplemented
            public float LastSeen { get; set; }
            public Vector3 LastPosition { get; set; }
            public bool Teleported { get; set; } //unimplemented
        }
        #endregion

        #region Draw 2D circle on minimap
        //Credits to @sunless
        public static void Draw2DCircle(Vector2 centre, float radius, Color color)
        {
            for (int i = 0; i < 20; i++)
            {
                float x1 = (float)(centre.X + radius * Math.Cos(i / 20.0 * 2 * Math.PI));
                float y1 = (float)(centre.Y + radius * Math.Sin(i / 20.0 * 2 * Math.PI));

                float x2 = (float)(centre.X + radius * Math.Cos((i + 1) / 20.0 * 2 * Math.PI));
                float y2 = (float)(centre.Y + radius * Math.Sin((i + 1) / 20.0 * 2 * Math.PI));

                Render.Line(x1, y1, x2, y2, color);
            }
        }
        #endregion

        #region Camp class with mobs
        public class Camp
        {
            public Camp(float spawnTime,
                float respawnTime,
                Vector3 position,
                List<Mob> mobs,
                bool isBig,
                GameObjectTeam team,
                bool dead = false)
            {
                SpawnTime = spawnTime;
                RespawnTime = respawnTime;
                Position = position;
                MinimapPosition = position.ToMiniMapPosition();
                Mobs = mobs;
                IsBig = isBig;
                Team = team;
                Dead = dead;
            }

            public float SpawnTime { get; set; }
            public float RespawnTime { get; private set; }
            public Vector3 Position { get; private set; }
            public Vector2 MinimapPosition { get; set; }
            public List<Mob> Mobs { get; set; } //private set
            public bool IsBig { get; set; }
            public float spawnTime { get; set; }
            public float NextRespawnTime { get; set; }
            public bool Dead { get; set; }

            public GameObjectTeam Team { get; set; }
        }

        public class Mob
        {
            public Mob(string name, bool isBig = false, bool dead = false)
            {
                Name = name;
                IsBig = isBig;
                Dead = dead;
            }

            public String Name { get; set; }
            public bool IsBig { get; set; }
            public bool Dead { get; set; }

        }
        #endregion

        #region Jungle camps for summoners rift
        private static List<Camp> jgcamps = new List<Camp>
        {
            //Order: Blue
            new Camp(
                        100, 300, new Vector3(3714f, 53.15f, 8300f), //3800.99f, 52.18f, 7883.53f
                        new List<Mob>(
                            new[]
                            {
                                new Mob("sru_blue1.1.1", true)
                            }), true,
                        GameObjectTeam.Order),

                    //Order: Wolves
                    new Camp(
                        97, 150, new Vector3(3824f, 52.46f, 6522f), //3849.95f, 52.46f, 6504.36f
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Murkwolf2.1.1", true), new Mob("SRU_MurkwolfMini2.1.2"),
                                new Mob("SRU_MurkwolfMini2.1.3")
                            }), false,
                        GameObjectTeam.Order),

                    //Order: Chicken
                    new Camp(
                        97, 150, new Vector3(6838f, 50.61f, 5782), //6943.41f, 52.62f, 5422.61f
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Razorbeak3.1.1", true),
                                new Mob("SRU_RazorbeakMini3.1.2"),
                                new Mob("SRU_RazorbeakMini3.1.3"),
                                new Mob("SRU_RazorbeakMini3.1.4")
                            }), false,
                         GameObjectTeam.Order),

                    //Order: Red
                    new Camp(
                        100, 300, new Vector3(7714.66f, 54.18f, 4300f), //7813.07f, 53.81f, 4051.33f
                        new List<Mob>(
                            new[]
                            {new Mob("SRU_Red4.1.1", true)}),
                        true,  GameObjectTeam.Order),

                    //Order: Krug
                    new Camp(
                        112, 150, new Vector3(8200f, 51.13f, 2654), //8370.58f, 51.09f, 2718.15f
                        new List<Mob>(new[]
                        {
                            new Mob("SRU_Krug5.1.2", true),
                            new Mob("SRU_KrugMini5.1.1"), new Mob("SRU_KrugMini5.1.2"), new Mob("SRU_KrugMini5.1.3"), new Mob("SRU_KrugMini5.1.4"), new Mob("SRU_KrugMini5.1.5"), new Mob("SRU_KrugMini5.1.6"), new Mob("SRU_KrugMini5.1.7"), new Mob("SRU_KrugMini5.1.8"), new Mob("SRU_KrugMini5.1.9")
                        }), false,
                         GameObjectTeam.Order),

                    //Order: Gromp
                    new Camp(
                        112, 150, new Vector3(2174f, 51.77f, 8620f), //2164.34f, 51.78f, 8383.02f
                        new List<Mob>(new[] {new Mob("SRU_Gromp13.1.1", true)}), false,
                         GameObjectTeam.Order),

                    //Chaos: Blue
                    new Camp(
                        100, 300, new Vector3(11006f, 51.72f, 7370f), //10984.11f, 51.72f, 6960.31f
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Blue7.1.1", true)
                            }), true,
                        GameObjectTeam.Chaos),

                    //Chaos: Wolves
                    new Camp(
                        97, 150, new Vector3(10918f, 62.67f, 8818f), //10983.83f, 62.22f, 8328.73f
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Murkwolf8.1.1", true), new Mob("SRU_MurkwolfMini8.1.2"),
                                new Mob("SRU_MurkwolfMini8.1.3")
                            }), false,
                        GameObjectTeam.Chaos),

                    //Chaos: Chicken
                    new Camp(
                        97, 150, new Vector3(7672f, 52.30f, 9970f), //7852.38f, 52.30f, 9562.62f
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Razorbeak9.1.1", true),
                                new Mob("SRU_RazorbeakMini9.1.2"),
                                new Mob("SRU_RazorbeakMini9.1.3"),
                                new Mob("SRU_RazorbeakMini9.1.4")
                            }), false,
                         GameObjectTeam.Chaos),

                    //Chaos: Red
                    new Camp(
                        100, 300, new Vector3(6960f, 55.99f, 11100), //7139.29f, 56.38f, 10779.34f
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Red10.1.1", true)
                            }), true,  GameObjectTeam.Chaos),

                    //Chaos: Krug
                    new Camp(
                        112, 150, new Vector3(6270f, 56.47f, 12630), //6476.17f, 56.48f, 12142.51f
                        new List<Mob>(new[]
                        {
                            new Mob("SRU_Krug11.1.2", true),
                            new Mob("SRU_KrugMini11.1.1"), new Mob("SRU_KrugMini11.1.2"), new Mob("SRU_KrugMini11.1.3"), new Mob("SRU_KrugMini11.1.4"), new Mob("SRU_KrugMini11.1.5"), new Mob("SRU_KrugMini11.1.6"), new Mob("SRU_KrugMini11.1.7"), new Mob("SRU_KrugMini11.1.8"), new Mob("SRU_KrugMini11.1.9")
                        }), false,
                         GameObjectTeam.Chaos),

                    //Chaos: Gromp
                    new Camp(
                        112, 150, new Vector3(12480, 51.77f, 6806), //12671.83f, 51.71f, 6306.60f
                        new List<Mob>(new[] {new Mob("SRU_Gromp14.1.1", true)}), false,
                         GameObjectTeam.Chaos),

                    //Neutral: Dragons
                    //Yes, the dragons spawn at 150, but to avoid having several overlayed timers displayed, I'll set only one dragon to be at 150. Broscience.
                    new Camp(
                        150, 360, new Vector3(9694f, -71.24f, 4720f), //9813.83f, -71.24f, 4360.19f
                        new List<Mob>(new[] {new Mob("SRU_Dragon_Air6.1.1", true)}), true,
                         GameObjectTeam.Neutral),
                    new Camp(
                        151, 360, new Vector3(9694f, -71.24f, 4720f),
                        new List<Mob>(new[] {new Mob("SRU_Dragon_Fire6.2.1", true)}), true,
                         GameObjectTeam.Neutral),
                    new Camp(
                        151, 360, new Vector3(9694f, -71.24f, 4720f),
                        new List<Mob>(new[] {new Mob("SRU_Dragon_Water6.3.1", true)}), true,
                         GameObjectTeam.Neutral),
                    new Camp(
                        151, 360, new Vector3(9694f, -71.24f, 4720f),
                        new List<Mob>(new[] {new Mob("SRU_Dragon_Earth6.4.1", true)}), true,
                         GameObjectTeam.Neutral),
                    new Camp(
                        151, 360, new Vector3(9694f, -71.24f, 4720f),
                        new List<Mob>(new[] {new Mob("RU_Dragon_Elder6.5.1", true)}), true,
                         GameObjectTeam.Neutral),

                    //Neutral: Rift Herald
                    new Camp(
                        240, 300, new Vector3(4824f, -71.24f, 10670f), //4993.14f, -71.24f, 10491.92f
                        new List<Mob>(new[] {new Mob("SRU_RiftHerald", true)}), true,
                         GameObjectTeam.Neutral),
                    //Neutral: Baron
                    new Camp(
                        1200, 420, new Vector3(4824f, -71.24f, 10670f),
                        new List<Mob>(new[] {new Mob("SRU_Baron12.1.1", true)}), true,
                         GameObjectTeam.Neutral),
                    //Dragon: Crab
                    new Camp(
                        150, 180, new Vector3(10512f, -62.81f, 5570f), //10647.70f, -62.81f, 5144.68f
                        new List<Mob>(new[] {new Mob("SRU_Crab15.1.1", true)}), false,
                         GameObjectTeam.Neutral),
                    //Baron: Crab
                    new Camp(
                        150, 180, new Vector3(4360f, -66.23f, 9936f), //4285.04f, -67.60f, 9597.52f
                        new List<Mob>(new[] {new Mob("SRU_Crab16.1.1", true)}), false, GameObjectTeam.Neutral)
        };
        #endregion
    }
}