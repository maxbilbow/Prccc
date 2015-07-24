using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RMX {
	public enum Testing {
		Misc, GameCenter, Achievements, Exceptions, GameDataLists, Singletons, Patches, Database, EventCenter
		
	}

	public class Settings : ASingleton<Settings> {

		public int MaxNumberOfClocks = 50; 
		public TextAsset Database;
		public float FingerSize = 0.3f;
		public ClockSpawner.SpawnMode ClockSpawnMode;// = ClockSpawner.SpawnMode.Inflate;
		public bool willPauseOnLoad = false;
		public bool newPersonalBest = false;
		#if UNITY_ANDROID
//		public bool beta = false;
		public bool printToScreen = false;
		#else
//		public bool beta = true;
		public bool printToScreen = true;
		#endif
//		public struct DebugSettings {
//			public bool beta;
//			public bool printToScreen;
//		}
//


		public float updateScoresEvery = 1f;
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
			if (!Database) {
				Debug.LogWarning("database asset not set");
			}
			if (Random.Range(1,10) > 5)
				ClockSpawnMode = ClockSpawner.SpawnMode.Multiply;
		}


		public bool IsDebugging(Testing feature) {
			if (!GameController.IsInitialized) {
				Debug.LogWarning ("GameController was not initialized before trying to test " + feature.ToString ());
				return false;
			} else {
				switch (feature) {
				case Testing.Misc:
					return DebugMisc;
				case Testing.GameCenter:
					return DebugGameCenter;
				case Testing.Achievements:
					return DebugAchievements;
				case Testing.Exceptions:
					return DebugExceptions;
				case Testing.Singletons:
					return DebugSingletons;
				case Testing.GameDataLists:
					return DebugGameDataLists;
				case Testing.Patches:
					return DebugPatches;
				case Testing.Database:
					return DebugDatabase;
				case Testing.EventCenter:
					return DebugEvents;
				default:
					Debug.LogWarning (feature.ToString () + " has not been recorded in Settings.IsTesting(feature)");
					return false;
				}
			}
		}
		void Update() {
#if UNITY_EDITOR

#endif

		}
//		const string tempName = "324329hrNhfeuwh9";
//		public static T CreateSingleton<T>() where T : ASingleton<T>
//		{
//			if (FindObjectOfType(T)) {// T.GetSingleton() != null) {
//				return T.current;
//			} else if (GameController.IsInitialized || typeof(T) == typeof(GameController)) {
//				var aSingleton = new GameObject (tempName).AddComponent<T> ();
//				if ((aSingleton as ASingleton<T>).Destroyed) {
//					return null;
//				}
//				aSingleton.gameObject.name = aSingleton.GetType ().Name;
//				if (!(aSingleton is GameController)) {
//					var parent = GameController.current.gameObject;//GameObject.Find(rootName);
//					aSingleton.transform.SetParent (parent.transform);
//				}
//				return aSingleton;
//			} else {
//				Debug.LogError("Gamecontroller should happen before this...");
//				GameController.lateInits.Add(Initialize);
//				return null;
//			}
//		}
	}
}
