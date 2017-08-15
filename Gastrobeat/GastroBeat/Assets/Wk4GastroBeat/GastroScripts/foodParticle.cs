using UnityEngine;
using Beat;
using DG.Tweening;
using System;

public class foodParticle : MonoBehaviour {


	public FoodSegment[] foodSegments;
	public Transform[] digestionPoints;



	public Transform[] preTeethPoints;
	public Transform[] esophagusPoints;
	public Transform[] smallIntestinePoints;
	public Transform[] largeIntestinePoints;



	public enum Segment{PreTeeth, Esophagus, SmallIntestine, LargeIntestine};

	public Segment currentSegment;

	/*
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
	*/


	Vector3 nextPosition;

	public int posIndex = 0;
	public int segIndex = 0;
	double nextTime;

	[SerializeField] int bolusSize;
	[SerializeField] int maxBolusSize;
	[SerializeField] float originalBolusSize;

	private bool _blocked;

	public bool Blocked {
		get {
			return _blocked;
		}
		set {
			_blocked = value;
		}
	}

	private bool _atGate;
	public bool AtGate {
		get {
			return _atGate;
		}
		set { 
			_atGate = value;
			if (value == true) {
				Blocked = true;

			}
		}
	}


	[SerializeField] bool _teethSuccess, _esophagusSuccess, _smallIntestineSuccess, _largeIntestineSuccess;



	/*
	float beatWindow;
	[SerializeField] float windowEnd;
	[SerializeField] float windowStart;
	bool inWindow;
	bool windowReset;


	[SerializeField] AudioClip teethFailSFX;
	[SerializeField] AudioClip esoFailSFX;
	[SerializeField] AudioClip smIntFailSFX;
	[SerializeField] AudioClip lgIntFailSFX;

	[SerializeField] AudioClip successSFX;
	*/


	//float inputTimeA;

	float teethGateTime;

	//myAge is used for evaluating regurgitation collisions with other boluses (boli?)
	public float myAge;


	GameObject gameManager;



	// Use this for initialization
	void Start () {

		//initialize master array of digestion points.. gotta be a better way to do this
		int numPoints = 0; 

		foreach (FoodSegment segment in foodSegments) {
			numPoints += segment.segmentPoints.Length;
		}

		digestionPoints = new Transform[numPoints]; 

		int digIndex = 0;

		foreach (FoodSegment segment in foodSegments) {
			for (int i = 0; i < segment.segmentPoints.Length; i++) {
				digestionPoints [digIndex] = segment.segmentPoints [i];
				digIndex ++;
			}
		}

		gameManager = GameObject.FindGameObjectWithTag ("GameManager");
		gameObject.transform.position = digestionPoints [0].position;

		nextPosition = digestionPoints [0].position;

		currentSegment = Segment.PreTeeth;

		/*
		beatWindow = Clock.instance.ThirtySecondLength();
		nextTime = Clock.instance.AtNextEighth();
		windowEnd = (float) (nextTime + Clock.instance.SixteenthLength());

		gameObject.transform.position = preTeethPoints [0].position;
		nextPosition = preTeethPoints [0].position;
		windowReset = true;

		*/
	

		//initialize gate statuses
		_teethSuccess = false;
		_esophagusSuccess = false;
		_smallIntestineSuccess = false;
		_largeIntestineSuccess = false;
		_atGate = false;

		posIndex = 0;

		bolusSize = 1;

		myAge = 0f;
	}

	// Fixed Update is called once per frame
	void FixedUpdate () {
		myAge += Time.fixedDeltaTime;

		//REFACTOR - TRYING TO MOVE THIS FUNCTIONALITY TO FoodSegment.cs
		/*

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

		inWindow = AudioSettings.dspTime <= windowEnd ? true : false;

		if (inWindow)
		{
			windowReset = true;
		}

		if (mySegment == Segment.PreTeeth)
		{
			if (index >= preTeethPoints.Length - 1)
			{
				atGate("Teeth");
			}
			else
				_atGate = false;
			nextPosition = preTeethPoints[index % preTeethPoints.Length].position;
		}
		else if (mySegment == Segment.Esophagus)
		{
			if (index >= esophagusPoints.Length - 1)
			{
				atGate("Stomach");
			}
			else
				_atGate = false;
			nextPosition = esophagusPoints[index % esophagusPoints.Length].position;
		}
		else if (mySegment == Segment.SmallIntestine)
		{
			if (index >= smallIntestinePoints.Length - 1)
			{
				atGate("SmallIntestine");
			}
			else
				_atGate = false;
			nextPosition = smallIntestinePoints[index % smallIntestinePoints.Length].position;
		}
		else if (mySegment == Segment.LargeIntestine)
		{
			if (index >= largeIntestinePoints.Length - 1)
			{
				atGate("LargeIntestine");
			}
			else
				nextPosition = largeIntestinePoints[index % largeIntestinePoints.Length].position;
		}


		//on every time interval, evaluate some stuff
		if (!inWindow && windowReset)
		{ //no need to track a cooldown separately

			index++;
			//if we're at the end of this chain, listen for input to set _[thisSegment]Success to true

			//evaluate this at the beginning of the frame - if we're at a gate and at the end of the window, then check to see if the player
			//was successful.  if not, set the food particle back
			if (_atGate)
			{
				//if we're at the end of this chain, check to see if player has input appropriate command during this beat
				//if they have not, send them back 4 points
				if (mySegment == Segment.PreTeeth && index >= preTeethPoints.Length && !_teethSuccess)
				{
					//FAILED

					nextPosition = preTeethPoints[preTeethPoints.Length - 3].position;
					index = preTeethPoints.Length - 3;

					//TriggerSFX

					FoodAudioManager.Instance.PlaySFX(teethFailSFX, 1.0f, Clock.instance.AtNextSixteenth());
				}
				else if (mySegment == Segment.Esophagus && index >= esophagusPoints.Length && !_esophagusSuccess)
				{
					//FAILED

					nextPosition = esophagusPoints[esophagusPoints.Length - 4].position;
					index = esophagusPoints.Length - 4;

					//TriggerSFX
					FoodAudioManager.Instance.PlaySFX(esoFailSFX, 1.0f, Clock.instance.AtNextSixteenth());
				}
				else if (mySegment == Segment.SmallIntestine && index >= smallIntestinePoints.Length && !_smallIntestineSuccess)
				{
					//FAILED

					nextPosition = smallIntestinePoints[smallIntestinePoints.Length - 4].position;
					index = smallIntestinePoints.Length - 4;

					//TriggerSFX
					FoodAudioManager.Instance.PlaySFX(smIntFailSFX, 1.0f, Clock.instance.AtNextSixteenth());
				}
				else if (mySegment == Segment.LargeIntestine && index >= largeIntestinePoints.Length && !_largeIntestineSuccess)
				{
					//FAILED

					nextPosition = smallIntestinePoints[smallIntestinePoints.Length - 4].position;
					index = largeIntestinePoints.Length - 4;

					//TriggerSFX
					FoodAudioManager.Instance.PlaySFX(lgIntFailSFX, 1.0f, Clock.instance.AtNextSixteenth());
				}

			}


			//update object position, then the index
			nextTime = Clock.instance.AtNextEighth();


			//windowStart = (float)nextTime - Clock.instance.SixteenthLength();
			//windowEnd = (float)nextTime + Clock.instance.SixteenthLength();

			//buttonsPressedThisWindow.Clear();
			//buttonsDoublePressedThisWindow.Clear();
			//windowReset = false;           
		}
		*/


	}

	public void MoveBolus () {
		if (!_blocked) {
			posIndex++;
			nextPosition = digestionPoints [posIndex].position;
		}

		gameObject.transform.DOMove(nextPosition, Clock.instance.SixteenthLength(), false);

	}


	//this SHOULD check for player input during every frame that they're at the gate
	//REVISED - this should be called at the start of the gate - to initialize an animation


	public void GateSuccess(string gateName, bool success) {

		switch (gateName) {
		case "Teeth":
			if (success) {
				_teethSuccess = true;
				currentSegment = Segment.Esophagus;
				nextPosition = esophagusPoints [0].position;
			} else {

				//TRIGGER ANIM

				_blocked = true;
			}
			break;
		case "Stomach":
			if (success) {
				_esophagusSuccess = true;
				currentSegment = Segment.SmallIntestine;
				nextPosition = smallIntestinePoints [0].position;
			} else {
				//TRIGGER Anim

				_blocked = true;
			}
			break;
		case "SmallIntestine":
			if (success) {
				_smallIntestineSuccess = true;
				currentSegment = Segment.LargeIntestine;
				nextPosition = largeIntestinePoints [0].position;
			} else {
				//TRIGGER Anim

				_blocked = true;
			}
			break;
		case "LargeIntestine":
			if (success) {
				_largeIntestineSuccess = true;
				DigestionComplete ();
			} else {
				//TRIGGER Anim

				_blocked = true;
			}
			break;
		}
			
		/*
		if (gateName == "Teeth") {
			if (success) {



				}
				//updateSegment

			}
		} else if (gateName == "Stomach") {
			if (!_esophagusSuccess && buttonsPressedThisFrame[Segment.Esophagus] && inWindow) {
				if (bolusSize > 1) {
					DecreaseBolusSize ();

				} else {
					_esophagusSuccess = true;
					FoodAudioManager.Instance.PlaySFX (successSFX, 1.0f);
					currentSegment = Segment.SmallIntestine;
					nextPosition = smallIntestinePoints [0].position;
					index = 0;
				}
			}
		} else if (gateName == "SmallIntestine") {
			if (!_smallIntestineSuccess && buttonsPressedThisFrame[Segment.SmallIntestine] && inWindow) {
				if (bolusSize > 1) {
					DecreaseBolusSize ();

				} else {

					_smallIntestineSuccess = true;
					FoodAudioManager.Instance.PlaySFX (successSFX, 1.0f);
					currentSegment = Segment.LargeIntestine;
					nextPosition = largeIntestinePoints [0].position;
					index = 0;
				}
			}
		} else if (gateName == "LargeIntestine") {
			if (!_largeIntestineSuccess && buttonsPressedThisFrame[Segment.LargeIntestine] && inWindow) {
				if (bolusSize > 1) {
					DecreaseBolusSize ();

				} else {
					_largeIntestineSuccess = true;

					DigestionComplete ();

				}
			}
		}
		*/


	}

	void OnTriggerEnter2D(Collider2D other) {

		//check if this is a bolus, then consume it into a larger object
		if (other.gameObject.tag == "Bolus" && myAge > other.GetComponent<foodParticle> ().myAge) {
			
			Destroy (other.gameObject);

		}
	}

	/*
	void IncreaseBolusSize() {
		bolusSize++;

		if (bolusSize > maxBolusSize) {
			gameManager.SendMessage("GameOver");
		}

		gameObject.transform.DOScale(new Vector3((float)bolusSize * 0.25f, (float)bolusSize * 0.25f, 1f), 0.4f);
	}

	void DecreaseBolusSize() {
		bolusSize--;

		gameObject.transform.DOScale(new Vector3((float)bolusSize * 0.25f, (float)bolusSize * 0.25f, 1f), 0.4f);
	}
	*/


	void DigestionComplete() {
		gameManager.SendMessage ("AddScore");

		Destroy (gameObject);
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
