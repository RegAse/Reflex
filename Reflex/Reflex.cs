﻿using MySql.Data.MySqlClient;
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
            Type type = typeof(T);
            List<Where> wh = new List<Where>();
            wh.Add(new Where(foreign,"=",value.ToString()));
            return Sele<T>(wh);
        }

        public int Save<T>(object thisobject)
        {
            Type type = thisobject.GetType();
            string name = thisobject.GetType().Name;
            if (name[name.Length - 1] == 'y')
            {
                name = name.Substring(0,name.Length - 1) + "ies";
            }
            else if (name[name.Length - 1] != 's')
            {
                name += "s";
            }
            return Insert<T>(name , thisobject);
        }

        public void Delete<T>(object thisobject)
        {
            throw new NotImplementedException();
        }

        public List<T> HasMany<T>(object thisclass)
        {
            Type type = typeof(T);
            List<Where> wh = new List<Where>();
            wh.Add(new Where(thisclass.GetType().Name + "_id","=", thisclass.GetType().GetProperty("id").GetValue(thisclass).ToString()));
            return Sele<T>(wh);
        }

        /// <summary>
        /// TODO Fix this function
        /// </summary>
        /// <param name="thisobject"></param>
        /// <returns></returns>
        public bool UsesTimestamps(object thisobject)
        {
            Type type = thisobject.GetType();
            if (type.GetProperty("timestamps") != null)
            {
                /* Reflex returns the timestamp your timestamps variable is equals */
                return (bool)type.GetProperty("timestamps").GetValue(this, null);
            }
            else
            {
                /* This is the Default, timestamps are defaulted so it returns true */
                return true;
            }
        }


        /// <summary>
        /// Insert into the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Insert<T>(string table, object data)
        {
            int id = 0;
            if (OpenConnection())
            {
                /* Check if timestamps are used */
                //bool usesTimestamps = UsesTimestamps(data);
                string query = "INSERT INTO `" + table + "` (";
                string values = ") VALUES (";
                //Run through every property (`name`, `directory`) VALUES ('Best','C:/')
                PropertyInfo[] pinfo = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < pinfo.Count(); i++)
                {
                    if ( pinfo[i].Name.Equals("created_at", StringComparison.InvariantCultureIgnoreCase)
                        || pinfo[i].Name.Equals("updated_at", StringComparison.InvariantCultureIgnoreCase))
                    {
                        query += "`" + pinfo[i].Name + "`";
                        values += '"' + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + '"';
                        if (i != pinfo.Count() - 1)
                        {
                            query += ",";
                            values += ",";
                        }
                    }
                    else if (pinfo[i].Name != "id")
                    {
                        query += "`" + pinfo[i].Name + "`";
                        values += '"' + pinfo[i].GetValue(this, null).ToString() +'"';
                        if (i != pinfo.Count() - 1)
                        {
                            query += ",";
                            values += ",";
                        }
                    }
                }

                query += values + ");SELECT LAST_INSERT_ID() as id;";
                //Console.WriteLine(query); Show executed query
                sqlquery = new MySqlCommand(query, sqlconnection);

                reader = sqlquery.ExecuteReader();
                while (reader.Read())
                {
                    id = int.Parse(reader.GetValue(0).ToString());
                }
                CloseConnection();
            }
            return id;
        }

        /// <summary>
        /// Database Configuration
        /// </summary>
        public static int count { get; set; }
        public static string Server { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string Database { get; set; }
        private static void Connect()
        {
            count++;
            Console.WriteLine("Connecting to Database T:"+ count +".");
            string server = (Server != null ? Server : "localhost");
            string database = (Database != null ? Database : "reflex");
            string uid = (Username != null ? Username : "root");
            string password = (Password != null ? Password : "");

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
