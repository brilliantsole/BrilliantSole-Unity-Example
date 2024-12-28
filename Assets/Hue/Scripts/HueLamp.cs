using UnityEngine;
using System.Collections.Generic;

using MiniJSON;
using System.Collections;

[ExecuteInEditMode]
public class HueLamp : MonoBehaviour
{
	public string devicePath;
	public bool on = true;
	public Color color = Color.white;

	private bool oldOn;
	private Color oldColor;

	public bool shouldUpdateHSV = false;
	public Vector3 hsv;

	public Dictionary<string, object> state = new();

	private IEnumerator SendMessage()
	{
		if (bridge == null)
		{
			Debug.LogError("HueBridge is not assigned.");
			yield break;
		}

		string url = $"http://{bridge.hostName}/api/{bridge.username}/lights/{devicePath}/state";

		state["on"] = on;
		state["hue"] = (int)(hsv.x / 360.0f * 65535.0f);
		state["sat"] = (int)(hsv.y * 255.0f);
		state["bri"] = (int)(hsv.z * 255.0f);

		string jsonData = Json.Serialize(state);
		byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

		using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Put(url, jsonBytes))
		{
			request.SetRequestHeader("Content-Type", "application/json");

			Debug.Log($"Sending request to: {url} with data: {jsonData}");

			yield return request.SendWebRequest();

			if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
			{
				Debug.LogError($"Error sending message: {request.error}");
			}
			else
			{
				Debug.Log("Message sent successfully.");
			}
		}
	}

	public HueBridge bridge;

	private readonly BS_Throttler throttler = new();

	void Update()
	{
		if (bridge == null)
		{
			Debug.LogError("no hue bridge found");
			return;
		}

		throttler.Throttle(UpdateLight, 0.1f);
		throttler.Update();
	}

	private void UpdateLight()
	{
		if (shouldUpdateHSV)
		{
			StartCoroutine(SendMessage());
			shouldUpdateHSV = false;
		}
		else if (oldOn != on || oldColor != color)
		{
			StartCoroutine(SendMessage());

			oldOn = on;
			oldColor = color;
		}
	}

	static Vector3 HSVFromRGB(Color rgb)
	{
		float max = Mathf.Max(rgb.r, Mathf.Max(rgb.g, rgb.b));
		float min = Mathf.Min(rgb.r, Mathf.Min(rgb.g, rgb.b));

		float brightness = rgb.a;

		float hue, saturation;
		if (max == min)
		{
			hue = 0f;
			saturation = 0f;
		}
		else
		{
			float c = max - min;
			if (max == rgb.r)
			{
				hue = (rgb.g - rgb.b) / c;
			}
			else if (max == rgb.g)
			{
				hue = (rgb.b - rgb.r) / c + 2f;
			}
			else
			{
				hue = (rgb.r - rgb.g) / c + 4f;
			}

			hue *= 60f;
			if (hue < 0f)
			{
				hue += 360f;
			}

			saturation = c / max;
		}

		return new Vector3(hue, saturation, brightness);
	}
}