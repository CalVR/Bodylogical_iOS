using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class StatsController : MonoBehaviour {
    [SerializeField] private ColorLibrary colorLibrary;
    [SerializeField] private Transform front;
    
    private ArchetypePerformer performer;
    private Mesh mesh;
    private Vector3 originalPos;
    
    public void Initialize(ArchetypePerformer archetypePerformer) {
        performer = archetypePerformer;
        originalPos = performer.Model.transform.position;
        
        mesh = new Mesh {name = "Stats"};
        GetComponent<MeshFilter>().mesh = mesh;
        BuildStats();
    }

    public void Toggle(bool on) {
        gameObject.SetActive(true);
        performer.Panel.ToggleDetailPanel(on);
        StartCoroutine(ToggleCoroutine(on));
    }

    private IEnumerator ToggleCoroutine(bool on) {
        if (on) {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.enabled = false;
            // Move the avatar to front of road
            yield return performer.MoveTo(front.position);
            mr.enabled = true;
        } else {
            // Move the avatar to middle of road
            yield return performer.MoveTo(originalPos);
            gameObject.SetActive(false);
        }
    }

    private void BuildStats() {
        int years = performer.ArchetypeHealth.NumYears;
        Vector3[] vPos = new Vector3[years * 2];
        Color[] vColor = new Color[years * 2];
        int[] vTri = new int[(years - 1) * 6];
        for (int i = 0; i < years; i++) {
            vPos[i * 2] = new Vector3(-1, 0, i);
            vPos[i * 2 + 1] = new Vector3(1, 0, i);

            HealthStatus status =
                HealthUtil.CalculateStatus(
                    performer.ArchetypeHealth.CalculateHealth(i, performer.ArchetypeData.gender));
            Color color = colorLibrary.StatusColorDict[status];
            vColor[i * 2] = color;
            vColor[i * 2 + 1] = color;

            if (i < years - 1) {
                vTri[i * 6 + 0] = i * 2;
                vTri[i * 6 + 1] = i * 2 + 2;
                vTri[i * 6 + 2] = i * 2 + 1;
                vTri[i * 6 + 3] = i * 2 + 1;
                vTri[i * 6 + 4] = i * 2 + 2;
                vTri[i * 6 + 5] = i * 2 + 3;
            }
        }

        mesh.vertices = vPos;
        mesh.colors = vColor;
        mesh.triangles = vTri;
        mesh.RecalculateNormals();
    }
}