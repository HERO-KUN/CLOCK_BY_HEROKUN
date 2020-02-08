using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Initializer : MonoBehaviour
{

    private const string PREF_ROOT = "Preferences";
    private const string PREF_PATH = "Preferences/prefs.json";

    public sbyte direction;

    public GameObject digitArea;

    public Image background;
    public Image backgroundBright;
    public Text title;
    public Text subtitle;

    public AnimationCurve ac;

    GameObject[] digitListHours;
    GameObject[] digitListMinutes;
    GameObject[] digitListSeconds;
    
    int movedCount = 0;

    int update = 0;

    float movementDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        ac = Interpolators.EaseOutCurve;

        digitListHours = new GameObject[] { digitArea.transform.GetChild(0).gameObject, digitArea.transform.GetChild(1).gameObject };
        digitListMinutes = new GameObject[] { digitArea.transform.GetChild(3).gameObject, digitArea.transform.GetChild(4).gameObject };
        digitListSeconds = new GameObject[] { digitArea.transform.GetChild(6).gameObject, digitArea.transform.GetChild(7).gameObject };

        GameObject[][] digitLists = new GameObject[][] { digitListHours, digitListMinutes, digitListSeconds };

        for(int i = 0; i < 3; i++) {
            for(int j = 0; j < digitLists[i][0].transform.childCount; j++) {
                digitLists[i][0].transform.GetChild(j).transform.localPosition = new Vector3(0, direction * j);
                digitLists[i][0].transform.GetChild(j).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 0.25f);
            }
            for (int j = 0; j < digitLists[i][1].transform.childCount; j++) {
                digitLists[i][1].transform.GetChild(j).transform.localPosition = new Vector3(0, direction * j);
                digitLists[i][1].transform.GetChild(j).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 0.25f);
            }
        }

        if (File.Exists(PREF_PATH)) {
            Preferences prefs = JsonUtility.FromJson<Preferences>(File.ReadAllText(PREF_PATH));
            title.text = prefs.title;
            subtitle.text = prefs.subtitle;
            if (File.Exists(string.Join("/", PREF_ROOT, prefs.background))) {
                Texture2D backgroundTexture = new Texture2D(1, 1);
                backgroundTexture.LoadImage(File.ReadAllBytes(string.Join("/", PREF_ROOT, prefs.background)));
                background.sprite = Sprite.Create(backgroundTexture, new Rect(0, 0, backgroundTexture.width, backgroundTexture.height), new Vector2(0.5f, 0.5f));
            } else {
                title.text = "cannot find background file.";
                subtitle.text = "";
                background.color = new Color(0, 0, 0);
                backgroundBright.color = new Color(0, 0, 0);
            }
            if (File.Exists(string.Join("/", PREF_ROOT, prefs.background_bright))) {
                Texture2D backgroundTexture = new Texture2D(1, 1);
                backgroundTexture.LoadImage(File.ReadAllBytes(string.Join("/", PREF_ROOT, prefs.background_bright)));
                backgroundBright.sprite = Sprite.Create(backgroundTexture, new Rect(0, 0, backgroundTexture.width, backgroundTexture.height), new Vector2(0.5f, 0.5f));
            } else {
                backgroundBright.enabled = false;
            }
        } else {
            title.text = "preference file not found.";
            subtitle.text = "";
            background.color = new Color(0, 0, 0);
            backgroundBright.color = new Color(0, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DateTime currentTime = DateTime.Now;
        
        if (update == currentTime.Second) return;

        update = currentTime.Second;

        MoveDigits(currentTime.Second, digitListSeconds);
        MoveDigits(currentTime.Minute, digitListMinutes);
        MoveDigits(currentTime.Hour, digitListHours);
    }

    private void MoveDigits(int data, GameObject[] digitObjects) {
        int firstDigit = data / 10;
        int secondDigit = data - (data / 10 * 10);

        if ((int)(digitObjects[0].transform.localPosition.y) != firstDigit || movedCount <= 6) {
            AddMovedCount();
            StartCoroutine(Interpolators.Curve(Interpolators.EaseOutCurve, digitObjects[0].transform.localPosition.y, firstDigit, movementDuration,
                (step) =>
                {
                    digitObjects[0].transform.localPosition = new Vector3(digitObjects[0].transform.localPosition.x, step, 0);
                }, null));

            StartCoroutine(Interpolators.Curve(Interpolators.LinearCurve, 0.25f, firstDigit == 0 ? 0.5f : 0.8f, 0.25f,
                (step) =>
                {
                    digitObjects[0].transform.GetChild(firstDigit).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, step);
                }, null));
            StartCoroutine(Interpolators.Curve(Interpolators.LinearCurve, firstDigit - 1 == 0 ? 0.5f : 0.8f, 0.25f, 0.25f,
                (step) =>
                {
                    digitObjects[0].transform.GetChild(firstDigit - 1 >= 0 ? firstDigit - 1 : digitObjects[0].transform.childCount - 1).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, step);
                }, null));
        }

        if ((int)(digitObjects[1].transform.localPosition.y) != secondDigit || movedCount <= 6) {
            AddMovedCount();
            StartCoroutine(Interpolators.Curve(Interpolators.EaseOutCurve, digitObjects[1].transform.localPosition.y, secondDigit, movementDuration,
                (step) =>
                {
                    digitObjects[1].transform.localPosition = new Vector3(digitObjects[1].transform.localPosition.x, step, 0);
                }, null));

            StartCoroutine(Interpolators.Curve(Interpolators.LinearCurve, 0.25f, 0.8f, 0.25f,
                (step) =>
                {
                    digitObjects[1].transform.GetChild(secondDigit).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, step);
                }, null));
            StartCoroutine(Interpolators.Curve(Interpolators.LinearCurve, 0.8f, 0.25f, 0.25f,
                (step) =>
                {
                    digitObjects[1].transform.GetChild(secondDigit - 1 >= 0 ? secondDigit - 1 : digitObjects[1].transform.childCount - 1).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, step);
                }, null));
        }

    }

    private void AddMovedCount() {
        movedCount++;
        if(movedCount > 10) {
            movedCount = 10;
        }
    }

}
