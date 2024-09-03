using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TriviaGame.ResourceClasses
{
    public class PicturesCollection : Collection
    {
        public PicturesCollection()
        {
            ReadItemsFromDB("SELECT Picture,Answer from pictures");
        }
    }
}
