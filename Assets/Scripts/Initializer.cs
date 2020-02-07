using System;
using UnityEngine;

public class Initializer : MonoBehaviour
{

    public sbyte direction;

    public GameObject digitArea;

    GameObject[] digitListHours;
    GameObject[] digitListMinutes;
    GameObject[] digitListSeconds;

    
    int movedCount = 0;

    int update = 0;

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
            StartCoroutine(Interpolators.Curve(Interpolators.EaseOutCurve, digitObjects[0].transform.localPosition.y, firstDigit, 0.25f,
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
            StartCoroutine(Interpolators.Curve(Interpolators.EaseOutCurve, digitObjects[1].transform.localPosition.y, secondDigit, 0.25f,
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
