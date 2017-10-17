using UnityEngine;
using System.Collections.Generic;

/* Component Tracker
 * Created by: Cesar Olimpio
 * Date created: 6/23/2014
 * Date modified: 6/23/2014
 * Version: 0.1
 */

public class ComponentTracker : MonoBehaviour {

	private static ComponentTracker instance;
	public static ComponentTracker Instance
	{
		get
		{
			if(instance == null)
				instance = new GameObject("Component Tracker").AddComponent<ComponentTracker>();
			return instance;
		}
	}

	private Dictionary<string, ComponentTrack> dict;

	void Awake()
	{
		instance = this;
		dict = new Dictionary<string, ComponentTrack> ();
	}

	public void Add(ComponentTrack localize, string identifier)
	{
		if (dict.ContainsKey (identifier))
			Debug.LogError ("The identifier \"" + identifier + "\" almost exists.");
		else
		{
			dict.Add(identifier, localize);
		}
	}

	public Type GetElement<Type>(string identifier) where Type : Component
	{
		if (!dict.ContainsKey (identifier))
						Debug.LogError ("The identifier \"" + identifier + "\" dont exists in the dictionary.");
		else
		{
			return (Type)dict[identifier].GetComponent(typeof(Type));
		}
		return null;
	}

	public GameObject GetObject(string identifier)
	{
		if (!dict.ContainsKey (identifier))
			Debug.LogError ("The identifier \"" + identifier + "\" dont exists in the dictionary.");
		else
		{
			return dict[identifier].gameObject;
		}
		return null;
	}
}
