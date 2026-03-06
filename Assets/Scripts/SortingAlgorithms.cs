using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SortingAlgorithms
{

    public static float result = 0;
    public static float longOpetarions = 0;
    public static float elementsMoved = 0;

    //Time Complexity: W: O(log²n)
    //Aux Space: O(n.log2n)
    #region BitonicSort

    public static IEnumerator BitonicSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        int n = data.Count;
        uint addedData = 0;
        int powerOfTwo = 1;
        while (powerOfTwo < n) powerOfTwo <<= 1;

        List<T> paddedData = new(data);


        addedData = Convert.ToUInt32(powerOfTwo - paddedData.Count);

        while (paddedData.Count < powerOfTwo)
            paddedData.Add((T)Convert.ChangeType(double.MaxValue, typeof(T)));


        IEnumerator BitonicMerge(int low, int count, bool ascending)
        {
            if (count > 1)
            {
                int k = count / 2;
                for (int i = low; i < low + k; i++)
                {
                    if ((i < data.Count) && (i + k < data.Count))
                    {
                        yield return vis.PaintBars(i, i + k, Color.yellow);

                        if ((paddedData[i].CompareTo(paddedData[i + k]) > 0) == ascending)
                        {
                            (paddedData[i], paddedData[i + k]) = (paddedData[i + k], paddedData[i]);
                            yield return vis.SwitchBars(i, i + k);
                        }
                    }
                    else
                    {
                        if ((paddedData[i].CompareTo(paddedData[i + k]) > 0) == ascending)
                            (paddedData[i], paddedData[i + k]) = (paddedData[i + k], paddedData[i]);
                    }
                }

                yield return BitonicMerge(low, k, ascending);
                yield return BitonicMerge(low + k, k, ascending);
            }
        }

        IEnumerator BitonicSortRec(int low, int count, bool ascending)
        {
            if (count > 1)
            {
                int k = count / 2;
                yield return BitonicSortRec(low, k, true);
                yield return BitonicSortRec(low + k, k, false);
                yield return BitonicMerge(low, count, ascending);
            }
        }


        yield return BitonicSortRec(0, paddedData.Count, true);

        paddedData.RemoveRange(paddedData.Count - Convert.ToInt32(addedData), Convert.ToInt32(addedData));

        for (int i = 0; i < n; i++)
        {
            data[i] = paddedData[i];

            yield return vis.SetBarHeight(i, (float)(object)data[i], Color.green);
        }
    }

    #endregion

    //Time Complexity: O(n2)
    //Aux Space: O(1)
    #region SelectionSort

    public static IEnumerator SelectionSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        int n = data.Count;
        for (int i = 0; i < n - 1; i++)
        {
            int minIndex = i;

            for (int j = i + 1; j < n; j++)
            {
                yield return vis.PaintBars(minIndex, j, Color.yellow);

                if (data[j].CompareTo(data[minIndex]) < 0)
                    minIndex = j;
            }

            if (minIndex != i)
            {
                (data[i], data[minIndex]) = (data[minIndex], data[i]);

                yield return vis.SwitchBars(i, minIndex);
            }
        }
    }

    #endregion

    //Time Complexity: O(n log n)
    //Aux Space: O(n)
    #region CocktailShakerSort

    public static IEnumerator CocktailSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        bool swapped = true;
        int start = 0;
        int n = data.Count;

        while (swapped)
        {
            swapped = false;

            // Left to right
            for (int i = start; i < n - 1; ++i)
            {
                if (data[i].CompareTo(data[i + 1]) > 0)
                {
                    (data[i], data[i + 1]) = (data[i + 1], data[i]);
                    yield return vis.SwitchBars(i, i + 1);
                    swapped = true;
                }
            }

            if (!swapped)
                break;

            swapped = false;
            n--; // shrink the end

            // Right to left
            for (int i = n - 1; i >= start; --i)
            {
                if (data[i].CompareTo(data[i + 1]) > 0)
                {
                    (data[i], data[i + 1]) = (data[i + 1], data[i]);
                    yield return vis.SwitchBars(i, i + 1);
                    swapped = true;
                }
            }

            start++; // increase the start
        }
    }

    #endregion

    //Time Complexity: O(n²)
    //Aux Space: O(1)
    #region QuickSort

    public static IEnumerator QuickSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        yield return QuickSort(vis, data, 0, data.Count - 1);
    }

    private static IEnumerator QuickSort<T>(SortVisualizer vis, IList<T> data, int low, int high)
        where T : IComparable<T>
    {
        if (low < high)
        {
            int pivotIndex = low;
            T pivot = data[high];

            for (int j = low; j < high; j++)
            {
                yield return vis.PaintBars(j, high, Color.magenta);
                if (data[j].CompareTo(pivot) < 0)
                {
                    (data[j], data[pivotIndex]) = (data[pivotIndex], data[j]);
                    yield return vis.SwitchBars(j, pivotIndex);
                    pivotIndex++;
                }
            }

            (data[pivotIndex], data[high]) = (data[high], data[pivotIndex]);
            yield return vis.SwitchBars(pivotIndex, high);

            yield return QuickSort(vis, data, low, pivotIndex - 1);
            yield return QuickSort(vis, data, pivotIndex + 1, high);
        }
    }

    #endregion

    //Time Complexity: O(d * (n + b)) 
    // d = Digit Count ((1, 10, 100) = 3).
    // n = Array Size.
    // b = Max numeric in numeric system (Decimal = 10 (0, 9))
    //Aux Space: O(n + b)
    #region RadixSort(LSD)

    public static IEnumerator RadixSortLSD<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        int n = data.Count;
        if (n <= 1) yield break;

        int[] intData = new int[n];
        for (int i = 0; i < n; i++)
            intData[i] = Convert.ToInt32(data[i]);

        int max = GetMax(intData);

        for (int exp = 1; max / exp > 0; exp *= 10)
        {
            yield return CountSort(vis, intData, 0, n - 1, exp, Color.green);

            for (int i = 0; i < n; i++)
                data[i] = (T)Convert.ChangeType(intData[i], typeof(T));
        }
    }

    private static int GetMax(int[] data)
    {
        int max = data[0];
        foreach (int t in data)
            if (t > max)
                max = t;
        return max;
    }

    private static IEnumerator CountSort(SortVisualizer vis, int[] data, int left, int right, int exp, Color color)
    {
        int n = right - left + 1;
        int[] output = new int[n];
        int[] count = new int[10];

        for (int i = left; i <= right; i++)
            count[(data[i] / exp) % 10]++;

        for (int i = 1; i < 10; i++)
            count[i] += count[i - 1];

        for (int i = right; i >= left; i--)
        {
            int digit = (data[i] / exp) % 10;
            int pos = count[digit] - 1;
            output[pos] = data[i];
            count[digit]--;
        }

        for (int i = 0; i < n; i++)
        {
            data[left + i] = output[i];
            yield return vis.SetBarHeight(left + i, output[i], color);
        }
    }

    #endregion

    // Time Complexity: B: O(n log n), A: O(n * 1.25) / O(n * 1.5) & W: O(n²) 
    // Espacio Auxiliar: O(1)
    #region ShellSort

    public static IEnumerator ShellSort<T>(SortVisualizer vis, IList<T> arr) where T : IComparable<T>
    {
        int n = arr.Count;

        for (int gap = n / 2; gap > 0; gap /= 2)
        {
            for (int i = gap; i < n; i++)
            {
                T temp = arr[i];
                int j = i;

                while (j >= gap && arr[j - gap].CompareTo(temp) > 0)
                {
                    arr[j] = arr[j - gap];

                    float height = Convert.ToSingle(arr[j]);
                    yield return vis.SetBarHeight(j, height, Color.red);

                    j -= gap;
                }

                arr[j] = temp;

                float tempHeight = Convert.ToSingle(temp);
                yield return vis.SetBarHeight(j, tempHeight, Color.green);
            }
        }
    }

    #endregion

    //Time Complexity: W: O(?), A: O(n*n!) & B: O(n)
    //Aux Space: O(1)
    #region BogoSort

    public static IEnumerator BogoSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        System.Random rand = new();

        while (!IsSorted<T>(vis))
        {
            vis.PaintAllBars(Color.yellow);
            yield return new WaitForSeconds(vis.speed * Time.deltaTime);

            for (int i = data.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);

                yield return vis.PaintBars(i, j, Color.cyan);
                yield return vis.SwitchBars(i, j);
            }

            yield return new WaitForSeconds(vis.speed * Time.deltaTime);
        }

        vis.PaintAllBars(Color.green);
    }

    private static bool IsSorted<T>(SortVisualizer vis) where T : IComparable<T>
    {
        for (int i = 1; i < vis.values.Length; i++)
        {
            if (vis.values[i - 1] > vis.values[i])
                return false;
        }

        return true;
    }

    #endregion

    //Time Complexity: O(d * (n + b))
    // d = Digit Count ((1, 10, 100) = 3).
    // n = Array Size.
    // b = Max numeric in numeric system (Decimal = 10 (0, 9))
    //Aux Space: O(n + b)
    #region RadixSort(MSD)

    public static IEnumerator RadixSortMSD<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        int n = data.Count;
        if (n <= 1) yield break;

        int[] intData = new int[n];
        for (int i = 0; i < n; i++)
            intData[i] = Convert.ToInt32(data[i]);

        int max = GetMax(intData);
        int maxDigits = max == 0 ? 1 : (int)Mathf.Floor(Mathf.Log10(max)) + 1;

        yield return RadixSortMSD(vis, data, intData, 0, n - 1, maxDigits - 1);
    }

    private static IEnumerator RadixSortMSD<T>(SortVisualizer vis, IList<T> data, int[] intData, int left, int right, int digit) where T : IComparable<T>
    {
        if (left >= right || digit < 0)
            yield break;

        int exp = (int)Mathf.Pow(10, digit);

        yield return CountSort(vis, intData, left, right, exp, Color.cyan);

        for (int i = left; i <= right; i++)
            data[i] = (T)Convert.ChangeType(intData[i], typeof(T));

        int[] count = new int[10];
        for (int i = left; i <= right; i++)
            count[(intData[i] / exp) % 10]++;

        int start = left;
        for (int i = 0; i < 10; i++)
        {
            int bucketSize = count[i];
            if (bucketSize > 1)
                yield return RadixSortMSD(vis, data, intData, start, start + bucketSize - 1, digit - 1);
            start += bucketSize;

        }
    }

    #endregion

    #region IntroSort

    public static IEnumerator IntroSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        if (data == null || data.Count <= 1)
            yield break;

        int maxDepth = 2 * (int)Mathf.Log(data.Count, 2);
        yield return IntroSort(vis, data, 0, data.Count - 1, maxDepth);
    }

    private static IEnumerator IntroSort<T>(SortVisualizer vis, IList<T> data, int left, int right, int maxDepth)
        where T : IComparable<T>
    {
        if (left >= right)
            yield break;

        if (maxDepth == 0)
        {
            // Fallback to HeapSort on subarray
            yield return HeapSort(vis, data, left, right);
        }
        else
        {
            // Partition returns pivot index via callback
            int pivotIndex = (left + right) / 2;
            int newPivot = -1;
            yield return Partition(vis, data, left, right, pivotIndex, result => newPivot = result);

            yield return IntroSort(vis, data, left, newPivot - 1, maxDepth - 1);
            yield return IntroSort(vis, data, newPivot + 1, right, maxDepth - 1);
        }
    }

    private static IEnumerator Partition<T>(SortVisualizer vis, IList<T> data, int left, int right, int pivotIndex,
        Action<int> setPivot) where T : IComparable<T>
    {
        T pivotValue = data[pivotIndex];
        (data[pivotIndex], data[right]) = (data[right], data[pivotIndex]);
        yield return vis.SwitchBars(pivotIndex, right);

        int storeIndex = left;
        for (int i = left; i < right; i++)
        {
            if (data[i].CompareTo(pivotValue) < 0)
            {
                (data[i], data[storeIndex]) = (data[storeIndex], data[i]);
                yield return vis.SwitchBars(i, storeIndex);
                storeIndex++;
            }
        }

        (data[storeIndex], data[right]) = (data[right], data[storeIndex]);
        yield return vis.SwitchBars(storeIndex, right);

        setPivot(storeIndex);
    }

    public static IEnumerator HeapSort<T>(SortVisualizer vis, IList<T> data, int left, int right)
        where T : IComparable<T>
    {
        int count = right - left + 1;

        for (int i = count / 2 - 1; i >= 0; i--)
            yield return Heapify(vis, data, count, i, left);

        for (int i = count - 1; i > 0; i--)
        {
            (data[left], data[left + i]) = (data[left + i], data[left]);
            yield return vis.SwitchBars(left, left + i);

            yield return Heapify(vis, data, i, 0, left);
        }
    }

    private static IEnumerator Heapify<T>(SortVisualizer vis, IList<T> data, int n, int i, int offset)
        where T : IComparable<T>
    {
        int largest = i;
        int l = 2 * i + 1;
        int r = 2 * i + 2;

        if (l < n && data[offset + l].CompareTo(data[offset + largest]) > 0)
            largest = l;
        if (r < n && data[offset + r].CompareTo(data[offset + largest]) > 0)
            largest = r;

        if (largest != i)
        {
            (data[offset + i], data[offset + largest]) = (data[offset + largest], data[offset + i]);
            yield return vis.SwitchBars(offset + i, offset + largest);

            yield return Heapify(vis, data, n, largest, offset);
        }
    }

    #endregion

    #region AdaptiveMergeSort

    public static IEnumerator AdaptiveMergeSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        if (data == null || data.Count <= 1)
            yield break;

        T[] temp = new T[data.Count];
        yield return Sort(vis, data, temp, 0, data.Count - 1);
    }

    private static IEnumerator Sort<T>(SortVisualizer vis, IList<T> data, T[] temp, int left, int right)
        where T : IComparable<T>
    {
        if (IsSorted(data, left, right))
            yield break;

        if (left < right)
        {
            int mid = (left + right) / 2;

            yield return Sort(vis, data, temp, left, mid);
            yield return Sort(vis, data, temp, mid + 1, right);

            yield return Merge(vis, data, temp, left, mid, right);
        }
    }

    private static bool IsSorted<T>(IList<T> data, int left, int right) where T : IComparable<T>
    {
        for (int i = left; i < right; i++)
        {
            if (data[i].CompareTo(data[i + 1]) > 0)
                return false;
        }

        return true;
    }

    private static IEnumerator Merge<T>(SortVisualizer vis, IList<T> data, T[] temp, int left, int mid, int right)
        where T : IComparable<T>
    {
        int i = left, j = mid + 1, k = left;

        while (i <= mid && j <= right)
        {
            if (data[i].CompareTo(data[j]) <= 0)
                temp[k++] = data[i++];
            else
                temp[k++] = data[j++];
        }

        while (i <= mid)
            temp[k++] = data[i++];

        while (j <= right)
            temp[k++] = data[j++];

        for (int x = left; x <= right; x++)
        {
            data[x] = temp[x];

            if (data[x] is float f)
                yield return vis.SetBarHeight(x, f, Color.blue);
            else
                yield return vis.SwitchBars(x, x);
        }
    }

    #endregion

    // Time Complexity: O(n²)
    // Aux Space: O(1)
    #region BubbleSort

    public static IEnumerator BubbleSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        int n = data.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                yield return vis.PaintBars(j, j + 1, Color.red);

                if (data[j].CompareTo(data[j + 1]) > 0)
                {
                    (data[j], data[j + 1]) = (data[j + 1], data[j]);

                    yield return vis.SwitchBars(j, j + 1);
                }
            }
        }
    }

    #endregion

    // Time Complexity: O(n²)
    // Aux Space: O(1)
    #region GnomeSort

    public static IEnumerator GnomeSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        int index = 0;

        while (index < data.Count)
        {
            if (index == 0)
                index++;

            if (data[index].CompareTo(data[index - 1]) >= 0)
                index++;
            else
            {
                (data[index], data[index - 1]) = (data[index - 1], data[index]);

                yield return vis.PaintBars(index, index - 1, Color.red);
                yield return vis.SwitchBars(index, index - 1);

                index--;
            }
        }
    }

    #endregion

    //Time Complexity: O(n log n)
    //Aux Space: O(n)
    #region MergeSort

    public static IEnumerator MergeSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        yield return MergeSort(vis, data, 0, data.Count - 1);
    }

    private static IEnumerator MergeSort<T>(SortVisualizer vis, IList<T> data, int left, int right)
    where T : IComparable<T>
    {
        if (left >= right)
            yield break;

        int mid = (left + right) / 2;
        longOpetarions++;

        IEnumerator leftSort = MergeSort(vis, data, left, mid);
        while (leftSort.MoveNext())
        {
            yield return leftSort.Current;
        }

        IEnumerator rightSort = MergeSort(vis, data, mid + 1, right);
        while (rightSort.MoveNext())
        {
            yield return rightSort.Current;
        }

        List<T> merged = new List<T>(right - left + 1);

        int i = left;
        int j = mid + 1;

        while (i <= mid && j <= right)
        {
            yield return vis.PaintBars(i, j, Color.blue);

            if (data[i].CompareTo(data[j]) <= 0)
            {
                merged.Add(data[i]);
                i++;
            }
            else
            {
                merged.Add(data[j]);
                j++;
            }
        }

        while (i <= mid)
        {
            merged.Add(data[i]);
            i++;
        }

        while (j <= right)
        {
            merged.Add(data[j]);
            j++;
        }

        for (int k = 0; k < merged.Count; k++)
        {
            data[left + k] = merged[k];
            yield return vis.SetBarHeight(left + k, Convert.ToSingle(merged[k]), Color.green);
        }

        result = longOpetarions * Mathf.Log(2, longOpetarions);
    }

    #endregion

    //Time Complexity: O(n log n)
    //Aux Space: O(log n) Recursive or O(1) using iterators
    #region HeapSort

    public static IEnumerator HeapSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        for (int i = data.Count / 2 - 1; i >= 0; i--)
            yield return Heapify(vis, data, data.Count, i);

        for (int i = data.Count - 1; i > 0; --i)
        {
            (data[0], data[i]) = (data[i], data[0]);
            yield return vis.SwitchBars(0, i);

            yield return Heapify(vis, data, i, 0);
        }
    }

    private static IEnumerator Heapify<T>(SortVisualizer vis, IList<T> data, int n, int i) where T : IComparable<T>
    {
        // Initialize largest as root
        int largest = i;

        // left index;
        int l = 2 * i + 1;

        // right index;
        int r = 2 * i + 2;

        // If left child is larger than root
        if (l < n && data[l].CompareTo(data[largest]) > 0)
            largest = l;

        // If right child is larger than largest so far
        if (r < n && data[r].CompareTo(data[largest]) > 0)
            largest = r;

        // If largest is not root
        if (largest != i)
        {
            (data[i], data[largest]) = (data[largest], data[i]);
            yield return vis.SwitchBars(largest, i);

            yield return Heapify(vis, data, n, largest);
        }
    }

    #endregion

    //Time Complexity: O(n²)
    //Aux Space: O(1)
    #region InsertionSort

    public static IEnumerator InsertionSort<T>(SortVisualizer vis, IList<T> data) where T : IComparable<T>
    {
        for (int i = 1; i < data.Count; i++)
        {
            int j = i;

            while (j > 0 && data[j - 1].CompareTo(data[j]) > 0)
            {
                yield return vis.PaintBars(j - 1, j, Color.cyan);

                (data[j - 1], data[j]) = (data[j], data[j - 1]);

                yield return vis.SwitchBars(j - 1, j);

                j--;
            }
        }
    }

    #endregion
}