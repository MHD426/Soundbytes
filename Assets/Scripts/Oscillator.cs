using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    //produced tone
    public double frequency = 440.0;
    //distance wave moves each frame, determined by freq.
    private double increment;
    //Phase is location on the wave
    private double phase;
    //Unity runs a 48kHz by default
    private double sampl_Freq = 48000.0;

    // gain is the 'power'/amplitude of the oscillator
    public float gain;
    //becareful with input volume, protect your ears
    public float volume = 0.1f;

    public float[] frequencies;
    public int thisFreq;
    private bool ascend = true;

    void Start(){
        frequencies = new float[8];
        frequencies[0] = 440;
        frequencies[1] = 494;
        frequencies[2] = 554;
        frequencies[3] = 587;
        frequencies[4] = 659;
        frequencies[5] = 740;
        frequencies[6] = 831;
        frequencies[7] = 880;

    }

    void Update(){

        if (Input.GetKeyDown(KeyCode.Space)){
            gain = volume;

            //set frequency to change on key press
            frequency = frequencies[thisFreq];
            
            
            if(ascend == true){
                thisFreq++;
                if(thisFreq == frequencies.Length-1){
                    ascend = false;
                }
            }
            else{
                thisFreq--;
                if(thisFreq == 0){
                    ascend = true;
                }
            }
            
        }
        if (Input.GetKeyUp(KeyCode.Space)){
            gain = 0;
        }
    }

    void OnAudioFilterRead(float[] bands, int channels){
        //movement on x-axis from waveform
        increment = frequency * 2 * Mathf.PI / sampl_Freq;

        //iterate through bands[] to determine the y-axis
        for(int i = 0; i < bands.Length; i += channels){
            phase += increment;
            //set waveform data
            bands[i] = (float) (gain * Mathf.Sin((float) phase)); 
            //copy data to other channel for both speakers
            if(channels == 2){
                bands[i+1] = bands[i];
            }
            //reset y-axis on waveform if revolution is done
            if (phase > (Mathf.PI * 2)){
                phase = 0.0;
            }
        }
    }

}
