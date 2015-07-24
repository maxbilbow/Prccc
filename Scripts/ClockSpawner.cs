using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RMX {
	public class ClockSpawner : ASingleton<ClockSpawner>, EventListener {

		public List<ClockBehaviour> clocks = new List<ClockBehaviour> ();





		public enum SpawnMode {
			Multiply, Inflate
		}

	 	SpawnMode spawnMode {
			get {
				return settings.ClockSpawnMode;
			}
		}	


	 	bool ShouldKillClocks {
			get {
				int time = (int) Time.fixedTime;
				return clocks.Count > (time < settings.MaxNumberOfClocks ? time : settings.MaxNumberOfClocks);
			}
		}

		bool firstLoad = true;

		ClockBehaviour inflatableClock;

		// Update is called once per frame
		void Update () {

				switch (spawnMode) {
				case SpawnMode.Multiply:
					if (GameCenter.HasPlayerAlreadyAchieved (UserData.TimeWaster))
						if (Input.touchCount > 1) {
							if (Spawn ())// && !GameCenter.current.HasAchieved (UserData.MakingTime))
								DidCauseEvent(Event.GC_AchievementGained, UserData.MakingTime);
							if (ShouldKillClocks) {
								if (clocks.Count > settings.MaxNumberOfClocks)// && !GameCenter.current.HasAchieved (UserData.OverTime))
									DidCauseEvent(Event.GC_AchievementGained, UserData.OverTime);
								var toDestroy = clocks [1];
								//					clocks.RemoveAt(1);
								Destroy (toDestroy.gameObject);
							}
						}
					break;
				case SpawnMode.Inflate:
					if (GameCenter.HasPlayerAlreadyAchieved (UserData.AmeteurCrastinator))
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
					var log = Bugger.StartNewLog(Testing.Exceptions,e.Message);
					if (log.isActive)
						Debug.Log(log);
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
			} else if (settings.ChanceGiven(UserData.TimeWaster)) {
				WillBeginEvent(Event.SpawnMultipleClocks);
				var count = Input.touchCount;
				forTouch = Random.Range(1,count);
				ClockBehaviour.New();
				DidFinishEvent(Event.SpawnMultipleClocks);
				return true;
			}
			return false;
		}


		
		public override void OnEventDidEnd(Event theEvent, object info) {
			switch (theEvent) {
			case Event.ResumeSession:
				if (settings.ClockSpawnMode == SpawnMode.Inflate) {
					settings.ClockSpawnMode = SpawnMode.Multiply;
					Spawn();
				} else
					settings.ClockSpawnMode = SpawnMode.Inflate;
				ClockBehaviour.CheckVisibleClocks();
				break;
			}
			if (Bugger.WillLog (Testing.EventCenter, theEvent.ToString() + " DidEnd!"))
				Debug.Log (Bugger.Last);
		}

	}

}
