//http://www.cnblogs.com/gameprogram/archive/2012/08/15/2640357.html
//http://www.blog.silentkraken.com/2010/04/06/audiomanager/
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using Beat;

public class FoodAudioManager : MonoBehaviour {

	private static FoodAudioManager instance = null;

	[SerializeField] GameObject myPrefabSFX;

	//[SerializeField] AudioSource myAudioSource;

	//[SerializeField] AudioMixerGroup SFXGroup;



	//========================================================================
	public static FoodAudioManager Instance {
		get { 
			return instance;
		}
	}

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}

		//DontDestroyOnLoad(this.gameObject);
	}
	//========================================================================

	public void PlaySFX (AudioClip g_SFX) {
		GameObject t_SFX = Instantiate (myPrefabSFX) as GameObject;
		t_SFX.name = "SFX_" + g_SFX.name;
		t_SFX.GetComponent<AudioSource> ().clip = g_SFX;
		//t_SFX.GetComponent<AudioSource> ().outputAudioMixerGroup = SFXGroup;
		t_SFX.GetComponent<AudioSource> ().Play ();
		DestroyObject(t_SFX, g_SFX.length);
	}

	public void PlaySFX (AudioClip g_SFX, float g_Pitch) {
		GameObject t_SFX = Instantiate (myPrefabSFX) as GameObject;
		t_SFX.name = "SFX_" + g_SFX.name;
		t_SFX.GetComponent<AudioSource> ().clip = g_SFX;
		t_SFX.GetComponent<AudioSource> ().pitch = g_Pitch;
		//t_SFX.GetComponent<AudioSource> ().outputAudioMixerGroup = SFXGroup;
		t_SFX.GetComponent<AudioSource> ().Play ();
		DestroyObject(t_SFX, g_SFX.length);
	}

	public void PlaySFX (AudioClip g_SFX, float g_Pitch, double g_Timing) {
		GameObject t_SFX = Instantiate (myPrefabSFX) as GameObject;
		t_SFX.name = "SFX_" + g_SFX.name;
		t_SFX.GetComponent<AudioSource> ().clip = g_SFX;
		t_SFX.GetComponent<AudioSource> ().pitch = g_Pitch;
		//t_SFX.GetComponent<AudioSource> ().outputAudioMixerGroup = SFXGroup;
		t_SFX.GetComponent<AudioSource> ().PlayScheduled (g_Timing);
		DestroyObject(t_SFX, g_SFX.length);
	}

	/*
	public void PlayReverseSFX (AudioClip g_SFX, float g_Pitch) {
		GameObject t_SFX = Instantiate (myPrefabSFX) as GameObject;
		t_SFX.name = "SFX_" + g_SFX.name;
		t_SFX.GetComponent<AudioSource> ().clip = g_SFX;
		t_SFX.GetComponent<AudioSource> ().timeSamples = t_SFX.GetComponent<AudioSource> ().clip.samples/3;

		t_SFX.GetComponent<AudioSource> ().pitch = -1f * g_Pitch;
		t_SFX.GetComponent<AudioSource> ().outputAudioMixerGroup = SFXGroup;
		t_SFX.GetComponent<AudioSource> ().Play ();
		DestroyObject(t_SFX, g_SFX.length);
	}

	public void PlayReverseSFX (AudioClip g_SFX, float g_Pitch, float g_Volume) {
		GameObject t_SFX = Instantiate (myPrefabSFX) as GameObject;
		t_SFX.name = "SFX_" + g_SFX.name;
		t_SFX.GetComponent<AudioSource> ().clip = g_SFX;
		t_SFX.GetComponent<AudioSource> ().timeSamples = t_SFX.GetComponent<AudioSource> ().clip.samples/3;
		t_SFX.GetComponent<AudioSource> ().volume = g_Volume;
		t_SFX.GetComponent<AudioSource> ().pitch = -1f * g_Pitch;
		t_SFX.GetComponent<AudioSource> ().outputAudioMixerGroup = SFXGroup;
		t_SFX.GetComponent<AudioSource> ().Play ();
		DestroyObject(t_SFX, g_SFX.length);
	}
	*/

	//================================================================================
	// Background Music Functions
	//================================================================================

	/*
	public void PlayBGM (AudioClip g_BGM) {
		if (myAudioSource.isPlaying == false) {
			myAudioSource.clip = g_BGM;
			myAudioSource.Play ();
			return;
		}

		if (g_BGM == myAudioSource.clip)
			return;

		myAudioSource.Stop ();
		myAudioSource.clip = g_BGM;
		myAudioSource.Play ();
	}

	public void PlayBGM (AudioClip g_BGM, float g_Volume) {
		if (myAudioSource.isPlaying == false) {
			myAudioSource.clip = g_BGM;
			myAudioSource.volume = g_Volume;
			myAudioSource.Play ();
			return;
		} else if (g_BGM == myAudioSource.clip) {
			myAudioSource.volume = g_Volume;
			return;
		}

		myAudioSource.Stop ();
		myAudioSource.clip = g_BGM;
		myAudioSource.volume = g_Volume;
		myAudioSource.Play ();
	}

	public void StopBGM () {
		myAudioSource.Stop ();
	}
	*/



}
