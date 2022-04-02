using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ArtManager : MonoBehaviour {

    public PaintingCanvas canvasPrefab;
    private List<PaintingCanvas> step1Canvases = new List<PaintingCanvas>();
    private void Awake() {
        ArtNetworking.Instance.LoadNextPageOfFiles((FileListResponse result) => {
            for (int i = 0; i < result.step1.Length; i++) {
                Debug.Log("Got Step1 Textures: " + result.step1.Length);
                FileData fileData = result.step1[i];
                StartCoroutine(GetTexture(fileData.path, fileData.id, OnGetStep1Texture));
            }
            for (int i = 0; i < result.step2.Length; i++) {
                FileData fileData = result.step2[i];
                StartCoroutine(GetTexture(fileData.path, fileData.id, OnGetStep2Texture));
            }
            void OnGetStep1Texture(Texture tex, string imageID) {
                CreateCanvasForTexture(tex, true, imageID);
            }
            void OnGetStep2Texture(Texture tex, string imageID) {
                CreateCanvasForTexture(tex, false, imageID);
            }
        });
    }

    private Vector3 lastCanvasPos = new Vector3(0, 1.9f, 2.38f);
    private void CreateCanvasForTexture(Texture tex, bool step1, string imageID) {
        lastCanvasPos += new Vector3(4, 0, 0);
        PaintingCanvas newCanvas = Instantiate(canvasPrefab, lastCanvasPos, Quaternion.identity);
        newCanvas.SetCanvasTexture(tex as Texture2D);
        newCanvas.ImageID = imageID;
        newCanvas.PaintingStatus = step1 ? PaintingStatus.NeedsFixing : PaintingStatus.Complete;

        step1Canvases.Add(newCanvas);
    }

    IEnumerator GetTexture(string url, string textureID, Action<Texture, string> onGetTexture) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.LogError(www.error);
        } else {
            onGetTexture?.Invoke(((DownloadHandlerTexture)www.downloadHandler).texture, textureID);
        }
    }
}
