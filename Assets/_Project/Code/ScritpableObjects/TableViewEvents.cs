using UnityEngine;
using System;

[CreateAssetMenu(
    fileName = "TableViewEvent",
    menuName = "Events/Table View Event")]
public class TableViewEvents : ScriptableObject
{
    public event Action<CameraMode> OnCameraModeRequested;
    public event Action<bool> OnPlayerInTrigger;

    public void RequestCameraMode(CameraMode mode)
    {
        OnCameraModeRequested?.Invoke(mode);
    }

    public void PlayerInTrigger(bool enter)
    {
        OnPlayerInTrigger?.Invoke(enter);
    }


}