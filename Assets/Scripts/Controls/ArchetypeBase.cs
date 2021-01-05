using UnityEngine;

/// <summary>
/// A base wrapper class for the display models.
/// </summary>
public class ArchetypeBase {
    public Archetype ArchetypeData { get; }
    public GameObject Model { get; }
    public Material Mat { get; }
    public Animator ArchetypeAnimator { get; }
    public GameObject InfoCanvas { get; }
    public GameObject Icon { get; }

    public ArchetypeBase(Archetype archetypeData, Transform parent) {
        ArchetypeData = archetypeData;

        Model = Object.Instantiate(ArchetypeManager.Instance.basePrefab, parent, false);
        Transform modelTransform = Model.transform.Find("model");
        
        GameObject figure = Object.Instantiate(Resources.Load<GameObject>($"Prefabs/{archetypeData.modelString}"),
            modelTransform, false);
        Mat = figure.transform.GetChild(0).GetComponent<Renderer>().material;
        ArchetypeAnimator = figure.transform.GetComponent<Animator>();

        // Set archetype info canvas
        InfoCanvas = Model.transform.Search("Basic Info Canvas").gameObject;
        InfoCanvas.transform.Search("Name").GetComponent<LocalizedText>().
            SetText("Archetypes.Name", new LocalizedParam(archetypeData.Name, true));
        InfoCanvas.transform.Search("Age").GetComponent<LocalizedText>().
            SetText("Archetypes.Age", new LocalizedParam(archetypeData.age));
        InfoCanvas.transform.Search("Occupation").GetComponent<LocalizedText>().
            SetText("Archetypes.Occupation", new LocalizedParam(archetypeData.Occupation, true));
        InfoCanvas.transform.Search("Disease").GetComponent<LocalizedText>().
            SetText("Archetypes.Status", new LocalizedParam(LocalizationDicts.statuses[archetypeData.status], true));

        Icon = Model.transform.Search("Button Canvas").gameObject;
    }

    public void Dispose() {
        Object.Destroy(Model);
    }
}