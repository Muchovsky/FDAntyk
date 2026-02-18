using UnityEngine;

public class SpawnPoint : MonoBehaviour {
	public GameObject playerPrefab;
	GameObject player;
	static SpawnPoint instance;

	void Awake() {
		if (instance == null)
			instance = this;
		player = Instantiate(playerPrefab);
		ResetPlayerPosition();
		player.name = "Player";
		gameObject.SetActive(false);
	}

	public static void ResetPlayerPosition() {
		if (instance == null) {
			Debug.LogError("No spawn point");
			return;
		}

		instance.player.transform.position = instance.transform.position;
	}
}