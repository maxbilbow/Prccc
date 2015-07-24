//using UnityEngine;
//using System.Collections;
using RMX;

namespace Procrastinate {
	public class Events : RMX.Events {
		public static Event SpawnMultipleClocks	 	= new Event("SpawnMultipleClocks");
		public static Event SpawnInflatableClock 	= new Event("SpawnInflatableClock");
		public static Event ClockIsAboutToBurst		 = new Event("ClockIsAboutToBurst");
		public static Event SomethingBurst			 = new Event("SomethingBurst");
	}
}