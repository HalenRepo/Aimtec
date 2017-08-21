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

                case "Elise":
                    var Elise = new Elise();
                    break;

                case "Fizz":
                    var Fizz = new Fizz();
                    break;

                /*case "Gnar":
                    var Gnar = new Gnar();
                    break;*/

                case "Khazix":
                    var Khazix = new KhaZix();
                    break;

                case "Kindred":
                    var Kindred = new Kindred();
                    break;

                case "Nami":
                    var Nami = new Nami();
                    break;

                case "Nidalee":
                     var Nidalee = new Nidalee();
                    break;

                case "Rumble":
                    var Rumble = new Rumble();
                    break;

                case "Soraka":
                    var Soraka = new Soraka();
                    break;

                default: Console.WriteLine("Champion not supported.");
                    break;


            }

        }
    }
}