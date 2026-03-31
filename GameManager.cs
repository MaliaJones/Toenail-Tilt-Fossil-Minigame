using System.Collections;
using TMPro.Examples;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject soilLayer1;
    [SerializeField] GameObject soilLayer2;
    private SpriteRenderer sp1;
    private SpriteRenderer sp2;

    [SerializeField] GameObject endingUI;

    void Start()
    {
        sp1 = soilLayer1.GetComponent<SpriteRenderer>();
        sp2 = soilLayer2.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        
    }

    public void FossilizePlayer() {
        StartCoroutine(Fossilize(1.5f));
    }
    private IEnumerator Fossilize(float durr) {
        Color startColor = new Color32(255, 255, 255, 0);
        Color endColor = new Color32(255, 255, 255, 255);
        Vector3 startPosS1 = new Vector3(0f, 3.6f, 0f);
        Vector3 startPosS2 = new Vector3(0f, 2.6f, 0f);
        Vector3 endPosS1 = startPosS1 - new Vector3(0f, 3.4f, 0f);
        Vector3 endPosS2 = startPosS2 - new Vector3(0f, 4.2f, 0f);
        float t = 0f;

        // 1st layer down
        while (t < durr) {
            t += Time.deltaTime / durr;
            soilLayer1.transform.position = Vector3.Lerp(startPosS1, endPosS1, t);
            sp1.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        // 2nd layer down
        t = 0.66f;
        while (t < durr) {
            t += Time.deltaTime / durr;
            soilLayer2.transform.position = Vector3.Lerp(startPosS2, endPosS2, t);
            sp2.color = Color32.Lerp(startColor, endColor, t);
            yield return null;
        }
        // ending UI on
        endingUI.SetActive(true);
    }

    public void RestartGame() { 
        sp1.color = new Color32(255, 255, 255, 0);
        sp2.color = new Color32(255, 255, 255, 0);
        endingUI.SetActive(false);

    }
}
