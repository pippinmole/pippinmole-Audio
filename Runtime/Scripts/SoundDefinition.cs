using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace pippinmole.Audio {
    [CreateAssetMenu(fileName = "Sound Definition", menuName = "Game/Audio/Sound Definition", order = 0)]
    public class SoundDefinition : ScriptableObject {

        public string Name;

        [Header("Clip")]
        public bool RandomClip;
        [HideIf("RandomClip")] public AudioClip Clip;
        [ShowIf("RandomClip")] public List<AudioClip> RandomClips;

        [Header("Properties")]
        [Range(0f, 1f)] public float Volume = 1f;
        [Range(.1f, 3f)] public float Pitch = 1f;
        public bool Loop;

        [Header("Pitch")]
        public bool RandomPitch;
        [ShowIf("RandomPitch")] public float MinPitch = 1f;
        [ShowIf("RandomPitch")] public float MaxPitch = 1f;

        [Header("Distance")]
        public float Range;
    }
}