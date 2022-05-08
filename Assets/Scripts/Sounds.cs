using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[System.Serializable]
public class Sounds : MonoBehaviour
{
    public string name;
    
    public AudioSource nSource;
    public AudioClip clip;
    public AudioClip[] nClips = new AudioClip[12];
    public Button[] buttons = new Button[12];
    public Text detailsH, detailsB;

    [Range(0f,1f)]
    public float volume;

    [Range(0.1f,5f)]
    public float pitch;
    
    void Start(){
        nSource.GetComponent<AudioSource>();
    }


    public void PlayNotesAscending(){
        detailsH.text = "Play";
        detailsB.text = "Plays all notes on staff in ascending order. Sing along!";
        StartCoroutine(PlayAudioAfterDelay(nSource, 1f));
        
    }
    IEnumerator PlayAudioAfterDelay(AudioSource aSrc, float delay){
        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[0]);

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[1]);

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[2]);

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[3]);

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[4]);

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[5]);

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[6]);

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[7]);
        /*
        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[8]);

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[9]);

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[10]);

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[11]);
        */
    }

    public void StartQuiz(){
        detailsH.text = "Quiz";
        detailsB.text = "Plays a random note, then press the correct note on the staff.";
        int rand;
        
        rand = Random.Range(0, 11);
        StartCoroutine(PlayAudioQuiz(nSource, 1f, rand));
        
    }
    IEnumerator PlayAudioQuiz(AudioSource aSrc, float delay, int rand){
        var waitForButton = new WaitForUIButtons(buttons);
        

        yield return new WaitForSeconds(delay);
        aSrc.PlayOneShot(nClips[rand]);
        yield return waitForButton.Reset();

        switch(rand){
            case 0:
                if(waitForButton.PressedButton == buttons[0]){
                    detailsB.text = "Correct! This note is C.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was C.";
                    break;
                }
            case 1:
                if(waitForButton.PressedButton == buttons[1]){
                    detailsB.text = "Correct! This note is D.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was D.";
                    break;
                }
            case 2:
                if(waitForButton.PressedButton == buttons[2]){
                    detailsB.text = "Correct! This note is E.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was E.";
                    break;
                }
            case 3:
                if(waitForButton.PressedButton == buttons[3]){
                    detailsB.text = "Correct! This note is F.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was F.";
                    break;
                }
            case 4:
                if(waitForButton.PressedButton == buttons[4]){
                    detailsB.text = "Correct! This note is G.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was G.";
                    break;
                }
            case 5:
                if(waitForButton.PressedButton == buttons[5]){
                    detailsB.text = "Correct! This note is A.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was A.";
                    break;
                }
            case 6:
                if(waitForButton.PressedButton == buttons[6]){
                    detailsB.text = "Correct! This note is B.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was B.";
                    break;
                }
            case 7:
                if(waitForButton.PressedButton == buttons[7]){
                    detailsB.text = "Correct! This note is C.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was C.";
                    break;
                }
            case 8:
                if(waitForButton.PressedButton == buttons[8]){
                    detailsB.text = "Correct! This note is D.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was D.";
                    break;
                }
            case 9:
                if(waitForButton.PressedButton == buttons[9]){
                    detailsB.text = "Correct! This note is E.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was E.";
                    break;
                }
            case 10:
                if(waitForButton.PressedButton == buttons[10]){
                    detailsB.text = "Correct! This note is F.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was F.";
                    break;
                }
            case 11:
                if(waitForButton.PressedButton == buttons[11]){
                    detailsB.text = "Correct! This note is G.";
                    break;
                }
                else{
                    detailsB.text = "Incorrect. The note was G.";
                    break;
                } 
            
        }

    }

}
