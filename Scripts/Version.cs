using UnityEngine;
using System.Collections;
using RMX;  

namespace Procrastinate {

	public enum UserData {
		// Saved Game Data
		gd_current_session, gd_current_procrastination, gd_total_time_Wasted,
		// Saved & Top Scores
		sc_longest_procrastination,
		// Top Scores
		sc_total_as_percent_of_dev, 
		
		// Achievements (time based)
		ach_ameteur_crastinator, ach_time_waster, ach_apathetic, ach_semi_pro, ach_pro_crastinator, 
		// Achievements (Event Based)
		ach_making_time, ach_overtime, ach_big_time,
		
		// Other System and Game Data
		current_version, gd_has_played_before
	
	}

	public class Version  {
	
		public const float v0_3_6 = 3.06f;
		public const float v0_3_5 = 3.05f;

//		public const string Key = UserData.current_version.ToString ();

		public static float latest {
			get {
				return v0_3_6;
			}
		}

		private static bool _failedPatch = false;
		private static float currentVersion {
			get {
				return SavedData.Get<float> (UserData.current_version);
			} set {
				SavedData.Set(UserData.current_version, value);
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
				var log = "";
				try {
					PatchX ();
					currentVersion = latest;
//					needsPatch = false;
					log += "Update Successful";
				} catch (UnityException e) {
					_failedPatch = true;
					log += "Update failed: " + e.Message;
				}
				if (Bugger.WillLog(Testing.Patches,log))
					Debug.Log (Bugger.Last);
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

//			foreach (UserData key in System.Enum.GetValues(typeof(UserData)))
//			{
//				Set (key);
//			}
			Set (UserData.gd_current_session);
			Set (UserData.gd_current_procrastination);
			Set (UserData.gd_total_time_Wasted);
			Set (UserData.sc_longest_procrastination);
			// Top Scores
			Set (UserData.sc_total_as_percent_of_dev);
			// Achievements
			Set (UserData.ach_ameteur_crastinator);
			Set (UserData.ach_time_waster);
			Set (UserData.ach_overtime);
			Set (UserData.ach_semi_pro);
			Set (UserData.ach_apathetic);
			Set (UserData.ach_pro_crastinator);
			Set (UserData.gd_has_played_before);
			currentVersion = v0_3_6;
		}
		
		private static void Patchv0_3_5() {
//			needsPatch = false;
			Setf (UserData.gd_current_session);
			Setf (UserData.gd_current_procrastination);
			Setf (UserData.gd_total_time_Wasted);
			Setf (UserData.sc_longest_procrastination);
			currentVersion = v0_3_5;
		}
		
		
		
		private static string OldKey(UserData data) {
			float version = currentVersion;
			if (version < v0_3_5) {
				switch (data) {
				case UserData.gd_current_session:
					return "last session";
				case UserData.gd_current_procrastination:
					return "last uninterupted";
				case UserData.gd_total_time_Wasted:
					return "Total Time Wasted";
				case UserData.sc_longest_procrastination:
					return "longestProcrastination";
				case UserData.ach_ameteur_crastinator:
					return "Ameteur Crastinator";
				case UserData.ach_time_waster:
					return "Time Waster";
				case UserData.ach_semi_pro:
					return "Semi-Pro";
				case UserData.ach_apathetic:
					return "Apathetic";
				case UserData.ach_pro_crastinator:
					return "Pro-Crastinator";
				}
			} else if (version < v0_3_6) {//change to next patch
				switch (data) {
					// Saved Game Data
				case UserData.gd_current_session:
					return "last_session";
				case UserData.gd_current_procrastination:
					return "last_uninterupted";
				case UserData.gd_total_time_Wasted:
					return "Total_Time_Wasted";
				case UserData.sc_longest_procrastination:
					return "longest_Procrastination";
					
					// Top Scores
				case UserData.sc_total_as_percent_of_dev:
					return "total_as_percent_of_dev";
					
					// Achievements
				case UserData.ach_ameteur_crastinator:
					return "AmeteurCrastinator";
				case UserData.ach_time_waster:
					return "Time_Waster";
				case UserData.ach_overtime:
					return "overtime";
				case UserData.ach_semi_pro:
					return "Semi_Pro";
				case UserData.ach_apathetic:
					return "Apathetic";
				case UserData.ach_pro_crastinator:
					return "Pro_Crastinator";
					
					// Other System and Game Data
				case UserData.gd_has_played_before:
					return "Has_Played_Before";
				case UserData.current_version:
					return UserData.current_version.ToString();
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
			var record = "";
			object value = null;// = PlayerPrefs.GetFloat
			var oldKey = OldKey (key);
			var newKey = key.ToString ();
			record += "Updating <color=orange>'" + oldKey + "'</color> to <color=green>'" + newKey + "'</color>: ";
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
					record += "\n Deleting old key";
				} else {
					record += "\n No need to delete old key";
				}
				record += "\n<color=green>SUCCESS</color>";
			} else {
				record += "\n<color=blue>Not updating: </color> Old Key does not exist.";
			}
			if (Bugger.WillLog(Testing.Patches,record))
				Debug.Log (Bugger.Last);

		}



	}
}
