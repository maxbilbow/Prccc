using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Procrastinate;
using RMX;  namespace Procrastinate {
	public class ClockSpawner : RMX.Singletons.ASingleton<ClockSpawner>, EventListener {

		public List<ClockBehaviour> clocks = new List<ClockBehaviour> ();



		public enum SpawnMode {
			Multiply, Inflate
		}

	 	SpawnMode spawnMode {
			get {
				return GameController.current.ClockSpawnMode;
			}
		}	


	 	bool ShouldKillClocks {
			get {
				int time = (int) Time.fixedTime;
				return clocks.Count > (time < GameController.current.MaxNumberOfClocks ? time : GameController.current.MaxNumberOfClocks);
			}
		}

		bool firstLoad = true;

		ClockBehaviour inflatableClock;

		// Update is called once per frame
		void Update () {

				switch (spawnMode) {
				case SpawnMode.Multiply:
					if (GameCenter.HasPlayerAlreadyAchieved (UserData.ach_time_waster))
						if (Input.touchCount > 1) {
							if (Spawn ())// && !GameCenter.current.HasAchieved (UserData.MakingTime))
								DidCauseEvent(RMX.Event.GC_AchievementGained, UserData.ach_making_time);
							if (ShouldKillClocks) {
						if (clocks.Count > GameController.current.MaxNumberOfClocks)// && !GameCenter.current.HasAchieved (UserData.OverTime))
							DidCauseEvent(RMX.Event.GC_AchievementGained, UserData.ach_overtime);
								var toDestroy = clocks [1];
								//					clocks.RemoveAt(1);
								Destroy (toDestroy.gameObject);
							}
						}
					break;
				case SpawnMode.Inflate:
					if (GameCenter.HasPlayerAlreadyAchieved (UserData.ach_ameteur_crastinator))
						if (Input.touchCount == 2) {
							forTouch = 1;
							if (!inflatableClock) {
						WillBeginEvent(Event.SpawnInflatableClock);
								inflatableClock = ClockBehaviour.New();
						DidFinishEvent(Event.SpawnInflatableClock, inflatableClock);
		//						inflatableClock.lastScale = inflatableClock.transform.localScale;
								inflatableClock.transform.localScale = new Vector3(0.1f,0.1f,0.1f);

							} else {
								if (inflatableClock.didPop ) {
									inflatableClock = null;
								}
							}
						}
					break;
				}

		}
	


		public Vector3 SpawnPoint {
			get {
				Vector3 pos;
				try {
					if (forTouch > 0 && forTouch < Input.touchCount) {
						pos = Input.touches[forTouch].position;
						pos = Camera.current.ScreenPointToRay(new Vector3(pos.x,pos.y,0)).origin;
						return pos;
					}
				} catch (System.Exception e) {
					if (Bugger.WillLog(RMXTests.Exceptions,e.Message))
						Debug.Log(Bugger.Last);
				} finally {
					pos = ClockBehaviour.original.startingPoint;
					pos.y += ClockBehaviour.original.collisionBody.radius * 2;
				}
				return pos;
			}
		}

		int forTouch = 0;
		bool Spawn() {
			if (firstLoad) {
				firstLoad = false;
				return false;
			} else if (GameController.current.ChanceGiven(UserData.ach_time_waster)) {
				WillBeginEvent(Event.SpawnMultipleClocks);
				var count = Input.touchCount;
				forTouch = Random.Range(1,count);
				ClockBehaviour.New();
				DidFinishEvent(Event.SpawnMultipleClocks);
				return true;
			}
			return false;
		}


		
		public override void OnEventDidEnd(System.Enum theEvent, object info) {
			if (theEvent.Equals(RMX.Event.ResumeSession))
			if (GameController.current.ClockSpawnMode == SpawnMode.Inflate) {
				GameController.current.ClockSpawnMode = SpawnMode.Multiply;
					Spawn();
				} else
				GameController.current.ClockSpawnMode = SpawnMode.Inflate;
				ClockBehaviour.CheckVisibleClocks();
		}

	}

}
