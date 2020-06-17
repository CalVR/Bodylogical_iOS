﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeProgressManager : MonoBehaviour {
    public static TimeProgressManager Instance { get; private set; }

    [SerializeField] private PlayPauseButton playPauseButton;
    [SerializeField] private LocalizedText headerText;
    [SerializeField] private SliderInteract sliderInteract;
    [SerializeField] private Text sliderText;

    private bool isTimePlaying;
    private IEnumerator timeProgressCoroutine;

    public HealthChoice Path { get; private set; }

    public float YearValue { get; private set; }
    private int year;

    public static readonly int maxYears = 20;

    public readonly Dictionary<HealthChoice, string> choicePathDictionary = new Dictionary<HealthChoice, string> {
        { HealthChoice.None, "General.PathRedVerbose" },
        { HealthChoice.Minimal, "General.PathYellowVerbose" },
        { HealthChoice.Optimal, "General.PathGreenVerbose" }
    };

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// When the slider is changed, update the year.
    /// </summary>
    /// <param name="value">Value.</param>
    public void UpdateYear(float value) {
        YearValue = value;
        if (Mathf.RoundToInt(value) != year) {
            year = Mathf.RoundToInt(value);
            UpdateHeaderText();
        }

        if (AppStateManager.Instance.currState == AppState.VisActivity) {
            ActivityManager.Instance.Visualize(YearValue / 5, Path);
        } else if (AppStateManager.Instance.currState == AppState.VisPrius) {
            bool healthChange = PriusManager.Instance.Visualize(YearValue / 5, Path);
            if (healthChange) {
                PriusManager.Instance.SetExplanationText();
                if (isTimePlaying) {
                    TimePlayPause();
                    TutorialManager.Instance.ShowStatus("Instructions.PriHealthChange");
                }
            }
        }
    }

    /// <summary>
    /// Switch the path based on the switcher.
    /// </summary>
    /// <param name="path">HealthChoice representation.</param>
    public void UpdatePath(int path) {
        Path = (HealthChoice)path;
        UpdateHeaderText();
        TutorialManager.Instance.ShowStatus("Instructions.PathSwitch",
             new LocalizedParam(choicePathDictionary[Path], true));

        if (AppStateManager.Instance.currState == AppState.VisActivity) {
            ActivityManager.Instance.Visualize(YearValue / 5, Path);
        } else if (AppStateManager.Instance.currState == AppState.VisPrius) {
            PriusManager.Instance.Visualize(YearValue / 5, Path);
            PriusManager.Instance.SetExplanationText();
        } else if (AppStateManager.Instance.currState == AppState.VisLineChart) {
            LineChartManager.Instance.Reload();
            ChoicePanelManager.Instance.SetValues();
        }
    }

    /// <summary>
    /// When "play/pause" button is clicked, start/stop time progression.
    /// </summary>
    public void TimePlayPause() {
        if (!isTimePlaying) { // stopped/paused, start
            timeProgressCoroutine = TimeProgress();
            StartCoroutine(timeProgressCoroutine);
        } else { // started, pause
            StopCoroutine(timeProgressCoroutine);
        }
        isTimePlaying = !isTimePlaying;
        playPauseButton.ChangeImage(isTimePlaying);
    }

    /// <summary>
    /// Update header text.
    /// </summary>
    public void UpdateHeaderText() {
        sliderText.text = year.ToString();
        headerText.SetText("Legends.HeaderText",
            new LocalizedParam(System.DateTime.Today.Year + year),
            new LocalizedParam(ArchetypeManager.Instance.selectedArchetype.age + year),
            new LocalizedParam(choicePathDictionary[Path], true));
    }

    /// <summary>
    /// When "stop" button is clicked, stop and reset time progression.
    /// </summary>
    public void TimeStop() {
        if (isTimePlaying) {
            TimePlayPause();
        }
        UpdateYear(0);
        sliderInteract.SetSlider(0);
        if (AppStateManager.Instance.currState == AppState.VisPrius) {
            PriusManager.Instance.SetExplanationText();
        }
    }

    /// <summary>
    /// Helper method to progress through time. Currently updates on a year to year basis.
    /// </summary>
    IEnumerator TimeProgress() {
        while (YearValue <= maxYears) {
            // update on a yearly basis
            if (Mathf.RoundToInt(YearValue) != year) {
                UpdateYear(YearValue);
            }

            sliderInteract.SetSlider(YearValue / maxYears);

            yield return null;
            YearValue += Time.deltaTime;
        }
        // after loop, stop.
        isTimePlaying = false;
        playPauseButton.ChangeImage(isTimePlaying);
        UpdateYear(maxYears);
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void Reset() {
        Path = HealthChoice.None;
        TimeStop();
    }
}