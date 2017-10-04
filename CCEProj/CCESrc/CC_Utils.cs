using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {
	public static class CC_Utils {

		public static FACTION StringToFaction(String s, bool bRequireSide=true) {
			FACTION f = FACTION.SPECIAL;
			try {
				f = (FACTION)Enum.Parse(typeof(FACTION), s.ToUpper());
			}
			catch (Exception e){
				if(bRequireSide)
					throw e;
			}
			return f;
		}
	}
}
