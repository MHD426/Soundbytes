using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreqBand : MonoBehaviour
{
    public float Val;
    public float Index;
    public float Freq;

    public FreqBand(float val, float index, float freq){
        this.Val = val;
        this.Index = index;
        this.Freq = freq;
    }

    void Init(int size){

    }
    // Start is called before the first frame update
    void Start()
    {
        Val = 0.0f;
        Index = 0.0f;
        Freq = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
