﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoggingVisualizer : Visualizer {
    public override HealthStatus Status { get; set; }

    public override void Initialize() {

    }

    public override bool Visualize(int index, HealthChoice choice) {
        throw new System.NotImplementedException();
    }

    public override void Pause() {

    }
}
