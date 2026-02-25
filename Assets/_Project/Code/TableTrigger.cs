using UnityEngine;

public class TableTrigger : MonoBehaviour
{
    [SerializeField] TableViewEvents tableViewEvents;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        tableViewEvents.RequestCameraMode(CameraMode.Table);
        tableViewEvents.PlayerInTrigger(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        tableViewEvents.RequestCameraMode(CameraMode.Main);
        tableViewEvents.PlayerInTrigger(false);
    }
}