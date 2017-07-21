namespace HeavenSeries
{
    using System;
    using Aimtec;
    using Aimtec.SDK.Events;

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
                case "Blitzcrank":
                    var Blitzcrank = new Blitzcrank();
                    break;

               /* case "Nidalee":
                     var Nidalee = new Nidalee();
                    break;*/

                default: Console.WriteLine("Champion not supported.");
                    break;


            }

        }
    }
}