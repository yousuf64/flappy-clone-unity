using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Animations;
using TMPro;

public class MainScript : MonoBehaviour {
	//Input Objects
	public RuntimeAnimatorController[] birdColors;
	private Rigidbody2D physics;
	public Sprite[] medals, background;
	public TextMeshProUGUI fieldScore, fieldFinalScore, fieldBest;
	public GameObject newBest, GameOverGUI;
	public SpriteRenderer IntroGUI;
	public AudioClip FlapSound, DeadSound, ScoredSound, ShotSound, UISound, PointSound;
	public SpriteRenderer medalObject, backgroundObject;

	//Variables
	private int score, best;
	public float gameOverWaitingTime, gameSpeed, flapHeight, gravity;

	//Pipe Config
	public GameObject Pipe;
	public int numberOfPipes;
	public float gapBetweenPipes, pipeMinPos, pipeMaxPos;

	void Awake(){
		physics = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		Physics2D.gravity = new Vector2 (0, -gravity);
		best = PlayerPrefs.GetInt ("High Score");
		//PlaySound.volume = PlayerPrefs.GetFloat ("Volume");
	}

	void Start () {
		SystemSetting ();
		animator.speed = 1.1f;
		GenerateBird ();
		backgroundObject.sprite = background [Random.Range(0,background.Length)]; //initialize background;
		StartCoroutine("_BirdUp");
	}
		
	Animator animator;
	void GenerateBird(){
		animator.runtimeAnimatorController = birdColors [Random.Range (0, birdColors.Length)];
	}

	void SystemSetting(){
		Application.targetFrameRate = 60;
		QualitySettings.antiAliasing = 0;
	}

	//bool shouldFlap = false;
	bool IsIntro = true, IsDead = false;
	void Update () {
		MoveCamera ();
		if (WasTouchedOrClicked() && !IsIntro) {
			Flap ();
		}else if(WasTouchedOrClicked() && IsIntro) {
			Flap ();
			StopCoroutine("_BirdUp");
			CancelInvoke ();
			animator.speed = 1.7f;
			IsIntro = false;
			physics.bodyType = RigidbodyType2D.Dynamic;
			StartCoroutine ("_introOutAnimation");
			SpawnPipes ();
		}
	}

	private void FixedUpdate(){
		if (!IsIntro) {
			FixRotation ();
		}
	}

	public Transform Camera;
	void MoveCamera(){
		Camera.Translate (Vector2.right * gameSpeed);
	}

	Input input;
	bool WasTouchedOrClicked()
	{
		if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	enum FlappyYAxisTravelState
	{
		GoingUp, GoingDown
	}
	FlappyYAxisTravelState flappyYAxisTravelState;
	Vector3 birdRotation;
	float degreesToAdd = 0;
	public float MaxDownRotation, MaxUpRotation, GoingUpSpeed, GoingDownSpeed;
	private void FixRotation()
	{
		if (physics.velocity.y > -2.45f)
			flappyYAxisTravelState = FlappyYAxisTravelState.GoingUp;
		else
			flappyYAxisTravelState = FlappyYAxisTravelState.GoingDown;

		switch (flappyYAxisTravelState)
		{
		case FlappyYAxisTravelState.GoingUp:
			degreesToAdd = GoingUpSpeed;
			break;
		case FlappyYAxisTravelState.GoingDown:
			degreesToAdd = -GoingDownSpeed;
			break;
		}
		birdRotation = new Vector3(0, 0, Mathf.Clamp(birdRotation.z + degreesToAdd, -MaxDownRotation, MaxUpRotation));
		transform.eulerAngles = birdRotation;
	}

	void OnTriggerEnter2D(Collider2D enemy)
	{
		if (!IsDead && enemy.gameObject.tag == "Pipeblank") {
			Scored ();
		} else if(enemy.gameObject.tag == "Pool"){
			RelocatePipe (enemy.transform);
		} else if (enemy.gameObject.tag == "FloorBlank") {
			RelocateFloor (enemy.transform);	
		}
	}

	void OnCollisionEnter2D(Collision2D enemy)
	{
		if (!IsDead)
			if (enemy.gameObject.tag == "Pipe") {
				Invoke ("deathLeapSound", 0.3f);
				BirdDied ();
			}
			else if (enemy.gameObject.tag == "Floor") {
				BirdDied ();
			}
	}

	void deathLeapSound(){
			MusicManager.Play (ShotSound);	
	}

	public float PipeSpawnDistance = 5f;
	void SpawnPipes (){
		float initialDistance = Camera.localPosition.x + PipeSpawnDistance;
		Instantiate (Pipe, new Vector3 (initialDistance, Random.Range (pipeMinPos, pipeMaxPos)), Quaternion.identity);
		for (int i = 1; i < numberOfPipes; i++) {
			Instantiate(Pipe, new Vector3(initialDistance + i * gapBetweenPipes, Random.Range(pipeMinPos,pipeMaxPos)), Quaternion.identity);
		}
	}

	void RelocatePipe(Transform enemyTransform){
		enemyTransform.localPosition = new Vector2 (enemyTransform.localPosition.x + (gapBetweenPipes * numberOfPipes), Random.Range (pipeMinPos,pipeMaxPos));
	}
		
	void RelocateFloor(Transform enemyTransform)
	{
		enemyTransform.Translate(Vector2.right * 10.47f);
	}

	void Flap()
	{
		MusicManager.Play (FlapSound);
		physics.velocity = Vector2.up * flapHeight;
	}

	void Scored()
	{
		score++;
		MusicManager.Play (ScoredSound);
		fieldScore.text = score.ToString();
	}

	void BirdDied(){
		MusicManager.Play (DeadSound);
		StartCoroutine ("_Flash");
		IsDead = true;
		animator.speed = 0;
		enabled = false;
		StartCoroutine ("_scoreOutAnimation");
		Invoke ("showGameOverGUI", gameOverWaitingTime);
	}

	void showGameOverGUI(){
		StartCoroutine ("_gameOverTitleAnimation");
	}
		
	void ShowMedal(int s){
		if (s < 10) {
			medalObject.sprite = medals [0];	
		} else if (s < 20) {
			medalObject.sprite = medals [1];	
		} else if (s < 30) {
			medalObject.sprite = medals [2];	
		} else if (s < 40) {
			medalObject.sprite = medals [3];	
		} else if (s < 50) {
			medalObject.sprite = medals [4];	
		} else if (s > 49) {
			medalObject.sprite = medals [5];	
		}	
	}
		

	IEnumerator _introOutAnimation(){
		float ii = 1;
		while (IntroGUI.color.a  >= 0f) {
			IntroGUI.transform.Translate (Vector2.up * 0.01f);
			ii = ii - 0.04f;
			IntroGUI.color = new Color(255f, 255f, 255f, ii);
			yield return null;
		}
		IntroGUI.transform.SetParent (null);
		IntroGUI.gameObject.SetActive (false);
		StartCoroutine ("_scoreInAnimation");
	}

	IEnumerator _scoreInAnimation(){
		float ii = fieldScore.color.a;
		while (fieldScore.color.a  <= 1f) {
			ii += 0.03f;
			fieldScore.color = new Color(255f, 255f, 255f, ii);
			yield return null;
		}
	}

	IEnumerator _scoreOutAnimation(){
		StopCoroutine ("_scoreInAnimation");
		float ii = fieldScore.color.a  ;
		while (fieldScore.color.a  > 0f) {
			ii -= 0.03f;
			fieldScore.color = new Color(255f, 255f, 255f, ii);

			yield return null;
		}
	}

	IEnumerator _gameOverPanel(){
		fieldBest.text = best.ToString ();
		float ii = GameOverGUI.transform.localPosition.x + 3f;
		MusicManager.Play (UISound);
		while (GameOverGUI.transform.localPosition.x < ii) {
			GameOverGUI.transform.Translate (Vector2.right * 0.2f);
			yield return null;
		}
		StartCoroutine ("_scoreCounter");
	}

	public Transform gameOverTitle;
	public Button muteButton;
	IEnumerator _gameOverTitleAnimation(){
		MusicManager.Play (UISound);
		GameOverGUI.transform.Translate (Vector2.right * Camera.localPosition.x);
		GameOverGUI.transform.Translate (Vector2.left * 3f);
		gameOverTitle.Translate (Vector2.right * 3f);
		GameOverGUI.SetActive (true);
		while (gameOverTitle.localPosition.y > -639.2f) {
			gameOverTitle.Translate (Vector2.down * 0.1f);
			yield return null;
		}
		yield return new WaitForSeconds (0.35f);
		gameOverTitle.SetParent (null);
		muteButton.interactable = true;
		StartCoroutine ("_gameOverPanel");

	}

	public GameObject playButton, backButton;
	IEnumerator _scoreCounter(){
		for (int s = 1; s <= score; s++) {
			fieldFinalScore.text = s.ToString();
			MusicManager.Play (PointSound);
			ShowMedal (s);
			if (s > best){
				fieldBest.text = s.ToString ();
				newBest.SetActive (true);
			}
			yield return new WaitForSeconds (0.07f);
		}
		if (score > best) {
			best = score;
			PlayerPrefs.SetInt ("High Score", best);
			PlayerPrefs.Save ();
		}
		playButton.SetActive (true);
		backButton.SetActive (true);
	}


	public SpriteRenderer flashObject;
	float fn = 0f;
	IEnumerator _Flash(){
		flashObject.transform.Translate (Vector2.right * Camera.localPosition.x);
		while (fn <= 0.7f) {
			fn = fn + (0.7f / 2f);
			flashObject.color = new Color(255f, 255f, 255f, fn);
			yield return null;
		}
		StartCoroutine ("_FlashOut");
	}

	IEnumerator _FlashOut(){
		while (fn >= 0f) {
			fn = fn - 0.16f;
			flashObject.color = new Color(255f, 255f, 255f, fn);
			yield return null;
		}
	}
		
	IEnumerator _BirdUp(){
		physics.velocity = Vector2.up * 0.115f;
		InvokeRepeating ("_BirdDown", 0, 0.365f);
		yield return null;
	}

	void _BirdDown(){
		physics.velocity *= -1f;	
	}
}