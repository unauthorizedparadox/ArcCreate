using ArcCreate.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ArcCreate.Gameplay.Audio.Practice
{
    public class PracticeTimeline : MonoBehaviour, IPointerClickHandler, IDragHandler
    {
        [SerializeField] private GameplayData gameplayData;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private RawImage image;
        [SerializeField] private RectTransform rect;
        private readonly int timingShaderId = Shader.PropertyToID("_CurrentSample");
        private readonly int lengthShaderId = Shader.PropertyToID("_AudioLength");
        private readonly int repeatFromShaderId = Shader.PropertyToID("_RepeatSampleFrom");
        private readonly int repeatToShaderId = Shader.PropertyToID("_RepeatSampleTo");

        public void OnPointerClick(PointerEventData eventData) => OnDrag(eventData);

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, viewCamera, out Vector2 local);
            float t = Mathf.Clamp(local.x / rect.rect.width, -0.5f, 0.5f) + 0.5f;
            int timing = Mathf.RoundToInt(t * Services.Audio.AudioLength);
            Services.Audio.AudioTiming = timing;
            Services.Audio.SetResumeAt(timing);
        }

        public void LoadWaveformFor(AudioClip clip)
        {
            if (image.texture != null)
            {
                Destroy(image.texture);
            }

            Texture2D texture = WaveformGenerator.EncodeTexture(clip);
            image.texture = texture;
            image.material.mainTexture = texture;
            image.enabled = true;
            image.material.SetFloat(lengthShaderId, WaveformGenerator.SecondToSample(clip.length, clip));
        }

        public void SetRepeatRange(bool repeatOn, int repeatFromTiming, int repeatToTiming)
        {
            if (!repeatOn)
            {
                repeatFromTiming = repeatToTiming = -1;
            }

            image.material.SetFloat(repeatFromShaderId, WaveformGenerator.SecondToSample(repeatFromTiming / 1000f, gameplayData.AudioClip.Value));
            image.material.SetFloat(repeatToShaderId, WaveformGenerator.SecondToSample(repeatToTiming / 1000f, gameplayData.AudioClip.Value));
        }

        private void Update()
        {
            image.material.SetFloat(
                timingShaderId,
                WaveformGenerator.SecondToSample(Services.Audio.AudioTiming / 1000f, gameplayData.AudioClip.Value));
        }
    }
}