﻿public enum AppState {
    /// <summary>
    /// User is choosing a language
    /// </summary>
    ChooseLanguage,
    /// <summary>
    /// User is finding a suitable Plane Surface
    /// </summary>
    FindPlane,
    /// <summary>
    /// User is placing the stage
    /// </summary>
    PlaceStage,
    /// <summary>
    /// User is picking the archetype
    /// </summary>
    PickArchetype,
    /// <summary>
    /// Expanding the information panels for the selected archetype
    /// </summary>
    ShowDetails,
    /// <summary>
    /// Awating user input
    /// </summary>
    Idle,
    /// <summary>
    /// Prius visualization
    /// </summary>
    VisPrius,
    /// <summary>
    /// Line chart visualization
    /// </summary>
    VisLineChart,
    /// <summary>
    /// Activity visualization
    /// </summary>
    VisActivity
}