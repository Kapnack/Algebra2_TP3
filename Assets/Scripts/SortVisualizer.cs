using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Bar
{
    public GameObject obj;
    public Material mat;

    public Bar(GameObject obj, Material mat)
    {
        this.obj = obj;
        this.mat = mat;
    }
}

public class SortVisualizer : MonoBehaviour
{
    [Header("Settings")] 
    public GameObject barPrefab;
    public int amount = 30;
    public float spacing = 0.5f;
    public float speed = 0.02f;

    private readonly List<Bar> _bars = new();
    [HideInInspector] public float[] values;

    public void GenerateBars()
    {
        CleanBars();

        values = new float[amount];
        
        for (int i = 0; i < amount; i++)
        {
            float height = Random.Range(1f, 10f);
            values[i] = height;

            GameObject bar = Instantiate(barPrefab, new Vector3(i * spacing, 0, 0), Quaternion.identity,
                transform);

            Bar goBar = new(bar, bar.GetComponent<Renderer>().material);

            bar.transform.localScale = new Vector3(0.4f, height, 0.4f);
            bar.GetComponent<Renderer>().material.color = Color.white;
            _bars.Add(goBar);
        }
    }

    private void CleanBars()
    {
        foreach (Transform t in transform)
            Destroy(t.gameObject);
        
        _bars.Clear();
    }

    public IEnumerator SwitchBars(int a, int b)
    {
        (values[a], values[b]) = (values[b], values[a]);
        
        (_bars[a].obj.transform.position, _bars[b].obj.transform.position) =
            (_bars[b].obj.transform.position, _bars[a].obj.transform.position);
        
        (_bars[a], _bars[b]) = (_bars[b], _bars[a]);
        
        yield return new WaitForSeconds(speed);
    }

    public IEnumerator SetBarHeight(int index, float newHeight, Color color)
    {
        Bar bar = _bars[index];
        bar.obj.transform.localScale = new Vector3(0.4f, newHeight, 0.4f);
        bar.mat.color = color;
        values[index] = newHeight;

        yield return new WaitForSeconds(speed);

        bar.mat.color = Color.white;
    }
        
    public IEnumerator PaintBars(int i, int j, Color color)
    {
        _bars[i].mat.color = color;
        _bars[j].mat.color = color;
        
        yield return new WaitForSeconds(speed);
        
        _bars[i].mat.color = Color.white;
        _bars[j].mat.color = Color.white;
    }

    public void PaintAllBars(Color color)
    {
        foreach (Bar b in _bars)
            b.mat.color = color;
    }
}