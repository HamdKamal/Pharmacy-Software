using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Pharmacy_Software
{
    class ClassDB
    {
        public string GetConnection()
        {
            string cn = "server = localhost; username = root; password =; database = pharm_db;";
            return cn;
        }
    }
}
