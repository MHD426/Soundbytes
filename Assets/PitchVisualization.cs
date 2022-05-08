using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchVisualization : MonoBehaviour
{
    public AudioSource aSrc;
    public PitchEstimation estimator;
    public LineRenderer lineSRH;
    public LineRenderer lineFreq;
    public Transform marker;
    public TextMesh textMin;
    public TextMesh textMax;
    public TextMesh textFreq;

    public float estRate = 30;
    // Start is called before the first frame update
    void Start()
    {
        //call at slower intervals than update
        InvokeRepeating(nameof(UpdateVis), 0, 1.0f / estRate);
    }

    // Update is called once per frame
    void UpdateVis()
    {
        //estimate the root/fundemental freq
        var freq = estimator.Estimate(aSrc);

        //estimate SRH
        var srh = estimator.SumResHarmonics;
        var numPts = srh.Count;
        var pos = new Vector3[numPts];
        for (int i = 0; i < numPts; i++){
            var tmpPos = (float)i / numPts - 0.5f;
            var val = srh[i] * .005f;
            pos[i].Set(tmpPos, val, 0);
        }
        lineSRH.positionCount = numPts;
        lineSRH.SetPositions(pos);

        //root freq
        if(float.IsNaN(freq)){

            lineFreq.positionCount = 0;
        }
        else{
            var min = estimator.minFreq;
            var max = estimator.maxFreq;
            var tmpPos = (freq - min) / (max - min) - .5f;

            lineFreq.positionCount = 2;
            lineFreq.SetPosition(0, new Vector3(tmpPos, +1, 0));
            lineFreq.SetPosition(1, new Vector3(tmpPos, -1, 0));

            marker.position = new Vector3(tmpPos, 0, 0);
            textFreq.text = string.Format("{0}\n{1:0.0} Hz", GetNoteName(freq), freq);
        }

        //Lower and upper freq limits
        textMin.text = string.Format("{0} Hz", estimator.minFreq);
        textMax.text = string.Format("{0} Hz", estimator.maxFreq);
    }

    string GetNoteName(float frequency){
        var noteNum = Mathf.RoundToInt(12 * Mathf.Log(frequency / 440) / Mathf.Log(2) + 69);
        string[] notes = {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
        return notes[noteNum % 12];
    }
}
