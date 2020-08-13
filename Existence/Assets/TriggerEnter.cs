using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerEnter : Selectable
{
    public AudioSource clip;
    public ParticleSystem particle;
    public Animation anim;

    private void Start() {
        TargetController.instance.OnTargetSelected += OnTargetSelected;
    }

    private void OnDisable() {
        TargetController.instance.OnTargetSelected -= OnTargetSelected;
    }

    private void OnTargetSelected(Selectable _s, bool _primary) {
        if (_s == this) {
            try{    
            clip.Play();
        }catch (Exception e){}
        try{    
            particle.Play();
        }catch (Exception e){}
        try{    
            anim.Play();
        }catch (Exception e){}
        }
    }
}
