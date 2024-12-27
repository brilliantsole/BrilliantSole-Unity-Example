using UnityEngine;
using System.Collections.Generic;
using System.Net;

using MiniJSON;
using System;
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

	private HueBridge bridge;

	private readonly BS_Throttler throttler = new();

	void OnEnable()
	{
		bridge = GameObject.Find("Hue Bridge").GetComponent<HueBridge>();
	}

	public void SetHSVColor()
	{
		if (bridge == null)
		{
			Debug.LogError("no hue bridge found");
			return;
		}
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + bridge.hostName + "/api/" + bridge.username + "/lights/" + devicePath + "/state");
		Debug.Log("http" + bridge.hostName + bridge.portNumber + "/api/" + bridge.username + "/lights/" + devicePath + "/state");
		request.Method = "PUT";

		//state["on"] = on;
		state["hue"] = (int)(hsv.x * 65535.0f);
		state["sat"] = (int)(hsv.y * 255.0f);
		state["bri"] = (int)(hsv.z * 255.0f);

		byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Json.Serialize(state));
		request.ContentLength = bytes.Length;

		System.IO.Stream s = request.GetRequestStream();
		s.Write(bytes, 0, bytes.Length);
		s.Close();

		HttpWebResponse response = (HttpWebResponse)request.GetResponse();

		response.Close();
	}

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
			SetHSVColor();
			shouldUpdateHSV = false;
		}

		if (oldOn != on || oldColor != color)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + bridge.hostName + "/api/" + bridge.username + "/lights/" + devicePath + "/state");
			Debug.Log("http" + bridge.hostName + bridge.portNumber + "/api/" + bridge.username + "/lights/" + devicePath + "/state");
			request.Method = "PUT";

			Vector3 hsv = HSVFromRGB(color);

			//Debug.Log(hsv);

			var state = new Dictionary<string, object>
			{
				["on"] = on,
				["hue"] = (int)(hsv.x / 360.0f * 65535.0f),
				["sat"] = (int)(hsv.y * 255.0f),
				["bri"] = (int)(hsv.z * 255.0f)
			};
			if ((int)(hsv.z * 255.0f) == 0) state["on"] = false;

			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Json.Serialize(state));
			request.ContentLength = bytes.Length;

			System.IO.Stream s = request.GetRequestStream();
			s.Write(bytes, 0, bytes.Length);
			s.Close();

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			response.Close();

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