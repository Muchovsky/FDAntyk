using UnityEngine;

public class TableTrigger : MonoBehaviour
{
    public TableViewEvents tableViewEvents;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        tableViewEvents.RequestCameraMode(CameraMode.Table);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        tableViewEvents.RequestCameraMode(CameraMode.Main);
    }
}