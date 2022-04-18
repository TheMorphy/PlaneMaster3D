using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    Animator anim;
    [SerializeField]
    GameObject build, system;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void OnAllConditionsComplete()
    {
        anim.Play("Build");
    }
    public void SwitchToSystem()
    {
        build.SetActive(false);
        system.SetActive(true);
    }
}
