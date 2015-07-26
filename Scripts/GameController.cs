using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RMX; 

namespace Procrastinate {
	public class GameController : AGameController<GameController>, IGameController {

		public int MaxNumberOfClocks = 50; 
		
		
		
		public float FingerSize = 0.3f;
		public ClockSpawner.SpawnMode ClockSpawnMode;// = ClockSpawner.SpawnMode.Inflate;
		public bool willPauseOnLoad = false;
		public bool newPersonalBest = false;

		public float updateScoresEvery = 1f;
		public float _totalDevTimeWastedInHours = 5;
		public float TotalDevTimeWasted {
			get {
				return _totalDevTimeWastedInHours * 60 * 60;
			}
		}
		
		private int _chance = 50;
		public bool Chance {
			get {
				return Random.Range(0,100) <= _chance;
			}
		}
		
		public bool ChanceGiven(UserData key) {
			return Chance && GameCenter.HasPlayerAlreadyAchieved (key);
		}


		public Vector2 velocity {
			get {
				return new Vector2(transform.forward.x, transform.forward.y);
			}
		}


		public static bool isFirstPlay {
			get {
				return PlayerPrefs.GetString(UserData.gd_total_time_Wasted.ToString()) != null;
			}
		}


	

//		public delegate ISingleton LateInit();


		protected override void StartSingletons() {
			GameCenter.Initialize ();
			GameData.Initialize ();
			DragRigidbody.Initialize ();
			ClockSpawner.Initialize ();
			PauseCanvas.Initialize ();

		}


		protected override void StartDesktop() {

		}

		protected override void StartMobile() {
			Gyro.Initialize();
		}




		
		

		
		protected override void PreStart() {
			_chance = Random.Range (10,90);
			
			#if DEBUG
			if (ClearAchievementsOnLoad) {
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
		
		
		public static bool ShouldDebug(string feature) {
			if (Singletons.GameControllerInitialized) {
				var settings = Singletons.GameController as GameController;
				if (feature == Tests.GameDataLists)
					return settings.DebugGameDataLists;
				return false;
			}
			throw new UnityException ("Setting should be initialized by now (Testing: " + feature);
			
		}
		
		public override bool IsDebugging(string feature) {
			return ShouldDebug(feature) || base.IsDebugging(feature);
		}
		
		void OnApplicationFocus(bool focusStatus) {
			if (!focusStatus) {
				PauseGame (true, null);
			}
		}

		public static void CheckForAnomalies() {
			ClockBehaviour.CheckVisibleClocks ();
		}

		public override void Patch() {
			Version.Patch ();
		}

		public bool isPaused {
			get {
				return Time.timeScale == 0;
			}
		}

		public override void PauseGame(bool pause, object args = null) {
			if (pause) {
				WillBeginEvent (Events.PauseSession, args);
				Time.timeScale = 0;
				DidFinishEvent (Events.PauseSession, args);
			} else {
				WillBeginEvent(Events.ResumeSession, args);
				Time.timeScale = 1;
				DidFinishEvent(Events.ResumeSession, args);
			}

		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				PauseGame(!isPaused, null);
			}
		}

		
	}
}
