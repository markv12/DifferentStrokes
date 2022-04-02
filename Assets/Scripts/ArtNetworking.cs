using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class ArtNetworking : Singleton<ArtNetworking> {
    static bool useLocalhost = false;

    static string localHostUrlBase = "http://localhost:5001/ld50-346003/us-central1/";
    static string publicUrlBase = "https://us-central1-ld50-346003.cloudfunctions.net/";
    static string urlBase = useLocalhost ? localHostUrlBase : publicUrlBase;

    static int FileListPageIndex = 0;

    public void LoadNextPageOfFiles() {
        string url = urlBase + "files/" + FileListPageIndex;

        Action<string> onComplete = (string result) => {
            FileListResponse fileListResponse = FileListResponse.CreateFromJsonString(result);
            // todo use this new data to generate a room etc
        };
        StartCoroutine(Get(url, onComplete));

        FileListPageIndex++;
    }

    public void SendUnfinishedImage(Texture2D imageTex) {
        string imageID = getFileId();
        byte[] pngData = imageTex.EncodeToPNG();
        string url = urlBase + "upload1/" + imageID;
        UnityWebRequest WebRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        UploadHandlerRaw UploadHandler = new UploadHandlerRaw(pngData);
        UploadHandler.contentType = "application/x-www-form-urlencoded";
        WebRequest.uploadHandler = UploadHandler;
        WebRequest.SendWebRequest();
    }

    public void SendFinishedImage(Texture2D imageTex, string imageID) {
        byte[] pngData = imageTex.EncodeToPNG();
        string url = urlBase + "upload2/" + imageID;
        UnityWebRequest WebRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        UploadHandlerRaw UploadHandler = new UploadHandlerRaw(pngData);
        UploadHandler.contentType = "application/x-www-form-urlencoded";
        WebRequest.uploadHandler = UploadHandler;
        WebRequest.SendWebRequest();
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

    public static Texture2D GetRandomTexture() {
        Texture2D result = new Texture2D(64, 64);
        Color[] pixels = result.GetPixels();
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = UnityEngine.Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1);
        }
        result.SetPixels(pixels);
        return result;
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
}
