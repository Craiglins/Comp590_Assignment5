using UnityEngine;

public class Lightsaber : MonoBehaviour
{
    public float extendSpeed = 0.1f;      
    public bool weaponActive;             
    private const float SCALE_MIN = 0f;   
    private float scaleMax;               
    private float extendDelta;            
    private float scaleCurrent;           
    private float localScaleX;            
    private float localScaleZ;

    public GameObject blade;              

    // 0 = collapsed, 1 = fully extended (used by LightsaberExtras)
    public float ExtendFraction => scaleMax > 0f ? scaleCurrent / scaleMax : 0f;

    void Awake()
    {
        localScaleX = transform.localScale.x;
        localScaleZ = transform.localScale.z;
        scaleMax = transform.localScale.y;
        scaleCurrent = scaleMax;
        extendDelta = scaleMax / extendSpeed;
        weaponActive = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            extendDelta = weaponActive ? -Mathf.Abs(extendDelta) : Mathf.Abs(extendDelta);
        }

        scaleCurrent += extendDelta * Time.deltaTime;
        scaleCurrent = Mathf.Clamp(scaleCurrent, SCALE_MIN, scaleMax);
        transform.localScale = new Vector3(localScaleX, scaleCurrent, localScaleZ);

        weaponActive = scaleCurrent > 0;

        if (weaponActive && !blade.activeSelf)
        {
            blade.SetActive(true);
        }
        else if (!weaponActive && blade.activeSelf)
        {
            blade.SetActive(false);
        }
    }
}
