using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class BallProgressCtrl : MonoBehaviour
{
    static readonly int ID_Ball = Shader.PropertyToID("_ball");
    static readonly int ID_progress = Shader.PropertyToID("_progress");
    static readonly int ID_wave1_speed = Shader.PropertyToID("_wave1_speed");
    static readonly int ID_wave2_speed = Shader.PropertyToID("_wave2_speed");
    
    static readonly int ID_wave1_frequency = Shader.PropertyToID("_wave1_frequency");
    static readonly int ID_wave2_frequency = Shader.PropertyToID("_wave2_frequency");
    
    static readonly int ID_wave1_amplitude = Shader.PropertyToID("_wave1_amplitude");
    static readonly int ID_wave2_amplitude = Shader.PropertyToID("_wave2_amplitude");

    static readonly int ID_wave_offset = Shader.PropertyToID("_wave_offset");
    static readonly int ID_wave_brightness = Shader.PropertyToID("_wave_brightness");
    static readonly int ID_gscale = Shader.PropertyToID("_gscale");

    private bool dirty = true;

    [SerializeField]
    private float radius;

    public float Radius
    {
        get { return radius; }
        set
        {
            if(radius != value)
            {
                dirty = true;
                radius = value;
            }
        }
    }

    [SerializeField]
    [Range(0, 1)]
    private float value;
    public float Value
    {
        get { return value; }
        set
        {
            if(this.value != value)
            {
                dirty = true;
                this.value = value;
            }
        }
    }

    [SerializeField]
    private float wave1_speed;
    [SerializeField]
    private float wave2_speed;
    [SerializeField]
    [Range(0, 1)]
    private float wave1_amplitude;
    [SerializeField]
    [Range(0, 1)]
    private float wave2_amplitude;
    [SerializeField]
    [Range(0, 10)]
    private float wave1_frequency;
    [SerializeField]
    [Range(0, 10)]
    private float wave2_frequency;
    [SerializeField]
    private float wave_offset;
    [SerializeField]
    private float wave_brightness;

    Graphic graphic;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (graphic != null)
            UpdateMaterials(true);
    }
#endif

    private void Awake()
    {
        graphic = GetComponent<Graphic>();
        UpdateMaterials(true);
    }

    private void OnDestroy()
    {
    }

    private void UpdateMaterials(bool with_static = false)
    {
        var material = graphic.materialForRendering;
        
        if(with_static)
        {
            material.SetFloat(ID_wave1_speed, wave1_speed);
            material.SetFloat(ID_wave2_speed, wave2_speed);
            material.SetFloat(ID_wave1_frequency, wave1_frequency);
            material.SetFloat(ID_wave2_frequency, wave2_frequency);
            material.SetFloat(ID_wave1_amplitude, wave1_amplitude);
            material.SetFloat(ID_wave2_amplitude, wave2_amplitude);
            material.SetFloat(ID_wave_offset, wave_offset);
            material.SetFloat(ID_wave_brightness, wave_brightness);
        }

        Vector4 ball = transform.position;
        ball.w = radius;
        material.SetFloat(ID_gscale, transform.lossyScale.x);
        material.SetVector(ID_Ball, ball);
        material.SetFloat(ID_progress,Value);
    }
}
