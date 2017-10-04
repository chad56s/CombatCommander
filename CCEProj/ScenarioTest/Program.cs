using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CombatCommander;
using System.Xml;
using System.Xml.Linq;

using System.IO;

namespace ScenarioTest {
	class Program {
		static void Main(string[] args) {

            Player axisCommander, alliesCommander;
            GameManager myGame;

            Directory.SetCurrentDirectory(@"..\..\..\..\");
			//xDocMap = XDocument.Load(@"C:\users\a3skszz\Documents\Chad\CCE\map1.xml");

			axisCommander = new ComputerPlayer(FACTION.AXIS);
			alliesCommander = new ComputerPlayer(FACTION.ALLIES);

			myGame = new GameManager("1", axisCommander, alliesCommander);

			myGame.Start();
		}
	}
}
