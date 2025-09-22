using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MainController : MonoBehaviour
{
    public int _onStation;
    public int _onLevel = 1;
    public IntroMainScript _introScript;
    public SalaCinemaController _scriptSala;
    public SFXManager _scriptSXF;
    public Camera _mainCamera;

    // For controlling Post Exposure
    private ColorGrading colorGrading;
    public float _postExposureQuantity;
    public Animator _loadingAnimator;
    public Animator _intermissionCurrentAnimator;

    public List<int> _gamesToPLay = new List<int>();
    public int _finalGameID;
    public int _totalGames;
    public int[] _changeAt;

    private void Awake()
    {
        //_scriptSXF = GameObject.Find("SFXController").GetComponent<SFXManager>();
    }
    void Start()
    {
        // Get the PostProcessVolume from the camera
        PostProcessVolume volume = _mainCamera.GetComponent<PostProcessVolume>();
        if (volume != null && volume.profile != null)
        {
            // Try to get the ColorGrading settings from the profile
            if (!volume.profile.TryGetSettings<ColorGrading>(out colorGrading))
            {
                Debug.LogWarning("ColorGrading not found in the PostProcessProfile!");
            }
        }
        else
        {
            Debug.LogWarning("PostProcessVolume or profile is missing on the camera!");
        }
    }

    void Update()
    {
        // Update post exposure value
        if (colorGrading != null)
        {
            colorGrading.postExposure.value = Mathf.Lerp(colorGrading.postExposure.value, _postExposureQuantity, 1 * Time.deltaTime);
              
        }
    }
}
