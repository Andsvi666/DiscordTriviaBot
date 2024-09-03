using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriviaGame.ResourceClasses;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;
using MySql.Data.MySqlClient;

namespace TriviaGame.Commands
{
    public class GameCommands : BaseCommandModule
    {
        //game status used to check if commands can be used
        bool gameOn = false;
        //to check of question or picture stage of the game is finished
        bool stageFinished = true;
        PicturesCollection pictures = new PicturesCollection();
        QuestionCollection questions = new QuestionCollection();
        List<Player> players = new List<Player>();
        //-----------------------------------Commands-----------------------------------
        //Command to show list of commands
        [Command("help")]
        [RequireRoles(RoleCheckMode.Any, "GameMaster")]
        public async Task Help(CommandContext ctx)
        {
            if (!gameOn)
            {
                DiscordEmbedBuilder message = new DiscordEmbedBuilder()
                {
                    Title = "List of commands",
                    Description = 
                    "Help - shows list of commands\n" +
                    "Start - sends embeded messege that lets users to join the game in given time (number + s, m or h).\n" +
                    "Picture - shows random picture so players can guess what it is. Type '--stop' to end guessing.\n" +
                    "Question - shows random question so player can try to answer it. Type '--stop' to end guessing.\n" +
                    "End - ends the current game and shows results.\n" +
                    "Score - shows a leaderboard of all players that ever played. Sorted list requires extra option " +
                    "for sorting: 'Score' - sort by score, 'GamesPlayed' - sort by games, 'Username' - sort by username.\n" +
                    "---------------------------------------------------------------------------\n" +
                    "Help and score can only be used when game is not happening.\n" +
                    "Commands for game in order: start, question/picture, end.",
                    Color = DiscordColor.Azure,
                };
                await ctx.Channel.SendMessageAsync(embed: message);
            }
        }

        //Command to gather members for the game
        [Command("start")]
        [RequireRoles(RoleCheckMode.Any, "GameMaster")]
        public async Task Start(CommandContext ctx, TimeSpan time)
        {
            if(!gameOn)
            {
                gameOn = true;
                pictures.SetAvailableItem();
                questions.SetAvailableItem();
                InteractivityExtension inter = ctx.Client.GetInteractivity();
                DiscordEmoji checkmark = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:", false);
                DiscordEmbedBuilder message = new DiscordEmbedBuilder()
                {
                    Title = $"Game will begin in {time}",
                    Description = $"Click {checkmark} to join the game",
                    Color = DiscordColor.Azure,
                };
                DiscordMessage embededMess = await ctx.Channel.SendMessageAsync(embed: message);
                await embededMess.CreateReactionAsync(checkmark);
                var results = await inter.CollectReactionsAsync(embededMess, time);
                List<string> usernames = new List<string>();
                //gets all users that clicked the verification
                if (results.Count > 0)
                {
                    foreach (DiscordUser user in results[0].Users)
                    {
                        if (!user.IsBot)
                        {
                            players.Add(new Player(user));
                            usernames.Add(user.Username);
                        }
                    }
                }
                DiscordEmbedBuilder message1 = new DiscordEmbedBuilder()
                {
                    Title = $"Players joined: {players.Count}",
                    Description = $"{string.Join(',', usernames)}",
                    Color = DiscordColor.Azure,
                };
                await ctx.Channel.SendMessageAsync(embed: message1);
                if(players.Count < 0)
                {
                    gameOn = false;
                }
            }
        }

        //Command shows random picture from the pictures folder 
        [Command("picture")]
        [RequireRoles(RoleCheckMode.Any, "GameMaster")]
        public async Task Picture(CommandContext ctx)
        {
            if (gameOn && stageFinished)
            {
                stageFinished = false;
                if (pictures.AvailableItemIndexes.Count > 0)
                {
                    int num = pictures.RandomItemIndex();
                    //Sending image
                    await ctx.Channel.SendMessageAsync(pictures.ItemsList[num]);
                    DiscordEmbedBuilder message1 = new DiscordEmbedBuilder()
                    {
                        Title = $"What movie is this?",
                        Color = DiscordColor.Azure,
                    };
                    await ctx.Channel.SendMessageAsync(message1);
                    //getting answers
                    string answer = pictures.AnswersList[num];
                    CheckResults(ctx, answer);
                    pictures.RemoveItem(num);
                }
                else
                {
                    DiscordEmbedBuilder message = new DiscordEmbedBuilder()
                    {
                        Title = "Error",
                        Description = "There is no more pictures to show",
                        Color = DiscordColor.Azure,
                    };
                    await ctx.Channel.SendMessageAsync(embed: message);
                    stageFinished = true;
                }
            }
        }

        //Command shows random question from the question file
        [Command("question")]
        [RequireRoles(RoleCheckMode.Any, "GameMaster")]
        public async Task Question(CommandContext ctx)
        {
            if(gameOn && stageFinished)
            {
                stageFinished = false;
                if (questions.AvailableItemIndexes.Count > 0)
                {
                    int num = questions.RandomItemIndex();
                    //sending question
                    DiscordEmbedBuilder message = new DiscordEmbedBuilder()
                    {
                        Title = $"Answer question",
                        Description = questions.ItemsList[num],
                        Color = DiscordColor.Azure,
                    };
                    await ctx.Channel.SendMessageAsync(embed: message);
                    //getting answers
                    string answer = questions.AnswersList[num];
                    CheckResults(ctx, answer);
                    questions.RemoveItem(num);
                }
                else
                {
                    DiscordEmbedBuilder message = new DiscordEmbedBuilder()
                    {
                        Title = "Error",
                        Description = "There is no more questions to show",
                        Color = DiscordColor.Azure,
                    };
                    await ctx.Channel.SendMessageAsync(embed: message);
                    stageFinished = true;
                }
            }
        }

        //Command displays leaderboard of all players that played
        [Command("end")]
        [RequireRoles(RoleCheckMode.Any, "GameMaster")]
        public async Task End(CommandContext ctx)
        {
            if(gameOn && stageFinished)
            {
                gameOn = false;
                string desp = "";
                foreach (Player p in players)
                {
                        desp = desp + p.GetInfoLine();
                }
                players.Clear();
                DiscordEmbedBuilder message = new DiscordEmbedBuilder()
                {
                    Title = "Game finished",
                    Description = desp,
                    Color = DiscordColor.Azure,
                };
                await ctx.Channel.SendMessageAsync(embed: message);
            }
        }

        //Command displays leaderboard of all players that ever played
        [Command("score")]
        [RequireRoles(RoleCheckMode.Any, "GameMaster")]
        public async Task Score(CommandContext ctx, string sortType = "")
        {
            if(!gameOn)
            {
                string title = "";
                string lines = "";
                title = "Scores" + new string('\u200a', 50) + "Games" + new string('\u200a', 50) + "Player";
                try
                {
                    lines = GetAllresults(sortType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                DiscordEmbedBuilder message = new DiscordEmbedBuilder()
                {
                    Title = title,
                    Description = lines,
                    Color = DiscordColor.Azure,

                };
                await ctx.Channel.SendMessageAsync(embed: message);
            }
        }


        //-----------------------------------Other functions-----------------------------------
        //Method returns list of info about each player sorted by given option
        public string GetAllresults(string type)
        {
            MySqlConnection connection = ConnectDatabase();
            if(connection != null)
            {
                connection.Open();
                string results = "";
                List<List<string>> values = new List<List<string>>();
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = GetCommandLineToSort(type);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string resLine = reader.GetString(0) + new string('\u200a', 100) + reader.GetString(1) + new string('\u200a', 100) + reader.GetString(2) + "\n";
                    results = results + resLine;
                }
                reader.Close();
                connection.Close();
                return results;
            }
            else
            {
                return "";
            }
        }

        //Method retuns correct sql query line by certain given type
        public string GetCommandLineToSort(string type)
        {
            string line = "SELECT  Score, GamesPlayed, Username FROM players";
            if(type == "Username" || type == "GamesPlayed" || type == "Score")
            {
                line = line + $" ORDER BY {type}";
            }
            return line;
        }

        //Method checks all sent messages since question is asked. Stops when leader writes "stop"
        public async void CheckResults(CommandContext ctx, string answer)
        {
            List<DiscordMessage> messages = new List<DiscordMessage>();
            while (true)
            {
                var result = await ctx.Client.GetInteractivity().WaitForMessageAsync(x => x.Channel == ctx.Channel);
                if (!result.TimedOut)
                {
                    if (result.Result.Content == "--stop" && result.Result.Author == ctx.Member)
                    {
                        break;
                    }
                    else
                    {
                        messages.Add(result.Result);
                    }
                }
            }
            GiveOutPoints(messages, answer, ctx);
        }

        //Method gives points to users that guessed correctly and displays results for question/picture
        public void GiveOutPoints(List<DiscordMessage> messages, string answer, CommandContext ctx)
        {
            List<ulong> ids = new List<ulong>();
            string users = "";
            int startPoints = 5;
            //Give out points to those that guessed correctly, 5 to  first, 3 to second, 1 to the rest
            foreach (DiscordMessage msg in messages)
            {
                if (msg.Content.ToLower().Equals(answer.ToLower()) && players.Any(p => p.User.Id == msg.Author.Id) && !ids.Contains(msg.Author.Id))
                {
                    users = users + msg.Author.Username + "  " + startPoints + "\n";
                    ids.Add(msg.Author.Id);
                    players.Where(p => p.User.Id == msg.Author.Id).First().InstancePoints += startPoints;
                    if (startPoints > 2)
                    {
                        startPoints = startPoints - 2;
                    }
                }
            }
            if(users == "")
            {
                users = "None";
            }
            DiscordEmbedBuilder message = new DiscordEmbedBuilder()
            {
                Title = $"Correct answer was '{answer}'. Users that guessed right:",
                Description = users,
                Color = DiscordColor.Azure,

            };
            ctx.Channel.SendMessageAsync(embed: message);
            stageFinished = true;
        }

        //Method connects to database
        public MySqlConnection ConnectDatabase()
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
