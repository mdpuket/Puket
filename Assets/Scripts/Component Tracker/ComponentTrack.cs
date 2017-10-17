using UnityEngine;
using System.Collections;

/* Component Track
 * Created by: Cesar Olimpio
 * Date created: 6/23/2014
 * Date modified: 6/23/2014
 * Version: 0.1
 */

public class ComponentTrack : MonoBehaviour {

	public string identifier;

	void Awake()
	{
		if(!string.IsNullOrEmpty(identifier))
			ComponentTracker.Instance.Add (this, identifier);
	}
}
