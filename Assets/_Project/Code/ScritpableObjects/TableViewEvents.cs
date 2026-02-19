using UnityEngine;
using System;

[CreateAssetMenu(
    fileName = "TableViewEvent",
    menuName = "Events/Table View Event")]
public class TableViewEvents : ScriptableObject
{
    public event Action<CameraMode> OnCameraModeRequested;

    public void RequestCameraMode(CameraMode mode)
    {
        OnCameraModeRequested?.Invoke(mode);
    }
}