﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartVisualizer : Visualizer {
    /// <summary>
    /// TODO: change to cross-section
    /// </summary>
    //public GameObject goodKidney, badKidney;

    public override HealthStatus Status { get; set; }

    public string connectionMsg = "Heart health is related to blood pressure and LDL.\n";

    /// <summary>
    /// messages[status][true]: expanded message.
    /// messages[status][false]: brief message.
    /// </summary>
    private readonly Dictionary<HealthStatus, Dictionary<bool, string>> messages = new Dictionary<HealthStatus, Dictionary<bool, string>> {
        {
            HealthStatus.Good, new Dictionary<bool, string> {
                { true, "Low blood pressure and cholesterol levels mean a healthy circulatory system." },
                { false, "Heart is normal." }
            }
        },{
            HealthStatus.Intermediate, new Dictionary<bool, string> {
                { true, "Rising blood pressure and cholesterol will start damaging arteries that can lead to clogging." },
                { false, "Heart has trouble pumping blood." }
            }
        },{
            HealthStatus.Bad, new Dictionary<bool, string> {
                { true, "High blood pressure and cholesterol will clog the blood pressure and cause problems sucha s stroke, heart attack, etc." },
                { false, "Arteries have high chance of clogging, potentials for heart attacks." }
            }
        }
    };

    public string ExplanationText {
        get {
            bool expand = PriusManager.Instance.currentPart == PriusType.Heart;
            if (expand) {
                return connectionMsg + messages[Status][true];
            }
            return messages[Status][false];
        }
    }

    /// <summary>
    /// Visualize the heart.
    /// </summary>
    /// <returns>true if it changes state, false otherwise.</returns>
    /// <param name="index">index, NOT the year.</param>
    public override bool Visualize(int index, HealthChoice choice) {
        int sbpScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.sbp].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].sbp[index]);

        int ldlScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.ldl].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].LDL[index]);


        HealthStatus currStatus = HealthUtil.CalculateStatus((sbpScore + ldlScore) / 2);

        if (index == 0) {
            Status = currStatus;
            return false;
        }

        //if (currStatus == HealthStatus.Bad) {
        //    goodKidney.SetActive(false);
        //    badKidney.SetActive(true);
        //} else {
        //    goodKidney.SetActive(true);
        //    badKidney.SetActive(false);
        //}

        bool changed = currStatus != Status;
        Status = currStatus;
        return changed;
    }
}
