using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ArtManager : MonoBehaviour {

    public PaintingCanvas canvasPrefab;
    private List<PaintingCanvas> step1Canvases;
    private void Awake() {
        ArtNetworking.Instance.LoadNextPageOfFiles((FileListResponse result) => {
            Debug.Log("Got page");
            for (int i = 0; i < result.step1.Length; i++) {
                Debug.Log("Requested Texture: " + result.step1[0].path);
                StartCoroutine(GetTexture(result.step1[i].path, OnGetTexture));
            }
            void OnGetTexture(Texture tex) {
                CreateCanvasForTexture(tex);
            }
        });
    }

    private Vector3 lastCanvasPos = new Vector3(0, 1.9f, 2.38f);
    private void CreateCanvasForTexture(Texture tex) {
        lastCanvasPos += new Vector3(3, 0, 0);
        PaintingCanvas newCanvas = Instantiate(canvasPrefab, lastCanvasPos, Quaternion.identity);
        newCanvas.SetCanvasTexture(tex as Texture2D);
        step1Canvases.Add(newCanvas);
    }

    IEnumerator GetTexture(string url, Action<Texture> onGetTexture) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.LogError(www.error);
        } else {
            onGetTexture?.Invoke(((DownloadHandlerTexture)www.downloadHandler).texture);
        }
    }
}
