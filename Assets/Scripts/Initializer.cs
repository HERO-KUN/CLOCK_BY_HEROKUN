using System;
using System.IO;
using System.Collections;
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

    GameObject[] digitListHours;
    GameObject[] digitListMinutes;
    GameObject[] digitListSeconds;

    private int update = 0;
    private bool firstUpdate = true;

    private const int movementDuration = 65;
    private readonly AnimationCurve movementCurve = new AnimationCurve();

    private readonly int[][] previousDigitData = { new int[2], new int[2], new int[2] };

    // Start is called before the first frame update
    void Start()
    {

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

        Keyframe startFrame = new Keyframe(0, 0) { inTangent = 3, outTangent = 3 };
        Keyframe lastFrame = new Keyframe(1, 1) { inTangent = 0, outTangent = 0};
        movementCurve.AddKey(startFrame);
        movementCurve.AddKey(lastFrame);

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

        update = DateTime.Now.Second;

    }

    // Update is called once per frame
    void Update()
    {
        DateTime currentTime = DateTime.Now;
        
        if (update == currentTime.Second) return;

        update = currentTime.Second;

        StartCoroutine(UpdateDigits(movementDuration, currentTime.Second, 2, digitListSeconds));
        StartCoroutine(UpdateDigits(movementDuration, currentTime.Minute, 1, digitListMinutes));
        StartCoroutine(UpdateDigits(movementDuration, currentTime.Hour, 0, digitListHours));
    }

    private IEnumerator UpdateDigits(int length, int time, int digitType, GameObject[] digitObjects) {
        int firstDigitData = time / 10;
        int secondDigitData = time - (time / 10 * 10);
        bool updateFirstDigit = previousDigitData[digitType][0] != firstDigitData;
        bool updateSecondDigit = previousDigitData[digitType][1] != secondDigitData;
        
        if ((!updateFirstDigit && !updateSecondDigit) && !firstUpdate) {
            yield break;
        }

        for (int i = 0; i < length; i++) {
            if (updateFirstDigit || firstUpdate) {
                digitObjects[0].transform.localPosition = new Vector3(digitObjects[0].transform.localPosition.x, Mathf.Lerp(previousDigitData[digitType][0], firstDigitData, movementCurve.Evaluate(i/(float)length)), 0);
                digitObjects[0].transform.GetChild(firstDigitData).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, i / (float)length * (firstDigitData == 0 ? 0.5f : 0.8f));
                digitObjects[0].transform.GetChild(previousDigitData[digitType][0]).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, (previousDigitData[digitType][0] == 0 ? 0.5f : 0.8f) - i / (float)length * (previousDigitData[digitType][0] == 0 ? 0.25f : 0.55f));
            }

            if (updateSecondDigit || firstUpdate) {
                digitObjects[1].transform.localPosition = new Vector3(digitObjects[1].transform.localPosition.x, Mathf.Lerp(previousDigitData[digitType][1], secondDigitData, movementCurve.Evaluate(i / (float)length)), 0);
                digitObjects[1].transform.GetChild(secondDigitData).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 0.25f + i / (float)length * 0.55f);
                digitObjects[1].transform.GetChild(previousDigitData[digitType][1]).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 0.8f - (i / (float)length * 0.55f));
            }

            yield return new WaitForEndOfFrame();
        }

        if (updateFirstDigit || firstUpdate) {
            digitObjects[0].transform.localPosition = new Vector3(digitObjects[0].transform.localPosition.x, firstDigitData, 0);
            previousDigitData[digitType][0] = firstDigitData;
        }
        if (updateSecondDigit || firstUpdate) {
            digitObjects[1].transform.localPosition = new Vector3(digitObjects[1].transform.localPosition.x, secondDigitData, 0);
            previousDigitData[digitType][1] = secondDigitData;
        }

        if (digitType == 0) {
            for (int i = 0; i < digitListHours[0].transform.childCount; i++) {
                if (i != firstDigitData)
                    digitListHours[0].transform.GetChild(i).GetComponent<TextMesh>().color = new Color(1, 1, 1, 0.25f);
            }
            for (int i = 0; i < digitListHours[1].transform.childCount; i++) {
                if (i != secondDigitData)
                    digitListHours[1].transform.GetChild(i).GetComponent<TextMesh>().color = new Color(1, 1, 1, 0.25f);
            }
        } else if (digitType == 1) {
            for (int i = 0; i < digitListMinutes[0].transform.childCount; i++) {
                if (i != firstDigitData)
                    digitListMinutes[0].transform.GetChild(i).GetComponent<TextMesh>().color = new Color(1, 1, 1, 0.25f);
            }
            for (int i = 0; i < digitListMinutes[1].transform.childCount; i++) {
                if (i != secondDigitData)
                    digitListMinutes[1].transform.GetChild(i).GetComponent<TextMesh>().color = new Color(1, 1, 1, 0.25f);
            }
        } else {
            for (int i = 0; i < digitListSeconds[0].transform.childCount; i++) {
                if (i != firstDigitData)
                    digitListSeconds[0].transform.GetChild(i).GetComponent<TextMesh>().color = new Color(1, 1, 1, 0.25f);
            }
            for (int i = 0; i < digitListSeconds[1].transform.childCount; i++) {
                if (i != secondDigitData)
                    digitListSeconds[1].transform.GetChild(i).GetComponent<TextMesh>().color = new Color(1, 1, 1, 0.25f);
            }
        }

        firstUpdate = false;

    }

}
