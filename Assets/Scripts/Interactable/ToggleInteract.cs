﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleInteract : MonoBehaviour, IInteractable {
    public bool on;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [Header("The value is toggled. Indicate what to do next.")]
    public BoolEvent toggled;
    public LocalizedText status;
    private Color? originalColor;

    // Toggle animation
    public ComponentAnimation toggleAnimation;

    // Tooltip
    [Header("If no tooltip needed, leave this blank.")]
    public string tooltipTextId;
    public LocalizedParam[] tooltipTextParams;

    public GameObject tooltipPrefab;
    private GameObject tooltip;
    private IEnumerator tooltipAnim;

    public enum Direction { Left, Right, Top, Bottom }
    public Direction tooltipDirection;

    // Following three variables in seconds
    private static readonly float showTime = 2.0f;
    private static readonly float hideTime = 0.5f;
    private static readonly float animationTime = 0.5f;

    private float origX;
    private float origY;

    public void OnCursorEnter() {
        if (gameObject.GetComponent<MeshRenderer>()) {
            if (originalColor == null) {
                originalColor = gameObject.GetComponent<MeshRenderer>().material.color;
            }

            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        if (tooltip == null && !string.IsNullOrEmpty(tooltipTextId)) {
            tooltip = Instantiate(tooltipPrefab, transform.parent);
            switch (tooltipDirection) {
                case Direction.Left:
                    tooltip.transform.localPosition = new Vector3(
                        -GetComponent<Collider>().bounds.size.x / 2.0f - 4.0f, 0, 0);
                    break;
                case Direction.Right:
                    tooltip.transform.localPosition = new Vector3(
                        GetComponent<Collider>().bounds.size.x / 2.0f + 4.0f, 0, 0);
                    break;
                case Direction.Top:
                    tooltip.transform.localPosition = new Vector3(0, 0,
                        GetComponent<Collider>().bounds.size.y / 2.0f + 3.0f);
                    break;
                case Direction.Bottom:
                    tooltip.transform.localPosition = new Vector3(0, 0,
                        -GetComponent<Collider>().bounds.size.y / 2.0f - 3.0f);
                    break;
            }

            tooltip.transform.GetChild(0).GetChild(0).GetComponent<LocalizedText>()
                .SetText(tooltipTextId, tooltipTextParams);
            tooltip.SetActive(false);
        }

        if (tooltip != null) {
            if (tooltipAnim != null) {
                StopCoroutine(tooltipAnim);
            }
            tooltipAnim = TooltipShow();
            StartCoroutine(tooltipAnim);
        }
    }

    public void OnCursorExited() {
        if (GetComponent<MeshRenderer>() && originalColor != null) {
            GetComponent<MeshRenderer>().material.color = (Color)originalColor;
            originalColor = null;
        }

        if (tooltip != null) {
            if (tooltipAnim != null) {
                StopCoroutine(tooltipAnim);
            }

            // OnCursorExited might be triggered when the button gets hidden.
            // In this case coroutines cannot be started, but we still wish to hide the tooltip.
            if (gameObject.activeInHierarchy) {
                tooltipAnim = TooltipHide();
                StartCoroutine(tooltipAnim);
            } else {
                tooltip.SetActive(false);
            }
        }
    }

    public void OnScreenTouch(Vector2 coord) {
        if (toggleAnimation == null || !toggleAnimation.IsAnimating) {
            Toggle();
            status.SetText(on ? "Buttons.ToggleOn" : "Buttons.ToggleOff");
            toggled.Invoke(on);
            toggleAnimation.Invoke();
        }
    }

    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) { }
    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }
    public void OnScreenLeave(Vector2 coord) { }

    /// <summary>
    /// Displays the tooltip.
    /// </summary>
    private IEnumerator TooltipShow() {
        yield return new WaitForSeconds(showTime);
        tooltip.SetActive(true);
        // For canvases z-scale doesn't matter
        origX = tooltip.transform.localScale.x;
        origY = tooltip.transform.localScale.y;

        tooltip.transform.localScale = new Vector3(0, 0, 1);
        int stepCount = (int)(animationTime / Time.deltaTime);
        for (int i = 1; i <= stepCount; i++) {
            tooltip.transform.localScale = new Vector3(i * origX / stepCount,
                i * origY / stepCount, 1.0f);
            yield return null;
        }
    }

    /// <summary>
    /// Hides the tooltip.
    /// </summary>
    private IEnumerator TooltipHide() {
        yield return new WaitForSeconds(hideTime);
        // For canvases z-scale doesn't matter
        int stepCount = (int)(animationTime / Time.deltaTime);
        for (int i = stepCount - 1; i >= 0; i--) {
            tooltip.transform.localScale = new Vector3(i * origX / stepCount, i * origY / stepCount, 1.0f);
            yield return null;
        }
        tooltip.SetActive(false);
        tooltip.transform.localScale = new Vector3(origX, origY, 1);
    }

    /// <summary>
    /// Toggle this instance. DOES NOT invoke clicked.
    /// </summary>
    public void Toggle() {
        on = !on;
    }
}
