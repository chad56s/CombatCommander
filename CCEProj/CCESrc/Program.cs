using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CW_Utils;
using System.Diagnostics;

namespace CombatCommander {
	class Program {

		static void Main(string[] args) {
            GameManager myGameMgr = new GameManager("1", new ComputerPlayer(FACTION.AXIS), new ComputerPlayer(FACTION.ALLIES));
            myGameMgr.PlayGame();
		}
	}
}
