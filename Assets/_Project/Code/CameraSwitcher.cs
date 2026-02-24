using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera tableCamera;
    public TableViewEvents tableViewEvents;

    void OnEnable()
    {
        tableViewEvents.OnCameraModeRequested += HandleTableView;
    }

    void OnDisable()
    {
        tableViewEvents.OnCameraModeRequested -= HandleTableView;
    }

    void Awake()
    {
        if (mainCamera == null || tableCamera == null)
        {
            Debug.Log("One camera is not set");
        }

        mainCamera.enabled = true;
        tableCamera.enabled = false;
    }

    void HandleTableView(CameraMode mode)
    {
        switch (mode)
        {
            case CameraMode.Main:
                SetMainCamera();
                break;
            case CameraMode.Table:
                SetTableCamera();
                break;

            default:
                SetMainCamera();
                break;
        }
    }

    void ToggleCameras()
    {
        mainCamera.enabled = !mainCamera.enabled;
        tableCamera.enabled = !tableCamera.enabled;
    }

    void SetMainCamera()
    {
        mainCamera.enabled = true;
        tableCamera.enabled = false;
    }

    void SetTableCamera()
    {
        mainCamera.enabled = false;
        tableCamera.enabled = true;
    }


    #if UNITY_EDITOR
    //DebugOnly
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ToggleCameras();
        }
    }
    #endif
}