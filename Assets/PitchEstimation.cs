using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchEstimation : MonoBehaviour
{
    public int minFreq = 240;
    public int maxFreq = 800;
    //the number of harmonic overtones
    public int overtones = 5;
    //smoothed avg of waveform, the larger it is the less precise it is
    public int movingAvg = 500;
    //voice threshold
    public int threshold = 7;
    const int samples = 1024;
    //number of entities on frequency axis for the summation of residual harmonics
    const int outResolution = 200;

    float[] spctrm = new float[samples];
    float[] specSum = new float[samples];
    float[] specRaw = new float[samples];
    float[] specRes = new float[samples];
    float[] sumResHarms = new float[samples];

    public List<float> SumResHarmonics => new List<float>(sumResHarms);

    //Estimate the fundamental/root frequency

    public float Estimate(AudioSource aSrc){

        var foldingFreq = AudioSettings.outputSampleRate / 2.0f;

        if(!aSrc.isPlaying){
            return float.NaN;
        }
        aSrc.GetSpectrumData(spctrm, 0, FFTWindow.Hanning);

        //calc the Log of the spectrum
        for(int i = 0; i < samples; i++){
            //if the amplitude becomes zero it will become -infinity
            specRaw[i] = Mathf.Log(spctrm[i] + .00000001f);

        }

        //Begin summing the values in the spectrum
        specSum[0] = 0;
        for(int i = 1; i < samples; i++){
            specSum[i] = specSum[i-1] + specRaw[i];
        }

        //find specRes
        var halfRng = Mathf.RoundToInt((movingAvg / 2) / foldingFreq * samples);
        for (int i = 0; i < samples; i++){
            // smooth out the spectrum
            var indexHi = Mathf.Min(i + halfRng, samples - 1);
            var indexLo = Mathf.Max(1 - halfRng + 1, 0);
            var uppr = specSum[indexHi];
            var lowr = specSum[indexLo];
            var smooth = (uppr - lowr) / (indexHi - indexLo);
            //remove smoothed out components from spectrum
            specRes[i] = specRaw[i] - smooth;
        }

        //Calc SRH
        float bestFreq = 0;
        float bestSRH = 0;
        for(int i = 0; i < outResolution; i++){
            var curFreq = (float)i / (outResolution - 1) * (maxFreq - minFreq) + minFreq;
        
            //Calc SRH at curFreq
            var curSRH = GetSpectrumAmplitude(specRes, curFreq, foldingFreq);
            for(int h = 2; h <= overtones; h++){
                //strong signals at h * freq yield better estimates
                curSRH += GetSpectrumAmplitude(specRes, curFreq * h, foldingFreq);
                //strong signals between freq * h  and * h-1 yield poor estimates
                curSRH -= GetSpectrumAmplitude(specRes, curFreq * (h - .5f), foldingFreq);
            }
            sumResHarms[i] = curSRH;

            //find the highest recorded frequency
            if(curSRH > bestSRH){
                bestFreq = curFreq;
                bestSRH = curSRH;
            }
        }

        //if SRH is below threshold, there is no clear root frequency
        if(bestSRH < threshold){
            return float.NaN;
        }
        
        return bestFreq;
    }

    float GetSpectrumAmplitude(float[] spectrum, float frequency, float foldingFreq){
        var pos = frequency / foldingFreq * spectrum.Length;
        var i0 = (int) pos;
        var i1 = i0 + 1;
        var delta = pos - i0;
        return (1 - delta) * spectrum[i0] + delta * spectrum[i1];
    }
}
