using MySql.Data.MySqlClient;
using System.Security.Cryptography.X509Certificates;
using TriviaGame.ResourceClasses;

namespace TriviaGame
{
    //To do list
    //connect and use database +
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}