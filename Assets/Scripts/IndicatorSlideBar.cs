﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorSlideBar : SlideBar {
  public override void SetProgress(int progress) {
    this.progress = progress;
    transform.localPosition = new Vector3(progress, transform.localPosition.y, transform.localPosition.z);
  }
}
