using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRates : MonoBehaviour
{
    private static Stack<float> modifiedBasicStackPercentage = new Stack<float>();
    private static Stack<float> modifiedPowerfulStackPercentage = new Stack<float>();
    private static Stack<float> modifiedEngineerStackPercentage = new Stack<float>();
    public static float percentageBasicRobot
    {
        get
        {
            if (modifiedBasicStackPercentage.Count != 0)
            {
                float rand = Random.Range(0, 100);
                if (rand < 50)
                {
                    return modifiedBasicStackPercentage.Pop();
                }
            }
            return 8;
        }
    }
    public static float percentagePowerfulRobot
    {
        get
        {
            if (modifiedPowerfulStackPercentage.Count != 0)
            {
                float rand = Random.Range(0, 100);
                if (rand < 50)
                {
                    return modifiedPowerfulStackPercentage.Pop();
                }
            }
            return 4;
        }
    }
    public static float percentageEngineerRobot
    {
        get
        {
            if (modifiedEngineerStackPercentage.Count != 0)
            {
                float rand = Random.Range(0, 100);
                if (rand < 50)
                {
                    return modifiedEngineerStackPercentage.Pop();
                }
            }
            return 2;
        }
    }
    public static void AddBasicSpawnBoost(float perc)
    {
        modifiedBasicStackPercentage.Push(perc);
    }
    public static void AddPowerfulSpawnBoost(float perc)
    {
        modifiedPowerfulStackPercentage.Push(perc);
    }
    public static void AddEngineerSpawnBoost(float perc)
    {
        modifiedEngineerStackPercentage.Push(perc);
    }
}
