using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexValue : MonoBehaviour
{
    [SerializeField] private int indexValue;

    public int GetIndexValue()
    {
        return indexValue;
    }

    public void SetIndexValue(int newIndex)
    {
        indexValue = newIndex;
    }
}