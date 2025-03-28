using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        var tubeLiquidModel = JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("levels-hard").text);
        var tubesCount = new HashSet<int>();
        for (var i = 0; i < tubeLiquidModel.TotalLevels; i++)
        {
            tubesCount.Add(tubeLiquidModel.GetTubeCount(i));
        }

        foreach (var mTubesCount in tubesCount)
        {
            Debug.Log($"Tubes Count {mTubesCount}");
        }
    }
}