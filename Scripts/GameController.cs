using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityStandardAssets.CrossPlatformInput;

namespace RMX.Procrastinate {
	public class GameController : RMX.Singletons.ASingleton<GameController> { //<GameController> , EventListener {
		public Vector2 defaultGravity = new Vector2 (0f, -9.81f);

		public Vector2 velocity {
			get {
				return new Vector2(transform.forward.x, transform.forward.y);
			}
		}


		public static bool isFirstPlay {
			get {
				return PlayerPrefs.GetString(SavedData.GetKey(UserData.TotalTime)) != null;
			}
		}




		GameData gameData {
			get {
				return GameData.current;
			}
		}


//		public delegate ISingleton LateInit();


		void StartSingletons() {
			Notifications.EventWillStart (Events.SingletonInitialization);
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
			Notifications.EventDidEnd (Events.SingletonInitialization);

		}


		void StartDesktop() {

		}

		void StartMobile() {
			Gyro.Initialize();
		}


		void Start() {
			Physics2D.gravity = defaultGravity;
			StartSingletons ();
			if (Settings.current.willPauseOnLoad) {
				PauseCanvas.current.Pause(true);
				gameController.PauseGame (Settings.current.willPauseOnLoad, SoundEffects.Args.MusicKeepsPlaying);
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
			Settings.current.newPersonalBest = gameData.currentProcrastination > gameData.longestProcrastination;
			if (Settings.current.newPersonalBest) {
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
				WillBeginEvent(Events.PauseSession);//
				PauseGame (true);
				DidFinishEvent(Events.PauseSession);
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
				WillBeginEvent (Events.PauseSession, args);
				Time.timeScale =  0 ;
				DidFinishEvent (Events.PauseSession, args);
			} else {
				WillBeginEvent(Events.ResumeSession, args);
				Time.timeScale = 1;
				DidFinishEvent(Events.ResumeSession, args);
			}

		}

		public override void OnEventDidEnd(IEvent theEvent, object args) {
			if (theEvent.IsType(Events.ResumeSession))
				UpdateScoresAndReset (true);
		}
		
	}
}
