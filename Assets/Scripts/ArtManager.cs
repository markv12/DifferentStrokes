using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ArtManager : MonoBehaviour {

    public PaintingCanvas canvasPrefab;
    public static ArtManager instance;
    private readonly List<PaintingCanvas> step1Canvases = new List<PaintingCanvas>();

    private readonly List<FileData> step1FileData = new List<FileData>();
    private int currentStep1Index = 0;
    private readonly List<FileData> step2FileData = new List<FileData>();
    private int currentStep2Index = 0;

    public bool Initialized { get; private set; }

    private FileData GetNextStep1FileData() {
        if(currentStep1Index < step1FileData.Count) {
            FileData result = step1FileData[currentStep1Index];
            currentStep1Index++;
            return result;
        }
        return null;
    }

    private FileData GetNextStep2FileData() {
        if (currentStep2Index < step2FileData.Count) {
            FileData result = step2FileData[currentStep2Index];
            currentStep2Index++;
            return result;
        }
        return null;
    }

    private void Awake() {
        instance = this;
        ArtNetworking.Instance.LoadNextPageOfFiles((FileListResponse result) => {
            step1FileData.AddRange(result.step1);
            step2FileData.AddRange(result.step2);
            Initialized = true;
        });
    }

    public void AddPaintingsForChunk(Transform[] spawnLocations) {
        int spawnLocationCount = spawnLocations.Length;
        int blankCount = spawnLocationCount / 3;
        int step1Count = blankCount;
        int step2Count = spawnLocationCount - (blankCount + step1Count);

        int index = 0;
        for (int i = 0; i < blankCount; i++) {
            Transform t = spawnLocations[index];
            index++;
            LoadFromData(null, t, true);
        }
        for (int i = 0; i < step1Count; i++) {
            Transform t = spawnLocations[index];
            index++;
            FileData data = GetNextStep1FileData();
            LoadFromData(data, t, true);
        }
        for (int i = 0; i < step2Count; i++) {
            Transform t = spawnLocations[index];
            index++;
            FileData data = GetNextStep2FileData();
            LoadFromData(data, t, false);
        }
    }

    private void LoadFromData(FileData data, Transform parent, bool step1) {
        if(data == null) {
            CreateCanvasForTexture(null, false, null, parent);
        } else {
            GetTexture(data.path, (Texture tex) => {
                CreateCanvasForTexture(tex, step1, data.id, parent);
            });
        }
    }

    private void CreateCanvasForTexture(Texture tex, bool step1, string imageID, Transform parent) {
        PaintingCanvas newCanvas = Instantiate(canvasPrefab, parent);
        newCanvas.transform.localPosition = new Vector3(-0.67f, -2.234f, 0.562f);
        newCanvas.transform.localRotation = Quaternion.Euler(0, 180, 0);
        if(tex != null) {
            newCanvas.SetCanvasTexture(tex as Texture2D);
        }
        newCanvas.ImageID = imageID;
        newCanvas.PaintingStatus = (tex == null) ? PaintingStatus.Blank : (step1 ? PaintingStatus.NeedsFixing : PaintingStatus.Complete);

        step1Canvases.Add(newCanvas);
    }

    void GetTexture(string url, Action<Texture> onGetTexture) {
        StartCoroutine(Get(url, onGetTexture));

        IEnumerator Get(string url, Action<Texture> onGetTexture) {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError(www.error);
            } else {
                onGetTexture?.Invoke(((DownloadHandlerTexture)www.downloadHandler).texture);
            }
        }
    }
}
