namespace Planet_Anto
{
    using UnityEngine;

    public class PlayerPositionChecker : MonoBehaviour
    {
        // Les deux positions définissant la zone
        public Transform positionA;
        public Transform positionB;

        // Position cible pour téléporter le joueur
        public Transform targetHub;

        // Le joueur à surveiller
        public Transform player;

        void Update()
        {
            CheckPlayerPosition();
        }

        /// <summary>
        /// Vérifie si le joueur est entre positionA et positionB.
        /// </summary>
        private void CheckPlayerPosition()
        {
            if (!IsPlayerInZone())
            {
                TeleportPlayerToTarget();
            }
        }

        /// <summary>
        /// Vérifie si le joueur est dans la zone définie par positionA et positionB.
        /// </summary>
        /// <returns>True si le joueur est dans la zone, sinon False.</returns>
        private bool IsPlayerInZone()
        {
            Vector3 minBounds = Vector3.Min(positionA.position, positionB.position);
            Vector3 maxBounds = Vector3.Max(positionA.position, positionB.position);

            return player.position.x >= minBounds.x && player.position.x <= maxBounds.x &&
                   player.position.y >= minBounds.y && player.position.y <= maxBounds.y &&
                   player.position.z >= minBounds.z && player.position.z <= maxBounds.z;
        }

        /// <summary>
        /// Téléporte le joueur à la position targetHub.
        /// </summary>
        private void TeleportPlayerToTarget()
        {
            player.position = targetHub.position;
            Debug.Log("Le joueur a été téléporté au hub cible.");
        }
    }

}