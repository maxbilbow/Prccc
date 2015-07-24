using UnityEngine;
using System.Collections;

namespace RMX {
	public class LookAndFeel : ASingleton<LookAndFeel> {

		public Font mainFont;
		public Background background;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public struct Background {
			public Color color;
		}
	}

}