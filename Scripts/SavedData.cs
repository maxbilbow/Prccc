using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

using RMX;  namespace Procrastinate {

	public enum UserData {
		// Saved Game Data
		CurrentSession, CurrentProcrastination, TotalTime,
		// Saved & Top Scores
		LongestProctrastination,
		// Top Scores
		PercentageOfDevTime, 

		// Achievements (time based)
		AmeteurCrastinator, TimeWaster, Apathetic, SemiPro, Pro, 
		// Achievements (Event Based)
		MakingTime, OverTime, BigTime,

		// Other System and Game Data
		Version, NotFirstTime
	}



	public class SavedData : ScriptableObject {

		private string key;
		private static bool first = true;


		void Awake() {
			if (first) {
				Version.Patch ();
				first = false;
			}
		}

		public static SavedData New(UserData key) {
			var sd = ScriptableObject.CreateInstance<SavedData>();
			sd.key = GetKey(key);
			return sd;
		}

		public string String {
			get {
				return PlayerPrefs.GetString(key);
			}
		}

		public long Long {
			get {
				float result;
//				if (!long.TryParse (PlayerPrefs.GetString (key), out result))
//					Debug.LogWarning(key.ToString() + " GET: " + PlayerPrefs.GetString (key));
				return float.TryParse (PlayerPrefs.GetString (key), out result) ? (long) result : -1;
			} 
//			set {
//				if (Bugger.WillTest(Tests.Exceptions)) {
//					var result = PlayerPrefs.GetString (key);
//					if (result == TRUE || result == FALSE)
//						Debug.LogWarning("\"" + value + "\" should be True or False");
//				}
//				PlayerPrefs.SetString(key, value.ToString());
//			}
		}
		const string TRUE = "True", FALSE = "False";
		public bool Bool {
			get {
				if (Settings.ShouldDebug(Tests.Exceptions)) {
					var result = PlayerPrefs.GetString (key);
					if (result != TRUE && result != FALSE && result != "") {
						var log = Bugger.StartNewLog(Tests.Exceptions, key + ": \"" + result + "\" should be True or False");
						Debug.Log(log);
					}
				}

//				Debug.LogWarning(key.ToString() + " GET: " + PlayerPrefs.GetString (key));
				return PlayerPrefs.GetString (key) == TRUE;
			} set {
				if (Settings.ShouldDebug(Tests.Exceptions)) {
					var result = PlayerPrefs.GetString (key);
					if (result != TRUE || result != FALSE)
						Debug.LogWarning(key + ": \"" + value + "\" should be a number");
				}
//				Debug.LogWarning(key.ToString() + " SET: " + value);
				PlayerPrefs.SetString(key, value ? TRUE : FALSE);
			}
		}

		public int Int {
			get {
				int result;
				return int.TryParse(PlayerPrefs.GetString(key), out result) ? result : -1;
			}
			set {
				if (Settings.ShouldDebug(Tests.Exceptions)) {
					var result = PlayerPrefs.GetString (key);
					if (result == TRUE || result == FALSE)
						Debug.LogWarning(key + ": \"" + value + "\" should be True or False");
				}
				PlayerPrefs.SetString(key,value.ToString());
			}
		}

		public float Float {
			get {
				float result;
				return float.TryParse(PlayerPrefs.GetString(key), out result) ? result : -1;
			} set {
				if (Settings.ShouldDebug(Tests.Exceptions)) {
					var result = PlayerPrefs.GetString (key);
					if (result == TRUE || result == FALSE)
						Debug.LogWarning(key + ": \"" + value + "\" should be True or False");
				}
				PlayerPrefs.SetString(key,value.ToString());
			}
		}

		public double Double {
			get {
				double result;
				return double.TryParse(PlayerPrefs.GetString(key), out result) ? result : -1;
			} set {
				if (Settings.ShouldDebug(Tests.Exceptions)) {
					var result = PlayerPrefs.GetString (key);
					if (result == TRUE || result == FALSE)
						Debug.LogWarning(key + ": \"" + value + "\" should be True or False");
				}
				PlayerPrefs.SetString(key,value.ToString());
			}
		}

		public override string ToString() {
			return this.String;
		}

		
		/// <summary>
		/// Gets the String Key used to get and set PlayerPrefs. This is NOT the same as the ID used by GameKit
		/// </summary>
		/// <returns>The key.</returns>
		/// <param name="data">Data.</param>
		public static string GetKey(UserData data) {
			switch (data) {
			// Saved Game Data
			case UserData.CurrentSession:
				return "gd_current_session";
			case UserData.CurrentProcrastination:
				return "gd_current_procrastination";
			case UserData.TotalTime:
				return "gd_total_time_Wasted";
			case UserData.LongestProctrastination:
				return "sc_longest_procrastination";

			// Top Scores
			case UserData.PercentageOfDevTime:
				return "sc_total_as_percent_of_dev";

			// Time Based Achievements
			case UserData.AmeteurCrastinator:
				return "ach_ameteur_crastinator";
			case UserData.TimeWaster:
				return "ach_time_waster";
			case UserData.SemiPro:
				return "ach_semi_pro";
			case UserData.Apathetic:
				return "ach_apathetic";
			case UserData.Pro:
				return "ach_pro_crastinator";

			// Event based Achievements
			case UserData.OverTime:
				return "ach_overtime";
			case UserData.MakingTime:
				return "ach_making_time";
			case UserData.BigTime:
				return "ach_big_time";

			// Other System and Game Data
			case UserData.NotFirstTime:
				return "gd_has_played_before";
			case UserData.Version:
				return Version.Key;
			}
			throw new System.Exception("No Key! " + data.ToString());
//			return null;
		}

		private static Dictionary<UserData, SavedData> _data = new Dictionary<UserData, SavedData>() {
			{ UserData.NotFirstTime, SavedData.New(UserData.NotFirstTime) } 
		};

		public static SavedData Get(UserData key) {
			if (!_data.ContainsKey (key)) {
				_data[key] = SavedData.New(key);
			}
			return _data[key];
		}
	}
}
