using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RMX; 

namespace Procrastinate {
	public class GameController : AGameController<GameController>, IGameController {


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


	

//		public delegate ISingleton LateInit();


		protected override void StartSingletons() {
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

		}


		protected override void StartDesktop() {

		}

		protected override void StartMobile() {
			Gyro.Initialize();
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


		public override void PauseGame(bool pause, object args) {
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


		
	}
}
