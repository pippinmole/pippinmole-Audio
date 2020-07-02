using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace pippinmole.Audio {
    [CreateAssetMenu(fileName = "Audio Manager", menuName = "Game/Managers/Audio Manager", order = 0)]
    public class AudioManager : ScriptableObject {

        private static AudioManager instance;
        public static AudioManager Instance => instance = instance == null ? Resources.Load<AudioManager>("Audio Manager") : instance;

        public AudioMixer MainAudioMixer;

        public enum EAudioSourceTypeEdit {
            Master,
            Music,
            UI,
            Game
        }

        public void SetAudioLevel(EAudioSourceTypeEdit sourceTypeEdit, float value) {
            var fromSource = 0f;
            var toSource = 100f;
            var fromTarget = -20f;
            var toTarget = 20f;

            float volume = (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;

            this.MainAudioMixer.SetFloat($"{sourceTypeEdit}", volume);
        }

        /// <summary>
        /// Plays a sound clip of given reference.
        /// </summary>
        /// <param name="clip">The audio clip you want to play</param>
        /// <param name="sourceType">This is the mixer group it will be put into.</param>
        /// <param name="worldPosition">This allows the definition to be used in the 3d scene with audio rolloff.</param>
        /// <param name="destroyOnFinish">If you want this object to be disposed of straight after the clip is played.</param>
        /// <returns></returns>
        public AudioSource PlaySound(AudioClip clip, EAudioType sourceType, Vector3 worldPosition = default, bool destroyOnFinish = true) {
            if (clip == null) return null;

            var source = this.CreateAudioSource(sourceType, worldPosition);

            source.clip = clip;
            source.Play();

            if (destroyOnFinish) {
                Object.Destroy(source.gameObject, source.clip.length);
            }

            return source;
        }

        public AudioSource PlaySound(SoundDefinition sound, EAudioType sourceType, Vector3 worldPosition = default) {
            return this.PlaySound(sound, sourceType, null, worldPosition);
        }

        /// <summary>
        /// Plays a sound definition of given reference.
        /// </summary>
        /// <param name="sound">This is the reference to the sound definition that is going to be played.</param>
        /// <param name="sourceType">This is the mixer group it will be put into.</param>
        /// <param name="worldPosition">This allows the definition to be used in the 3d scene with audio rolloff.</param>
        /// <returns></returns>
        public AudioSource PlaySound(SoundDefinition sound, EAudioType sourceType, Transform parent = null, Vector3 worldPosition = default) {
            if (sound == null) {
                Debug.LogWarning($"The Sound Definition passed in as parameter is null!");
                return null;
            }
            if (sound.Clip == null && !sound.RandomClip) {
                Debug.LogWarning($"The clip from {sound.Name} is missing!");
                return null;
            }

            if (sound.RandomPitch) {
                sound.Pitch = Random.Range(sound.MinPitch, sound.MaxPitch);
            }

            var audioSource = this.CreateAudioSource(sourceType, worldPosition);
            audioSource.transform.SetParent(parent);

            audioSource.name = sound.Name + " Audio Source";
            audioSource.playOnAwake = false;

            audioSource.clip = sound.RandomClip ? sound.RandomClips[(int)Random.Range(0, sound.RandomClips.Count)] : sound.Clip;

            audioSource.volume = sound.Volume;
            audioSource.pitch = sound.Pitch;
            audioSource.loop = sound.Loop;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.maxDistance = sound.Range;
            audioSource.minDistance = Mathf.Min(0f, sound.Range);

            if (sourceType == EAudioType.UI)
                audioSource.spatialBlend = 0f;
            else
                audioSource.spatialBlend = worldPosition == default ? 0.0f : 1.0f;

            audioSource.Play();

            if (!sound.Loop) {
                Object.Destroy(audioSource.gameObject, audioSource.clip.length);
            }

            return audioSource;
        }

        private AudioSource CreateAudioSource(EAudioType sourceType, Vector3 worldPosition = default) {
            var audioSource = new GameObject().AddComponent<AudioSource>();

            audioSource.transform.position = worldPosition;
            audioSource.transform.rotation = Quaternion.identity;

            if (this.MainAudioMixer != null) {
                var mixerGroups = this.MainAudioMixer.FindMatchingGroups("Master");

                for (int i = 0; i < mixerGroups.Length; i++) {
                    if (mixerGroups[i].name == sourceType.ToString()) {
                        audioSource.outputAudioMixerGroup = mixerGroups[i];
                        break;
                    }
                }

            }
            return audioSource;
        }
    }

    public enum EAudioType {
        Music,
        UI,
        Game
    }
}