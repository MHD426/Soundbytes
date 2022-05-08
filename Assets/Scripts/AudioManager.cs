using UnityEngine.Audio;
using System;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource aSource;

    public static float[] samples = new float[512];
    int sampleRate;



    void Start(){
        sampleRate = AudioSettings.outputSampleRate;
        //Debug.Log(sampleRate);
        aSource = GetComponent<AudioSource>();
        aSource.clip = Microphone.Start(null, true, 10, sampleRate);
        aSource.loop = true;
        //wait until microphone is recording
        while (!(Microphone.GetPosition(null) > 0)){}
        aSource.Play();
        //Debug.Log(FindBuckets());
    }

    void Update(){
        //GetSpectrumDataFromAudioSource();
        

    }

    void GetSpectrumDataFromAudioSource(){
        aSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
    }


    /*
    string FindBuckets(){
        string fvals = "";
        double[] freqs = new double[20] {261.63, 277.18,293.66,311.13,329.63,349.23,369.99,392,415.30,440,466.16,493.88 ,523.25,554.37,587.33,622.25,659.25,698.46,739.99,783.99};
        for(int i = 0; i < freqs.Length; i++){
            double tmp = freqs[i] * .34133333333;
            fvals += tmp.ToString();
            fvals += ", ";
        }
        return fvals;
    }
    
    1/48000Hz (sampleRate) * 8192 sample buckets * 2 = .341333333s. Multiply by desired freq to find bucket
    C4 - 261.63Hz - bucket(s) 89 - 90    
    C#4 - 277.18Hz - bucket(s) 94-95
    D4 - 293.66Hz - bucket(s)  100-101
    D#4 - 311.13Hz - bucket(s) 106-107
    E4 - 329.63Hz - bucket(s) 112-113
    F4 - 349.23Hz - bucket(s) 119-120
    F#4 - 369.99Hz - bucket(s)  126-127
    G4 - 392Hz - bucket(s) 133-134
    G#4 - 415.3Hz - bucket(s)  141-142
    A4 - 440Hz - bucket(s) 150-151
    A#4 - 466.16Hz - bucket(s) 159-160  
    B4 - 493.88Hz - bucket(s) 168-169 
    C5 - 523.25Hz - bucket(s) 178-179
    C#5 - 554.37Hz - bucket(s) 189-190 
    D5 - 587.33Hz - bucket(s) 200-201 
    D#5 - 622.25Hz - bucket(s) 212-213
    E5 - 659.25Hz - bucket(s)  225 
    F5 - 698.46Hz - bucket(s) 238-239
    F#5 - 739.99Hz - bucket(s) 252-253
    G5 - 783.99Hz - bucket(s) 267-268
    Need buckets 86 - 270 -> about 184 buckets
    */
}
