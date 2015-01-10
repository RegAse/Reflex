using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Reflex
{
    public class Reflex
    {
        private static MySqlConnection sqlconnection;
        private static MySqlCommand sqlquery;
        private static MySqlDataReader reader = null;

        public Reflex()
        {
            //Connect to database 'Reflex'
            Connect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>All records in table</returns>
        public static List<T> All<T>()
        {
            Connect();
            List<T> results = new List<T>();
            Type type = typeof(T);
            string tablename = null;
            if (type.GetProperty("table", BindingFlags.Static | BindingFlags.NonPublic) != null)
            {
                /* Reflex uses the table from the model */
                tablename = type.GetProperty("table", BindingFlags.Static | BindingFlags.NonPublic).GetValue(type).ToString();
            }
            else
            {
                /* This is the Default, Gets the type and adds a 's' to it */
                tablename = type.Name + "s";
            }

            //Database
            if (OpenConnection())
            {
                string query = "SELECT * FROM " + tablename;
                sqlquery = new MySqlCommand(query, sqlconnection);

                reader = sqlquery.ExecuteReader();
                while (reader.Read())
                {
                    Object result = Activator.CreateInstance(type);
                    PropertyInfo[] info = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var item in info)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i).Equals(item.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                item.SetValue(result, reader.GetValue(i), null);
                            }
                        }
                    }
                    results.Add((T)Convert.ChangeType(result, typeof(T)));
                }

                CloseConnection();
            }
            return (List<T>)Convert.ChangeType(results, typeof(List<T>));
        }

        public static List<T> Sele<T>(List<Where> where)
        {
            Connect();
            List<T> results = new List<T>();
            Type type = typeof(T);
            string tablename = null;
            if (type.GetProperty("table", BindingFlags.Static | BindingFlags.NonPublic) != null)
            {
                /* Reflex uses the table from the model */
                tablename = type.GetProperty("table", BindingFlags.Static | BindingFlags.NonPublic).GetValue(type).ToString();
            }
            else
            {
                /* This is the Default, Gets the type and adds a 's' to it */
                tablename = type.Name + "s";
            }

            //Database
            if (OpenConnection())
            {
                string query = "SELECT * FROM " + tablename;
                if (where.Count > 0)
                {
                    query += " WHERE ";
                    int count = 0;
                    foreach (Where item in where)
                    {
                        if (count > 0)
                        {
                            query += " AND " + item.column + " " + item.oper + " '" + item.value + "'";
                            Console.WriteLine(query);
                        }
                        else
                        {
                            query += item.column + " " + item.oper + " '" + item.value + "'";
                            Console.WriteLine(query);
                        }
                        count++;
                    }
                }
                sqlquery = new MySqlCommand(query, sqlconnection);

                reader = sqlquery.ExecuteReader();
                while (reader.Read())
                {
                    Object result = Activator.CreateInstance(type);
                    PropertyInfo[] info = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var item in info)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i).Equals(item.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                item.SetValue(result, reader.GetValue(i), null);
                            }
                        }
                    }
                    results.Add((T)Convert.ChangeType(result, typeof(T)));
                }

                CloseConnection();
            }
            return (List<T>)Convert.ChangeType(results, typeof(List<T>));
        }

        public static QueryBuilder Where(string column, string oper, string value)
        {
            QueryBuilder qb = new QueryBuilder();
            qb.Where(column,oper,value);
            return qb;
        }

        public static QueryBuilder OrderBy(string column, string order)
        {
            QueryBuilder qb = new QueryBuilder();
            qb.OrderBy(column, order);
            return qb;
        }

        public List<T> HasMany<T>(object value ,string primary, string foreign)
        {
            List<Where> wh = new List<Where>();
            wh.Add(new Where(foreign,"=",value.ToString()));
            return Sele<T>(wh);
        }

        public List<T> HasMany<T>(object value)
        {
            throw new NotImplementedException();
        }

        public T Insert<T>(string table)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO `library`(`name`, `directory`) VALUES ('Best','C:/');SELECT LAST_INSERT_ID() as id;";
                sqlquery = new MySqlCommand(query, sqlconnection);

                reader = sqlquery.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetValue(0));
                }
                CloseConnection();
            }
            return (T)Convert.ChangeType("s", typeof(T));
        }

        /// <summary>
        /// Database Configuration
        /// </summary>
        private static void Connect()
        {
            string server = "localhost";
            string database = "reflex";
            string uid = "root";
            string password = "";

            string constring = "server= " + server + ";userid=" + uid + ";password= " + password + ";database=" + database;

            sqlconnection = new MySqlConnection(constring);
        }

        private static bool OpenConnection()
        {
            try
            {
                sqlconnection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        private static bool CloseConnection()
        {
            try
            {
                sqlconnection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

    }
}
