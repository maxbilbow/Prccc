using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityStandardAssets.CrossPlatformInput;

namespace RMX {
	public class GameController : ASingleton<GameController> , EventListener {
		public Vector2 defaultGravity = new Vector2 (0f, -9.81f);

		public Vector2 velocity {
			get {
				return new Vector2(transform.forward.x, transform.forward.y);
			}
		}

		public PauseCanvas pauseCanvas {
			get {
				return PauseCanvas.current;
			}
		}
		public static bool isFirstPlay {
			get {
				return PlayerPrefs.GetString(SavedData.GetKey(UserData.TotalTime)) != null;
			}
		}


//		public delegate ISingleton LateInit();


		void StartSingletons() {
			Notifications.EventWillStart (Event.SingletonInitialization);
			Settings.Initialize ();
			Bugger.Initialize ();
			GameCenter.Initialize ();
			GameData.Initialize ();
			DataReader.Initialize ();
			Timer.Initialize ();
			DragRigidbody.Initialize ();
			ClockSpawner.Initialize ();
			PauseCanvas.Initialize ();
			Notifications.Initialize ();
			PauseCanvas.Initialize ();
			#if MOBILE_INPUT
			StartMobile();
			#else
			StartDesktop();
			#endif
			Notifications.EventDidEnd (Event.SingletonInitialization);

		}


		void StartDesktop() {

		}

		void StartMobile() {
			Gyro.Initialize();
		}


		void Start() {
			Physics2D.gravity = defaultGravity;
			StartSingletons ();
			if (settings.willPauseOnLoad) {
				pauseCanvas.Pause(true);
				gameController.PauseGame (settings.willPauseOnLoad, SoundEffects.Args.MusicKeepsPlaying);
			}
		}
	

		void Update()
		{
			UpdateScoresAndReset(false);
		}


//		public void OnApplicationPause(bool paused) {
//			PauseGame(paused);
//		}

		public void UpdateScoresAndReset(bool reset) {
			var newTotal = SavedData.Get(UserData.TotalTime).Float + Time.deltaTime;
			var currentTotal = gameData.currentProcrastination + Time.deltaTime;
			SavedData.Get(UserData.TotalTime).Float = newTotal;
			SavedData.Get(UserData.CurrentSession).Float = Time.fixedTime;
			SavedData.Get(UserData.CurrentProcrastination).Float = currentTotal;
			settings.newPersonalBest = gameData.currentProcrastination > gameData.longestProcrastination;
			if (settings.newPersonalBest) {
				SavedData.Get(UserData.LongestProctrastination).Float = gameData.currentProcrastination;
			}
			if (reset) {
				SavedData.Get(UserData.CurrentProcrastination).Float = 0;
			}
		}

		void OnApplicationQuit() {
			UpdateScoresAndReset (false);
//			long ofDevTime = gameData.GetLong (UserData.OfDevTime);
//			SavedData.Get (UserData.CurrentProcrastination).ReportToGameCenter ();
			GameCenter.current.ReportScore(gameData.PercentageOfDevTimeWastedX10000, UserData.PercentageOfDevTime);
//			Debug.LogWarning (gameData.PercentageOfDevTimeWastedX10000);
			PlayerPrefs.Save ();
		}
		
		void OnApplicationFocus(bool focusStatus) {
			if (!focusStatus) {
				WillBeginEvent(Event.PauseSession);//
				PauseGame (true);
				DidFinishEvent(Event.PauseSession);
			}
		}

		public static void CheckForAnomalies() {
			ClockBehaviour.CheckVisibleClocks ();
		}

		public void PauseGame(bool pause) {
			PauseGame (pause, null);
		}
		public void PauseGame(bool pause, object args) {
			if (pause) {
				WillBeginEvent (Event.PauseSession, args);
				Time.timeScale =  0 ;
				DidFinishEvent (Event.PauseSession, args);
			} else {
				WillBeginEvent(Event.ResumeSession, args);
				Time.timeScale = 1;
				DidFinishEvent(Event.ResumeSession, args);
			}

		}

		public override void OnEventDidEnd(Event theEvent, object args) {
			switch (theEvent) {
			case Event.ResumeSession:
				UpdateScoresAndReset (true);
				break;
			}
		}
		
	}
}
