using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections;
using TMPro;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Timeline;
using Unity.VisualScripting;
using TMPro.Examples;

public class PlayerControlls : MonoBehaviour
{
    // instance vars
    [Header("Movement Settings")]
    [SerializeField] private int pushForceUp;
    [SerializeField] private int turnForceLeft;
    [SerializeField] private int turnForceRight;
    [SerializeField] private int pushForceSides;
    private Rigidbody2D rb;
    private bool canPushUp;
    private bool justDied;
    private bool deathProcessGoing;
    [SerializeField] private float forceUpRechargeTime;

    [Header("Canvas Elements & Game Logistics")]
    [SerializeField] GameObject points;
    [SerializeField] GameObject spaceBar;
    [SerializeField] RectTransform progressBarMask;
    [SerializeField] GameObject progressBarParticles;
    private Image pb_bar_image;
    private RectTransform pb_bar_rectTransf;
    private bool gameStarted;
    private int score;
    [SerializeField] GameObject startScreen;
    [SerializeField] TMP_Text pointsTxt;
    [SerializeField] TMP_Text finalScoreTxt;

    [Header("Sfx & Particles")]
    [SerializeField] AudioSource deadSfx;
    [SerializeField] AudioSource bubbleSfx;
    [SerializeField] AudioSource bubbleSfx2;
    [SerializeField] GameObject bubbleParticle;
    [SerializeField] List<AudioClip> popAudioClips;
    [SerializeField] AudioSource popAudioSrc;

    private Animator anim;


    private void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        anim = this.gameObject.GetComponentInChildren<Animator>();
        pb_bar_image = progressBarParticles.GetComponent<Image>();
        pb_bar_rectTransf = progressBarParticles.GetComponent<RectTransform>();
        RestartGame();
    }

    private void RestartGame() {
        pb_bar_image.enabled = false;
        rb.gravityScale = 0f;
        anim.speed = 0f;
        gameStarted = false;
        justDied = false;
        deathProcessGoing = false;
        score = 0;
        startScreen.SetActive(true);
    }

    public void StartGame()
    {
        rb.gravityScale = 0.2f;
        anim.speed = 1f;
        canPushUp = true;
        startScreen.SetActive(false);
        points.SetActive(true);
        spaceBar.SetActive(true);
        StartCoroutine(PlayerAnimation());
    }

    void Update()
    {
        pointsTxt.text = score.ToString();
    }
    private void FixedUpdate()
    {
    }

    // INPUT CONTROLLS
    public void OnSpace(InputAction.CallbackContext val) {
        if (!justDied) {
            // regular Space
            if (gameStarted) {
                float v = val.ReadValue<float>();
                if (v == 1 && canPushUp)
                {
                    Vector2 force = new Vector2(0f, v * pushForceUp);
                    rb.linearVelocityY = 0;
                    rb.AddForce(force);
                    bubbleSfx.Play();
                    canPushUp = false;
                    progressBarMask.sizeDelta = new Vector2(20f, 100f);
                    pb_bar_image.enabled = true;
                    Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y - 0.5f, 0f);
                    GameObject bubbleGO = Instantiate(bubbleParticle, spawnPos, Quaternion.Euler(0f, 0f, -180f));
                    StartCoroutine(DestroyBubbleAfterTime(bubbleGO));
                    StartTimer();
                }
            }
            else if (val.ReadValue<float>() == 1) {
                // START GAME Space
                gameStarted = true;
                StartGame();
                EnemySpawner enemySpawnerScript = FindAnyObjectByType<EnemySpawner>();
                PlanktonSpawner planktonSpawnerScript = FindAnyObjectByType<PlanktonSpawner>();
                enemySpawnerScript.StartGame();
                planktonSpawnerScript.StartGame();
            }
        }
        // restart game
        else if (!deathProcessGoing) {
            Camera.main.orthographicSize = 5;
            Camera.main.transform.position = new Vector3(0f, 0f, 0f);
            this.gameObject.transform.position = new Vector3(0f, 0f, 0f);
            this.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            FindAnyObjectByType<GameManager>().RestartGame();
            FindAnyObjectByType<OceanFloor>().ResetVars();
            anim.Play("player_idle", 0, 0f);
            RestartGame();
        }
    }
    public void OnTilt(InputAction.CallbackContext val) {
        if (!justDied){
            double tilt = val.ReadValue<Vector2>().x;
            // right tilt
            if (tilt == 1){
                rb.AddTorque((float)tilt * turnForceRight * -1);
                Vector2 sideForce = new Vector2(pushForceSides, 0f);
                rb.linearVelocityX = 0;
                rb.AddForce(sideForce);
                if (Mathf.Abs(rb.angularVelocity) >= 50) {
                    bubbleSfx2.Play();
                    Vector3 spawnPos = new Vector3(transform.position.x-1f, transform.position.y-0.5f, 0f);
                    GameObject bubbleGO = Instantiate(bubbleParticle, spawnPos, Quaternion.identity);
                    StartCoroutine(DestroyBubbleAfterTime(bubbleGO));
                }
            }
            // left tilt
            else if (tilt == -1){
                rb.AddTorque((float)tilt * turnForceLeft * -1);
                Vector2 sideForce = new Vector2(-pushForceSides, 0f);
                rb.linearVelocityX = 0;
                rb.AddForce(sideForce);
                if (Mathf.Abs(rb.angularVelocity) >= 50) {
                    bubbleSfx2.Play();
                    Vector3 spawnPos = new Vector3(transform.position.x+1f, transform.position.y-0.5f, 0f);
                    GameObject bubbleGO = Instantiate(bubbleParticle, spawnPos, Quaternion.identity);
                    StartCoroutine(DestroyBubbleAfterTime(bubbleGO));
                }
            }
        }
    }

    // TIMERS/ABILITY RECHARGES
    public void StartTimer()
    {
        StartCoroutine(Timer2());
    }
    private IEnumerator Timer() {
        for (float prog = 20; prog < 80f; prog += 60/50f) {
            progressBarMask.sizeDelta = new Vector2(prog, 100f);
            float rightSide = Mathf.Clamp(progressBarMask.anchoredPosition.x + (progressBarMask.sizeDelta.x*progressBarMask.localScale.x)+40, -812.9753f, -433f);
            pb_bar_rectTransf.anchoredPosition = new Vector3(rightSide, -468f, 0f);
            yield return new WaitForSeconds(forceUpRechargeTime/50f);
        }
        pb_bar_image.enabled = false;
        canPushUp = true;
    }
    private IEnumerator Timer2() {
        float elapsed = 0f;
        while (elapsed < forceUpRechargeTime) { 
            float t = elapsed / forceUpRechargeTime; // normalized 0-1 range
            float prog = Mathf.Lerp(20f, 80f, t); // convert to width of UI obj
            // increase bar insides
            progressBarMask.sizeDelta = new Vector2(prog, 100f);
            // particles follow right side of ^
            float rightSide = Mathf.Clamp(progressBarMask.anchoredPosition.x + (progressBarMask.sizeDelta.x * progressBarMask.localScale.x) + 40, -812.9753f, -433f);
            pb_bar_rectTransf.anchoredPosition = new Vector3(rightSide, -468f, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }
        pb_bar_image.enabled = false;
        canPushUp = true;
    }

    // EXTERNAL
    public void KillPlayer() {
        // slow down player
        rb.linearVelocity /= 3;
        // sfx
        deadSfx.Play();
        // logistics
        justDied = true;
        deathProcessGoing = true;
        FindAnyObjectByType<PlanktonSpawner>().playerAlive = false;
        FindAnyObjectByType<EnemySpawner>().playerAlive = false;
        points.SetActive(false);
        spaceBar.SetActive(false);
        finalScoreTxt.text = "0";
        StartCoroutine(DeathSequence(2));
    }
    public void IncreaseScore() {
        // pop audio choose rand
        popAudioSrc.clip = popAudioClips[Random.Range(0, popAudioClips.Count)];
        popAudioSrc.Play();
        score += 1;
    }

    // ANIMATION
    private IEnumerator PlayerAnimation() {
        while (justDied == false) {
            yield return new WaitForSeconds(5);
            anim.Play("player_idle", 0 , 0f);
        }
        anim.Play("player_fossilized", 0, 0f);
    }
    private IEnumerator DestroyBubbleAfterTime(GameObject go) {
        yield return new WaitForSeconds(1);
        Destroy(go);
    }
    private IEnumerator DeathSequence(int durr) {
        Vector3 startPos = Camera.main.transform.position;
        Vector3 endPos = new Vector3(Mathf.Clamp(this.gameObject.transform.position.x , - 5.5f, 5.5f), this.gameObject.transform.position.y + 0.5f, 0f);
        float startSize = Camera.main.orthographicSize;
        float t = 0f;
        FindFirstObjectByType<GameManager>().FossilizePlayer();
        while (t < durr) {
            t += Time.deltaTime / durr;
            Camera.main.transform.position = Vector3.Lerp(startPos, endPos , t);
            Camera.main.orthographicSize = Mathf.Lerp(startSize, 2, t);
            yield return null;
        }
        StartCoroutine(ScoreUpEffect());
        deathProcessGoing = false;
    }
    private IEnumerator ScoreUpEffect() {
        int score_var = 0;
        while (score_var < score) {
            score_var += 1;
            finalScoreTxt.text = score_var.ToString();
            yield return new WaitForSeconds(0.05f);
        }
    }

    // TRIGGERS
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "waterLineTrigger") {
            // going out of the water
            if (transform.position.y > collision.transform.position.y) {
                rb.gravityScale = 1f;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "waterLineTrigger")
        {
            // going back in the water, slow down
            if (transform.position.y > collision.transform.position.y)
            {
                rb.gravityScale = 0.2f;
                rb.linearVelocityY = -0.5f;
            }
        }
    }
}

