﻿using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {
    public static DebugText Instance { get; private set; }

    [SerializeField] private Text debugText;
    [SerializeField] private int maxLine = 10;

    private int lineCount;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void Log(string content) {
        if (lineCount + 1 > maxLine) {
            debugText.text = "";
            lineCount = 0;
        }

        debugText.text += $"\n{content}";
        lineCount++;
    }
}