﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A manager that controls the human archetypes.
/// </summary>
public class HumanManager : MonoBehaviour {
    public static HumanManager Instance { get; private set; }

    public Archetype SelectedArchetype { get; set; }
    public GameObject SelectedHuman { get { return SelectedArchetype.HumanObject; } }
    public bool IsHumanSelected { get; private set; }
    public bool StartSelectHuman { get; set; }

    private float coolingTime;
    private bool yearPanelShowed;

    #region Unity routines
    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start() {
        IsHumanSelected = false;
        yearPanelShowed = false;
        coolingTime = 0;
    }

    // Update is called once per frame
    void Update() {
        if (!IsHumanSelected) {
            if (StartSelectHuman && CheckHumanSelection()) {
                IsHumanSelected = true;
                StartSelectHuman = false;
            }
        } else {
            IsHumanSelected = false;
        }

        if (coolingTime < 3) {
            coolingTime += Time.deltaTime;
        }
    }
    #endregion

    #region Phase3
    /// <summary>
    /// Checks if a human model is selected.
    /// </summary>
    /// <returns><c>true</c>, if a model is selected, <c>false</c> otherwise.</returns>
    private bool CheckHumanSelection() {
        // DebugText.Instance.Log("Checking Human Selection...");
        foreach (Archetype human in ArchetypeContainer.Instance.profiles) {
            if (human.HumanObject.GetComponentInChildren<HumanInteract>().isSelected) {
                SelectedArchetype = human;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Starts a coroutine to move the selected model to center of stage.
    /// </summary>
    public bool MoveSelectedHumanToCenter() {
        if (!IsHumanSelected) {
            return false;
        }

        StartCoroutine(MoveHumanTowardCenter());
        return true;
    }

    /// <summary>
    /// Moves the human toward center.
    /// </summary>
    IEnumerator MoveHumanTowardCenter() {
        if (IsHumanSelected && SelectedHuman != null) {
            float movedDist = 0;

            Vector3 startpos = SelectedHuman.transform.position;
            Vector3 endpos = StageManager.Instance.CenterTransform.position;

            float journeyLength = Vector3.Distance(startpos, endpos);

            while (movedDist < journeyLength) {
                float fracJourney = movedDist / journeyLength;
                SelectedHuman.transform.position = Vector3.Lerp(startpos, endpos, fracJourney);
                movedDist += Time.deltaTime;
                yield return null;
            }

            SelectedHuman.transform.position = endpos;
            SelectedHuman.transform.rotation = StageManager.Instance.stage.transform.rotation;
        }

        yield return null;
    }

    /// <summary>
    /// Toggles all unselected human.
    /// </summary>
    public void ToggleUnselectedHuman(bool on) {
        foreach (Archetype human in ArchetypeContainer.Instance.profiles) {
            if (human.HumanObject != SelectedHuman) {
                human.HumanObject.SetActive(on);
            }
        }
    }

    /// <summary>
    /// Disables/Enables the collider of the selected human body.
    /// </summary>
    public void ToggleInteraction(bool on) {
        GameObject model = SelectedHuman.transform.Find("model").gameObject;
        model.GetComponent<CapsuleCollider>().enabled = on;
        model.GetComponent<HumanInteract>().enabled = on;
    }
    #endregion

    #region Phase4
    /// <summary>
    /// Expand selected profile details.
    /// </summary>
    public void ExpandSelectedHumanInfo() {
        if (SelectedHuman == null) {
            return;
        }

        SelectedHuman.transform.Search("BasicInfoCanvas").gameObject.SetActive(false);
        DetailPanelManager.Instance.ToggleDetailPanel(true);
        DetailPanelManager.Instance.SetValues();
        coolingTime = 0f;
    }

    /// <summary>
    /// After clicking "predict", the three paths would appear.
    /// </summary>
    public void FireChoicesNextPeriod() {
        SelectedHuman.transform.Search("BasicInfoCanvas").gameObject.SetActive(false);
        DetailPanelManager.Instance.ToggleDetailPanel(false);

        ChoicePanelManager.Instance.ToggleChoicePanels(true);

        ButtonSequenceManager.Instance.SetPredictButton(false);
        TutorialText.Instance.ShowDouble("These are the paths Bodylogical generated", "Click on any panel to continue.", 5.0f);
    }

    /// <summary>
    /// The year panels are shown, but ribbons are not drawn yet.
    /// </summary>
    public void FireNextPeriod(int choice) {
        if (yearPanelShowed) {
            return;
        }

        StartCoroutine(EnableYearPanels(choice));
        ButtonSequenceManager.Instance.SetLineChartButton(true);
        TutorialText.Instance.Show("Please select \"Line Chart\" to Create Ribbon Charts.", 12.0f);
    }

    /// <summary>
    /// Hide the choice panels, shift the person to the left, and display the year panels.
    /// </summary>
    /// <returns>The year panels.</returns>
    /// <param name="choice">Choice.</param>
    public IEnumerator EnableYearPanels(int choice) {
        ChoicePanelManager.Instance.ToggleChoicePanels(false);
        yield return MoveSelectedHumanToLeft();

        YearPanelManager.Instance.ToggleYearPanels(true);
        yield return new WaitForSeconds(2f);

        yearPanelShowed = true;
        yield return null;
    }

    /// <summary>
    /// Moves the selected human to the left of the stage.
    /// </summary>
    /// <returns><c>true</c>, if selected human to left was moved, <c>false</c> otherwise.</returns>
    public bool MoveSelectedHumanToLeft() {
        if (!IsHumanSelected) {
            return false;
        }

        StartCoroutine(MoveHumanTowardLeft());
        return true;
    }

    /// <summary>
    /// Moves the human toward left.
    /// </summary>
    IEnumerator MoveHumanTowardLeft() {
        if (IsHumanSelected && SelectedHuman != null) {
            float movedDist = 0;

            Vector3 startpos = SelectedHuman.transform.localPosition;
            Vector3 center = StageManager.Instance.CenterTransform.localPosition;
            Vector3 endpos = new Vector3(center.x - 0.2f, center.y, center.z);

            float journeyLength = Vector3.Distance(startpos, endpos);

            while (movedDist < journeyLength) {
                float fracJourney = movedDist / journeyLength;
                SelectedHuman.transform.localPosition = Vector3.Lerp(startpos, endpos, fracJourney);
                movedDist += Time.deltaTime;
                yield return null;
            }

            SelectedHuman.transform.localPosition = endpos;
            // We always move from center to left, so no need for keeping rotation.
            //SelectedHuman.transform.rotation = StageManager.Instance.stage.transform.rotation;
        }

        yield return null;
    }

    /// <summary>
    /// Reset to choice panels.
    /// </summary>
    public void ResetPeriod() {
        if (!yearPanelShowed) {
            return;
        }

        MoveSelectedHumanToCenter();

        YearPanelManager.Instance.Reset();
        YearPanelManager.Instance.ToggleYearPanels(false);
        ChoicePanelManager.Instance.ToggleChoicePanels(true);

        yearPanelShowed = false;
    }
    #endregion
}
