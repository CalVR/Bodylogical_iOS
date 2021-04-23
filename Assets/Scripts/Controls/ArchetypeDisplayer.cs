using UnityEngine;

/// <summary>
/// A base wrapper class for the display models.
/// </summary>
public class ArchetypeDisplayer : ArchetypeModel {
    public VisualizeIcon icon;
    private static readonly int Greetings = Animator.StringToHash("Greetings");

    public void Initialize() {
        ArchetypeHealth = new LongTermHealth();
    }
    
    public void SetGreetingPose(bool on) {
        Anim.SetBool(Greetings, on);
    }

    public void Reset() {
        SetGreetingPose(false);
        panel.Reset();
        icon.ResetIcon();
    }

    public void UpdateStats(float i) {
        Health health = ArchetypeHealth[i];
        panel.UpdateStats(health);
        
        // Change weight
        SetWeight(health[HealthType.weight]);
    }
}