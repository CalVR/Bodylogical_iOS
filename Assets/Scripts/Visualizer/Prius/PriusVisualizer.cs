﻿using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An overall prius visualzer that controls the miniscule organs.
/// </summary>
public class PriusVisualizer : Visualizer {
    public override string VisualizerName { get { return "Internals"; } }
    public Color goodColor, intermediateColor, badColor;
    public Image heartIndicator, liverIndicator, kidneyIndicator;
    public GameObject goodHeart, badHeart, goodLiver, badLiver;
    public GameObject goodLeftKidney, badLeftKidney, goodRightKidney, badRightKidney;
    public HeartVisualizer heartVisualizer;
    public LiverVisualizer liverVisualizer;
    public KidneyVisualizer kidneyVisualizer;

    public bool KidneyLeft {
        get {
            return kidneyVisualizer.leftSelected;
        }
        set {
            kidneyVisualizer.leftSelected = value;
        }
    }

    public override HealthStatus Status { get; set; }

    /// <summary>
    /// Detailed text that would be shown on the panel.
    /// </summary>
    public string ExplanationText {
        get {
            StringBuilder builder = new StringBuilder();
            switch (PriusManager.Instance.currentPart) {
                case PriusType.Human:
                    builder.AppendLine(heartVisualizer.ExplanationText);
                    builder.AppendLine(liverVisualizer.ExplanationText);
                    builder.Append(kidneyVisualizer.ExplanationText);
                    break;
                case PriusType.Heart:
                    builder.AppendLine(heartVisualizer.ExplanationText);
                    break;
                case PriusType.Kidney:
                    builder.Append(kidneyVisualizer.ExplanationText);
                    break;
                case PriusType.Liver:
                    builder.AppendLine(liverVisualizer.ExplanationText);
                    break;
                case PriusType.Pancreas:
                    break;
            }

            return builder.ToString();
        }
    }

    public override bool Visualize(int index, HealthChoice choice) {
        bool heartChanged = heartVisualizer.Visualize(index, choice);
        heartIndicator.color = SetColor(heartVisualizer.Status);
        SetOrgan(heartVisualizer.Status, goodHeart, badHeart);

        bool kidneyChanged = kidneyVisualizer.Visualize(index, choice);
        kidneyIndicator.color = SetColor(kidneyVisualizer.Status);
        SetOrgan(kidneyVisualizer.Status, goodLeftKidney, badLeftKidney);
        SetOrgan(kidneyVisualizer.Status, goodRightKidney, badRightKidney);

        bool liverChanged = liverVisualizer.Visualize(index, choice);
        liverIndicator.color = SetColor(liverVisualizer.Status);
        SetOrgan(liverVisualizer.Status, goodLiver, badLiver);

        return heartChanged || kidneyChanged || liverChanged;
    }

    private Color SetColor(HealthStatus status) {
        if (status == HealthStatus.Bad) {
            return badColor;
        }

        if (status == HealthStatus.Intermediate) {
            return intermediateColor;
        }

        return goodColor;
    }

    /// <summary>
    /// TODO: This setting is for demo only. Need to set to HealthStatus.Bad (or have three models).
    /// </summary>
    /// <param name="status">Status.</param>
    /// <param name="good">Good.</param>
    /// <param name="bad">Bad.</param>
    private void SetOrgan(HealthStatus status, GameObject good, GameObject bad) {
        bad.SetActive(status != HealthStatus.Good);
        good.SetActive(status == HealthStatus.Good);
    }

    /// <summary>
    /// Moves the organ.
    /// </summary>
    /// <param name="stl">move small to large (true) or large to small (false)</param>
    /// <param name="type">Type.</param>
    /// <param name="small">Small.</param>
    /// <param name="large">Large.</param>
    public void MoveOrgan(bool stl, PriusType type, GameObject small, GameObject large) {
        if (stl) {
            StartCoroutine(MoveSmallToLarge(type, small, large));
        } else {
            StartCoroutine(MoveLargeToSmall(type, small, large));
        }
    }

    private IEnumerator MoveSmallToLarge(PriusType type, GameObject small, GameObject large) {
        Vector3 startPos = small.transform.position;
        Vector3 endPos = large.transform.position;

        Vector3 startScale = small.transform.localScale;
        Vector3 endScale = startScale * 5.0f;

        for (int i = 0; i < 100; i++) {
            Vector3 curr = Vector3.Lerp(startPos, endPos, (float)i / 100);
            small.transform.position = curr;
            small.transform.localScale = Vector3.Lerp(startScale, endScale, (float)i / 100);
            yield return null;
        }

        small.transform.position = startPos;
        small.transform.localScale = startScale;
        small.SetActive(false);
        large.SetActive(true);
        ShowOrgan(type);
    }

    private IEnumerator MoveLargeToSmall(PriusType type, GameObject small, GameObject large) {
        Vector3 startPos = large.transform.position;
        Vector3 endPos = small.transform.position;

        Vector3 startScale = large.transform.localScale;
        Vector3 endScale = startScale * 0.2f;

        for (int i = 0; i < 100; i++) {
            Vector3 curr = Vector3.Lerp(startPos, endPos, (float)i / 100);
            large.transform.position = curr;
            large.transform.localScale = Vector3.Lerp(startScale, endScale, (float)i / 100);
            yield return null;
        }

        large.transform.position = startPos;
        large.transform.localScale = startScale;
        large.SetActive(false);
        small.SetActive(true);
    }

    public void ShowOrgan(PriusType type) {
        switch (type) {
            case PriusType.Liver:
                liverVisualizer.ShowOrgan();
                break;
            case PriusType.Kidney:
                kidneyVisualizer.ShowOrgan();
                break;
            case PriusType.Heart:
                heartVisualizer.ShowOrgan();
                break;
        }
    }

    /// <summary>
    /// Reserved when the animations for prius is ready.
    /// </summary>
    public override void Pause() {
        throw new System.NotImplementedException();
    }
}