﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeLiverDisplay : OrganDisplay {
    public GameObject liver;

    public SkinnedMeshRenderer LiverRenderer { get { return liver.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>(); } }

    public override void DisplayOrgan(int score) {
        LiverRenderer.SetBlendShapeWeight(0, 100 - score);
    }
}
