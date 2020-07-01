﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RangeLoader : MonoBehaviour {
    public static RangeLoader Instance { get; private set; }

    private List<HealthRange> ranges;
    
    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// populates the dictionary.
    /// </summary>
    void Start() {
        TextAsset rangeAsset = Resources.Load<TextAsset>("Data/Ranges");
        ranges = CSVParser.LoadCsv<HealthRange>(rangeAsset.text);
    }

    public HealthRange GetRange(HealthType type, Gender gender) {
        var selectedRanges = from r in ranges
                             where r.type == type && (r.gender == gender || r.gender == Gender.Either)
                             select r;

        return selectedRanges.First();
    }

    public int CalculatePoint(HealthType type, Gender gender, float value) {
        HealthRange range = GetRange(type, gender);
        if (value < range.min) {
            return 100;
        }

        if (value < range.warning) { // 100 - 60
            return (int)((40 * value + 60 * range.min - 100 * range.warning) / (range.min - range.warning));
        }

        if (value < range.upper) { // 60 - 30
            return (int)((30 * value + 30 * range.warning - 60 * range.upper) / (range.warning - range.upper));
        }

        if (value < range.max) {
            return (int)((30 * value - 30 * range.max) / (range.upper - range.max));
        }

        return 0; // value >= max
    }
}
