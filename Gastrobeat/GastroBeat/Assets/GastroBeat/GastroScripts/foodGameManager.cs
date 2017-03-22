using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Beat;

public class foodGameManager : MonoBehaviour {

	public GameObject bolusPrefab;

	Transform foodStartPos;

	public Transform[] preTeethPoints;
	public Transform[] esophagusPoints;
	public Transform[] smallIntestinePoints;
	public Transform[] largeIntestinePoints;

	[SerializeField] AudioClip[] preTeethSFX;
	[SerializeField] AudioClip[] esophagusSFX;
	[SerializeField] AudioClip[] smallIntestineSFX;
	[SerializeField] AudioClip[] largeIntestineSFX;
	[SerializeField] AudioClip fartSFX;

	[SerializeField] Text teethText;
	[SerializeField] Text stomachText;
	[SerializeField] Text smIntestineText;
	[SerializeField] Text lgIntestineText;
	public Color lightUpColor;
	public Text scoreText;
	public Canvas gameOverScreen;

	public int bpm = 60;

	[SerializeField] GameObject poopPrefab;

	double nextTime;
	public float foodProbability = 0.05f;

	int bolusCount;

	float beatTriggerCoolDown;

	int scoreCount;


	[SerializeField] int maxBolusCount = 8;

	float textLightCoolDown;


	// Use this for initialization
	void Start () {
		Time.timeScale = 1.0f;
		Clock.instance.SetBPM (bpm);
		scoreCount = 0;

		bolusCount = 0;
		nextTime = Clock.instance.AtNextEighth ();
		foodStartPos = preTeethPoints [0];
		beatTriggerCoolDown = Clock.instance.SixteenthLength ();

		gameOverScreen.enabled = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		beatTriggerCoolDown += Time.fixedDeltaTime;
		textLightCoolDown += Time.fixedDeltaTime;

		GameObject[] boluses = GameObject.FindGameObjectsWithTag ("Bolus");
		bolusCount = boluses.Length;





		if (AudioSettings.dspTime >= nextTime && beatTriggerCoolDown > Clock.instance.EighthLength()) {
			beatTriggerCoolDown = 0f;
			//play SFX based on bolus positions
			if (bolusCount > 0) {
				bool _teethSFX = false;
				bool _esoSFX = false;
				bool _smIntSFX = false;
				bool _lgIntSFX = false;

				foreach (GameObject go in boluses) {
					

					if (go.GetComponent<foodParticle> ().mySegment == foodParticle.Segment.PreTeeth && !_teethSFX) {
						FoodAudioManager.Instance.PlaySFX (preTeethSFX[Random.Range(0,preTeethSFX.Length)]
							, 1.0f + (Random.value - 0.5f) / 5f, Clock.instance.AtNextSixteenth ());
						_teethSFX = true;
					} else if (go.GetComponent<foodParticle> ().mySegment == foodParticle.Segment.Esophagus && ! _esoSFX) {
						FoodAudioManager.Instance.PlaySFX (esophagusSFX[Random.Range(0,esophagusSFX.Length)]
							, 1.0f + (Random.value - 0.5f) / 5f, Clock.instance.AtNextEighth ());
						_esoSFX = true;
					} else if (go.GetComponent<foodParticle> ().mySegment == foodParticle.Segment.SmallIntestine && !_smIntSFX) {
						FoodAudioManager.Instance.PlaySFX (smallIntestineSFX[Random.Range(0, smallIntestineSFX.Length)]
							, 1.0f + (Random.value - 0.5f) / 5f, Clock.instance.AtNextSixteenth ());
						_smIntSFX = true;
					} else if (go.GetComponent<foodParticle> ().mySegment == foodParticle.Segment.LargeIntestine && !_lgIntSFX) {
						FoodAudioManager.Instance.PlaySFX (largeIntestineSFX[Random.Range(0,preTeethSFX.Length)]
							, 1.0f + (Random.value - 0.5f) / 5f, Clock.instance.AtNextThirtySecond ());
						_lgIntSFX = true;
					}

				}


			}
			if (Random.value <= foodProbability && bolusCount < maxBolusCount) {
				
				GameObject newBolus = Instantiate (bolusPrefab);
				newBolus.GetComponent<foodParticle> ().preTeethPoints = preTeethPoints;
				newBolus.GetComponent<foodParticle> ().esophagusPoints = esophagusPoints;
				newBolus.GetComponent<foodParticle> ().smallIntestinePoints = smallIntestinePoints;
				newBolus.GetComponent<foodParticle> ().largeIntestinePoints = largeIntestinePoints;
			}
			nextTime = Clock.instance.AtNextEighth ();
		}
		
	}

	void Update() {
		scoreText.text = scoreCount.ToString();
		if (Input.GetKeyDown (KeyCode.R)) {
			Time.timeScale = 1.0f;
			ResetScene ();
		}
	}

	void AddScore() {
		scoreCount++;
		Instantiate (poopPrefab, largeIntestinePoints [largeIntestinePoints.Length - 1].position, Quaternion.identity);
		FoodAudioManager.Instance.PlaySFX (fartSFX, 1.0f, Clock.instance.AtNextSixteenth ());
	}

	void GameOver() {
		Invoke ("ResetScene", 1.0f);

		Time.timeScale = 0.3f;

		gameOverScreen.enabled = true;

	}

	void ResetScene() {
		SceneManager.LoadScene (0);
	}

	public void LightUpText(string gateName) {
		if (textLightCoolDown >= Clock.instance.SixteenthLength ()) {
			textLightCoolDown = 0f;
			if (gateName == "Teeth") {
				teethText.color = lightUpColor;
				teethText.DOColor (Color.black, Clock.instance.QuarterLength ());
			} else if (gateName == "Stomach") {
				stomachText.color = lightUpColor;
				stomachText.DOColor (Color.black, Clock.instance.QuarterLength ());
			} else if (gateName == "SmallIntestine") {
				smIntestineText.color = lightUpColor;
				smIntestineText.DOColor (Color.black, Clock.instance.QuarterLength ());
			} else if (gateName == "LargeIntestine") {
				lgIntestineText.color = lightUpColor;
				lgIntestineText.DOColor (Color.black, Clock.instance.QuarterLength ());
			}
		}

	}

}
