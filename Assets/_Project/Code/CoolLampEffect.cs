using System.Collections.Generic;
using UnityEngine;

public class CoolLampEffect : MonoBehaviour {
	public Color _color1 = Color.red;
	public Color _color2 = Color.yellow;
	[SerializeField] private List<Light> Lights;

	private void FixedUpdate() {
		float lerpFactor = Mathf.PingPong(Time.time * 1, 1.0f);
		if (GetComponent<MeshRenderer>().material != null) {
			GetComponent<Renderer>().materials[1] = new Material(Shader.Find("Universal Render Pipeline/Lit"));
			GetComponent<Renderer>().materials[1].color = Color.Lerp(_color1, _color2, lerpFactor);
		}

		foreach (var light1 in Lights) {
			light1.color = Color.Lerp(_color1, _color2, lerpFactor);
		}
	}
}