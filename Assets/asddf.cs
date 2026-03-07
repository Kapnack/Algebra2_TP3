using UnityEngine;

public class asddf : MonoBehaviour
{

    void Start()
    {
        int n = 8;

        float a = 0;

        for (int i = 0; i < n; i++)
        {
            ++a;
            Debug.Log(a);
        }
        a = 0;

        Debug.Log("--------------------------------------");

        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; ++j)
            {
                a++;
                Debug.Log(a);
            }

        a = 0;

        Debug.Log("--------------------------------------");


        int temp = n;
        while (temp > 1)
        {
            temp /= 2;
            a++;
        }
        Debug.Log(a);
        Debug.Log("--------------------------------------");

        a = 0;
        for (int i = 0; i < n; i++)
            for (int aux = n; aux > 1; aux /= 2)
                a++;

        Debug.Log(a);
        Debug.Log(n * Mathf.Log(n, 2));
    }
}
