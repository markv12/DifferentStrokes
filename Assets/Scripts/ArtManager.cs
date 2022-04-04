using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ArtManager : MonoBehaviour {

    public PaintingCanvas canvasPrefab;
    public static ArtManager instance;
    private readonly List<PaintingCanvas> allCanvases = new List<PaintingCanvas>();

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
        } else {
            LoadNextPage();
        }
        return null;
    }

    private FileData GetNextStep2FileData() {
        if (currentStep2Index < step2FileData.Count) {
            FileData result = step2FileData[currentStep2Index];
            currentStep2Index++;
            return result;
        } else {
            LoadNextPage();
        }
        return null;
    }

    private bool gotToEndOfPages = false;
    private bool isLoadingPage = false;
    private void Awake() {
        instance = this;
        LoadNextPage();
    }

    private void LoadNextPage() {
        if (!isLoadingPage && !gotToEndOfPages) {
            isLoadingPage = true;
            ArtNetworking.Instance.LoadNextPageOfFiles((FileListResponse result) => {
                step1FileData.AddRange(result.step1);
                step2FileData.AddRange(result.step2);
                isLoadingPage = false;
                Initialized = true;
                if(!result.HasImages) {
                    gotToEndOfPages = true;
                }
            });
        }
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
            LoadFromData(null, t, PaintingStatus.Blank);
        }
        for (int i = 0; i < step1Count; i++) {
            Transform t = spawnLocations[index];
            index++;
            FileData data = GetNextStep1FileData();
            LoadFromData(data, t, PaintingStatus.NeedsFixing);
        }
        for (int i = 0; i < step2Count; i++) {
            Transform t = spawnLocations[index];
            index++;
            FileData data = GetNextStep2FileData();
            LoadFromData(data, t, PaintingStatus.Complete);
        }
    }

    public void AddPaintingsForHallOfFame(Transform[] hallOfFameLocations) {
        ArtNetworking.Instance.LoadTopFiles((FileData[] files) => {
            if(files != null) {
                for (int i = 0; i < files.Length; i++) {
                    if(i < hallOfFameLocations.Length) {
                        FileData fileData = files[i];
                        Transform t = hallOfFameLocations[i];
                        LoadFromData(fileData, t, PaintingStatus.HallOfFame);
                    }
                }
            }
        });
    }

    private void LoadFromData(FileData data, Transform parent, PaintingStatus status) {
        if(data == null) {
            CreateCanvasForTexture(null, PaintingStatus.Blank, null, parent);
        } else {
            GetTexture(data.path, (Texture tex) => {
                CreateCanvasForTexture(tex, status, data, parent);
            });
        }
    }

    private void CreateCanvasForTexture(Texture loadedTex, PaintingStatus status, FileData data, Transform parent) {
        PaintingCanvas newCanvas = Instantiate(canvasPrefab, parent);

        newCanvas.transform.localRotation = Quaternion.Euler(0, 180, 0);
        if (loadedTex != null) {
            newCanvas.SetCanvasTexture(loadedTex as Texture2D);
        }
        if (data != null) {
            newCanvas.ImageID = data.id;
        }
        newCanvas.PaintingStatus = (loadedTex == null) ? PaintingStatus.Blank : status;

        if (ShouldLoadOriginalTexture(loadedTex, status, data)) {
            GetTexture(data.originalPath, (Texture originalTex) => {
                newCanvas.SetOriginalTexture(originalTex as Texture2D);
            });
        }

        allCanvases.Add(newCanvas);
    }

    private static bool ShouldLoadOriginalTexture(Texture loadedTex, PaintingStatus status, FileData data) {
        return loadedTex != null
            && (status == PaintingStatus.Complete || status == PaintingStatus.HallOfFame)
            && data != null
            && !string.IsNullOrWhiteSpace(data.originalPath);
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
