﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class ActivityManager : MonoBehaviour {
    public static ActivityManager Instance { get; private set; }

    public GameObject animationObjects;

    // TODO: change into generic visualizer.
    public SoccerAnimationVisualizer visualizer;

    private bool isLeft;
    private bool initialized = false;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Switch to Animations view.
    /// </summary>
    public IEnumerator StartAnimations() {
        if (TimeProgressManager.Instance.Path == HealthChoice.NotSet) {
            yield return HumanManager.Instance.MoveSelectedHumanToCenter();
            TutorialText.Instance.ShowDouble("First click the path to visualize", "Then use buttons to move through time", 3);
        } else {
            yield return HumanManager.Instance.MoveSelectedHumanToLeft();
            isLeft = true;
            Visualize(0, TimeProgressManager.Instance.Path);
        }
        yield return null;
    }

    //public void PauseAnimations() {
    //    visualizer.Pause();
    //}

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// </summary>
    public void ToggleAnimation(bool on) {
        ButtonSequenceManager.Instance.SetActivitiesButton(!on);

        animationObjects.SetActive(false); // only appears after selecting a path
        ButtonSequenceManager.Instance.SetTimeControls(on);

        ButtonSequenceManager.Instance.SetLineChartButton(on);
        ButtonSequenceManager.Instance.SetPriusButton(on);
        ButtonSequenceManager.Instance.SetTimeControls(on);
        isLeft = false;
        visualizer.Pause();
    }

    /// <summary>
    /// Play the animation.
    /// </summary>
    public void Visualize(int index, HealthChoice choice) {
        if (!isLeft) {
            HumanManager.Instance.MoveSelectedHumanToLeft();
        }
        isLeft = true;
        if (!initialized) {
            visualizer.Initialize();
            initialized = true;
        }
        animationObjects.SetActive(true);
        visualizer.Visualize(index, choice);
    }
}
