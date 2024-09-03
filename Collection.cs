using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriviaGame
{
    public class Collection
    {
        public int NumberOfItems { get; protected set; }
        public List<string> ItemsList { get; protected set; }
        public List<string> AnswersList { get; protected set; }
        public List<int> AvailableItemIndexes { get; protected set; }

        public Collection()
        {
            NumberOfItems = 0;
            ItemsList = new List<String>();
            AnswersList = new List<string>();
            AvailableItemIndexes = new List<int>();
        }

        //Method reads all items by  given sql line
        protected void ReadItemsFromDB(string line)
        {
            MySqlConnection connection = ConnectDatabase();
            if (connection != null)
            {
                connection.Open();
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = line;
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    NumberOfItems++;
                    ItemsList.Add(reader.GetString(0));
                    AnswersList.Add(reader.GetString(1));

                }
                reader.Close();
                connection.Close();
            }
        }

        //marks item as removed by removing it is index from index list
        public void RemoveItem(int index)
        {
            AvailableItemIndexes.Remove(index);
        }

        //method sets up list of indexes for available items
        public void SetAvailableItem()
        {
            AvailableItemIndexes = new List<int>();
            for (int i = 0; i < NumberOfItems; i++)
            {
                AvailableItemIndexes.Add(i);
            }
        }

        //method returns index for one of the available items
        public int RandomItemIndex()
        {
            Random rnd = new Random();
            int num = rnd.Next(0, NumberOfItems);
            while (true)
            {
                if (AvailableItemIndexes.Contains(num))
                {
                    break;
                }
                num = rnd.Next(0, NumberOfItems);
            }
            return num;
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
