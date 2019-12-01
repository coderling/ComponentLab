using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class WaterVertex : MonoBehaviour
{
    private static int _WaveCountID = Shader.PropertyToID("_DirectionalWaveCount");
    private static int _WaveDirOrCenterID = Shader.PropertyToID("_WaveDirOrCenter");
    private static int _WaveLengthID = Shader.PropertyToID("_WaveLength");
    private static int _AmplitudeID = Shader.PropertyToID("_Amplitude");

    public int waveCount = 4;
    public Vector2[] diries;
    public Vector2[] centers;

    public Vector4 lengths;

    public Vector4 amplitudes;

    public GameObject target;

    private MaterialPropertyBlock materialPropertyBlock;

    private void OnEnable()
    {
        OnValidate();
    }

    private void UpdateMat()
    {
        if(materialPropertyBlock == null)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        materialPropertyBlock.SetInt(_WaveCountID, waveCount);
        List<Vector4> dirOrCenters = new List<Vector4>();
        //优先方向波
        for (int i = 0; i < diries.Length; ++i)
        {
            if(dirOrCenters.Count >= waveCount)
            {
                break;
            }
            Vector4 dir = Vector4.zero;
            var normallize_dir = diries[i].normalized;
            dir.x = normallize_dir.x;
            dir.y = normallize_dir.y;
            dirOrCenters.Add(dir);
        }

        for (int i = 0; i < centers.Length; ++i)
        {
            if(dirOrCenters.Count >= waveCount)
            {
                break;
            }

            Vector4 center = Vector4.zero;
            center.x = centers[i].x;
            center.y = centers[i].y;
            center.z = 1;
            dirOrCenters.Add(center);
        }

        materialPropertyBlock.SetVectorArray(_WaveDirOrCenterID, dirOrCenters);
        materialPropertyBlock.SetVector(_WaveLengthID, lengths);
        materialPropertyBlock.SetVector(_AmplitudeID, amplitudes);

        if(target != null)
        {
            var renderer = target.GetComponent<Renderer>();
            if(renderer != null)
            {
                renderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
    }

    private void OnValidate()
    {
        if(diries == null)
        {
            diries = new Vector2[waveCount];
        }
        else
        {
            System.Array.Resize(ref diries, waveCount);
        }

        if(centers == null)
        {
            centers = new Vector2[waveCount];
        }
        else
        {
            System.Array.Resize(ref centers, waveCount);
        }

        UpdateMat();
    }

    private void Update()
    {

    }
}