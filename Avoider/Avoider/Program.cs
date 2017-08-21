using System;
using Aimtec;
using Aimtec.SDK.Events;

namespace Avoider
{
    class Program
    {
        static void Main(string[] args)
        {
            GameEvents.GameStart += GameEvents_GameStart;
        }

        private static void GameEvents_GameStart()
        {

            switch (ObjectManager.GetLocalPlayer().ChampionName)
            {
                default:
                    var s = new Avoider();
                    break;




            }

        }
    }
}