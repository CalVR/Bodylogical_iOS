﻿using System.Collections.Generic;
using UnityEngine;

public class DisplayInternals : MonoBehaviour {
    [SerializeField] private GameObject internals;
    [SerializeField] private GameObject organs;
    [SerializeField] private float attenuation = 0.9f;
    [SerializeField] private float cutoff = 0.8f;

    private Material archetypeMat;
    private Material planeMat;
    private float planeStartAlpha;

    private List<GameObject> boxes;
    private List<GameObject> texts;
    private List<Material> boxMaterials;
    private List<float> boxAlphas;

    private float radius;
    public bool AvatarHidden { get; private set; }
    // Shader property hashes 
    private static readonly int vWireColor = Shader.PropertyToID("_V_WIRE_Color");
    private static readonly int alphaScale = Shader.PropertyToID("_AlphaScale");

    private void Start() {
        radius = GetComponent<SphereCollider>().radius;
        // Start() will be called when the game object is enabled.
        // At this time, the archetype will already be selected.
        archetypeMat = ArchetypeManager.Instance.Selected.Mat;
    }

    private void OnTriggerStay(Collider other) {
        if (other.name.Contains("Camera")) {
            internals.SetActive(true);
            float distance = Vector3.Distance(transform.position, other.transform.position);
            float percent = distance / radius * attenuation;

            bool newAvatarHidden = percent <= cutoff;

            if (AvatarHidden && newAvatarHidden) {
                return; // still within cutoff range, don't do anything
            }

            if (newAvatarHidden) {
                // avatarHidden is false, just got in range
                // Display text and set to original transparency
                foreach (GameObject text in texts) {
                    text.SetActive(true);
                }
                organs.SetActive(false);
                ArchetypeManager.Instance.Selected.Model.SetActive(false);

                for (int i = 0; i < boxes.Count; i++) {
                    Color boxColor = boxMaterials[i].color;
                    boxColor.a = boxAlphas[i];
                    boxMaterials[i].color = boxColor;

                    // Box wireframe color alpha default to 1
                    Color wireColor = boxMaterials[i].GetColor(vWireColor);
                    wireColor.a = 1;
                    boxMaterials[i].SetColor(vWireColor, wireColor);
                }

                Color planeColor = planeMat.color;
                planeColor.a = planeStartAlpha;
                planeMat.color = planeColor;
            } else if (AvatarHidden) {
                // newAvatarHidden is false, just got out of range
                // Hide text and reset transparency
                foreach (GameObject text in texts) {
                    text.SetActive(false);
                }
                ArchetypeManager.Instance.Selected.Model.SetActive(true);
                organs.SetActive(true);
                archetypeMat.SetFloat(alphaScale, percent);
            } else {
                // Adjust transparency
                for (int i = 0; i < boxes.Count; i++) {
                    Color boxColor = boxMaterials[i].color;
                    boxColor.a = boxAlphas[i] * (1 - percent);
                    boxMaterials[i].color = boxColor;

                    // Box wireframe color alpha default to 1
                    Color wireColor = boxMaterials[i].GetColor(vWireColor);
                    wireColor.a = 1 - percent;
                    boxMaterials[i].SetColor(vWireColor, wireColor);
                }

                Color planeColor = planeMat.color;
                planeColor.a = planeStartAlpha * (1 - percent);
                planeMat.color = planeColor;
            }

            AvatarHidden = newAvatarHidden;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.name.Contains("Camera")) {
            internals.SetActive(false);
            archetypeMat.SetFloat(alphaScale, 1);
        }
    }

    /// <summary>
    /// Set up the materials and alpha values.
    /// </summary>
    public void Initialize() {
        planeMat = internals.transform.Find("Plane").GetComponent<Renderer>().material;
        planeStartAlpha = planeMat.color.a;

        boxes = internals.transform.SearchAllWithName("Box");
        texts = internals.transform.SearchAllWithName("Side", true);
        texts.AddRange(internals.transform.SearchAllWithName("Header"));

        boxMaterials = new List<Material>(boxes.Count);
        boxAlphas = new List<float>(boxes.Count);
        foreach (GameObject box in boxes) {
            Material mat = box.GetComponent<Renderer>().material;
            boxMaterials.Add(mat);
            boxAlphas.Add(mat.color.a);
        }

        Reset();
    }

    /// <summary>
    /// After stage transition, all canvases will be set to active. They need
    /// to be reset to false.
    /// </summary>
    public void Reset() {
        foreach (GameObject text in texts) {
            text.SetActive(false);
        }
    }
}
