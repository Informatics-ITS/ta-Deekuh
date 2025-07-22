using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonUtilityWrapper : MonoBehaviour
{
    [System.Serializable]
    private class Wrapper
    {
        public List<string> items;
    }

    public static List<string> ParseJsonArray(string jsonArray)
    {
        try
        {
            // Basic check: input must start with [ and end with ]
            jsonArray = jsonArray.Trim();
            if (!jsonArray.StartsWith("[") || !jsonArray.EndsWith("]"))
            {
                Debug.LogWarning("ParseJsonArray: Input is not a valid JSON array.");
                return new List<string>();  // return empty list = don't update anything
            }

            // Wrap the array in an object so JsonUtility can parse it
            string wrappedJson = "{\"items\":" + jsonArray + "}";

            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(wrappedJson);

            if (wrapper == null || wrapper.items == null)
            {
                Debug.LogWarning("ParseJsonArray: Parsed wrapper or items is null.");
                return new List<string>();  // return empty list = don't update anything
            }

            return wrapper.items;
        }
        catch (Exception e)
        {
            Debug.LogError("ParseJsonArray: Exception while parsing JSON array: " + e.Message);
            return new List<string>();  // return empty list = don't update anything
        }
    }
}
