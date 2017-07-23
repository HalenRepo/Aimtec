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

                case "Kindred":
                    var Kindred = new Kindred();
                    break;

                case "Nidalee":
                     var Nidalee = new Nidalee();
                    break;

                case "Soraka":
                    var Soraka = new Soraka();
                    break;

                /*case "Thresh":
                 var Thresh = new Thresh();
                break;*/


                default: Console.WriteLine("Champion not supported.");
                    break;


            }

        }
    }
}