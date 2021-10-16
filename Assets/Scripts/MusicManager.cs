using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour {

	private static AudioSource aud;
	private Image sr;
	public Sprite[] sprites;
	void Awake(){
		sr = GetComponent<Image> ();
		aud = GetComponent<AudioSource> ();
		GetComponent<Button> ().interactable = false;
	}

	void Start () {
		aud.volume = PlayerPrefs.GetFloat ("Volume", 1f);
		FixImage ();
	}

	void FixImage(){
		sr.sprite = sprites [(int)aud.volume];
	}

	public static void Play(AudioClip clip){
		aud.PlayOneShot (clip);
	}

	public void Toggle(){
		if (aud.volume == 0) {
			aud.volume = 1;
		}else{
			aud.volume = 0;
		}
		FixImage ();
		PlayerPrefs.SetFloat ("Volume", aud.volume);

	}

	void OnDestroy(){
		PlayerPrefs.SetFloat ("Volume", aud.volume);
		PlayerPrefs.Save ();
	}
}
