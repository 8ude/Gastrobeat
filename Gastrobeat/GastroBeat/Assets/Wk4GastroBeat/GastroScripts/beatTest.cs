using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;

public class beatTest : MonoBehaviour {

	AudioSource thisSource;
	double nextTime;
	bool updatedSixteenth;

	double coolDownTime;


	// Use this for initialization
	void Start () {
		thisSource = GetComponent<AudioSource> ();
		//coolDownTime = Clock.instance.SixteenthLength();
		StartCoroutine(PlaySomeNotes());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		

		//Debug.Log (nextTime);

		//This doesn't work but makes for a cool phasing effect?
		/*
		if (coolDownTime >= Clock.instance.SixteenthLength()) {
			Clock.instance.SyncFunction (() => {
				thisSource.Play ();
			}, Clock.BeatValue.Sixteenth);
			coolDownTime -= Clock.instance.SixteenthLength();
		}
		*/

		/*
		if (AudioSettings.dspTime >= nextTime ) {

			thisSource.Play();
			nextTime = Clock.instance.AtNextSixteenth ();
		}
	*/

		//coolDownTime += Time.fixedDeltaTime;
	
	
	}

	IEnumerator PlaySomeNotes () {
		while (true) {
			thisSource.Play ();
			yield return new WaitForSeconds (Clock.instance.EighthLength ());
		}

	}


}
