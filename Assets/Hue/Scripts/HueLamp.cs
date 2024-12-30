using UnityEngine;
using System.Collections.Generic;

using MiniJSON;
using System.Collections;
using System;

[ExecuteInEditMode]
public class HueLamp : MonoBehaviour
{
	public string devicePath;
	public bool on = true;
	public Color color = Color.white;

	private bool oldOn;
	private Color oldColor;

	public Action<Color> OnColorUpdate;

	private bool shouldUpdateHSV = false;

	[Range(0.0f, 1.0f)]
	[SerializeField]
	private float hue = 0.0f;
	private float oldHue = 0.0f;
	public float Hue
	{
		get => hue;
		set
		{
			hue = value;
			oldHue = value;
			shouldUpdateHSV = true;
		}
	}

	[Range(0.0f, 1.0f)]
	[SerializeField]
	private float saturation = 0.0f;
	private float oldSaturation = 0.0f;
	public float Saturation
	{
		get => saturation;
		set
		{
			saturation = value;
			oldSaturation = value;
			shouldUpdateHSV = true;
		}
	}

	[Range(0.0f, 1.0f)]
	[SerializeField]
	private float brightness = 0.0f;
	private float oldBrightness = 0.0f;
	public float Brightness
	{
		get => brightness;
		set
		{
			brightness = value;
			oldBrightness = value;
			shouldUpdateHSV = true;
		}
	}

	public Dictionary<string, object> state = new();

	private IEnumerator SendMessage(bool includeOn = false)
	{
		if (bridge == null)
		{
			Debug.LogError("HueBridge is not assigned.");
			yield break;
		}

		string url = $"http://{bridge.hostName}/api/{bridge.username}/lights/{devicePath}/state";
		if (includeOn)
		{
			state["on"] = on;
		}
		else
		{
			state["hue"] = (int)(Hue * 65535.0f);
			state["sat"] = (int)(Saturation * 255.0f);
			state["bri"] = (int)(Brightness * 255.0f);
		}

		string jsonData = Json.Serialize(state);
		byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

		using UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Put(url, jsonBytes);
		request.SetRequestHeader("Content-Type", "application/json");

		//Debug.Log($"Sending request to: {url} with data: {jsonData}");

		yield return request.SendWebRequest();

		if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
		{
			Debug.LogError($"Error sending message: {request.error}");
		}
		else
		{
			//Debug.Log("Message sent successfully.");
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
		if (shouldUpdateHSV || oldHue != hue || oldSaturation != saturation || oldBrightness != brightness)
		{
			oldHue = hue;
			oldSaturation = saturation;
			oldBrightness = brightness;

			StartCoroutine(SendMessage());
			shouldUpdateHSV = false;
			UpdateComputedColor();
		}
		else if (oldOn != on)
		{
			StartCoroutine(SendMessage(true));
			oldOn = on;
			UpdateComputedColor();
		}
		else if (oldColor != color)
		{
			var hsv = HSVFromRGB(color);
			hue = hsv.x;
			saturation = hsv.y;
			brightness = hsv.z;
			StartCoroutine(SendMessage());
			oldColor = color;
			UpdateComputedColor();
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
			hue /= 360.0f;

			saturation = c / max;
		}

		return new Vector3(hue, saturation, brightness);
	}


	[SerializeField]
	private Color computedColor = new();
	public Color ComputedColor => on ? computedColor : Color.black;
	private void UpdateComputedColor()
	{
		computedColor = Color.HSVToRGB(Hue, Saturation, Brightness);
		OnColorUpdate?.Invoke(ComputedColor);
	}
}