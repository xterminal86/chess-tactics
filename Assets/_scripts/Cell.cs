﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour 
{
  public Image ImageComponent;
  public Image IsValidIndicator;

  public Unit UnitPresent;
  public Vector2Int ArrayCoordinates = Vector2Int.zero;
}
