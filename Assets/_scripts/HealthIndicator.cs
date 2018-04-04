using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthIndicator : MonoBehaviour 
{
  public RectTransform ParentTransform;
  public RectTransform HealthBorder;
  public GameObject HealthFillPrefab;

  Unit _unitRef;

  float _healthBorderSizeDelta = 0.26f;
  float _healthBarOffset = 0.26f;

  List<GameObject> _healthBars = new List<GameObject>();
  public void Init(Unit unitToMonitor)
  {
    _unitRef = unitToMonitor;

    HealthBorder.sizeDelta = new Vector2(0.32f + _healthBorderSizeDelta * (_unitRef.Health - 1), 0.32f);

    for (int i = 0; i < _unitRef.Health; i++)
    {
      GameObject o = Instantiate(HealthFillPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
      RectTransform rt = o.GetComponent<RectTransform>();
      rt.localPosition = new Vector3(0.03f + _healthBarOffset * i, 0.0f, 0.0f);
      rt.SetParent(ParentTransform, false);
      _healthBars.Add(o);
    }
  }

  public void UpdateHealthIndicator()
  {
    foreach (var bar in _healthBars)
    {
      bar.SetActive(false);
    }

    for (int i = 0; i < _unitRef.Health; i++)
    {
      _healthBars[i].SetActive(true);
    }
  }
}
