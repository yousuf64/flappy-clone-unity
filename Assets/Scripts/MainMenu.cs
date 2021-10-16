using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenu : MonoBehaviour {
	public Transform[] floor;
	public SpriteRenderer backgroundObject;
	public Sprite[] backgrounds;
	public float Speed;
	Rigidbody2D rgbd;

	// Update is called once per frame
	void Awake(){
		SystemSetting ();
		GetComponent<Animator> ().speed = 1.4f;
		rgbd = GetComponent<Rigidbody2D> ();
	}

	void SystemSetting(){
		Application.targetFrameRate = 60;
		QualitySettings.antiAliasing = 0;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void Start(){
		SwingUp ();
		backgroundObject.sprite = backgrounds [Random.Range (0, backgrounds.Length)];
	}

	void Update () {

		floor[0].transform.Translate(Vector2.left * Speed);
		floor[1].transform.Translate(Vector2.left * Speed);
	}

	void OnTriggerEnter2D(Collider2D enemy){
		if (enemy.gameObject.tag == "FloorBlank") {
			//float XPos = enemy.gameObject.transform.localPosition.x + (12.56f );
			//float YPos = enemy.gameObject.transform.localPosition.y;
			enemy.transform.Translate(Vector2.right * 10.47f);
		}
	}

	void SwingUp(){
		rgbd.velocity = Vector2.up * 0.25f;
		InvokeRepeating ("Switch", 0, 1.2f);
	}

	void Switch(){
		rgbd.velocity *= -1f;
	}
}
