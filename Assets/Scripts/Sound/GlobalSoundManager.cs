using UnityEngine;

public class GlobalSoundManager : SingletonBehaviour<GlobalSoundManager> {

	[SerializeField] private AudioSource _audioSourcePrefab;

	public void PlaySoundAt(AudioClip sound, Vector3 position, float volume = 0.5f, float pitch = 1f) {
		// create the audio source
		var source = Instantiate(_audioSourcePrefab, transform);
		source.transform.position = position;
		source.clip = sound;
		source.volume = volume;
		source.pitch = pitch;
		source.loop = false;
		// play
		source.Play();
		// Destroy after
		Destroy(source.gameObject, sound.length);
	}

}