﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class AnimationManager : MonoBehaviour {
    public static AnimationManager Instance { get; private set; }

    public GameObject animationObjects;

    private bool isLeft;

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
        if (StageManager.Instance.Path == null) {
            yield return HumanManager.Instance.MoveSelectedHumanToCenter();
            TutorialText.Instance.ShowDouble("First click the path to visualize", "Then use buttons to move through time", 3);
        } else {
            yield return HumanManager.Instance.MoveSelectedHumanToLeft();
            Visualize();
        }
        yield return null;
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// </summary>
    public void ToggleAnimation(bool on) {
        ButtonSequenceManager.Instance.SetPropsButton(!on);

        //roomVisualization.SetActive(on);
        animationObjects.SetActive(false); // only appears after clicking "play"
        ButtonSequenceManager.Instance.SetTimeSlider(on);
        ButtonSequenceManager.Instance.SetPathButtons(on);

        ButtonSequenceManager.Instance.SetLineChartButton(on);
        ButtonSequenceManager.Instance.SetInternals(on);
        ButtonSequenceManager.Instance.SetTimeSlider(on);
    }

    /// <summary>
    /// Play the animation.
    /// </summary>
    public void Visualize() {
        //RoomVisualizer visualizer = roomVisualization.GetComponent<RoomVisualizer>();
        //visualizer.UpdateHeader(StageManager.Instance.Year, StageManager.Instance.Path);
        //visualizer.Visualize(GetPoint());
        if (!isLeft) {
            HumanManager.Instance.MoveSelectedHumanToLeft();
        }
        animationObjects.SetActive(true);
    }
}
