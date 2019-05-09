﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerAnimationVisualizer : Visualizer {
    public override string VisualizerKey { get { return "General.ActSoc"; } }

    public Transform ArchetypeTransform { get { return HumanManager.Instance.SelectedHuman.transform; } }
    /// <summary>
    /// To be determined at runtime, so use property.
    /// </summary>
    /// <value>The archetype animator.</value>
    public Animator ArchetypeAnimator { get { return HumanManager.Instance.HumanAnimator; } }
    public override HealthStatus Status { get; set; }

    public Vector3 companionOriginalLocalPos;
    // TODO: reconsider if we need animation or simply Lerp
    //public Animator soccerAnimator;
    public GameObject soccer;
    public Vector3 leftPoint, rightPoint;

    private IEnumerator soccerMovement;
    private bool movingRight = true;
    private float soccerSpeed;

    public override void Initialize() {
        ActivityManager.Instance.CurrentTransform.localPosition = companionOriginalLocalPos;
    }

    public override bool Visualize(int index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);
        // Let people face each other
        ArchetypeTransform.localEulerAngles = new Vector3(0, -90, 0);
        ActivityManager.Instance.CurrentTransform.localEulerAngles = new Vector3(0, 90, 0);

        if (soccerMovement == null) {
            soccerMovement = Kick();
            StartCoroutine(soccerMovement);
        }

        if (newStatus != Status) {
            Status = newStatus;
            return true;
        }
        return false;
    }

    public override void Pause() {
        if (soccerMovement != null) {
            StopCoroutine(soccerMovement);
            soccerMovement = null;
            ActivityManager.Instance.CurrentAnimator.SetTrigger("Idle");
            ArchetypeAnimator.SetTrigger("Idle");
        }

        ArchetypeTransform.localEulerAngles = new Vector3(0, 0, 0);
        ActivityManager.Instance.CurrentTransform.localEulerAngles = new Vector3(0, 0, 0);
    }

    private HealthStatus GenerateNewSpeed(int index, HealthChoice choice) {
        int score = HealthDataContainer.Instance.choiceDataDictionary[choice].CalculateHealth(index,
          HumanManager.Instance.SelectedArchetype.gender);

        soccerSpeed = score * 0.001f;

        return HealthUtil.CalculateStatus(score);
    }

    private IEnumerator Kick() {
        float stepLength = 0;
        while (true) {
            if (movingRight) {
                ArchetypeAnimator.SetTrigger("Soccer");
            } else {
                ActivityManager.Instance.CurrentAnimator.SetTrigger("Soccer");
            }

            yield return new WaitForSeconds(1.5f);

            if (movingRight) {
                ArchetypeAnimator.SetTrigger("Idle");
            } else {
                ActivityManager.Instance.CurrentAnimator.SetTrigger("Idle");
            }

            Vector3 startPos, endPos;
            if (movingRight) {
                startPos = leftPoint;
                endPos = rightPoint;
            } else {
                startPos = rightPoint;
                endPos = leftPoint;
            }

            while (stepLength < 1.0f) {
                soccer.transform.localPosition = Vector3.Lerp(startPos, endPos, stepLength);
                stepLength += soccerSpeed;
                yield return null;
            }

            soccer.transform.localPosition = endPos;
            movingRight = !movingRight;
            stepLength = 0;
            yield return null;
        }
    }
}
