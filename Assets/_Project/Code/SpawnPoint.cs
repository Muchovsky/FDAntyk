using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    GameObject player;

    void Awake()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not assigned");
            return;
        }

        player = Instantiate(playerPrefab);
        ResetPlayerPosition();
        player.name = "Player";
    }

    void ResetPlayerPosition()
    {
        player.transform.SetPositionAndRotation(
            transform.position,
            transform.rotation
        );
    }
}