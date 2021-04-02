﻿using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the panels that display an avatar's basic data.
/// </summary>
public class DetailPanel : MonoBehaviour {
    [SerializeField] private PanelItem weight, glucose, hba1c, bloodPressure;
    [SerializeField] private Image[] panels;
    [SerializeField] private ColorLibrary colorLibrary;

    // Only used for displayers.
    [SerializeField] private ArchetypeModel model;

    [SerializeField] private float cycleInterval = 0.2f;

    private readonly bool[] panelOpened = new bool[4];
    private bool lockIcon;

    private ExpandableWindow[] windows;

    private ExpandableWindow[] Windows => windows ?? (windows = GetComponentsInChildren<ExpandableWindow>(true));

    public bool AllClicked => panelOpened.All(opened => opened);
    
    private LongTermHealth longTermHealth;

    private void OnEnable() {
        if (longTermHealth == null) {
            return;
        }

        StartCoroutine(CycleData());
    }

    /// <summary>
    /// Updates the items on the detail panels.
    /// </summary>
    public void SetValues(LongTermHealth health, bool setColor = false) {
        longTermHealth = health;

        if (setColor) {
            Color c = colorLibrary.ChoiceColorDict[health.choice];

            foreach (Image panel in panels) {
                float alpha = panel.color.a;
                c.a = alpha;
                panel.color = c;
            }
        }
    }

    private IEnumerator CycleData() {
        while (true) {
            foreach (Health h in longTermHealth) {
                SetValues(h);
                yield return new WaitForSeconds(cycleInterval);
            }
        }
    }

    private void SetValues(Health h) {
        weight.SetValue(0, h[HealthType.weight]);
        weight.SetValue(1, h[HealthType.bmi]);

        glucose.SetValue(0, h[HealthType.glucose]);
        hba1c.SetValue(0, h[HealthType.aic]);

        bloodPressure.SetValue(0, h[HealthType.sbp]);
        bloodPressure.SetValue(1, h[HealthType.dbp]);
    }

    public void Toggle(bool on) {
        gameObject.SetActive(on);
        if (on) {
            foreach (ExpandableWindow window in Windows) {
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
            ((ArchetypeDisplayer) model).icon.SetActive(true);
        }
    }

    public void Reset() {
        Toggle(false);
        for (int i = 0; i < 4; i++) {
            panelOpened[i] = false;
        }

        lockIcon = false;
        foreach (ExpandableWindow window in Windows) {
            window.Reset();
        }
    }
}