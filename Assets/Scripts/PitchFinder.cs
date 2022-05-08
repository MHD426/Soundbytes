/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


[RequireComponent (typeof(AudioSource))]
public class PitchFinder : MonoBehaviour
{
    public float rmsVal; //RootMeanSquare
    public float dbVal; //decibels/volume
    public float pitchVal; //Hz

    public int qSamples = 1024;
    public int binSize = 1024;
    public float refVal = 0.1f;
    public float threshold = 0.1f;

    private List<PeakFinder> peaks = new List<PeakFinder>();
    float[] samples;
    float[] spectrum;
    int sampleRate;

    public Text display;
    bool mute = true;
    public AudioMixer mstrMxr;

    void Start(){
        samples = new float[qSamples];
        spectrum = new float[binSize];
        sampleRate = AudioSettings.outputSampleRate;

        //start Microphone, attach to audio source
        GetComponent<AudioSource>().clip = Microphone.Start(null, true, 10, sampleRate);
        GetComponent<AudioSource>().loop = true;
        //wait until microphone is recording
        while (!(Microphone.GetPosition(null) > 0)){}
        GetComponent<AudioSource>().Play();

        //mute the mixer
        mstrMxr.SetFloat("masterVolume", -80f);
    }

    void Update(){
        AnalyzeSound();
        if(display != null){
            display.text = "RMS: " + rmsVal.ToString("F2") + " (" + dbVal.ToString("F1") + " dB)\n" + "Pitch: "+ pitchVal.ToString("F0") + " Hz";
        }
    }

    void AnalyzeSound(){
        float[] samples = new float[qSamples];
        GetComponent<AudioSource>().GetOutputData(samples, 0); //fill samples array
        int i = 0;
        float sum = 0f;
        //calculate rms
        for(i = 0; i < qSamples; i++){
            sum += samples[i] * samples[i];
        }
        rmsVal = Mathf.Sqrt(sum/qSamples);

        //calculate dbVal and set minimum bound
        dbVal = 20*Mathf.Log10(rmsVal/refVal);
        if(dbVal < -160){
            dbVal = -160;
        }

        //get sound spectrum
        GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        float maxVal = 0f;
        //find maxVal
        for(i = 0; i < binSize; i++){
            if(spectrum[i] > maxVal && spectrum[i] > threshold){
                peaks.Add(new PeakFinder(spectrum[i], i));
                //collect the 5 peaks in the sample of highest amplitude, sort from high->low
                if(peaks.Count > 5){
                    peaks.Sort(new CompareAmp());
                }
            } 
        }

        //calculate pitch
        float freqN = 0f;
        if(peaks.Count > 0){
            maxVal = peaks[0].amplitude;
            int maxNum = peaks[0].index;

            freqN = maxNum;
            //interpolate using neighbors
            if(maxNum > 0 && maxNum < binSize - 1){
                var dL = spectrum[maxNum - 1] / spectrum[maxNum];
                var dR = spectrum[maxNum + 1] / spectrum[maxNum];
                freqN = .5f * (dR * dR - dL * dL);
            }
        }
        pitchVal = freqN * (sampleRate / 2f) / binSize;
        peaks.Clear();
    }
}
*/