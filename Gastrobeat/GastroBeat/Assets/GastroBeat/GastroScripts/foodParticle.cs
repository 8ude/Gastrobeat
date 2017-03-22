using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;
using DG.Tweening;

public class foodParticle : MonoBehaviour {

	//public Transform[] digestionPoints;



	public Transform[] preTeethPoints;
	public Transform[] esophagusPoints;
	public Transform[] smallIntestinePoints;
	public Transform[] largeIntestinePoints;

	public enum Segment{PreTeeth, Esophagus, SmallIntestine, LargeIntestine};

	public Segment mySegment;

	Vector3 nextPosition;
	
	[SerializeField] int index = 0;
	double nextTime;

	[SerializeField] int bolusSize;
	[SerializeField] int maxBolusSize;
	[SerializeField] float originalBolusSize;

	[SerializeField] bool _blocked;
	[SerializeField] bool _teethSuccess, _esophagusSuccess, _smallIntestineSuccess, _largeIntestineSuccess;

	[SerializeField] bool _atGate;

	float beatWindow;
	[SerializeField] float windowEnd;
	[SerializeField] float windowStart;

	[SerializeField] AudioClip teethFailSFX;
	[SerializeField] AudioClip esoFailSFX;
	[SerializeField] AudioClip smIntFailSFX;
	[SerializeField] AudioClip lgIntFailSFX;

	[SerializeField] AudioClip successSFX;


	//float inputTimeA;

	float teethGateTime;

	float beatTriggerCoolDown;

	//myAge is used for evaluating regurgitation collisions with other boluses (boli?)
	public float myAge;


	GameObject gameManager;





	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager");

		mySegment = Segment.PreTeeth;
		beatWindow = Clock.instance.ThirtySecondLength ();
		nextTime = Clock.instance.AtNextEighth ();

		gameObject.transform.position = preTeethPoints [0].position;
		nextPosition = preTeethPoints [0].position;

		//initialize gate statuses
		_teethSuccess = false;
		_esophagusSuccess = false;
		_smallIntestineSuccess = false;
		_largeIntestineSuccess = false;
		_atGate = false;
		index = 0;

		//need this to keep beat update from firing too much;
		beatTriggerCoolDown = Clock.instance.SixteenthLength ();

		bolusSize = 1;

		myAge = 0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		beatTriggerCoolDown += Time.fixedDeltaTime;
		myAge += Time.fixedDeltaTime;


		//evaluate this at the beginning of the frame - if we're at a gate and at the end of the window, then check to see if the player
		//was successful.  if not, set the food particle back
		if (_atGate && AudioSettings.dspTime >= windowEnd) {

			//if we're at the end of this chain, check to see if player has input appropriate command during this beat
			//if they have not, send them back 4 points
			if (mySegment == Segment.PreTeeth && index >= preTeethPoints.Length && !_teethSuccess) {
				//FAILED

				nextPosition = preTeethPoints [preTeethPoints.Length - 3].position;
				index = preTeethPoints.Length - 3;

				//TriggerSFX

				FoodAudioManager.Instance.PlaySFX (teethFailSFX, 1.0f, Clock.instance.AtNextSixteenth());
			} else if (mySegment == Segment.Esophagus && index >= esophagusPoints.Length && !_esophagusSuccess) {
				//FAILED

				nextPosition = esophagusPoints [esophagusPoints.Length - 4].position;
				index = esophagusPoints.Length - 4;

				//TriggerSFX
				FoodAudioManager.Instance.PlaySFX (esoFailSFX, 1.0f, Clock.instance.AtNextSixteenth());
			} else if (mySegment == Segment.SmallIntestine && index >= smallIntestinePoints.Length && !_smallIntestineSuccess) {
				//FAILED

				nextPosition = smallIntestinePoints [smallIntestinePoints.Length - 4].position;
				index = smallIntestinePoints.Length - 4;

				//TriggerSFX
				FoodAudioManager.Instance.PlaySFX (smIntFailSFX, 1.0f, Clock.instance.AtNextSixteenth());
			} else if (mySegment == Segment.LargeIntestine && index >= largeIntestinePoints.Length && !_largeIntestineSuccess) {
				//FAILED

				nextPosition = smallIntestinePoints [smallIntestinePoints.Length - 4].position;
				index = largeIntestinePoints.Length - 4;

				//TriggerSFX
				FoodAudioManager.Instance.PlaySFX (lgIntFailSFX, 1.0f, Clock.instance.AtNextSixteenth());
			}

		}

		//on every time interval, evaluate some stuff
		if (AudioSettings.dspTime >= nextTime  && beatTriggerCoolDown > Clock.instance.EighthLength()) {

			beatTriggerCoolDown = 0f;
			
			//update object position, then the index

			gameObject.transform.DOMove(nextPosition, Clock.instance.SixteenthLength(), false);

			nextTime = Clock.instance.AtNextEighth ();
			//Debug.Log (AudioSettings.dspTime);
			//Debug.Log (Clock.instance.AtNextEighth ());

			index++;

			//if this puts object at a gate, then set the windowEnd time for evaluation
			if (mySegment == Segment.PreTeeth && index == preTeethPoints.Length - 1) {
				windowStart = (float)nextTime - Clock.instance.SixteenthLength ();
				windowEnd = (float)nextTime + Clock.instance.SixteenthLength ();
			} else if (mySegment == Segment.Esophagus && index == esophagusPoints.Length - 1) {
				windowStart = (float)nextTime - Clock.instance.SixteenthLength ();
				windowEnd = (float)nextTime + Clock.instance.SixteenthLength ();
			} else if (mySegment == Segment.SmallIntestine && index == smallIntestinePoints.Length - 1) {
				windowStart = (float)nextTime - Clock.instance.SixteenthLength ();
				windowEnd = (float)nextTime + Clock.instance.SixteenthLength (); 
			} else if (mySegment == Segment.LargeIntestine && index == largeIntestinePoints.Length - 1) {
				windowStart = (float)nextTime - Clock.instance.SixteenthLength ();
				windowEnd = (float)nextTime + Clock.instance.SixteenthLength ();
			}

		}








		//if we're at the end of this chain, listen for input to set _[thisSegment]Success to true
		if (mySegment == Segment.PreTeeth) {
			if (index >= preTeethPoints.Length - 1) {
				atGate ("Teeth");
			} else
				_atGate = false;
				nextPosition = preTeethPoints [index % preTeethPoints.Length].position;
		} else if (mySegment == Segment.Esophagus) {
			if (index >= esophagusPoints.Length - 1) {
				atGate ("Stomach");
			} else
				_atGate = false;
			nextPosition = esophagusPoints[index % esophagusPoints.Length].position;
		} else if (mySegment == Segment.SmallIntestine) {
			if (index >= smallIntestinePoints.Length - 1) {
				atGate ("SmallIntestine");
			} else
				_atGate = false;
			nextPosition = smallIntestinePoints[index % smallIntestinePoints.Length].position;
		} else if (mySegment == Segment.LargeIntestine) {
			if (index >= largeIntestinePoints.Length - 1) {
				atGate ("LargeIntestine");
			} else
				nextPosition = largeIntestinePoints[index % largeIntestinePoints.Length].position;
		}
	}


	//this SHOULD check for player input during every frame that they're at the gate

	void atGate (string gateName) {
		_atGate = true;
		gameManager.BroadcastMessage ("LightUpText", gateName);
		if (gateName == "Teeth") {

		
			if (!_teethSuccess && Input.GetKeyDown (KeyCode.A) && AudioSettings.dspTime <= windowEnd) {

				//I think this means the player needs to mash on the button in order to digest the food;
				if (bolusSize > 1) {
					DecreaseBolusSize();
			
				} else {
					_teethSuccess = true;
					FoodAudioManager.Instance.PlaySFX (successSFX, 1.0f, Clock.instance.AtNextSixteenth());

					mySegment = Segment.Esophagus;
					nextPosition = esophagusPoints [0].position;
					index = 0;
				}
				//updateSegment

			}
		} else if (gateName == "Stomach") {
			if (!_esophagusSuccess && Input.GetKeyDown (KeyCode.S) && AudioSettings.dspTime <= windowEnd) {
				if (bolusSize > 1) {
					DecreaseBolusSize ();

				} else {
					_esophagusSuccess = true;
					FoodAudioManager.Instance.PlaySFX (successSFX, 1.0f, Clock.instance.AtNextSixteenth());
					mySegment = Segment.SmallIntestine;
					nextPosition = smallIntestinePoints [0].position;
					index = 0;
				}
			}
		} else if (gateName == "SmallIntestine") {
			if (!_smallIntestineSuccess && Input.GetKeyDown (KeyCode.W) && AudioSettings.dspTime <= windowEnd) {
				if (bolusSize > 1) {
					DecreaseBolusSize ();

				} else {

					_smallIntestineSuccess = true;
					FoodAudioManager.Instance.PlaySFX (successSFX, 1.0f, Clock.instance.AtNextSixteenth());
					mySegment = Segment.LargeIntestine;
					nextPosition = largeIntestinePoints [0].position;
					index = 0;
				}
			}
		} else if (gateName == "LargeIntestine") {
			if (!_largeIntestineSuccess && Input.GetKeyDown (KeyCode.D) && AudioSettings.dspTime <= windowEnd) {
				if (bolusSize > 1) {
					DecreaseBolusSize ();

				} else {
					_largeIntestineSuccess = true;

					DigestionComplete ();

				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {

		//check if this is a bolus, then consume it into a larger object
		if (other.gameObject.tag == "Bolus" && myAge > other.GetComponent<foodParticle> ().myAge) {
			//gameObject.GetComponent<Collider2D>().enabled = false;
			
			IncreaseBolusSize ();
			Destroy (other.gameObject);

		}
	}

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
