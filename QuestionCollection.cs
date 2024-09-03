using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TriviaGame.ResourceClasses
{
    public class QuestionCollection : Collection
    {
        public QuestionCollection()
        {
            ReadItemsFromDB("SELECT Question,Answer from questions");
        }
    }
}
