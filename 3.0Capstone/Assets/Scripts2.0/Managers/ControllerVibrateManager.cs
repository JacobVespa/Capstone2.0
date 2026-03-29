using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ControllerVibrateManager : MonoBehaviour
{
    public static ControllerVibrateManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    //Single short burst — use for hammer attacks or one-shot impacts.
    public void Burst(int playerIndex, float lowFreq = 0.5f, float highFreq = 0.8f, float duration = 0.15f, float delay = 0f)
    {
        Gamepad pad = GetGamepad(playerIndex);
        if (pad == null) return;

        StartCoroutine(BurstRoutine(pad, lowFreq, highFreq, duration, delay));
    }

    // Repeated short pulses — use for turret recoil.
    public void RecoilBurst(int playerIndex, int shots = 3, float intervalSeconds = 0.1f,
                            float lowFreq = 0.6f, float highFreq = 0.9f, float pulseDuration = 0.06f)
    {
        Gamepad pad = GetGamepad(playerIndex);
        if (pad == null) return;

        StartCoroutine(RecoilRoutine(pad, shots, intervalSeconds, lowFreq, highFreq, pulseDuration));
    }

    /// Stops vibration immediately on a given player's controller.
    public void StopVibration(int playerIndex)
    {
        Gamepad pad = GetGamepad(playerIndex);
        pad?.SetMotorSpeeds(0f, 0f);
    }

    /// Stops vibration on all connected gamepads.
    public void StopAllVibration()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad pad)
                pad.SetMotorSpeeds(0f, 0f);
        }
    }

    // ── Coroutines ────────────────────────────────────────────────────────────

    private IEnumerator BurstRoutine(Gamepad pad, float lowFreq, float highFreq, float duration, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        pad.SetMotorSpeeds(lowFreq, highFreq);
        yield return new WaitForSeconds(duration);
        pad.SetMotorSpeeds(0f, 0f);
    }

    private IEnumerator RecoilRoutine(Gamepad pad, int shots, float interval,
                                       float lowFreq, float highFreq, float pulseDuration)
    {
        for (int i = 0; i < shots; i++)
        {
            pad.SetMotorSpeeds(lowFreq, highFreq);
            yield return new WaitForSeconds(pulseDuration);
            pad.SetMotorSpeeds(0f, 0f);

            // Wait out the rest of the interval before the next pulse
            float remaining = interval - pulseDuration;
            if (remaining > 0f)
                yield return new WaitForSeconds(remaining);
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Gamepad GetGamepad(int playerIndex)
    {
        int gamepadsSeen = 0;

        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad pad)
            {
                if (gamepadsSeen == playerIndex)
                    return pad;
                gamepadsSeen++;
            }
        }

        return null;
    }

    private void OnApplicationQuit()
    {
        StopAllVibration();
    }

    private void OnDisable()
    {
        StopAllVibration();
    }
}