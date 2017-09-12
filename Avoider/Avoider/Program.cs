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
            new Avoider();
        }
    }
}