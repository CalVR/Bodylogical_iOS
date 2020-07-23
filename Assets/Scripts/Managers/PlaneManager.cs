﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour {
    public static PlaneManager Instance { get; private set; }
    public float maxScale;
    [SerializeField] private Transform freeTutorialTransform;
    public bool PlaneFound { get; private set; }
    private List<GameObject> planes;
    private PlaneFinder finder;
    private IEnumerator scan;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        finder = GetComponent<PlaneFinder>();
    }

    private IEnumerator Scan() {
        TutorialParam param = new TutorialParam("Tutorials.PlaneTitle", "Tutorials.PlaneText");
        TutorialManager.Instance.ShowTutorial(param, freeTutorialTransform, () => finder.planes.Count > 0,
            mode: TutorialRemindMode.Follow, postCallback: () => {
                param = new TutorialParam("Tutorials.ConfirmTitle", "Tutorials.ConfirmText");
                TutorialManager.Instance.ShowTutorial(param, freeTutorialTransform, () => PlaneFound,
                    mode: TutorialRemindMode.Follow);
            });
        
        while (!PlaneFound) {
            if (finder.planes.Count > 0 && InputManager.Instance.TouchCount > 0 &&
                InputManager.Instance.TapCount > 1) {
                // Double tap
                TutorialManager.Instance.ClearInstruction();
                planes = GetComponent<PlaneFinder>().Finish();
                PlaneFound = true;
            }

            yield return null;
        }
    }

    public void BeginScan() {
        GetComponent<PlaneFinder>().Begin();
        scan = Scan();
        StartCoroutine(scan);
    }

    public void EndScan() {
        if (scan != null) {
            StopCoroutine(scan);
            scan = null;
        }
    }

    public void RestartScan() {
        gameObject.GetComponent<PlaneFinder>().Reset();
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
        PlaneFound = false;
        scan = Scan();
        StartCoroutine(scan);
    }

    public void HidePlanes() {
        if (planes != null) {
            // If in debug/editor, MainPlane will be null.
            foreach (GameObject p in planes) {
                p.SetActive(false);
            }
        }
    }
}