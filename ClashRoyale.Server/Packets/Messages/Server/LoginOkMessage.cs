using System;
using ClashRoyale.Server.Core;
using ClashRoyale.Server.Logic;
using ClashRoyale.Server.Managers;
using ClashRoyale.Server.Utilities;

namespace ClashRoyale.Server.Packets.Messages.Server
{
    internal class LoginOkMessage : Message
    {
        public LoginOkMessage(Device Device) : base(Device)
        {
            PacketID = 22280;
        }

        internal override void Encode()
        {
            Data.AddInt64(Device.Player.UserID);
            Data.AddInt64(Device.Player.UserID);

            Data.AddString(Device.Player.Token);

            Data.AddString(null); // FacebookID
            Data.AddString(null); // Gamecenter ID

            Data.AddVInt(3); // Server Major
            Data.AddVInt(830); // Server Minor
            Data.AddVInt(830); // Server Minor
            Data.AddVInt(0); // Server Build

            Data.AddString("prod");

            Data.AddVInt(0); // Session Count
            Data.AddVInt(0); // Login Count
            Data.AddVInt(0); // No idea :p

            Data.AddString("1475268786112433");

            Data.AddString(TimeManager.GetUnixTime);

            Data.AddString(TimeManager.GetUnixTime);

            Data.AddByte(0);

            Data.AddString(null);
            Data.AddString(null);
            Data.AddString(null);

            Data.AddString(Device.Player.Country);
            Data.AddString(Device.Player.City);
            Data.AddString(Device.Player.Region);

            Data.AddVInt(1);
            Data.AddVInt(455500);
            Data.AddVInt(115500);
            
            Data.AddVInt(2);
            Data.AddString("");
            Data.AddString("https://game-assets.clashroyaleapp.com");
            Data.AddVInt(1);
            Data.AddString("");
        }

        internal override void Process()
        {
            Resources.Players.Save(this.Device.Player);

            Console.WriteLine("Saving?");
        }
    }
}
