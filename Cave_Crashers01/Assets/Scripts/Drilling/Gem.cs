using UnityEngine;

public class Gem : MonoBehaviour
{

    public string gemName;
    public GemSize gemSize;

    public Gem(string name, GemSize size)
    {
        gemName = name;
        gemSize = size;
    }

    public enum GemSize
    {
        Small, //default value of 0
        Medium, //default value of 1
        Large //default value of 2
    }

}
