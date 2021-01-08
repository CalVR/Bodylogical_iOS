﻿using UnityEngine;

/// <summary>
/// A wrapper class for the models in the visualizations.
/// </summary>
public class ArchetypePerformer : ArchetypeModel {
    public HealthChoice Choice { get; }
    public Lifestyle ArchetypeLifestyle { get; }
    public LongTermHealth ArchetypeHealth { get; }
    
    public SwitchIcon Icon { get; }
    public ActivityController Activity { get; }
    public PriusController Prius { get; }
    public StatsController Stats { get; }

    public Visualization CurrentVisualization { get; private set; } = Visualization.Activity;
    
    public ArchetypePerformer(Archetype archetypeData, Transform parent, HealthChoice choice, Lifestyle lifestyle,
        LongTermHealth health, BackwardsProps props)
        : base(ArchetypeManager.Instance.performerPrefab, archetypeData, parent) {
        Choice = choice;
        ArchetypeLifestyle = lifestyle;
        ArchetypeHealth = health;

       Icon = Model.GetComponentInChildren<SwitchIcon>();
       Icon.Initialize(this);
       Activity = Model.GetComponentInChildren<ActivityController>(true);
       Activity.Initialize(this, props);
       Prius = Model.GetComponentInChildren<PriusController>(true);
       Prius.Initialize(this);
       Stats = Model.GetComponentInChildren<StatsController>(true);
    }

    /// <summary>
    /// Switches to the next visualization.
    /// </summary>
    public void NextVisualization() {
        
    }

    /// <summary>
    /// Updates the current visualization. This is usually caused by a change in year.
    /// </summary>
    /// <returns></returns>
    public bool UpdateVisualization() {
        return false;
    }
}