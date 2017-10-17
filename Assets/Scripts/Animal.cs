using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour {

	// estará dormindo se as horas forem maiores que X ou menores que Y
	public static Vector2 sleepingTime = new Vector2(20f, 6f);

	public int idleEffects = 2;
	public float triggerIdleEffects = 10f;
	// public string[] actions, actionNames;
	public ActionObject[] actions;
	public AudioClip sleepingClip;

	private Animator animator;
	private float idleSince;
	private bool sleeping = false;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		idleSince = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		// trigger idle effect
		if (idleSince + triggerIdleEffects < Time.time) {
			int r = (int)(UnityEngine.Random.value*idleEffects);
			animator.SetTrigger("idleEffect" + r);
			idleSince = Time.time;
		}

		if (sleeping) idleSince = Time.time;
	}

	public void TriggerAnimation (string key) {
		// animator.SetTrigger(key);
		// idleSince = Time.time;
		if(sleeping) return;
		
		for (int i = 0; i < actions.Length; i++) {
			if (actions[i].actionId == key) {
				animator.SetTrigger(actions[i].actionId);
				idleSince = Time.time;
				for (int j = 0; j < actions[i].particles.Length; j++) {
					IEnumerator coroutine = ParticleAnimation(actions[i].particles[j]);
					StartCoroutine(coroutine);
				}
				// for (int j = 0; j < actions[i].audioclips.Length; j++) {
				// 	IEnumerator coroutine = AudioclipAnimation(actions[i].audioclips[j], false);
				// 	StartCoroutine(coroutine);
				// }
				break;
			}
		}
	}

	private IEnumerator ParticleAnimation (ParticleAnimationObject animationObj) {
		yield return new WaitForSeconds(animationObj.start);
		animationObj.particle.Play();
		yield return new WaitForSeconds(animationObj.duration);
		animationObj.particle.Stop();
	}

	private IEnumerator AudioclipAnimation(AudioclipAnimationObject animationObj, bool loop) {
		AudioSource source = Camera.main.gameObject.GetComponent<AudioSource>();
		if (source) {
			source.Stop();
			yield return new WaitForSeconds(animationObj.start);
			source.clip = animationObj.clip;
			source.loop = loop;
			source.Play();
			// AudioSource.PlayClipAtPoint(animationObj.clip, Camera.main.transform.position);
		}
	}

	private void VerifySleeping () {
		// animator.SetBool("sleeping", (System.DateTime.Now.Hour >= sleepingTime.x || System.DateTime.Now.Hour < sleepingTime.y));
		Debug.Log("------------- SLEEPING: " + (MenuUIController.alarmClock.CompareTo(DateTime.Now) > 0));
		animator.SetBool("sleeping", (MenuUIController.alarmClock.CompareTo(DateTime.Now) > 0));
		sleeping = animator.GetBool("sleeping");
		if (sleeping && sleepingClip) {
			// StartCoroutine(AudioclipAnimation(new AudioclipAnimationObject(sleepingClip), true));
		}
	}

	public void Tracked () {
		VerifySleeping();
		ARUIController.Instance.OnTrackAnimal(this);
	}

	public void Untracked () {
		ARUIController.Instance.OnLostTacking();
	}
}

[System.Serializable]
public class ActionObject {
	public string actionId, actionName;
	public ParticleAnimationObject[] particles;
	public AudioclipAnimationObject[] audioclips;

	public ActionObject() {}
}

[System.Serializable]
public class ParticleAnimationObject {
	public float start, duration;
	public ParticleSystem particle;

	public ParticleAnimationObject() {}
}

[System.Serializable]
public class AudioclipAnimationObject {
	public float start;
	public AudioClip clip;

	public AudioclipAnimationObject(AudioClip clip) {
		this.clip = clip;
		start = 0f;
	}

	public AudioclipAnimationObject(AudioClip clip, float start) {
		this.clip = clip;
		this.start = start;
	}
}
