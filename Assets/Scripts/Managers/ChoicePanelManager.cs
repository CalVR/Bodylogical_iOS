﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanelManager : MonoBehaviour {
    public static ChoicePanelManager Instance { get; private set; }

    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Image background;
    [SerializeField] private LocalizedText title;
    [SerializeField] private LocalizedText message;
    [SerializeField] private LocalizedText data;

    [SerializeField] private Color noneColor, minimalColor, optimalColor;

    private readonly Dictionary<HealthChoice, string> texts = new Dictionary<HealthChoice, string> {
        { HealthChoice.None, "Legends.InfoCurrentTitle" },
        { HealthChoice.Minimal, "Legends.InfoMinimalTitle" },
        { HealthChoice.Optimal, "Legends.InfoOptimalTitle" }
    };

    private readonly Dictionary<HealthChoice, string> messages = new Dictionary<HealthChoice, string> {
        { HealthChoice.None, "Legends.InfoCurrent" },
        { HealthChoice.Minimal, "Legends.InfoMinimal" },
        { HealthChoice.Optimal, "Legends.InfoOptimal" }
    };

    private Dictionary<HealthChoice, Color> colors;

    public bool Active => choicePanel.activeInHierarchy;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        colors = new Dictionary<HealthChoice, Color> {
            { HealthChoice.None, noneColor },
            { HealthChoice.Minimal, minimalColor },
            { HealthChoice.Optimal, optimalColor }
        };
    }

    public void ToggleChoicePanels(bool on) {
        choicePanel.SetActive(on);
    }

    public void SetValues() {
        HealthChoice choice = TimeProgressManager.Instance.Path;
        background.color = colors[choice];
        title.SetText(texts[choice]);
        message.SetText(messages[choice]);

        Lifestyle lifestyle = ArchetypeManager.Instance.Selected.archetype.lifestyleDict[choice];
        data.SetText("Legends.InfoTemplate",
            new LocalizedParam(lifestyle.sleepHours),
            new LocalizedParam(lifestyle.exercise),
            new LocalizedParam(lifestyle.calories),
            new LocalizedParam(LocalizationDicts.statuses[lifestyle.adherence], true)
        );
    }
}
