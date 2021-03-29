﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the panels that display an avatar's basic data.
/// </summary>
public class DetailPanel : MonoBehaviour {
    [SerializeField] private PanelItem sleep, diet, exercise;
    [SerializeField] private LocalizedText adherence;
    [SerializeField] private Image[] panels;
    [SerializeField] private ColorLibrary colorLibrary;
    // Only used for displayers.
    [SerializeField] private ArchetypeModel model;
    private bool[] panelOpened = new bool[4];
    private bool lockIcon;

    private ExpandableWindow[] windows;

    public bool AllClicked {
        get {
            foreach (bool opened in panelOpened) {
                if (!opened) {
                    return false;
                }
            }

            return true;
        }
    }

    private void Start() {
        windows = GetComponentsInChildren<ExpandableWindow>();
    }

    /// <summary>
    /// Updates the items on the detail panels.
    /// </summary>
    public void SetValues(Lifestyle lifestyle, bool setColor = false) {
        // TODO
        // sleep.SetValue(lifestyle.sleepHours);
        // diet.SetValue(lifestyle.calories);
        // exercise.SetValue(lifestyle.exercise);
        // adherence.SetText(LocalizationDicts.statuses[lifestyle.adherence]);
        //
        // if (setColor) {
        //     Color c = colorLibrary.ChoiceColorDict[lifestyle.choice];
        //
        //     foreach (Image panel in panels) {
        //         float alpha = panel.color.a;
        //         c.a = alpha;
        //         panel.color = c;
        //     }
        // }
    }

    public void Toggle(bool on) {
        gameObject.SetActive(on);
        if (on) {
            foreach (ExpandableWindow window in windows) {
                window.Pulse();
            }
        }
    }

    /// <summary>
    /// Monitors the number of expandable panels that has been opened.
    /// This method will only be called on the panel attached to an ArchetypeDisplayer.
    /// </summary>
    public void OnPanelClick(int index) {
        if (lockIcon) {
            return;
        }

        panelOpened[index] = true;


        if (AllClicked) {
            // All four panels are opened, show icon
            lockIcon = true;
            ((ArchetypeDisplayer) model).Icon.SetActive(true);
        }
    }

    public void Reset() {
        Toggle(false);
        for (int i = 0; i < 4; i++) {
            panelOpened[i] = false;
        }

        lockIcon = false;
        foreach (ExpandableWindow window in windows) {
            window.Reset();
        }
    }
}