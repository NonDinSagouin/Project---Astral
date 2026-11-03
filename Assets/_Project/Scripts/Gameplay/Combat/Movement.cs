using UnityEngine;

namespace Gameplay.Combat
{
    public class Movement : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float duration = 0.3f;

        [SerializeField] private float maxSpeed = 1f;
        [SerializeField] private LeanTweenType accelerationEase = LeanTweenType.easeInQuad;
        [SerializeField] private LeanTweenType decelerationEase = LeanTweenType.easeOutQuad;

        public void MoveTo(Vector3 position)
        {
            LeanTween.cancel(gameObject);

            Vector3 start = transform.position;
            Vector3 middle = Vector3.Lerp(start, position, 0.5f);
            float half = duration * 0.5f;

            // Accélération : de l'arrêt jusqu'à la vitesse max au milieu du trajet
            LeanTween.move(gameObject, middle, half)
                .setEase(accelerationEase)
                .setOnComplete(() =>
                {
                    // Décélération : de la vitesse max jusqu'à l'arrêt
                    LeanTween.move(gameObject, position, half)
                        .setEase(decelerationEase);
                });
        }

        void OnDestroy()
        {
            LeanTween.cancel(gameObject);
        }
    }
}