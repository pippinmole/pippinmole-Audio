using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace pippinmole.Audio {
    public class SoundDefinitionSource : MonoBehaviour {

        [SerializeField] private SoundDefinition _definition;
        [SerializeField] private bool _playOnAwake;
        [SerializeField] private EAudioType _audioType;

        private void Start() {
            if (this._playOnAwake)
                this.Play();
        }

        public void Play() => AudioManager.Instance.PlaySound(this._definition, this._audioType, this.transform, this.transform.position);
    }
}