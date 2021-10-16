using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

	AudioSource aud ;
	void Awake(){
		aud = GetComponent<AudioSource> ();
		aud.volume = PlayerPrefs.GetFloat ("Volume");
	}

	public void StartClassic()
	{
		aud.volume = PlayerPrefs.GetFloat ("Volume");

		Initiate.Fade ("mainGame", Color.black, 2f);
	}
		
	public void Rate(){
		aud.volume = PlayerPrefs.GetFloat ("Volume");

		Application.OpenURL ("http://play.google.com/store/apps/details?id=com.starboy.flappy2018");
	}

	public void GoBack(){
		aud.volume = PlayerPrefs.GetFloat ("Volume");
		Initiate.Fade ("startMenu", Color.black, 2f);
	}
}
