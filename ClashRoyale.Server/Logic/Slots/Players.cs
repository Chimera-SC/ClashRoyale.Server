using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClashRoyale.Server.Database;
using ClashRoyale.Server.Database.Models;
using Newtonsoft.Json;

namespace ClashRoyale.Server.Logic.Slots
{
    internal class Players : ConcurrentDictionary<long, Player>
    {
        internal long Seed;
        private static readonly object InitLock = new object();
        private static volatile bool Initialized;
        private static readonly Random TokenRandom = new Random();
        private static readonly object TokenRandomLock = new object();

        public Players()
        {
            EnsureDatabaseInitialized();
            Seed = MySQL.GetSeed("Players", "UserID");
        }

        private static void EnsureDatabaseInitialized()
        {
            if (Initialized)
                return;

            lock (InitLock)
            {
                if (Initialized)
                    return;

                using (var Database = new Context())
                {
                    var Warmup = Database.Players.FirstOrDefault();
                }

                Initialized = true;
            }
        }

        private static string GenerateToken(int length = 40)
        {
            const string Chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            var Buffer = new char[length];

            lock (TokenRandomLock)
            {
                for (var i = 0; i < length; i++)
                    Buffer[i] = Chars[TokenRandom.Next(Chars.Length)];
            }

            return new string(Buffer);
        }

        internal void Add(Player Player)
        {
            if (ContainsKey(Player.UserID))
            {
                if (!TryUpdate(Player.UserID, Player, Player))
                {
                    // Debug.WriteLine("[*] " + this.GetType().Name + " : " + "Unsuccessfuly updated the specified player to the dictionnary.");
                }
            }
            else
            {
                if (!TryAdd(Player.UserID, Player))
                {
                }
            }
        }

        internal void Remove(Player Player)
        {
            Player TmpPlayer;
            if (ContainsKey(Player.UserID))
                if (!TryRemove(Player.UserID, out TmpPlayer))
                {
                }
        }

        internal Player GetPlayer(Device Device, long UserID, bool Store = true)
        {
            if (!ContainsKey(UserID))
            {
                Player Player = null;
                using (var Database = new Context())
                {
                    var Data = Database.Players.Find(UserID);
                    if (Data != null)
                        if (!string.IsNullOrEmpty(Data.Data))
                        {
                            Player = new Player(null, UserID);
                            JsonConvert.PopulateObject(Data.Data, Player);
                            if (Store)
                                Add(Player);
                        }
                }
                return Player;
            }
            return this[UserID];
        }

        internal Player CreatePlayer(Device Device, long UserID = 0, bool Store = true)
        {
            var Player = new Player(Device, Interlocked.Increment(ref Seed));
            Player.Token = GenerateToken();

            using (var Database = new Context())
            {
                var PlayerModel = new PlayerModel
                {
                    UserID = Player.UserID,
                    Data = JsonConvert.SerializeObject(Player, Formatting.Indented)
                };
                Database.Players.Add(PlayerModel);
                Database.SaveChanges();
                if (Store)
                    Add(Player);
            }

            return Player;
        }

        internal void Save(Player Player)
        {
            using (Context Database = new Context())
            {
                PlayerModel Data = Database.Players.Find(Player.UserID);
                if (Data != null)
                {
                    Data.Data = JsonConvert.SerializeObject(Player, Formatting.Indented);
                    Database.SaveChanges();
                }
            }
        }

        internal void SaveAll()
        {
            Player[] Players = this.Values.ToArray();
            Parallel.ForEach(Players, Player =>
            {
                try
                {
                    this.Save(Player);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }
    }
}