using UnityEngine;

public static class ArtNetworking {
    public static void SendUnfinishedImage(Texture2D imageTex) {
        byte[] pngData = imageTex.EncodeToPNG();

        //Send Image
    }

    public static void SendFinishedImage(Texture2D imageTex) {
        byte[] pngData = imageTex.EncodeToPNG();

        //Send Image
    }

    public static void SendLike(string imageID) {

    }

    public static void SendDislike(string imageID) {

    }
}
