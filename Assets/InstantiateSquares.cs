using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateSquares : MonoBehaviour
{
    public static float[] freqBand = new float[8];
    public static float[] samples = new float[1024];
    public GameObject sampleSquarePrefab;
    GameObject[] sampleSquare = new GameObject[512];
    public float maxScale;
    public float startXPos = -4f;
    public float startYPos = -2.4f;
    public int tunerRange = 20;
    // Start is called before the first frame update
    void Start()
    {
        int ret = Mathf.RoundToInt(Mathf.Log(samples.Length));
        for(int i = 0; i < 512; i++){
            
                GameObject instanceSampleSquare = (GameObject)Instantiate (sampleSquarePrefab);
                instanceSampleSquare.transform.position = new Vector3(startXPos + (float)(i) * .04247f, startYPos, 0f);
                instanceSampleSquare.transform.parent = this.transform;
                instanceSampleSquare.name = "SampleSquare " + i;
                sampleSquare[i] = instanceSampleSquare;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 512; i++){

            if(sampleSquare != null){
                sampleSquare[i].transform.localScale = new Vector3(1, (DetectPitch.samples[i] * maxScale) * 2, 1);
            } 
        }
    }

}
