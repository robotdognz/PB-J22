using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Dungeon
{
    public class CameraTrigger : Trigger
    {
        public Vector2Int CameraPosition;
        
        [Header("Follow Cam")]
        public bool FollowCamX;
        public bool FollowCamY;
        [Space]
        public int FollowXMin;
        public int FollowXMax;
        [Space]
        public int FollowYMin;
        public int FollowYMax;
   

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(CameraPosition.x * 10, CameraPosition.y * 10, 0), new Vector3(10, 10, 0.1f));

            Gizmos.color = Color.yellow;
            if (FollowCamX)
            {
                Gizmos.DrawLine(new Vector3(FollowXMin * 10, CameraPosition.y * 10, 0), new Vector3(FollowXMax, CameraPosition.y * 10, 0));
            }
            if (FollowCamY)
            {
                Gizmos.DrawLine(new Vector3(CameraPosition.x * 10, FollowYMin * 10, 0), new Vector3(CameraPosition.x * 10, FollowYMax * 10, 0));
            }
        }

        protected override void OnPlayerStay()
        {
            base.OnPlayerStay();

            CameraController.Target = new Vector3(CameraPosition.x * 10, CameraPosition.y * 10, -10);
            CameraController.FollowX = FollowCamX;
            CameraController.FollowY = FollowCamY;
            CameraController.FollowCamMin = new Vector2(FollowXMin * 10, FollowYMin * 10);
            CameraController.FollowCamMax = new Vector2(FollowXMax * 10, FollowYMax * 10);
        }
    }
}