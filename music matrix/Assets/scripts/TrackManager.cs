using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrackManager : MonoBehaviour {
    float waitForSeconds = 0.25f;
    int octaveOffsetMultiplier = 3;
    int octaveOffset = 12;
    int timingCount = TileManager.gridWidth-1;
    [SerializeField]
    private GameObject readHeadMapObject;
    private Tilemap readHeadMap;
    [SerializeField]
    private TileBase baseTile;
    [SerializeField]
    private GameObject tileMapObject;
    private Tilemap tileMap;

    public NoteManager.Notes key;
    private List<NoteManager.Notes> scale = new List<NoteManager.Notes>();
    private static int[] majorScale = new int[7] {0, 2, 4, 5, 7, 9, 11};

    void Start () {
        generateScale();
        readHeadMap = readHeadMapObject.GetComponent<Tilemap>();
        tileMap = tileMapObject.GetComponent<Tilemap>();
        StartCoroutine(Loop());
    }

    void generateScale()
    {

        int scaleOffset = ((int)key);
        for (int i = 0; i < Mathf.RoundToInt(TileManager.gridHeight/7); i++) {
            foreach (int offset in majorScale)
                {

                //print("adding to list: " + (NoteManager.Notes)(offset + scaleOffset + (octaveOffset *(i+1))));
                //remove the + X from the end of the next line, its just to make the noise not so audible.
                scale.Add((NoteManager.Notes)(offset + scaleOffset + (octaveOffset * (i + 3))));
                }
            }
    }


    void Update () {
        updateReadHead();
	}

    void updateReadHead() {
        readHeadMap.ClearAllTiles();
        Vector3Int readHeadPos = new Vector3Int(timingCount, -1, 0);
        readHeadMap.SetTile(readHeadPos, baseTile);
    }

    void selectedTilesAtTiming(int timing) {
        Vector3Int tilePos;
        for (int i = 0; i < TileManager.gridHeight; i++){
            tilePos = new Vector3Int(timing, i, 0);
            if (!tileMap.HasTile(tilePos)) {
                
                NoteManager.Notes note = getScaleNoteFromInt(i);
              
                print(note);
                contactSC(note);
            }
        }
    }

    NoteManager.Notes getNoteFromInt(int y) {
        y += 1;
        return((NoteManager.Notes)y);
    }

    NoteManager.Notes getScaleNoteFromInt(int y){
        return (scale[y]);
    }

    IEnumerator Loop(){
        while (true) {
            
            timingCount += 1;
            timingCount %= TileManager.gridWidth;
            
            selectedTilesAtTiming(timingCount);
           
            yield return new WaitForSeconds(waitForSeconds);

        }
        
    }

    public static void contactSC(NoteManager.Notes note)
    {
        //OSC Send
        List<string> args = new List<string>();
        args.Add("0.3f");
        args.Add(NoteManager.noteToFreq[note].ToString());
        args.Add("0.3f");
        OSCHandler.Instance.SendMessageToClient("SuperCollider", "/play" + "VoiceA", args);
      

    }


}
