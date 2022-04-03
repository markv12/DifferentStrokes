using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class ArtNetworking : Singleton<ArtNetworking> {
    private const bool useLocalhost = false;
    private const string localHostUrlBase = "http://localhost:5001/ld50-346003/us-central1/";
    private const string publicUrlBase = "https://us-central1-ld50-346003.cloudfunctions.net/";
    private const string urlBase = useLocalhost ? localHostUrlBase : publicUrlBase;

    private int fileListPageIndex = 0;
    public void LoadNextPageOfFiles(Action<FileListResponse> onGetFiles) {
        string url = urlBase + "files/" + fileListPageIndex;

        Action<string> onComplete = (string result) => {
            FileListResponse fileListResponse = FileListResponse.CreateFromJsonString(result);
            onGetFiles?.Invoke(fileListResponse);
        };
        StartCoroutine(Get(url, onComplete));

        fileListPageIndex++;
    }

    public void loadTopFiles(Action<FileListResponse> onGetFiles) {
        string url = urlBase + "top";

        Action<string> onComplete = (string result) => {
            // todo parse 'em, use 'em
        };
        StartCoroutine(Get(url, onComplete));
    }

    public void SendUnfinishedImage(Texture2D imageTex) {
        string imageID = getFileId();
        byte[] pngData = imageTex.EncodeToPNG();
        string url = urlBase + "uploadv1/" + imageID;
        Debug.Log(url);

        Action<string> onComplete = (string result) => {
            Debug.Log(result);
        };
        StartCoroutine(Post(url, pngData, onComplete));
    }

    public void SendFinishedImage(Texture2D imageTex, string imageID) {
        byte[] pngData = imageTex.EncodeToPNG();
        string url = urlBase + "uploadv2/" + imageID;
        Action<string> onComplete = (string result) => {
            Debug.Log(result);
        };
        StartCoroutine(Post(url, pngData, onComplete));
    }

    public void SendLike(string imageID) {
        string url = urlBase + "like/" + imageID;
        StartCoroutine(Get(url, null));
    }

    public void SendDislike(string imageID) {
        string url = urlBase + "dislike/" + imageID;
        StartCoroutine(Get(url, null));
    }

     string getFileId() {
        System.DateTime myTime = System.DateTime.Now;
        return myTime.Ticks.ToString() + "@" + UnityEngine.Random.value.ToString().Substring(2);
    }

     IEnumerator Get(string uri, Action<string> onComplete) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    onComplete?.Invoke(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    IEnumerator Post(string uri, byte[] pngData, Action<string> onComplete) {
        using (UnityWebRequest webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST)) {
            UploadHandlerRaw UploadHandler = new UploadHandlerRaw(pngData);
            UploadHandler.contentType = "application/x-www-form-urlencoded";
            webRequest.uploadHandler = UploadHandler;
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    onComplete?.Invoke(webRequest.result.ToString());
                    break;
            }
        }
    }
}
