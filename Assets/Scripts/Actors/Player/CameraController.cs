using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Cameras 
{
    public class CameraController : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private Transform pivot, target;
        [SerializeField]
        private float minHeight, maxHeight;
        [SerializeField]
        private float rotationSpeed, scrollSpeed;
        private Vector3 scrollVector, lerpVector;
        #endregion

        #region Unity Functions
        // Start is called before the first frame update
        void Start()
        {
            scrollVector = Vector3.zero; lerpVector = Vector3.zero;
            transform.LookAt(pivot);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            cameraMovement();
        }
        #endregion

        #region Methods
        protected virtual void cameraMovement()
        {
            pivot.position = Vector3.Lerp(pivot.position, target.position + new Vector3(0, 0.75f, 0), 
                Time.deltaTime * Vector3.Distance(pivot.position, target.position + new Vector3(0, 0.75f, 0)));

            float xAxis = Input.GetAxis("Mouse X");
            if (Input.GetMouseButton(1) && (xAxis > 0 || xAxis < 0))
            {
                pivot.Rotate(new Vector3(0, xAxis * Time.deltaTime * rotationSpeed, 0));
            }

            scrollVector += cameraScroll();
            if (lerpVector != scrollVector) 
            { 
                transform.Translate(Vector3.Lerp(lerpVector, scrollVector, scrollSpeed * Time.deltaTime) * 400);
                transform.LookAt(pivot); 
            }
            else
                lerpVector = Vector3.zero; scrollVector = Vector3.zero;
        }

        protected virtual Vector3 cameraScroll()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && (transform.localPosition + scrollVector).y > minHeight)
                return ((Vector3.down * scrollSpeed) + (Vector3.forward * scrollSpeed * 3));

            if (Input.GetAxis("Mouse ScrollWheel") < 0 && (transform.localPosition + scrollVector).y < maxHeight)
                return ((Vector3.up * scrollSpeed) + (-Vector3.forward * scrollSpeed * 3));

            return Vector3.zero;
        }
        #endregion
    }
}
