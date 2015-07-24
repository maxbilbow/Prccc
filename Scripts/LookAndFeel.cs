using UnityEngine;
using System.Collections;

namespace RMX {
	public class LookAndFeel : Singletons.ASingleton<LookAndFeel> {

		public Font mainFont;
		public Background background;


		public struct Background {
			public Color color;
		}
	}

}