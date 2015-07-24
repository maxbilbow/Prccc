using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

using RMX;  namespace Procrastinate {
	public class PauseCanvas : Singletons.ASingleton<PauseCanvas>  {

		Canvas canvas;
		Button infoButton;
		// Canvas Variables
		public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
		public bool pixelPerfect = false;

		// CanvasScala vars
		public CanvasScaler.ScaleMode uiScaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize;
		public CanvasScaler.Unit physicalUnit = CanvasScaler.Unit.Points;
		public float fallbackScreenDPI = 96f;
		public float defaultSpriteDPI = 96f;
		public float referencePixelsPerUnit = 100f;

		// Raycaster Variables
		public bool ignoreReversedGraphics = true;
		public GraphicRaycaster.BlockingObjects blockingObjects = GraphicRaycaster.BlockingObjects.None;
//		public GraphicRaycaster.BlockingObjects blockingObjects = GraphicRaycaster.BlockingObjects.None;

		// Image Variables
		public Sprite sourceImage;
		public Color color = Color.white;
		public Material material;
		public bool preserveAspect = false;

		// Triggers
		public List<EventTrigger.Entry> triggers = new List<EventTrigger.Entry>() {
			{ new Trigger() }
		};

		static void UnPauseGame(BaseEventData data) {
			GameController.current.PauseGame (false);
		}

		class Trigger : EventTrigger.Entry {

			public Trigger() {
				eventID = EventTriggerType.PointerDown;
				callback = new Callback();
				callback.AddListener(UnPauseGame);
			}
			               
		}

		class Callback : EventTrigger.TriggerEvent {

		}


		// Use this for initialization
		 void Start () {
		 	canvas = gameObject.GetComponent<Canvas> ();
			if (!canvas) {
				canvas = gameObject.AddComponent<Canvas> ();
				canvas.renderMode = renderMode;
				canvas.pixelPerfect = pixelPerfect;
				canvas.enabled = false;
			}

			_canvasReady = true;
			if (!gameObject.GetComponent<CanvasScaler> ()) {
				var scalar = gameObject.AddComponent<CanvasScaler> ();
				scalar.uiScaleMode = uiScaleMode;
				scalar.physicalUnit = physicalUnit;
				scalar.fallbackScreenDPI = fallbackScreenDPI;
				scalar.defaultSpriteDPI = defaultSpriteDPI;
				scalar.referencePixelsPerUnit = referencePixelsPerUnit;
			}

			if (!gameObject.GetComponent<GraphicRaycaster> ()) {
				var raycaster = gameObject.AddComponent<GraphicRaycaster> ();
				raycaster.ignoreReversedGraphics = ignoreReversedGraphics;
				raycaster.blockingObjects = blockingObjects;
//				raycaster.blockingMask
			}

			if (!gameObject.GetComponent<CanvasRenderer> ()) {
//				var renderer = 
				gameObject.AddComponent<CanvasRenderer> ();
			}

			if (!gameObject.GetComponent<Image> ()) {
				var image = gameObject.AddComponent<Image> ();
				image.sprite = sourceImage;
				image.color = color;
				image.material = material;
				image.preserveAspect = preserveAspect;

			}

			if (!gameObject.GetComponent<EventTrigger> ()) {
				var trigger = gameObject.AddComponent<EventTrigger> ();
				trigger.triggers = triggers;
			}


			infoButton = GetComponentInChildren<Button> ();
			infoButton.onClick.AddListener (toggleInfo);

			if (Settings.current.willPauseOnLoad) {
//				PauseCanvas.current.Pause(true);
				gameController.PauseGame (Settings.current.willPauseOnLoad, SoundEffects.Args.MusicKeepsPlaying);
			}
		}

		static void toggleInfo() {
			current.information = !current.information;
		}

		bool information = false;
		// Update is called once per frame
		void Update () {
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				gameController.PauseGame(!paused);
			}
		}

		public void ShowInfo() {
			information = !information;
		}

		public bool Paused {
			get {
				return paused;
			}
		}

		bool _canvasReady = false;
		bool paused {
			get {
				return _canvasReady? canvas.enabled : false;
			} set {
				canvas.enabled = value;
			}
		}

//		public void OnApplicationPause(bool paused) {
//			GameController.current.PauseGame(paused);
//		}

		//		bool newSession = true;
		void OnGUI(){
			if (paused) {
				//				myStyle.font = myFont;
				string text = !information ? this.text :
					"In total you've only managed to waste " + string.Format("{0:N2}%",GameData.current.PercentageOfDevTimeWasted) + 
						"\n of the time I've lost developing this game." +
						"\n\n Try again?";
				GUIStyle style = new GUIStyle ();
				style.fontSize = 50;
				style.richText = true;
				style.wordWrap = true;
				style.alignment = TextAnchor.MiddleCenter;
				style.padding.left = style.padding.right = style.padding.top = style.padding.bottom = 50;
				
				GUI.Label (new Rect (0, 0, Screen.width, Screen.height), text, style);
				
			} else if (information) {
				information = false;
			}
		}
		


		string text = "";
		
		
	

		
		public override void OnEventDidStart(IEvent theEvent, object info) {
			if (theEvent.IsType( Events.PauseSession))
				Pause(true);
			else if (theEvent.IsType(Events.ResumeSession))
				Pause(false);
		}

		
		public void Pause(bool pause) {
			if (pause && !paused) {
				float time;
				if (Settings.current.willPauseOnLoad) {
					time = GameData.current.currentSessionTime;
					text = "Congratulations. During your last session, you wasted " + Timer.GetTimeDescription (GameData.current.currentSessionTime);
					Settings.current.willPauseOnLoad = false;
				} else {
					time = GameData.current.currentProcrastination;
					text = "Congratulations. You have wasted " + Timer.GetTimeDescription (GameData.current.currentProcrastination);
					if (Settings.current.newPersonalBest) {
						text += "\nA NEW PERSONAL BEST!";
						Settings.current.newPersonalBest = false;
					}
					
				}		
				List<string> activities = GameData.current.WhatYouCouldHaveDone (time);
				var rand = Random.Range (0, activities.Count); 
				text += "\n\nDuring that time you could have " + activities [rand];
			}
			paused = pause;
			
		}

//		public override string ToString ()
//		{
//			string s = 
//				"              Last time: " + GameData.currentSessionTime + ", last coninuous: " + gameData.currentProcrastination + ", total time: " + gameData.totalTime +
//					"\nLast Uniterrupted: " + Timer.GetTimeDescription(gameData.currentProcrastination) + " and top: " + Timer.GetTimeDescription(gameData.longestProcrastination) +
//					"\n     Last Session: " + Timer.GetTimeDescription(gameData.currentSessionTime) +
//					"\n            Total: " + Timer.GetTimeDescription(gameData.totalTime)
//					;
//			return s;
//		}
	
	}

}