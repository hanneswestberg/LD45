using System.Collections;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public static ParticleController instance;

    public ParticleSystem regular_matter;
    public ParticleSystem anti_matter;
    public ParticleSystem dark_matter;
    public ParticleSystem regular_explode;
    public ParticleSystem anti_explode;
    public ParticleSystem dark_explode;
    public ParticleSystemForceField force_field;

    public float max_mass = 50f;
    public float max_size = 10f;
    public float max_gravity = 2f;
    public float min_gravity = 0.1f;
    public float max_particles = 1000f;
    public float min_particles = 0f;

    private bool paused = false;
    private bool exploding = false;

    BangManager bm;
    UIManager ui;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        bm = BangManager.instance;
        ui = UIManager.instance;
    }

    public void StartParticles() {
        ToggleAll("play");
        StartCoroutine(UpdateParticles());
    }

    public void ToggleAll(string state)
    {
        if (state == "play")
        {
            regular_matter.Play();
            anti_matter.Play();
            dark_matter.Play();
        }
        else if (state == "pause")
        {
            regular_matter.Pause();
            anti_matter.Pause();
            dark_matter.Pause();
        }
        else if (state == "stop")
        {
            regular_matter.Stop();
            anti_matter.Stop();
            dark_matter.Stop();
            regular_matter.Clear();
            anti_matter.Clear();
            dark_matter.Clear();
        }

    }

    void Update()
    {
        if (ui.UserInputPause)
        {
            ToggleAll("pause");
            paused = true;
        }
        else if (paused)
        {
            ToggleAll("play");
            paused = false;
        }

        if (ui.UserInputExplode && !exploding)
        {
            exploding = true;
            ToggleAll("stop");
            StartCoroutine(Explode());
        }
        else if (!ui.UserInputExplode)
        {
            exploding = false;
        }
    }

    IEnumerator Explode()
    {
        regular_explode.Play();
        anti_explode.Play();
        dark_explode.Play();
        yield return new WaitForSeconds(3.0f);
        regular_explode.Stop();
        anti_explode.Stop();
        dark_explode.Stop();
    }

    IEnumerator UpdateParticles()
    {
        while (Application.isPlaying)
        {
            yield return new WaitForSeconds(1f);

            float norm_mass = Mathf.Clamp(Mathf.Log10(bm.CurrentMass) / 5f, -1f, 1f);

            if(norm_mass < 0f && UIManager.instance.UserInputMassGainRatio > 0f) {
                norm_mass = 0.05f;
            }

            int num_part = (int)Mathf.Min(min_particles + norm_mass * max_particles, max_particles);

            force_field.startRange = Mathf.Min(3f - 3f * norm_mass, 3f);
            force_field.gravity = Mathf.Min(min_gravity + norm_mass * max_gravity, max_gravity);

            float mass_regu = norm_mass * bm.CurrentMassRatioRegular;
            float mass_anti = norm_mass * bm.CurrentMassRatioAnti;
            float mass_dark = norm_mass * bm.CurrentMassRatioDark;

            var regularEmission = regular_matter.emission;
            regularEmission.enabled = (bm.PhaseIsOnGoing && ui.UserInputMassGainRatio > 0f && !bm.IsGameFinishedAndWaitingForUserToExplode);
            var antiEmission = anti_matter.emission;
            antiEmission.enabled = (bm.PhaseIsOnGoing && ui.UserInputMassGainRatio > 0f && !bm.IsGameFinishedAndWaitingForUserToExplode);
            var darkEmission = dark_matter.emission;
            darkEmission.enabled = (bm.PhaseIsOnGoing && ui.UserInputMassGainRatio > 0f && !bm.IsGameFinishedAndWaitingForUserToExplode);

            var main = regular_matter.main;
            if(norm_mass > 0f) {
                main.maxParticles = (int)(mass_regu * num_part);
                main = anti_matter.main;
                main.maxParticles = (int)(mass_anti * num_part);
                main = dark_matter.main;
                main.maxParticles = (int)(mass_dark * num_part);
            }
        }
    }
}
