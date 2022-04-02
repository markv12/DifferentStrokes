using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private const string AUDIO_MANAGER_PATH = "AudioManager";
	private static AudioManager instance;
	public static AudioManager Instance
	{
		get
		{
			if (instance == null) {
				GameObject audioManagerObject = (GameObject)Resources.Load(AUDIO_MANAGER_PATH);
				GameObject instantiated = Instantiate(audioManagerObject);
				DontDestroyOnLoad(instantiated);
				instance = instantiated.GetComponent<AudioManager>();
			}
			return instance;
		}
	}

	[Header("Sound Effects")]
	public AudioSource[] audioSources;

	private int audioSourceIndex = 0;

	public AudioClip success;
	public AudioClip startDrawing;
	public AudioClip[] brushes;
	public AudioClip talk;

	//public void PlaySuccessSound(float intensity) {
	//	PlaySFX(success, 1.4f * intensity);
	//}

	public void PlayStartDrawingSound(float intensity) {
		PlaySFX(startDrawing, 1.4f * intensity);
	}

	public void PlayBrushSound(float intensity) {
		AudioClip sound = brushes[Random.Range(0, brushes.Length)];
		PlaySFX(sound, 1.0f * intensity);
	}

	public void PlayTalkSound(float intensity) {
		PlaySFX(talk, 1.0f * Random.Range(0.5f, 1.5f) * intensity, Random.Range(0.5f, 1.5f));
	}

	public void PlaySFX(AudioClip clip, float volume)
	{
		AudioSource source = GetNextAudioSource();
		source.volume = volume;
		source.PlayOneShot(clip);
	}
	public void PlaySFX(AudioClip clip, float volume, float pitch) {
		AudioSource source = GetNextAudioSource();
		source.volume = volume;
		source.PlayOneShot(clip);
		source.pitch = pitch;
	}

	private AudioSource GetNextAudioSource()
	{
		AudioSource result = audioSources[audioSourceIndex];
		audioSourceIndex = (audioSourceIndex + 1) % audioSources.Length;
		return result;
	}
}
