using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthIndicator : MonoBehaviour 
{
  public RectTransform ParentTransform;
  public RectTransform HealthBorder;
  public GameObject HealthFillPrefab;

  public int Health = 2;

  float _healthBorderSizeDelta = 0.26f;
  float _healthBarOffset = 0.26f;

  List<GameObject> _healthBars = new List<GameObject>();
  public void Init(int health)
  {
    Health = health;

    HealthBorder.sizeDelta = new Vector2(0.32f + _healthBorderSizeDelta * (Health - 1), 0.32f);

    for (int i = 0; i < Health; i++)
    {
      GameObject o = Instantiate(HealthFillPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
      RectTransform rt = o.GetComponent<RectTransform>();
      rt.localPosition = new Vector3(0.03f + _healthBarOffset * i, 0.0f, 0.0f);
      rt.SetParent(ParentTransform, false);
      _healthBars.Add(o);
    }
  }

  public void ReceiveDamage(int damage)
  {
    if (damage > Health)
    {
      return;
    }

    int counter = 0;
    for (int i = _healthBars.Count - 1; i > 0; i--)
    {
      if (_healthBars[i].activeSelf)
      {
        _healthBars[i].SetActive(false);
      }

      Health--;

      counter++;

      if (counter == damage)
      {
        break;
      }
    }
  }
}
