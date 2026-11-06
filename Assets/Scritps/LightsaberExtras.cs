using UnityEngine;

// Part 5: attach to the outer "Lightsaber" object.
// - Point light that lights the scene in the blade color, fading with extension
// - Procedurally generated ignition/retract + hum sounds (no audio files needed)
// - Mouse swing (hold right mouse button); hum pitch/volume rises with swing speed
// - Keys 1-5 change blade color
[RequireComponent(typeof(AudioSource))]
public class LightsaberExtras : MonoBehaviour
{
    public Lightsaber saber;           // BladeBase's Lightsaber script
    public Renderer bladeRenderer;     // Blade cylinder's Mesh Renderer
    public Light bladeLight;           // Point Light (child of Blade)

    [Header("Shader property reference (check Graph Inspector > Reference)")]
    public string colorProperty = "_BladeColor";

    public Color[] colors = {
        new Color(0.1f, 0.4f, 1f),  // blue
        new Color(0.1f, 1f, 0.2f),  // green
        new Color(1f, 0.05f, 0.05f),// red
        new Color(0.6f, 0.1f, 1f),  // purple
        new Color(1f, 0.8f, 0.1f)   // yellow
    };

    public float maxLightIntensity = 3f;
    public float mouseSensitivity = 3f;

    AudioSource hum;
    AudioSource sfx;
    AudioClip igniteClip, retractClip;
    bool wasActive = true;
    float swingSpeed;
    float yaw, pitch;
    MaterialPropertyBlock mpb;

    void Start()
    {
        mpb = new MaterialPropertyBlock();

        hum = GetComponent<AudioSource>();
        hum.clip = MakeHum();
        hum.loop = true;
        hum.volume = 0.25f;
        hum.Play();

        sfx = gameObject.AddComponent<AudioSource>();
        igniteClip = MakeSweep(0.35f, 80f, 220f);
        retractClip = MakeSweep(0.35f, 220f, 60f);

        Vector3 e = transform.localEulerAngles;
        yaw = e.y; pitch = e.x;

        SetColor(colors[0]);
    }

    void Update()
    {
        // Color switching
        for (int i = 0; i < colors.Length && i < 9; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) SetColor(colors[i]);

        // Mouse swing while holding right mouse button
        float dx = 0f, dy = 0f;
        if (Input.GetMouseButton(1))
        {
            dx = Input.GetAxis("Mouse X") * mouseSensitivity;
            dy = Input.GetAxis("Mouse Y") * mouseSensitivity;
            yaw += dx;
            pitch = Mathf.Clamp(pitch - dy, -80f, 80f);
            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        float instSpeed = Mathf.Sqrt(dx * dx + dy * dy) / Mathf.Max(Time.deltaTime, 0.0001f);
        swingSpeed = Mathf.Lerp(swingSpeed, instSpeed, 10f * Time.deltaTime);

        // Ignite / retract sounds on state change
        bool extending = saber.weaponActive;
        if (Input.GetKeyDown(KeyCode.Space))
            sfx.PlayOneShot(saber.ExtendFraction > 0.5f ? retractClip : igniteClip, 0.6f);
        wasActive = extending;

        // Light + hum follow extension; swing boosts hum
        float f = saber.ExtendFraction;
        float swing01 = Mathf.Clamp01(swingSpeed / 600f);
        bladeLight.intensity = maxLightIntensity * f * Random.Range(0.92f, 1f); // slight flicker
        hum.volume = f * (0.2f + 0.5f * swing01);
        hum.pitch = 1f + 0.5f * swing01;
    }

    void SetColor(Color c)
    {
        bladeRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(colorProperty, c);
        bladeRenderer.SetPropertyBlock(mpb);
        bladeLight.color = c;
    }

    // Low buzzy hum: two detuned saws + a little noise
    AudioClip MakeHum()
    {
        int rate = 44100, len = rate; // 1 second loop
        float[] d = new float[len];
        for (int i = 0; i < len; i++)
        {
            float t = (float)i / rate;
            float s = Saw(t * 90f) * 0.5f + Saw(t * 91f) * 0.4f + Mathf.Sin(2 * Mathf.PI * 180f * t) * 0.2f;
            d[i] = (s + (Random.value - 0.5f) * 0.05f) * 0.5f;
        }
        AudioClip clip = AudioClip.Create("Hum", len, 1, rate, false);
        clip.SetData(d, 0);
        return clip;
    }

    // Pitch sweep for ignition/retraction
    AudioClip MakeSweep(float dur, float f0, float f1)
    {
        int rate = 44100, len = (int)(rate * dur);
        float[] d = new float[len];
        float phase = 0f;
        for (int i = 0; i < len; i++)
        {
            float k = (float)i / len;
            float freq = Mathf.Lerp(f0, f1, k);
            phase += freq / rate;
            float env = Mathf.Sin(Mathf.PI * k); // fade in/out
            d[i] = (Saw(phase) * 0.6f + Saw(phase * 1.01f) * 0.4f) * env * 0.6f;
        }
        AudioClip clip = AudioClip.Create("Sweep", len, 1, rate, false);
        clip.SetData(d, 0);
        return clip;
    }

    static float Saw(float x) => 2f * (x - Mathf.Floor(x + 0.5f));
}
