using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.ViewControllers
{
    public class ChatCharacter_VC : QuackMonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const int FREQUENCY = 8000;
        private const int AUDIO_TIME_LIMIT_IN_SEC = 300;

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private UnityEvent _onPress;


        private ChatCharacterData _characterData;

        protected override void OnStart()
        {
            base.OnStart();

            _animator.SetTrigger("phase_1");
        }

        public void Initialize(ChatCharacterData characterData)
        {
            _characterData = characterData;

            AnimatorOverrideController overrideController = (AnimatorOverrideController)Resources.Load("Animations\\Controllers\\" + _characterData.Data.animationController);
            overrideController.runtimeAnimatorController = _animator.runtimeAnimatorController;

            // Put this line at the end because when you assign a controller on an Animator, unity rebind all the animated properties 
            _animator.runtimeAnimatorController = overrideController;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StartMicrophone();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopMicrophone();
        }

        public void StartMicrophone()
        {
#if UNITY_EDITOR
            _audioSource.clip = AudioClip.Create("playRecordClip", 10, 1, FREQUENCY, false);
#else
            _audioSource.clip = Microphone.Start(null, false, AUDIO_TIME_LIMIT_IN_SEC, FREQUENCY);
#endif
            // while (!(Microphone.GetPosition(null) > 0)) { } // Wait until the recording has started
        }

        public void StopMicrophone()
        {
            int lastTime =
#if UNITY_EDITOR
                10;
#else
            Microphone.GetPosition(null);
            if (lastTime == 0)
                return;
#endif
            //Stops the recording of the device	
            Microphone.End(null);
            var samples = new float[_audioSource.clip.samples];
            _audioSource.clip.GetData(samples, 0);

            var ClipSamples = new float[lastTime];
            Array.Copy(samples, ClipSamples, ClipSamples.Length - 1);

            _audioSource.clip = AudioClip.Create("playRecordClip", ClipSamples.Length, 1, FREQUENCY, false);
            _audioSource.clip.SetData(ClipSamples, 0);

            // send audio to server
            var record = new RecordData();
            record.ClipSamples = ClipSamples;
            record.UserId = Client.UserData.Id;
            record.ChatRoomId = AppManager.Instance.ChatUuid;
            record.ChatCharacterKey = _characterData.Key;

            AppManager.Instance.ChatRoomManager.SaveRecord(record);

            // play audio
            _audioSource.Play(); // Play the audio source!
        }
    }
}
