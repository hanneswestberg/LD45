using System.Collections;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem regular_matter;
    public ParticleSystem anti_matter;
    public ParticleSystem dark_matter;
    public ParticleSystemForceField force_field;

    public float max_mass = 50f;
    public float max_size = 10f;
    public float max_gravity = 2f;
    public float min_gravity = 0.1f;
    public float max_particles = 1000f;
    public float min_particles = 0f;

    private bool paused = false;

    BangManager bm;
    UIManager ui;

    void Start()
    {
        bm = BangManager.instance;
        ui = UIManager.instance;

        ToggleAll(true);
        StartCoroutine(UpdateParticles());
    }

    void ToggleAll(bool enabled)
    {
        if (enabled)
        {
            regular_matter.Play();
            anti_matter.Play();
            dark_matter.Play();
        }
        else
        {
            regular_matter.Pause();
            anti_matter.Pause();
            dark_matter.Pause();
        }

    }

    void Update()
    {
        if (ui.UserInputPause)
        {
            ToggleAll(false);
            paused = true;
        }
        else if (paused)
        {
            ToggleAll(true);
            paused = false;
        }
    }

    IEnumerator UpdateParticles()
    {
        while (Application.isPlaying)
        {
            yield return new WaitForSeconds(1f);

            float norm_mass = bm.CurrentMass / max_mass;
            float vol = bm.CurrentVolatility;

            int num_part = (int)Mathf.Min(norm_mass * max_particles + min_particles, max_particles);

            force_field.startRange = Mathf.Min(norm_mass * max_size, 10);
            force_field.gravity = Mathf.Min(norm_mass * max_gravity + min_gravity, max_gravity);

            float mass_regu = norm_mass * bm.CurrentMassRatioRegular;
            float mass_anti = norm_mass * bm.CurrentMassRatioAnti;
            float mass_dark = norm_mass * bm.CurrentMassRatioDark;

            var main = regular_matter.main;
            main.maxParticles = (int)(mass_regu * num_part);
            main = anti_matter.main;
            main.maxParticles = (int)(mass_anti * num_part);
            main = dark_matter.main;
            main.maxParticles = (int)(mass_dark * num_part);
        }
    }
}
