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

		public override bool IsDebugging(string feature) {
			if (!Singletons.GameControllerInitialized) {
				Debug.LogWarning ("GameController was not initialized before trying to test " + feature);
				return false;
			} else {
				if (feature == Testing.Misc)
					return DebugMisc;
				else if (feature == Testing.GameCenter)
					return DebugGameCenter;
				else if (feature == Testing.Achievements)
					return DebugAchievements;
				else if (feature == Testing.Exceptions)
			         return DebugExceptions;
		        else if (feature == Testing.Singletons)
			         return DebugSingletons;
	      	   else if (feature == Tests.GameDataLists)
					return DebugGameDataLists;
				else if (feature == Testing.Patches)
					return DebugPatches;
				else if (feature == Testing.Database)
					return DebugDatabase;
				else if (feature == Testing.EventCenter)
					return DebugEvents;
				else
					Debug.LogWarning (feature.ToString () + " has not been recorded in Settings.IsTesting(feature)");
				return false;
			}
		}
	}
}
