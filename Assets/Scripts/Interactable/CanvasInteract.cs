﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasInteract : MonoBehaviour, IInteractable {
    public Image panel;

    [Header("This UI canvas is clicked. Indicate what to do next.")]
    public UnityEvent clicked;

    /// <summary>
    /// Percentage of darkness added to the original color when the canvas is hovered.
    /// </summary>
    private static readonly float dark = 0.4f;

    public void OnCursorEnter() {
        if (panel != null) {
            panel.color -= new Color(dark, dark, dark, 0f);
        }
    }

    public void OnCursorExited() {
        panel.color += new Color(dark, dark, dark, 0f);
    }

    public void OnScreenTouch(Vector2 coord) {
        clicked.Invoke();
    }

    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) { }

    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }

    public void OnScreenLeave(Vector2 coord) { }
}
