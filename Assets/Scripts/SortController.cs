using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SortVisualizer))]
public class SortController : MonoBehaviour
{
    private SortVisualizer _visualizer;

    private void Start()
    {
        _visualizer = GetComponent<SortVisualizer>();

        _visualizer.GenerateBars();

        List<float> data = new(_visualizer.values);

        StartCoroutine(RunSort(SortingAlgorithms.MergeSort, data, nameof(SortingAlgorithms.MergeSort)));
    }

    private IEnumerator RunSort<T>(Func<SortVisualizer, IList<T>, IEnumerator> sortMethod, IList<T> data, string name)
        where T : IComparable<T>
    {
        Debug.Log($"Executing {name}");

        yield return sortMethod(_visualizer, data);

        _visualizer.PaintAllBars(Color.green);

        Debug.Log($"{name} completed");
        Debug.Log($"Time Complexity: ${data.Count * Mathf.Log(2, data.Count)} == {SortingAlgorithms.result}");
    }
}