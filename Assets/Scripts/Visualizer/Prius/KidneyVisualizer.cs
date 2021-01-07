using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class KidneyVisualizer : OrganVisualizer {
    [SerializeField] private GameObject leftKidney, rightKidney;
    [SerializeField] private Image indicator;
    [SerializeField] private SlideBarPointer slidebar;

    private readonly Dictionary<HealthStatus, string> messages = new Dictionary<HealthStatus, string> {
        {HealthStatus.Good, "Legends.PriKidneyGood"},
        {HealthStatus.Moderate, "Legends.PriKidneyIntermediate"},
        {HealthStatus.Bad, "Legends.PriKidneyBad"}
    };

    private SkinnedMeshRenderer LeftRenderer => leftKidney.GetComponent<SkinnedMeshRenderer>();
    private SkinnedMeshRenderer RightRenderer => rightKidney.GetComponent<SkinnedMeshRenderer>();
    public override string ExplanationText => LocalizationManager.Instance.FormatString(messages[status]);

    public override bool Visualize(float index, HealthChoice choice) {
        bool kidneyChanged = UpdateStatus(index, choice);
        //indicator.color = PriusManager.Instance.colorLibrary.StatusColorDict[status];

        if (gameObject.activeInHierarchy) {
            // Both kidneys will be shown.
            leftKidney.SetActive(true);
            rightKidney.SetActive(true);
            LeftRenderer.SetBlendShapeWeight(0, 100 - score);
            RightRenderer.SetBlendShapeWeight(0, 100 - score);
        }

        slidebar.SetProgress(score);
        return kidneyChanged;
    }

    /// <summary>
    /// Kidney is related to sbp and aic values. Use these two to generate a new status.
    /// </summary>
    /// <returns>true if the status has changed since the last call, false otherwise.</returns>
    public override bool UpdateStatus(float index, HealthChoice choice) {
        return false;
        // Archetype data = ArchetypeManager.Instance.Selected.ArchetypeData;
        // LongTermHealth health = data.healthDict[choice];
        // score = health.CalculateHealth(index, data.gender, HealthType.sbp, HealthType.aic);
        // HealthStatus currStatus = HealthUtil.CalculateStatus(score);
        //
        // // Floats are inaccurate; equals index == 0
        // if (Mathf.Abs(index) <= 0.001f) {
        //     status = currStatus;
        //     return false;
        // }
        //
        // bool changed = currStatus != status;
        // status = currStatus;
        //
        // return changed;
    }
}