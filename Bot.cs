using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TriviaGame.Commands;
using MySql.Data.MySqlClient;
using System.Data.SqlTypes;

namespace TriviaGame
{
    public class Bot
    {
        string prefix = "?";
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            //set up configurations for the client
            DiscordConfiguration config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = ReadToken(),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            //connects to client with made configs
            Client = new DiscordClient(config);
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                //Waits for 2 minutes untill timeout
                Timeout = TimeSpan.FromMinutes(2)
            });

            //configurations for commands
            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new String[] { prefix },
                EnableMentionPrefix = true,
                EnableDms = false,
                EnableDefaultHelp = false,

            };


            //Enable commands
            Commands = Client.UseCommandsNext(commandsConfig);

            //Register commands
            Commands.RegisterCommands<GameCommands>();

            //Makes bot come online
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        //initialize bot when it is ready
        private Task onClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        //Method reads token from a file
        public static string ReadToken()
        {
            MySqlConnection connection = ConnectDatabase();
            if (connection != null)
            {
                var tokken = "";
                connection.Open();
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT Tokken from tokkens WHERE status=1";
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tokken = reader.GetString(0);
                }
                connection.Close();
                if (tokken != null)
                {
                    return tokken;
                }
                return "";
            }
            else
            {
                return "";
            }
        }


        //Method connects to database
        public static MySqlConnection ConnectDatabase()
        {
            string config =
                "server = 127.0.0.1;" +
                "user = root;" +
                "database = trivia_bot_data";
            try
            {
                MySqlConnection connection = new MySqlConnection(config);
                return connection;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}