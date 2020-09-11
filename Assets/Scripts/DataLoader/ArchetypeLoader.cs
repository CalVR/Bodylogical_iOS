﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A container for preset archetypes.
/// </summary>
public class ArchetypeLoader : MonoBehaviour {
    public static ArchetypeLoader Instance { get; private set; }

    /// <summary>
    /// The profiles with data.
    /// </summary>
    public List<Archetype> Profiles { get; private set; }

    public GameObject modelTemplate;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Loads all archetype-related data.
    /// </summary>
    void Start() {
        TextAsset archetypes = Resources.Load<TextAsset>("Data/Archetypes");
        Profiles = CSVParser.LoadCsv<Archetype>(archetypes.text);

        TextAsset lifestyle = Resources.Load<TextAsset>("Data/P1Lifestyle");
        List<Lifestyle> lifestyles = CSVParser.LoadCsv<Lifestyle>(lifestyle.text);
        foreach (Archetype archetype in Profiles) {
            archetype.lifestyleDict = lifestyles.ToDictionary(x => x.choice, x => x);
        }
    }
}
