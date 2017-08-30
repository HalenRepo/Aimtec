namespace Minimap_Pro
{
    using System;
    using Aimtec;
    using Aimtec.SDK.Events;
    using MiniMap_Pro;
    using Aimtec.SDK.Menu;

    class Program
    {
        static void Main(string[] args)
        {
            GameEvents.GameStart += GameEvents_GameStart;

        }

        private static void GameEvents_GameStart()
        {
            var s = new Minimap_Pro();
        }
    }
}