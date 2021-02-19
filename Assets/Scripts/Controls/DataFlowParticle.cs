﻿using System.Collections;
using UnityEngine;

/// <summary>
/// Particle effect that would travel along a route.
/// </summary>
public class DataFlowParticle : MonoBehaviour {
    [SerializeField] private Vector3[] route;
    [SerializeField] private float speed;
    [SerializeField] private ParticleSystem trail;
    [SerializeField] private ParticleSystem glow;

    private Color BaseColor { get; set; }
    private ParticleSystem.MainModule trailModule;
    private ParticleSystem.MainModule glowModule;
    private IEnumerator travel;
    private float RealSpeed => speed * transform.lossyScale.x;

    /// <summary>
    /// During initialization, move the particle to the beginning of the route to prevent "flashing".
    /// </summary>
    public void Initialize() {
        trailModule = trail.main;
        glowModule = glow.main;
        transform.localPosition = route[0];
        BaseColor = trailModule.startColor.color; // Give a basic color for debugging
    }

    public void Visualize() {
        if (travel == null) {
            SetActive(true);
            travel = Travel();
            StartCoroutine(travel);
        }
    }

    public void Stop() {
        if (travel != null) {
            StopCoroutine(travel);
            travel = null;
            SetActive(false);
        }
    }

    /// <summary>
    /// Loop to travel along the route.
    /// </summary>
    private IEnumerator Travel() {
        transform.localPosition = route[0];

        while (true) {
            SetParticleColor();
            for (int i = 0; i < route.Length - 1; i++) {
                float traveledDist = 0;
                float totalDist = Vector3.Distance(route[i], route[i + 1]);
                Vector3 dir = Vector3.Normalize(route[i + 1] - route[i]);
                while (traveledDist < totalDist) {
                    transform.localPosition += dir * RealSpeed;
                    traveledDist += RealSpeed;
                    yield return null;
                }
            }

            // Stop for a few seconds
            yield return new WaitForSeconds(2);

            // Move the particle to the beginning of the route
            // There is a bug in Unity that when a gameObject is disabled, the coroutine attached to it
            // will automatically stop.
            // Therefore, disable all the children instead.
            SetActive(false);
            transform.localPosition = route[0];
            yield return new WaitForSeconds(2);
            SetActive(true);
        }
    }

    private void SetActive(bool on) {
        foreach (Transform t in transform) {
            t.gameObject.SetActive(on);
        }
    }

    private void SetParticleColor() {
        Color deltaColor = new Color(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        trailModule.startColor = BaseColor + deltaColor;
        glowModule.startColor = BaseColor;
    }

    /// <summary>
    /// Draws the path out in the scene view for better debugging.
    /// </summary>
    private void OnDrawGizmosSelected() {
        for (int i = 1; i < route.Length; i++) {
            Gizmos.DrawLine(transform.TransformPoint(route[i - 1]), transform.TransformPoint(route[i]));
        }
    }
}