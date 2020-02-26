﻿using AAEmu.Game.Core.Managers;
using AAEmu.Game.Core.Managers.World;
using AAEmu.Game.Models.Game;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Core.Packets.G2C;

namespace AAEmu.Game.Scripts.Commands
{
    public class Snow : ICommand
    {

        public void OnLoad()
        {
            ///register script
            CommandManager.Instance.Register("snow", this);
        }

        public void Execute(Character character, string[] args)
        {
            // If no argument is provided send usage information
            if (args.Length == 0)
            {
                character.SendMessage("[Snow] /snow <true/false>");
                return;
            }

            // determine if we recived true,false or something else             
            if (bool.TryParse(args[0], out var isSnowing))
            {
                //Set Snowing state to user input, This will 
                // enable Snow on all players who login to the server
                WorldManager.Instance.IsSnowing = isSnowing;

                //Turn snow on or off for all online characters,
                //put this on the script level so it only gets executed once when GM enables/disables snow
                WorldManager.Instance.BroadcastPacketToServer(new SCOnOffSnowPacket(isSnowing));

                //announce to all players on the server snow was enabled or disabled
                WorldManager.Instance.BroadcastPacketToServer(new SCNoticeMessagePacket(3, "9#0823d1", 3000, "  Snow Was set to " + isSnowing.ToString()));
            }
            else
            {
                // user input was invalid notify them
                character.SendMessage("[Snow] Use true or false.");
            }


        }
    }
}
