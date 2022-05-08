using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> noteObjects = new List<GameObject>();

    public bool isRandomized;

    public Vector3 notePos;

    // Start is called before the first frame update
    void Start()
    {
        SpawnAllNoteObj();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnAllNoteObj(){
        if(noteObjects.Count > 0){
            for(int index = 0; index < noteObjects.Count; index++){
                //GetNotePos(index);
                Instantiate(noteObjects[index], transform.position, Quaternion.identity);
            }
        }
    }

    public void SpawnNoteObjRnd(){
        

        int index = isRandomized ? Random.Range(0, noteObjects.Count) : 0;
        if(noteObjects.Count > 0){
            //GetNotePos(index);
            Instantiate(noteObjects[index], transform.position, Quaternion.identity);
        }

        
    }

    void GetNotePos(int index){
        index++;
        if(index == 1 || index == 2){
            //C4/C#4
            notePos.Set(-1.6f,-.382f,0f);
        }
        else if(index == 3 || index == 4){
            //D4/D#4
            notePos.Set(-1.235f,-.248f,0f);
        }
        else if(index == 5){
            //E4
            notePos.Set(-.87f,-.125f,0f);
        }
        else if(index == 6 || index == 7){
            //F4/F#4
            notePos.Set(-.505f,-.02f,0f);
        }
        else if(index == 8 || index == 9){
            //G4/G#4
            notePos.Set(-.14f,.1f,0f);
        }
        else if(index == 10 || index == 11){
            //A4/A#4
            notePos.Set(.216f,.205f,0f);
        }
        else if(index == 12){
            //B4
            notePos.Set(.581f,-.235f,0f);
        }
        else if(index == 13 || index == 14){
            //C5/C#5
            notePos.Set(.946f,-.125f,0f);
        }
        else if(index == 15 || index == 16){
            //D5/D#5
            notePos.Set(1.311f,-.015f,0f);
        }
        else if(index == 17){
            //E5
            notePos.Set(1.676f,0.09f,0f);
        }
        else if(index == 18 || index == 19){
            //F5/F#5
            notePos.Set(2.041f,.19f,0f);
        }
        else if(index == 20){
            //G5
            notePos.Set(2.406f,0.31f,0f);
        }
    }
}
