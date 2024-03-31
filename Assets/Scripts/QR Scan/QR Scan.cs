using UnityEngine;
using UnityEngine.InputSystem;
using LGProjects.Android.Utility;
using UnityEngine.SceneManagement;

public class QRScan : MonoBehaviour
{
    public QRManager QRManager;
    public InputAction Back;

    private void OnEnable()
    {
        Back.Enable();
    }

    private void OnDisable()
    {
        Back.Disable();
    }

    private void Start()
    {
        if (QRManager == null)
            QRManager = FindAnyObjectByType<QRManager>();
        
        Back.performed += OnBack;
    }
    
    private void OnBack(InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene("Main");
    }
}
