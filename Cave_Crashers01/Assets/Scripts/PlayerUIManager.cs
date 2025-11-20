using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] PlayerBody body;

    public GameObject freeUI;

    public GameObject drivingUI;

    public GameObject turretUI;

    public void SetToFree()
    {
        freeUI.SetActive(true);
        drivingUI.SetActive(false);
        turretUI.SetActive(false);
    }

    public void SetToDriving()
    {
        freeUI.SetActive(false);
        drivingUI.SetActive(true);
        turretUI.SetActive(false);
    }

    public void SetToTurret()
    {
        freeUI.SetActive(false);
        drivingUI.SetActive(false);
        turretUI.SetActive(true);
    }
}
