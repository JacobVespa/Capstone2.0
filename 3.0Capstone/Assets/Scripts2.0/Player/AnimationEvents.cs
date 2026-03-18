using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] private Animator hammerAnims;

    public void PlayHammerPulse()
    {
        hammerAnims.Play("HammerPulse");
    }

}
