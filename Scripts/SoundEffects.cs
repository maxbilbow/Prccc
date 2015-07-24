using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RMX {
	public class SoundEffects : ASingleton<SoundEffects>, EventListener {
		public const string POP = "pop";

		public enum Args
		{
			MusicKeepsPlaying
		}

		public Dictionary<string,AudioSource> tracks = new Dictionary<string,AudioSource>();
		// Use this for initialization
		void Start () {
			foreach (AudioSource track in this.GetComponentsInChildren<AudioSource> ()) {
				tracks[track.name.ToLower()] = track;
			}

		}

		// Update is called once per frame
		void Update () {
		
		}

		void Play(string name) {
			var track = current.tracks [name.ToLower ()];
			if (!track.isPlaying) 
				track.Play ();
		}

		void Play(string name, ulong delay) {
			current.tracks [name.ToLower ()].Play (delay);
		}

		public override void OnEventDidStart(Event theEvent, object info) {
			switch (theEvent) {
			case Event.ClockIsAboutToBurst:
				tracks ["poppy1"].Play ();
				break;
			case Event.PauseSession:
				if (info == null || (Args) info != SoundEffects.Args.MusicKeepsPlaying)
					tracks["music"].Pause();
				break;
			default:
				return;
			}
		}

		public override void OnEvent(Event theEvent, object info) {
			switch (theEvent) {
			case Event.SomethingBurst:
				Play (POP);
				break;
			default:
				return;
			}
		}

		public override void OnEventDidEnd(Event theEvent, object info) {
			switch (theEvent) {
			case Event.ClockIsAboutToBurst:
				tracks ["poppy2"].PlayDelayed (1);
				break;
			case Event.ResumeSession:
				tracks["music"].UnPause();
				break;
			default:
				return;
			}

		}


	}
}