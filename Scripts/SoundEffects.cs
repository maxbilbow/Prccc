using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RMX;  
namespace Procrastinate {
	public class SoundEffects : Singletons.ASingleton<SoundEffects> {
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

		public override void OnEventDidStart(IEvent theEvent, object info) {
			if (theEvent.IsType(Events.ClockIsAboutToBurst))
				tracks ["poppy1"].Play ();
			else if (theEvent.IsType(Events.PauseSession))
				if (info == null || (Args) info != SoundEffects.Args.MusicKeepsPlaying)
					tracks["music"].Pause();
		}

		public override void OnEvent(IEvent theEvent, object info) {
			if (theEvent.IsType(Events.SomethingBurst))
				Play (POP);
		}

		public override void OnEventDidEnd(IEvent theEvent, object info) {
			if (theEvent.IsType(Events.ClockIsAboutToBurst))
				tracks ["poppy2"].PlayDelayed (1);
			else if (theEvent.IsType( Events.ResumeSession))
				tracks["music"].UnPause();
		}


	}
}