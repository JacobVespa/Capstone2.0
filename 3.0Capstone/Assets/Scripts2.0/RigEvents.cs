using System;
using UnityEngine;

public class RigEvents
{

    public event Action SpeedStart;
    public void CallSpeedStart() => SpeedStart?.Invoke();
   
}
