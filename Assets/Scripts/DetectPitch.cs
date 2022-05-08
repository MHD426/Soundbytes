using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

using System;

[RequireComponent(typeof(AudioSource))]
public class DetectPitch : MonoBehaviour
{

    struct FreqBand{
        public float Val;
        public float Index;
        public float Freq;

        public FreqBand(float val, float index, float freq){
            this.Val = val;
            this.Index = index;
            this.Freq = freq;
        }
    }

    //Microphone input
    TMPro.TMP_Dropdown displayInputDevices;
    List<string> inDevices = new List<string>();
    string curDevice;
    string[] micDevices;
    bool enableMic;
    AudioSource aSource;
    AudioClip aClip;
    public float upperThresh = 25.0f;
    public float lowerThresh = 0.05f;

    //Sample Data Storage
    public static float[] samples = new float[2048];
    public static float[] samplesdB = new float[2048];
    public static float[] samplesVal = new float[2048];
    public static float[,] samplesValStored = new float[512,3];
    float[] samplesMaxVal = new float[3];
    float[] samplesFreq = new float[10];
    FreqBand[] savedSamples = new FreqBand[10];

    //UI display
    Text pitch;
    Text avgPitch;
    Text note;
    Text harmonicNote;
    String noteStr = "Note display";

    float saveddB = 0.0f;
    float savedVal = 0.0f;
    float fndmtlFreq = 0.0f;
    float bandFreq;

    private float rmsVal;
    private float dbVal;
    private float refVal;

    // Start is called before the first frame update
    void Start()
    {
        refVal = 0.001f;
        aSource = GetComponent<AudioSource>();
        bandFreq = 24000.0f / samples.Length;
        aSource = GetComponent<AudioSource>();
        aClip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
        aSource.clip = aClip;
        aSource.loop = true;
        //wait until microphone is recording
        while (!(Microphone.GetPosition(null) > 0)){}
        aSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumDataFromAudioSource();
        CalcAvgPitch();
        GetNameofNote();
        //DebugAudioFile();
        Debug.Log(noteStr);
    }

    /*
    public void StartRecording(){
        if (Microphone.devices.Length > 0){
            micDevices = Microphone.devices;
            aSource.outputAudioMixerGroup = mxrGrpMic;
            aSource.clip = Microphone.Start(micDevices[0], false, 60, AudioSettings.outputSampleRate);
        }

        if(Microphone.IsRecording(micDevices[0])){
            while(!(Microphone.GetPosition(micDevices[0]) > 0)){}
            aSource.Play();
        }

        for(int i = 0; i < micDevices.Length; i++){
            inDevices.Add(micDevices[i]);
        }
    }*/

    void GetSpectrumDataFromAudioSource(){
        aSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
    }

    private bool DetectValue(float val){
        int i = 0;
        float maxVal = 0.0f;

        for(i = 0; i < (samples.Length / 4); i ++){
            //Find highest value
            if(samples[i] > maxVal){
                maxVal = samples[i];
            }
        }

        //set a lower threshold
        if(maxVal < 0.00001){
            maxVal = 0;
        }
        if(maxVal > val){
            if(samplesMaxVal[0] > maxVal && samplesMaxVal[1] > maxVal && samplesMaxVal[2] > maxVal){
                for(int j = 0; j < 512; j++){
                    samples[j] = samplesValStored[j,0];
                }

                float[] samplesMaxVal = new float[3];

                return true;
            }
            else{
                for(int k = 0; k < samplesMaxVal.Length; k++){
                    if(samplesMaxVal[k] < maxVal){
                        samplesMaxVal[k] = maxVal;
                        for(i = 0; i < (samples.Length / 4); i++){
                            samplesValStored[i,k] = samples[i];
                        }
                        break;
                    }
                }
                savedVal = maxVal;
                return false;
            }
        }
        return false;
    }

    private bool DetectValOpening(float val){
        int i = 0;
        float maxVal = 0.0f;

        for(i = 0; i < (samples.Length / 4); i ++){
            //Find highest value
            if(samples[i] > maxVal){
                maxVal = samples[i];
            }
        }

        //set a lower threshold
        if(maxVal < 0.00001){
            maxVal = 0;
        }
        //Look for the opening, otherwise return false
        if(maxVal > val){
            if(savedVal > maxVal){
                savedVal = 0.0f;
                return true;
            }
            else{
                savedVal = maxVal;
                return false;
            }
        }
        return false;
    }

    private bool DetectValClosing(float val){
        int i = 0;
        float maxVal = 0.0f;

        for(i = 0; i < (samples.Length / 4); i++){
            //Find highest value
            if(samples[i] > maxVal){
                maxVal = samples[i];
            }
        }

        //set a lower threshold
        if(maxVal < 0.00001){
            maxVal = 0;
        }
        //Look for the closing, otherwise return false
        if(maxVal > val){
            if(savedVal > maxVal){
                savedVal = 0.0f;
                return true;
            }
            else{
                savedVal = maxVal;
                return false;
            }
        }
        return false;
    }

    private bool DetectdBsOpening(float openDbs){
        aSource.GetOutputData(samplesdB, 0);
        int i = 0;
        float sum = 0.0f;
        for(i = 0; i < samplesdB.Length; i++){
            //set the sum equal to the sum of all squared samples
            sum += samplesdB[i] * samplesdB[i];
        }
        //calculate rms and decibels
        rmsVal = Mathf.Sqrt(sum / samplesdB.Length);
        dbVal = 20 * Mathf.Log10(rmsVal / refVal);

        //set min threshold for decibels
        if(dbVal < -160){
            dbVal = -160;
        }

        //look for the opening, otherwise return false
        if(dbVal > openDbs){
            if(saveddB > dbVal){
                saveddB = 0;
                return true;
            }
            else{
                saveddB = dbVal;
                return false;
            }
        }
        return false;
    }

    private bool DetectdBsClosing(float closeDbs){
        aSource.GetOutputData(samplesdB, 0);
        int i = 0;
        float sum = 0.0f;
        for(i = 0; i < samplesdB.Length; i++){
            //set the sum equal to the sum of all squared samples
            sum += samplesdB[i] * samplesdB[i];
        }
        //calculate rms and decibels
        rmsVal = Mathf.Sqrt(sum / samplesdB.Length);
        dbVal = 20 * Mathf.Log10(rmsVal / refVal);

        //set min threshold for decibels
        if(dbVal < -160){
            dbVal = -160;
        }

        //look for the closeing, otherwise return false
        if(dbVal < closeDbs){
            if(saveddB > dbVal){
                saveddB = 0;
                //NoteInteractionHandler.noteID = -1;
                return true;
            }
            else{
                saveddB = dbVal;
                return false;
            }
        }
        return false;
    }

    void AddToArray(FreqBand[] sampleArr, int size, FreqBand sample){
        List<FreqBand> save = new List<FreqBand>();

        for(int i = 0; i < sampleArr.Length; i ++){
            //check if new value is higher than existing
            if(sample.Val < sampleArr[i].Val){
                //save existing val from list
                save.Add(sampleArr[i]);
            }
            else{
                //save the new value
                save.Add(sample);
                //save the rest of the list if it isn't empty
                for(int j = 0; j < size; j++){
                    if(sampleArr[j].Val > 0.0f){
                        save.Add(sampleArr[j]);
                    }
                    else{
                        break;
                    }   
                }
                break;
            }
        }
        int counter = 0;
         foreach(FreqBand ret in save){
             if(counter < size){
                 sampleArr[counter] = ret;
                 counter++;
             }
             else{
                 break;
             }
         }
    }

    private void CalcAvgPitch(){
        DetectdBsClosing(upperThresh);
        if(DetectValue(lowerThresh)){
            //Check all samples
            for(int i = 0; i < (samples.Length / 4); i ++){
                //hold sample value
                float tempVal = samples[i];

                //high-pass filter
                if(tempVal > 0.001f){
                    //Iterate through saved vals
                    float tempFreq = i * bandFreq;
                    FreqBand tempFB = new FreqBand(tempVal, i, tempFreq);

                    AddToArray(savedSamples, savedSamples.Length, tempFB);
                }
            }

            if(savedSamples[0].Freq > 246.94f){ //using 250 as tuner range starts at C4/261.63Hz
                FreqBand tempFB2 = new FreqBand(0.0f, 0.0f, 0.0f);

                //Delete freq samples whose band index is adjacent based on lowest value
                for(int i = 0; i < savedSamples.Length; i++){
                    tempFB2 = savedSamples[i];
                    for(int j = 0; j < savedSamples.Length; j++){
                        //check that not comparing the freq with itself
                        if(i != j){
                            //Look for adjacent freq bands
                            if(tempFB2.Index + 1 == savedSamples[j].Index || tempFB2.Index - 1 == savedSamples[j].Index){
                                //delete lower value band
                                if(tempFB2.Val > savedSamples[j].Val){
                                    savedSamples[j] = new FreqBand(0.0f, 0.0f, 0.0f);
                                }
                            }
                        }
                    }
                    savedSamples[i] = tempFB2;
                }

                //save freqs in list for higher accuracy
                for(int i = 0; i < savedSamples.Length; i++){
                    samplesFreq[i] = savedSamples[i].Freq;
                }

                fndmtlFreq = 1046.5f; //using 1000Hz as it is just above target tuner range
                FreqBand fndmtlSample = new FreqBand(0.0f,0.0f,0.0f);
                for(int i = 0; i < samplesFreq.Length; i++){
                    //find and save lowest detected freq
                    if(samplesFreq[i] > 0.0f && samplesFreq[i] < fndmtlFreq){
                        fndmtlFreq = samplesFreq[i];
                    }
                }
            }
        }
        else{
            savedSamples = new FreqBand[10];
        }
    }

    IEnumerator OpenMicDelay(float time){
        yield return new WaitForSeconds(time);
    }

    private void GetNameofNote(){
        //freqs in target tuner range, C4 - G5
        if(fndmtlFreq > 250.0f){
            noteStr = null;
            //assign noteStr the musical note based on freq from C4 to B5
            for(int i = 0; i < 24; i++){
                float testPitchIter = Mathf.Pow(2.0f, i / 12.0f) * 261.63f;
            

                //get a variable tolerance[], where tolerance[0] is the difference between pitch -1 and pitch and [1] is +1 and pitch
                float[] tolerance = GetTolerance(i);

                //identify and name lowest freq
                if(fndmtlFreq > testPitchIter - tolerance[0]){
                    if(fndmtlFreq < testPitchIter + tolerance[1]){
                        //check the harmonic
                        for(int j = 0; j < samplesFreq.Length; j++){
                            if(samplesFreq[j] > 0.0f){
                                noteStr = DetectHarmonic(samplesFreq[j], testPitchIter, tolerance, 2.0f);
                                if(noteStr != null){
                                    //NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                noteStr = DetectHarmonic(samplesFreq[j], testPitchIter, tolerance, 3.0f);
                                if(noteStr != null){
                                    //NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                noteStr = DetectHarmonic(samplesFreq[j], testPitchIter, tolerance, 4.0f);
                                if(noteStr != null){
                                    //NotesInteractionHandler.noteID = i;
                                    break;
                                }
                            }
                        }
                    }
                }
                if(noteStr != null){
                    break;
                }
            }
        }
        noteStr = null;
        fndmtlFreq = 0.0f;
    }

    private float[] GetTolerance(int index){
        float[] ret = { 0.0f, 0.0f };
        float testPitchIter = Mathf.Pow(2.0f, index / 12.0f) * 261.63f;
        float testNoteIterBelow = Mathf.Pow(2.0f, (index - 1) / 12.0f) * 261.63f;
        float testNoteIterAbove = Mathf.Pow(2.0f, (index + 1) / 12.0f) * 261.63f;
        ret[0] = testPitchIter - testNoteIterBelow; //Minus Tolerance
        ret[1] = testNoteIterAbove - testPitchIter; //Plus Tolerance

        return ret;
    }

    private String DetectHarmonic(float sample, float testNoteIter, float[] tolerance, float harmNum){
        float harmPitch = testNoteIter * harmNum;
        if(sample > harmPitch - tolerance[0]){
            if(sample < harmPitch + tolerance[1]){
                return FindPitchName(testNoteIter);
            }
        }
        return null;
    }

    private String FindPitchName(float pitch){
        String[] notes = new string[24] {"C4","C#4","D4","D#4","E4","F4","F#4","G4","G#4","A4","A#4","B4","C5","C#5","D5","D#5","E5","F5","F#5","G5","G#5","A5","A#5","B5"};
        for(int i = 0; i < 24; i ++){
            float testNoteIter = Mathf.Pow(2.0f, i / 12.0f) * 261.63f;
            float[] tolerance = GetTolerance(i);
            if(pitch > testNoteIter - tolerance[0]){
                if(pitch < testNoteIter + tolerance[1]){
                    Debug.Log(pitch);
                    return notes[i];
                }
            }
        }
        return null;
    }

    private void DebugAudioFile()
    {
        Debug.Log(samples.ToString());
        //float samplespersecond = aClip.samples / aClip.length;
        //Debug.Log(samplespersecond.ToString());
        Debug.Log(aClip.samples.ToString());
    }

    public void DropDownValueChanged(int value)
    {
        Microphone.End(curDevice);
        aSource.Stop();
        aSource.clip = null;


        aSource.clip = Microphone.Start(micDevices[value], true, 600, AudioSettings.outputSampleRate);
        curDevice = micDevices[value];
        aSource.Play();
    }
}
