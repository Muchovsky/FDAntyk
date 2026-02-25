using UnityEngine;

public class PaperView : MonoBehaviour
{
   
    [SerializeField] ScribbleSurface scribbleSurface;
    [SerializeField] CutSurface cutSurface;
    [SerializeField] TableViewEvents tableViewEvents;

    void OnEnable()
    {
        tableViewEvents.OnPlayerInTrigger += SetIsInteractable;
    }

    void OnDisable()
    {
        tableViewEvents.OnPlayerInTrigger -= SetIsInteractable;
    }

    void SetIsInteractable(bool value)
    {
        scribbleSurface.SetIsInteractable(value);
    }
}