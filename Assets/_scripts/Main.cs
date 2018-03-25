using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour 
{
  public RectTransform GridHolder;
  public GameObject CellPrefab;

  int _size = 8;

	void Start () 
  {
    List<Color> colors = new List<Color>();

    colors.Add(Color.white);
    colors.Add(new Color(0.2f, 0.2f, 0.2f));

    int colorIndex = 0;

    bool flipFlag = true;

    for (int x = 0; x < _size; x++)
    {
      flipFlag = !flipFlag;

      for (int y = 0; y < _size; y++)
      {
        colorIndex = flipFlag ? 0 : 1;

        var go = Instantiate(CellPrefab);

        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(GridHolder, false);
        rt.localPosition = new Vector3(y, x, 0.0f);

        go.GetComponent<Cell>().ImageComponent.color = colors[colorIndex];

        flipFlag = !flipFlag;
      }
    }
	}

}
