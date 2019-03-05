﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoggingVisualizer : Visualizer {
    public Transform ArchetypeTransform { get { return HumanManager.Instance.SelectedHuman.transform; } }
    /// <summary>
    /// To be determined at runtime, so use property.
    /// </summary>
    /// <value>The archetype animator.</value>
    public Animator ArchetypeAnimator { get { return HumanManager.Instance.ModelTransform.GetComponent<Animator>(); } }
    public override HealthStatus Status { get; set; }


    public Transform companionTransform;
    public Animator CompanionAnimator { get { return companionTransform.GetComponent<Animator>(); } }
    public Transform leftPoint, rightPoint;
    /// <summary>
    /// This cannot be determined at runtime because Awake() won't be called if
    /// the object is disabled and Pause() would shift the companion's position,
    /// even if the object is disabled.
    /// </summary>
    public Vector3 companionOriginalLocalPos;

    private IEnumerator archetypeMovement;
    private IEnumerator companionMovement;
    private float archetypeMovementSpeed;
    private float companionMovementSpeed = 0.003f;
    private bool archetypeRunning = true;
    private bool archetypeTriggerSet;

    public override void Initialize() {
        companionTransform.localPosition = companionOriginalLocalPos;
        HumanManager.Instance.ModelTransform.localPosition = new Vector3(0, 0, 0);
    }

    public override bool Visualize(int index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);

        if (archetypeMovement == null) {
            ArchetypeTransform.localEulerAngles = new Vector3(0, -90, 0);
            CompanionAnimator.transform.localEulerAngles = new Vector3(0, 90, 0);
            archetypeMovement = ArchetypeJog();
            StartCoroutine(archetypeMovement);
            companionMovement = CompanionJog();
            StartCoroutine(companionMovement);
        }

        if (newStatus != Status) {
            Status = newStatus;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Stops the animation, moves the people back to original position.
    /// </summary>
    public override void Pause() {
        if (archetypeMovement != null) {
            StopCoroutine(archetypeMovement);
            archetypeMovement = null;
            StopCoroutine(companionMovement);
            companionMovement = null;
            CompanionAnimator.Play("Idle");
            ArchetypeAnimator.Play("Idle");
            ArchetypeTransform.localPosition = leftPoint.localPosition;
            ArchetypeTransform.localEulerAngles = new Vector3(0, 0, 0);
            companionTransform.localPosition = companionOriginalLocalPos;
            CompanionAnimator.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    /// <summary>
    /// Generates a new speed for both the archetype and the companion.
    /// Speed is calculated using the health scores AND the age.
    /// 
    /// </summary>
    /// <returns>The new speed.</returns>
    /// <param name="index">Index. Range from 0-4. The larger it is, the older the people are.</param>
    /// <param name="choice">Choice.</param>
    private HealthStatus GenerateNewSpeed(int index, HealthChoice choice) {
        int score = HealthDataContainer.Instance.choiceDataDictionary[choice].CalculateHealth(index,
          HumanManager.Instance.UseAlt);
        float yearMultiplier = 1 - index * 0.05f;

        companionMovementSpeed = 0.003f * yearMultiplier;
        CompanionAnimator.SetFloat("JoggingSpeed", yearMultiplier);

        archetypeMovementSpeed = score * 0.00003f * yearMultiplier;
        float archetypeAnimationSpeed = score * 0.01f * yearMultiplier;
        if (archetypeAnimationSpeed <= 0.5f) { // switch to walking
            if (archetypeRunning) {
                archetypeTriggerSet = false;
            }
            archetypeRunning = false;
            ArchetypeAnimator.SetFloat("WalkingSpeed", score * 0.02f * yearMultiplier);
        } else {
            if (!archetypeRunning) {
                archetypeTriggerSet = false;
            }
            archetypeRunning = true;
            ArchetypeAnimator.SetFloat("JoggingSpeed", score * 0.01f * yearMultiplier);
        }


        return HealthUtil.CalculateStatus(score);
    }

    private IEnumerator ArchetypeJog() {
        archetypeTriggerSet = false; // TODO: will this solve the running bug?
        float stepLength = 0;
        float totalDist = Vector3.Distance(leftPoint.localPosition, rightPoint.localPosition);
        bool archetypeMovingRight = true;
        while (true) {
            Vector3 startPos, endPos;
            if (archetypeMovingRight) {
                startPos = leftPoint.localPosition;
                endPos = rightPoint.localPosition;
            } else {
                startPos = rightPoint.localPosition;
                endPos = leftPoint.localPosition;
            }

            while (stepLength < 1.0f) {
                if (archetypeRunning) {
                    if (!archetypeTriggerSet) {
                        archetypeTriggerSet = true;
                        ArchetypeAnimator.ResetTrigger("Walk");
                        ArchetypeAnimator.SetTrigger("Jog");
                    }
                } else {
                    if (!archetypeTriggerSet) {
                        archetypeTriggerSet = true;
                        ArchetypeAnimator.ResetTrigger("Jog");
                        ArchetypeAnimator.SetTrigger("Walk");
                    }
                    // Walking animation would move the character.
                    // Keep moving the mode to original so that we can use archetypetransform to customize speed.
                    HumanManager.Instance.ModelTransform.localPosition = new Vector3(0, 0, 0);
                }

                ArchetypeTransform.localPosition = Vector3.Lerp(startPos, endPos, stepLength);
                stepLength += archetypeMovementSpeed;

                yield return null;
            }

            ArchetypeTransform.localPosition = endPos;
            archetypeMovingRight = !archetypeMovingRight;
            stepLength = 0.0f;
            if (archetypeMovingRight) {
                ArchetypeTransform.localEulerAngles = new Vector3(0, -90, 0);
            } else {
                ArchetypeTransform.localEulerAngles = new Vector3(0, 90, 0);
            }
            yield return null;
        }
    }

    private IEnumerator CompanionJog() {
        CompanionAnimator.SetTrigger("Jog");

        float stepLength = 0;
        bool companionMovingRight = true;
        while (true) {
            Vector3 startPos, endPos;
            if (companionMovingRight) {
                startPos = new Vector3(leftPoint.localPosition.x, leftPoint.localPosition.y, companionOriginalLocalPos.z);
                endPos = new Vector3(rightPoint.localPosition.x, rightPoint.localPosition.y, companionOriginalLocalPos.z);
            } else {
                startPos = new Vector3(rightPoint.localPosition.x, rightPoint.localPosition.y, companionOriginalLocalPos.z);
                endPos = new Vector3(leftPoint.localPosition.x, leftPoint.localPosition.y, companionOriginalLocalPos.z);
            }

            while (stepLength < 1.0f) {
                companionTransform.localPosition = Vector3.Lerp(startPos, endPos, stepLength);
                stepLength += companionMovementSpeed;
                yield return null;
            }

            companionTransform.localPosition = endPos;
            companionMovingRight = !companionMovingRight;
            stepLength = 0;
            if (companionMovingRight) {
                companionTransform.localEulerAngles = new Vector3(0, 90, 0);
            } else {
                companionTransform.localEulerAngles = new Vector3(0, -90, 0);
            }
            yield return null;
        }
    }
}
