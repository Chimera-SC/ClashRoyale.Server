using System;
using System.Collections.Generic;
using System.Data;
using ClashRoyale.Server.Managers;
using MySql.Data.MySqlClient;

namespace ClashRoyale.Server.Database
{
    internal class MySQL
    {
        internal static string Hostname = "localhost";
        internal static string Username = "root";
        internal static string Password = "";
        internal static string Database = "cr_server";

        internal static string Credentials => "server=" + Hostname + ";user=" + Username + ";password=" + Password +
                                              ";database=" + Database + ";";

        internal static List<ReplayManager> GetReplays
        {
            get
            {
                var Query = "SELECT * FROM Replays";
                var List = new List<ReplayManager>();

                using (var Conn = new MySqlConnection(Credentials))
                {
                    try
                    {
                        Conn.Open();

                        using (var CMD = new MySqlCommand(Query, Conn))
                        using (var Reader = CMD.ExecuteReader())
                        {
                            while (Reader.Read())
                                List.Add(new ReplayManager
                                {
                                    ReplayID = long.Parse(Reader["ReplayID"].ToString()),
                                    ViewCount = int.Parse(Reader["ViewCount"].ToString()),
                                    Arena = int.Parse(Reader["Arena"].ToString()),
                                    JSON = Reader["Data"].ToString()
                                });
                        }
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("There was an exception while handling the table Replays");
                        Console.WriteLine(Ex);
                    }
                }

                return List;
            }
        }

        internal static int GetSeed(string TableName, string Key)
        {
            var SQL = "SELECT coalesce(MAX(" + Key + "), 0) FROM " + TableName;
            var Seed = -1;

            using (var Conn = new MySqlConnection(Credentials))
            {
                try
                {
                    Conn.Open();

                    using (var CMD = new MySqlCommand(SQL, Conn))
                    {
                        CMD.Prepare();
                        Seed = Convert.ToInt32(CMD.ExecuteScalar());
                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("There was an exception while handling the table " + TableName);
                    Console.WriteLine(Ex);
                }
            }

            return Seed;
        }

        internal static int GetAmount(string Table)
        {
            var Query = "SELECT count(*) FROM " + Table;

            using (var Conn = new MySqlConnection(Credentials))
            {
                try
                {
                    Conn.Open();

                    using (var CMD = new MySqlCommand(Query, Conn))
                    {
                        CMD.Prepare();
                        return Convert.ToInt32(CMD.ExecuteScalar());
                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("There was an exception while handling the table " + Table);
                    Console.WriteLine(Ex);
                }
            }

            return 0;
        }
    }
}