using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using RMX;  namespace Procrastinate {
	public class DebugHUD : Bugger.DebugHUD {

		
		string GetTime(object key) {
			var time = SavedData.Get<float> (key);
			var timeString = time > 0 ? TextFormatter.TimeDescription (time) : time.ToString ();
			return "\n – " + key.ToString () + ": <color=yellow>" + timeString + "</color>";
		}

		void Start() {
			Show ();
		}

		protected override string DebugData {
			get {
				string info = "DEBUG =>";
				foreach (KeyValuePair<UserData,string> data in GameCenter.UniqueID) {
					var val = data.Key == UserData.sc_longest_procrastination ? GetTime (data.Key)
					: SavedData.Get<string> (data.Key).Length == 0 ? "False" 
						: SavedData.Get<string> (data.Key);
					info += "\n – " + data.Key.ToString () + ": " + val;
				}
				info += GetTime (UserData.gd_current_procrastination);
				info += GetTime (UserData.gd_current_session);
				info += GetTime (UserData.gd_total_time_Wasted);
				info += "\nClockSpawnMode: " + GameController.current.ClockSpawnMode;
				info += "\n" + DragRigidbody.current;
				return info;
			}

		}

	}
}