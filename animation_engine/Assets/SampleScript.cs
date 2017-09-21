using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animations;

[RequireComponent(typeof(AnimationEngine))]
public class SampleScript : MonoBehaviour
{

    AnimationEngine animationEngine;

    // Use this for initialization
    void Start()
    {
        animationEngine = GetComponent<AnimationEngine>();
        // TODO, fix 0, 1
        animationEngine.AddAnimations(new int[] { 1 });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
