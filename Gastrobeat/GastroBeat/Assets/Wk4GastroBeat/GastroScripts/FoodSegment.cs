using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;
using DG.Tweening;
using System;

public class FoodSegment : MonoBehaviour {

	///<summary>
	/// tracks beats, activates gates
	/// </summary> 

	//public Transform[] digestionPoints;

	//points in this particular segment
	public Transform[] segmentPoints;

	Transform segmentGate;

	//reference to the bolus objects in this particular segment
	public List<GameObject> boliInSegment;


	bool bolusInGate;
	GameObject gateBolus;



	public enum Segment{PreTeeth, Esophagus, SmallIntestine, LargeIntestine};

	public Segment mySegment;

	public string mySegmentName;

	public class SegmentPress<T>
	{
		private readonly T[] _SegmentPressArray = new T[(int)Segment.LargeIntestine+1];

		public void Clear()
		{
			Array.Clear(_SegmentPressArray, 0, (int)Segment.LargeIntestine+1);
		}

		public T this[Segment i]
		{
			get
			{
				return _SegmentPressArray[(int)i];
			}
			set
			{
				_SegmentPressArray[(int)i] = value;
			}
		}
	}

	SegmentPress<bool> buttonsPressedThisFrame = new SegmentPress<bool>();
	SegmentPress<bool> buttonsDoublePressedThisWindow = new SegmentPress<bool>();
	SegmentPress<bool> buttonsPressedThisWindow = new SegmentPress<bool>();

	double nextTime;

	float beatWindow;
	[SerializeField] float windowEnd;
	[SerializeField] float windowStart;
	bool inWindow;
	bool windowReset;


	[SerializeField] AudioClip teethFailSFX;
	[SerializeField] AudioClip esoFailSFX;
	[SerializeField] AudioClip smIntFailSFX;
	[SerializeField] AudioClip lgIntFailSFX;

	[SerializeField] AudioClip segmentFailSFX;

	[SerializeField] AudioClip successSFX;


	//float inputTimeA;

	float teethGateTime;


	GameObject gameManager;

	void Awake() {

		segmentPoints = new Transform[transform.childCount];

		for (int i = 0; i < transform.childCount; i++) {
			segmentPoints[i] = transform.GetChild (i);
		}
	}


	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager");
		bolusInGate = false;

		segmentGate = segmentPoints [segmentPoints.Length - 1];
		gateBolus = null;
	}

	// Fixed Update is called once per frame
	void FixedUpdate () {


		#region storing button presses
		//first we clear out the array that stores our button presses
		buttonsPressedThisFrame.Clear();

		//now we store any buttons that were pressed this frame.
		if(Input.GetKeyDown(KeyCode.A))
		{
			if (buttonsPressedThisWindow[Segment.PreTeeth])
			{
				buttonsDoublePressedThisWindow[Segment.PreTeeth] = true;
			}
			buttonsPressedThisFrame[Segment.PreTeeth] = true;
			buttonsPressedThisWindow[Segment.PreTeeth] = true;

		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			if (buttonsPressedThisWindow[Segment.Esophagus])
			{
				buttonsDoublePressedThisWindow[Segment.Esophagus] = true;
			}
			buttonsPressedThisFrame[Segment.Esophagus] = true;
			buttonsPressedThisWindow[Segment.Esophagus] = true;
		}
		else if (Input.GetKeyDown(KeyCode.W))
		{
			if (buttonsPressedThisWindow[Segment.SmallIntestine])
			{
				buttonsDoublePressedThisWindow[Segment.SmallIntestine] = true;
			}
			buttonsPressedThisFrame[Segment.SmallIntestine] = true;
			buttonsPressedThisWindow[Segment.SmallIntestine] = true;
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			if (buttonsPressedThisWindow[Segment.LargeIntestine])
			{
				buttonsDoublePressedThisWindow[Segment.LargeIntestine] = true;
			}
			buttonsPressedThisFrame[Segment.LargeIntestine] = true;
			buttonsPressedThisWindow[Segment.LargeIntestine] = true;
		}

		#endregion

		inWindow = AudioSettings.dspTime <= windowEnd ? true : false;

		if (inWindow)
		{
			windowReset = true;
		}



		//on every time interval, evaluate some stuff
		if (!inWindow && windowReset)
		{ //no need to track a cooldown separately

			//if we're at the end of this chain, listen for input to set _[thisSegment]Success to true

			if (buttonsPressedThisWindow [mySegment]) {
				OpenGate ();
			}


			windowStart = (float)nextTime - Clock.instance.SixteenthLength();
			windowEnd = (float)nextTime + Clock.instance.SixteenthLength();

			buttonsPressedThisWindow.Clear();
			buttonsDoublePressedThisWindow.Clear();
			windowReset = false;  
			TrackBoli ();
		}


		//this might have to move to late update or something

	}


	void TrackBoli () {

		boliInSegment.Clear ();

		gateBolus = null;

		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Bolus")) {
			if ((int)go.GetComponent<foodParticle> ().currentSegment == (int)mySegment) {
				boliInSegment.Add (go);
				go.GetComponent<foodParticle> ().MoveBolus ();
			

				if (go.transform.position == segmentGate.transform.position) {
				
					bolusInGate = true;
					gateBolus = go;
					gateBolus.GetComponent<foodParticle> ().AtGate = true;
				} else if (!go.GetComponent<foodParticle> ().Blocked) {
					go.GetComponent<foodParticle> ().MoveBolus ();
				}
			}
		}
	}

	void DigestionComplete() {
		gameManager.SendMessage ("AddScore");
	}

	void OpenGate() {
		if (bolusInGate) {
			gateBolus.GetComponent<foodParticle> ().GateSuccess(mySegmentName, true);
		}
	}



	/* have each button input be list - adding button entry; at end of window go through list and determine what to do
	 * 
	 * have version where you care about "perfect" - any input not on beat then won't digest?
	 * 
	 * so long as window is open & push button - progress  --- simplest implementation
	 * 
	 * cache events 
	 * 
	 */




}
