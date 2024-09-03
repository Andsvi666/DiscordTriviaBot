using DSharpPlus.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriviaGame
{
    internal class Player
    {
        public DiscordUser User { get; private set; }
        //points from all  games
        private int Points { get; set; }
        //points during current game
        public int InstancePoints { get; set; }
        private int NumberOfGames { get; set; }
        public Player(DiscordUser user)
        {
            User = user;
            string id = user.Id.ToString();
            ReadPlayerDataFromDB(id);
            InstancePoints = 0;
        }

        //Method gets user points from previous games and number of games from a file
        private void ReadPlayerDataFromDB(string id)
        {
            MySqlConnection connection = ConnectDatabase();
            if (connection != null)
            {
                connection.Open();
                
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT GamesPlayed,Score FROM players " +
                    $"WHERE DiscordUserID={id}";
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    NumberOfGames = reader.GetInt32(0);
                    Points = reader.GetInt32(1);
                }
                reader.Close();
                connection.Close();
            }
        }

        //Method writes new line in players file or adds points to already existing member line
        private void WritePlayerDAtaToDB()
        {
            MySqlConnection connection = ConnectDatabase();
            if(connection != null)
            {
                //to check both if list is empty or user wasnt in the list
                bool val = false;
                Points += InstancePoints;
                NumberOfGames++;
                connection.Open();
                MySqlCommand readCmd = connection.CreateCommand();
                readCmd.CommandText = "SELECT * FROM players";
                MySqlDataReader reader = readCmd.ExecuteReader();
                if(reader.HasRows)
                {
                    val = true;
                    while (reader.Read())
                    {
                        if (reader.GetString(0) == User.Id.ToString())
                        {
                            reader.Close();
                            MySqlCommand updateCmd = connection.CreateCommand();
                            updateCmd.CommandText = "UPDATE players " +
                                $"SET GamesPlayed = {NumberOfGames}, Score = {Points} " +
                                $"WHERE DiscordUserID = {User.Id}";
                            updateCmd.ExecuteNonQuery();
                            val = false;
                            break;
                        }
                    }
                }
                else
                {
                    val = true;
                }
                //if user wasnt in the list or list was empty then user is added
                if(val)
                {
                    reader.Close();
                    MySqlCommand addCmd = connection.CreateCommand();
                    addCmd.CommandText = $"INSERT INTO players(DiscordUserID, Username, GamesPlayed, Score) VALUES('{User.Id}', '{User.Username}', {NumberOfGames}, {Points})";
                    addCmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        //Method returns player info line and resets instance points
        public string GetInfoLine()
        {
            string line = $"score: {InstancePoints}";
            line = line + new string('\u200a', 80);
            line = line + $"{User.Username}\n";
            WritePlayerDAtaToDB();
            InstancePoints = 0;
            return line;
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
