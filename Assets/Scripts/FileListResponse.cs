using System;
using Newtonsoft.Json;

[Serializable]
public class FileListResponse {
    public FileData[] step1;
    public FileData[] step2;

    public bool HasImages => (step1 != null && step1.Length > 0) || (step2 != null && step2.Length > 0);

    public static FileListResponse CreateFromJsonString(string json) {
        return JsonConvert.DeserializeObject<FileListResponse>(json);
    }
}

[Serializable]
public class FileData {
    public string id;
    public int iteration;
    public long date;
    public int likes;
    public int dislikes;
    public string path;
    public string originalPath;
}
