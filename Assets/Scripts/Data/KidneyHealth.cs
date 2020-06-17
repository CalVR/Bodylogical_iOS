﻿using System.Collections.Generic;
using UnityEngine;

public static class KidneyHealth {
    public static int score;
    public static HealthStatus status;
    public static string connectionMsg = "Legends.PriKidneyGeneral";

    private static readonly Dictionary<HealthStatus, string> messages
        = new Dictionary<HealthStatus, string> {
        { HealthStatus.Good, "Legends.PriKidneyGood" },
        { HealthStatus.Moderate, "Legends.PriKidneyIntermediate" },
        { HealthStatus.Bad, "Legends.PriKidneyBad" }
    };

    public static string ExplanationText => LocalizationManager.Instance.FormatString(messages[status]);

    public static bool UpdateStatus(float index, HealthChoice choice) {
        float sbpValue = Mathf.Lerp(
            HealthLoader.Instance.choiceDataDictionary[choice].sbp[(int)Mathf.Floor(index)],
            HealthLoader.Instance.choiceDataDictionary[choice].sbp[(int)Mathf.Ceil(index)],
            index % 1);
        int sbpScore = RangeLoader.Instance.CalculatePoint(HealthType.sbp,
            ArchetypeManager.Instance.selectedArchetype.gender,
            sbpValue);

        float aicValue = Mathf.Lerp(
            HealthLoader.Instance.choiceDataDictionary[choice].aic[(int)Mathf.Floor(index)],
            HealthLoader.Instance.choiceDataDictionary[choice].aic[(int)Mathf.Ceil(index)],
            index % 1);
        int aicScore = RangeLoader.Instance.CalculatePoint(HealthType.aic,
            ArchetypeManager.Instance.selectedArchetype.gender,
            aicValue);

        score = (sbpScore + aicScore) / 2;
        HealthStatus currStatus = HealthUtil.CalculateStatus(score);

        // Floats are inaccurate; equals index == 0
        if (Mathf.Abs(index) <= 0.001f) {
            status = currStatus;
            return false;
        }

        bool changed = currStatus != status;
        status = currStatus;

        return changed;
    }
}
