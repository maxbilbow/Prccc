using UnityEngine;
using System.Collections;
using RMX;  

namespace Procrastinate {
	public class Version  {
	
		public const float v0_3_6 = 3.06f;
		public const float v0_3_5 = 3.05f;

		public const string Key = "current_version";

		public static float latest {
			get {
				return v0_3_6;
			}
		}

		private static bool _failedPatch = false;
		private static float currentVersion {
			get {
				return PlayerPrefs.GetFloat (Version.Key);
			} set {
				PlayerPrefs.SetFloat(Version.Key, value);
			}
		}

		public static bool needsPatch {
			get {
				return currentVersion < latest && !_failedPatch;
			}
		}
		
		enum Type {
			Float, Int, String
		}

		public static void Patch() {
			if (needsPatch) {
				var log = Bugger.StartNewLog(Testing.Patches);
				try {
					PatchX ();
					currentVersion = latest;
//					needsPatch = false;
					log.message += "Update Successful";
				} catch (UnityException e) {
					_failedPatch = true;
					log.message += "Update failed: " + e.Message;
				}
				if (log.isActive)
					Debug.Log (log);
			} 
//			else if (Bugger.WillTest (Testing.Patches)) {
//				var log = Bugger.StartLog(Testing.Patches);
//				log.message = "Does not need patch: v" + currentVersion;
//				Debug.Log (log);
//			}
		}


		private static void PatchX() {
			if (currentVersion < v0_3_5) {
				Patchv0_3_5();
			}
			if (currentVersion < v0_3_6) {
				Patchv0_3_6();
			}
		}

		private static void Patchv0_3_6() {
//			needsPatch = false;
			Set (UserData.CurrentSession);
			Set (UserData.CurrentProcrastination);
			Set (UserData.TotalTime);
			Set (UserData.LongestProctrastination);
			// Top Scores
			Set (UserData.PercentageOfDevTime);
			// Achievements
			Set (UserData.AmeteurCrastinator);
			Set (UserData.TimeWaster);
			Set (UserData.OverTime);
			Set (UserData.SemiPro);
			Set (UserData.Apathetic);
			Set (UserData.Pro);
			Set (UserData.NotFirstTime);
			currentVersion = v0_3_6;
		}
		
		private static void Patchv0_3_5() {
//			needsPatch = false;
			Setf (UserData.CurrentSession);
			Setf (UserData.CurrentProcrastination);
			Setf (UserData.TotalTime);
			Setf (UserData.LongestProctrastination);
			currentVersion = v0_3_5;
		}
		
		
		
		private static string OldKey(UserData data) {
			float version = currentVersion;
			if (version < v0_3_5) {
				switch (data) {
				case UserData.CurrentSession:
					return "last session";
				case UserData.CurrentProcrastination:
					return "last uninterupted";
				case UserData.TotalTime:
					return "Total Time Wasted";
				case UserData.LongestProctrastination:
					return "longestProcrastination";
				case UserData.AmeteurCrastinator:
					return "Ameteur Crastinator";
				case UserData.TimeWaster:
					return "Time Waster";
				case UserData.SemiPro:
					return "Semi-Pro";
				case UserData.Apathetic:
					return "Apathetic";
				case UserData.Pro:
					return "Pro-Crastinator";
				}
			} else if (version < v0_3_6) {//change to next patch
				switch (data) {
					// Saved Game Data
				case UserData.CurrentSession:
					return "last_session";
				case UserData.CurrentProcrastination:
					return "last_uninterupted";
				case UserData.TotalTime:
					return "Total_Time_Wasted";
				case UserData.LongestProctrastination:
					return "longest_Procrastination";
					
					// Top Scores
				case UserData.PercentageOfDevTime:
					return "total_as_percent_of_dev";
					
					// Achievements
				case UserData.AmeteurCrastinator:
					return "AmeteurCrastinator";
				case UserData.TimeWaster:
					return "Time_Waster";
				case UserData.OverTime:
					return "overtime";
				case UserData.SemiPro:
					return "Semi_Pro";
				case UserData.Apathetic:
					return "Apathetic";
				case UserData.Pro:
					return "Pro_Crastinator";
					
					// Other System and Game Data
				case UserData.NotFirstTime:
					return "Has_Played_Before";
				case UserData.Version:
					return Version.Key;
				}
			}
			return null;
		}


		private static void Setf(UserData key) {
			Set (key, Type.Float);
		}

		private static void Set(UserData key) {
			Set (key, Type.String);
		}

		private static void Set(UserData key, Type type) {
			var record = Bugger.StartNewLog (Testing.Patches);
			object value = null;// = PlayerPrefs.GetFloat
			var oldKey = OldKey (key);
			var newKey = SavedData.GetKey (key);
			record.message += "Updating <color=orange>'" + oldKey + "'</color> to <color=green>'" + newKey + "'</color>: ";
			if (PlayerPrefs.HasKey (oldKey)) {
				switch (type) {
				case Type.Float:
					value = PlayerPrefs.GetFloat (oldKey);
					break;
				case Type.Int:
					value = PlayerPrefs.GetInt (oldKey);
					break;
				case Type.String:
					value = PlayerPrefs.GetString (oldKey);
					break;
				default:
					throw new UnityException (record + "\n <color=red>FAILED: Could not be updated! </color>");
				}

				PlayerPrefs.SetString (newKey, value.ToString ());
				if (oldKey != newKey) {
					PlayerPrefs.DeleteKey (oldKey);
					record.message += "\n Deleting old key";
				} else {
					record.message += "\n No need to delete old key";
				}
				record.message += "\n<color=green>SUCCESS</color>";
			} else {
				record.message += "\n<color=blue>Not updating: </color> Old Key does not exist.";
			}
			if (record.isActive)
				Debug.Log (record);

		}



	}
}
