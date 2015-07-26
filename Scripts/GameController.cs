using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RMX; 



namespace Procrastinate {

	public class GameController : AGameController<GameController> {
		public int MaxNumberOfClocks = 50; 
		
		
		
		public float FingerSize = 0.3f;
		public ClockSpawner.SpawnMode ClockSpawnMode;// = ClockSpawner.SpawnMode.Inflate;
		 bool willPauseOnLoad = false;
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

		protected override void PostStart ()
		{

		}
		
		void OnApplicationFocus(bool focusStatus) {
			if (!focusStatus) {
				PauseGame(true);
			}
		}

		void OnApplicationPause(bool pause) {
			if (pause) {
				PauseGame(true);
			}
		}


		public void PauseGame(bool Pause) {
			if (willPauseOnLoad) {
				PauseGame (true, Event.FirstPause);
				willPauseOnLoad = false;
			} else {
				PauseGame(true, null);
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

		public override void PauseGame(bool pause, object args) {
			if (Bugger.WillLog (RMXTests.Misc, "Pause: " + pause + ", args: " + (args != null ? args.ToString():"none")))
				Debug.Log (Bugger.Last);
			if (pause && !isPaused) {
				WillBeginEvent (RMX.Event.PauseSession, args);
				Time.timeScale = 0;
				DidFinishEvent (RMX.Event.PauseSession, args);
			} else if (!pause && isPaused) {
				WillBeginEvent (RMX.Event.ResumeSession, args);
				Time.timeScale = 1;
				willPauseOnLoad = false;
				DidFinishEvent (RMX.Event.ResumeSession, args);
			} else if (Bugger.WillLog (RMXTests.Misc, "Pause: " + pause + ", args: " + (args != null ? args.ToString():args))) {
					Debug.LogWarning ("Superflouous PauseGame(" + pause + ") call");
			}

		}

		public override bool IsDebugging(System.Enum feature) {
			if (_buildForRelease)
				return false;
			else if (feature.Equals(Tests.GameDataLists))
				return DebugGameDataLists;
			else 
				return base.IsDebugging (feature);
		
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				PauseGame(!isPaused, null);
			}
		}

		
	}
}
