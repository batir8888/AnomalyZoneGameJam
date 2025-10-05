using StarterAssets;
using UnityEngine;

namespace Batyr.Scripts
{
    public class CursorManager : MonoBehaviour
    {
        private Camera _mainCamera;
        private StarterAssetsInputs  _starterAssetsInputs;

        private void Start()
        {
            _mainCamera = Camera.main;
            _starterAssetsInputs = FindObjectOfType<StarterAssetsInputs>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance: 2f))
                {
                    if (hit.collider.TryGetComponent(out Terminal _))
                    {
                        _starterAssetsInputs.cursorLocked = false;
                        _starterAssetsInputs.cursorInputForLook = false;
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                _starterAssetsInputs.cursorLocked = true;
                _starterAssetsInputs.cursorInputForLook = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}