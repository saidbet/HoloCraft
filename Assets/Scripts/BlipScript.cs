using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlipScript : MonoBehaviour {

	void Start () {
        StartCoroutine(StartAnimation());
    }
	
    private IEnumerator StartAnimation()
    {
        
        Animation anim = gameObject.GetComponent<Animation>();
        
        anim["ping"].speed = 3f;
        anim.Play(anim.clip.name);

        yield return new WaitUntil(() => anim.isPlaying == false);
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
    }
}
