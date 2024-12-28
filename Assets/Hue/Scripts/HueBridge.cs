using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System.Linq;
using System;

public class HueBridge : MonoBehaviour
{
	public string hostName = "127.0.0.1";
	public int portNumber = 8000;
	public string username = "newdeveloper";

	public Action<HueBridge, IReadOnlyList<HueLamp>> OnDiscoveredHueLamps;
	public IReadOnlyList<HueLamp> HueLamps => GetComponentsInChildren<HueLamp>();

	public void DiscoverLights()
	{
		StartCoroutine(DiscoverLightsCoroutine());
	}

	private IEnumerator DiscoverLightsCoroutine()
	{
		List<HueLamp> hueLamps = new();

		string url = $"http://{hostName}/api/{username}/lights";
		Debug.Log($"Requesting lights from {url}");

		using UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url);
		yield return request.SendWebRequest();

		if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
		{
			Debug.LogError($"Error retrieving lights: {request.error}");
			yield break;
		}

		try
		{
			string jsonResponse = request.downloadHandler.text;
			var lights = (Dictionary<string, object>)Json.Deserialize(jsonResponse);

			foreach (string key in lights.Keys)
			{
				var light = (Dictionary<string, object>)lights[key];
				HueLamp hueLamp = null;
				foreach (var exisitingHueLamp in HueLamps)
				{
					if (exisitingHueLamp.devicePath.Equals(key))
					{
						hueLamp = exisitingHueLamp;
						hueLamp.gameObject.name = (string)light["name"];
						hueLamp.name = hueLamp.gameObject.name;
						break;
					}
				}

				if (hueLamp == null && light["type"].Equals("Extended color light"))
				{
					GameObject gameObject = new()
					{
						name = (string)light["name"]
					};
					gameObject.transform.parent = transform;
					gameObject.AddComponent<HueLamp>();
					hueLamp = gameObject.GetComponent<HueLamp>();
					hueLamp.bridge = this;
					hueLamp.name = gameObject.name;
					hueLamp.devicePath = key;
				}

				hueLamps.Add(hueLamp);
			}
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Error parsing lights: {ex.Message}");
		}

		foreach (var hueLamp in HueLamps)
		{
			if (!hueLamps.Contains(hueLamp))
			{
				Destroy(hueLamp.gameObject);
			}
		}
		OnDiscoveredHueLamps?.Invoke(this, hueLamps);
	}
}
