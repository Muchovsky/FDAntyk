using TMPro;
using UnityEngine;

public class UI : MonoBehaviour {
	void Update() {
		var tests = FindObjectsOfType<TMP_Text>();
		foreach (var text in tests) {
			if (double.TryParse(text.text, out _)) {
				text.text = Time.time.ToString();
			}
		}
	}
}