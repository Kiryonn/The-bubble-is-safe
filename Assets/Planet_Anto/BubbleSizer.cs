
    using UnityEngine;

    public class BubbleSizer
    {
        private float minScale; // Échelle minimale de la bulle
        private float maxScale; // Échelle maximale de la bulle

        // Constructeur pour initialiser les échelles min et max
        public BubbleSizer(float minScale, float maxScale)
        {
            this.minScale = minScale;
            this.maxScale = maxScale;
        }

        /// <summary>
        /// Calcule la taille de la bulle en fonction d'un pourcentage de stress.
        /// </summary>
        /// <param name="stressPercentage">Le pourcentage de stress (entre 0 et 100).</param>
        /// <returns>La nouvelle taille de la bulle.</returns>
        public float AdjustScale(float stressPercentage)
        {
            // Clamp le pourcentage entre 0 et 100 pour éviter les erreurs
            stressPercentage = Mathf.Clamp(stressPercentage, 0f, 100f);

            // Interpole linéairement entre minScale et maxScale
            float newScale = Mathf.Lerp(minScale, maxScale, stressPercentage / 100f);
            return newScale;
        }
    }
