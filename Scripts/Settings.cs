using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using RMX; 

namespace Procrastinate {	
	public class Settings : ASettings<Settings> {

		public int MaxNumberOfClocks = 50; 
		public TextAsset _database;

		public override TextAsset Database {
			get {
				return _database;
			}
		}
	
		public float FingerSize = 0.3f;
		public ClockSpawner.SpawnMode ClockSpawnMode;// = ClockSpawner.SpawnMode.Inflate;
		public bool willPauseOnLoad = false;
		public bool newPersonalBest = false;

//		public struct DebugSettings {
//			public bool beta;
//			public bool printToScreen;
//		}
//


		public float updateScoresEvery = 1f;

		public override bool PrintToScreen {
			get {
				return _printToScreen;
			} set {
				_printToScreen = value;
			}
		}
		public bool _printToScreen = true;
		public bool DebugMisc;
		public bool DebugGameCenter;
		public bool DebugAchievements;
		public bool DebugExceptions;
		public bool DebugSingletons;
		public bool DebugGameDataLists;
		public bool DebugDatabase;
		public bool DebugPatches;
		public bool DebugEvents;
		public bool ClearAchievementsOnLoad;


	
		public float maxDisplayTime = 5f;

		public override float MaxDisplayTime  {
			get {
				return maxDisplayTime;
			}
		}
	
		/// <summary>
		/// The dev time wasted.
		/// </summary>
		public float TotalDevTimeWasted = 5 * 60 * 60;


		private int _chance = 50;
		public bool Chance {
			get {
				return Random.Range(0,100) <= _chance;
			}
		}
		public bool ChanceGiven(UserData key) {
			return Chance && GameCenter.HasPlayerAlreadyAchieved (key);
		}


		void Start() {
			_chance = Random.Range (10,90);

#if DEBUG
			if (Settings.current.ClearAchievementsOnLoad) {
				//				Debug.LogWarning("Deleting: " + key);
				//				PlayerPrefs.DeleteKey (key);
				PlayerPrefs.DeleteAll();
			}
#endif
			if (!_database) {
				Debug.LogWarning("database asset not set");
			}
			if (Random.Range(1,10) > 5)
				ClockSpawnMode = ClockSpawner.SpawnMode.Multiply;
		}



		void Update() {
#if UNITY_EDITOR

#endif
		}

		public static bool ShouldDebug(string feature) {
//			return false;
			if (Singletons.Settings != null) {
				var settings = Singletons.Settings as Settings;
				if (feature == Testing.Misc)
					return settings.DebugMisc;
				else if (feature == Testing.GameCenter)
					return settings.DebugGameCenter;
				else if (feature == Testing.Achievements)
					return settings.DebugAchievements;
				else if (feature == Testing.Exceptions)
					return settings.DebugExceptions;
				else if (feature == Testing.Singletons)
					return settings.DebugSingletons;
				else if (feature == Tests.GameDataLists)
					return settings.DebugGameDataLists;
				else if (feature == Testing.Patches)
					return settings.DebugPatches;
				else if (feature == Testing.Database)
					return settings.DebugDatabase;
				else if (feature == Testing.EventCenter)
					return settings.DebugEvents;
				else
					Debug.LogWarning (feature.ToString () + " has not been recorded in Settings.IsTesting(feature)");
				return false;
			} else {
				Debug.LogWarning("Setting not initialized so debugging anyway: " + feature);
				return true;
			}
		}

		public override bool IsDebugging(string feature) {
			return ShouldDebug(feature);
		}
	}
}
