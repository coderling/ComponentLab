using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UIGPUAni.Editor
{
    public class ExportAnimationTexture
    {
        public struct EditorFrame
        {
            public Vector3 pos;
            public Vector3 scale;
            public Vector3 rotation;
            public Vector3 other_params;
            public float alpha;
            public float time;
        }

        public class EditorAnimationCurve
        {
            public List<EditorFrame> frames = new List<EditorFrame>();
            public float length;
        }

        [MenuItem("UIGPUANI/BakeAnimation")]
        private static void BakeAnimationTexture()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var f_path = System.IO.Path.GetDirectoryName(path) + "/" + System.IO.Path.GetFileNameWithoutExtension(path);
            var animation = CreateAnimation(f_path);
            List<EditorAnimationCurve> curves = new List<EditorAnimationCurve>();
            BakeAnimationPrefab(path, curves, animation);
            SetAnimation(animation, curves, f_path + ".png");
        }

        private static GPUAnimation CreateAnimation(string f_path)
        {
            var animation = AssetDatabase.LoadAssetAtPath<GPUAnimation>(f_path + ".asset");
            if(animation == null)
            {
                animation = ScriptableObject.CreateInstance<GPUAnimation>();
                AssetDatabase.CreateAsset(animation, f_path + ".asset");
            }
            return animation;
        }

        static EditorFrame[] origin = new EditorFrame[GPUAniHelper.frame_count];
        static Bounds offset_bounds;
        private static void BakeAnimationPrefab(string path, List<EditorAnimationCurve> list, GPUAnimation animation)
        {
            offset_bounds.center = Vector3.zero;
            offset_bounds.size = Vector3.zero;
            bool is_set_center = false;
            var pb_instance = PrefabUtility.LoadPrefabContents(path);

            var ani = pb_instance.GetComponent<Animation>();
            var clip = ani.clip;
            if (clip == null)
                return;

            var graphics = pb_instance.GetComponentsInChildren<UnityEngine.UI.Graphic>(false);

            for(int index = 0; index < graphics.Length; ++index)
            {
                var g = graphics[index];
                var ani_g = g.GetComponent<GPUAniGraphic>();
                if(ani_g == null)
                {
                    ani_g = g.gameObject.AddComponent<GPUAniGraphic>();
                }
                for (int i = 0; i < GPUAniHelper.frame_count; ++i)
                {
                    origin[i].pos = g.transform.position;
                    origin[i].scale = g.transform.lossyScale;
                    origin[i].alpha = g.color.a;
                }

                float time = 0;
                float d_time = 1.0f / GPUAniHelper.frame_count;
                var curve = new EditorAnimationCurve();
                curve.length = clip.length;
                ani_g.offset = list.Count;
                ani_g.animation_length = clip.length;
                ani_g.gpu_animation = animation;
                list.Add(curve);
                for(int i = 0; i < GPUAniHelper.frame_count; ++i)
                {
                    clip.SampleAnimation(pb_instance, time * clip.length);
                    var frame = new EditorFrame();
                    frame.pos = g.transform.position - origin[i].pos;
                    if(!is_set_center)
                    {
                        offset_bounds.center = frame.pos;
                        is_set_center = true;
                    }
                    else
                    {
                        offset_bounds.Encapsulate(frame.pos);
                    }

                    var o_scale = origin[i].scale;
                    var c_scale = g.transform.lossyScale;
                    frame.scale = new Vector3(c_scale.x / o_scale.x, c_scale.y / o_scale.y, c_scale.z / o_scale.z);
                    frame.alpha = g.color.a;
                    frame.time = time;
                    curve.frames.Add(frame);
                    time += d_time;
                }
            }

            var t_path = System.IO.Path.GetDirectoryName(path) + "/" + System.IO.Path.GetFileNameWithoutExtension(path) + "_used.prefab";
            if(System.IO.File.Exists(t_path))
            {
                System.IO.File.Delete(t_path);
            }

            Object.DestroyImmediate(ani);
            PrefabUtility.SaveAsPrefabAsset(pb_instance, t_path);
            PrefabUtility.UnloadPrefabContents(pb_instance);
        }
        
        private static void SetAnimation(GPUAnimation animation, List<EditorAnimationCurve> list, string tex_path)
        {
            float bound_max = Mathf.Max(offset_bounds.size.x, offset_bounds.size.y, offset_bounds.size.z);
            animation.space_width = bound_max;
            int h = GPUAniHelper.frame_data_height * list.Count;
            int w = Mathf.CeilToInt(h * 1.0f / 2048) * GPUAniHelper.frame_count;
            h = h > 2018 ? 2048 : h;
            if(w > 2048)
            {
                EditorUtility.DisplayDialog("提示", "数据过大，导出贴图超过2048x2048, 无法导出", "确认");
                return;
            }
            Vector2Int range = Vector2Int.zero;
            range.y = h / GPUAniHelper.frame_data_height;
            range.x = w / (GPUAniHelper.frame_count);

            animation.range = range;
            animation.animation_texture = new Texture2D(w, h, TextureFormat.RGBA32, false, true);
            var texture = animation.animation_texture;
            int c_index = 0;
            foreach(var curve in list)
            {
                int x = (c_index / range.y) * (GPUAniHelper.frame_count);
                int y = c_index % range.y * GPUAniHelper.frame_data_height;
                for (int i = 0; i < curve.frames.Count; ++i)
                {
                    var fr = curve.frames[i];
                    // pos
                    
                    Color pos = new Color(fr.pos.x / bound_max * 0.5f + 0.5f, fr.pos.y / bound_max  * 0.5f + 0.5f, fr.pos.z / bound_max * 0.5f + 0.5f, fr.alpha);
                    texture.SetPixel(x + i, y, pos);
                    Color scale = new Color(fr.scale.x, fr.scale.y, fr.scale.z, fr.alpha);
                    texture.SetPixel(x + i, y + 1, scale);
                }
                c_index++;
            }


            if(System.IO.File.Exists(tex_path))
            {
                System.IO.File.Delete(tex_path);
            }
            var data = animation.animation_texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(tex_path, data);
            AssetDatabase.Refresh();
            var importer = AssetImporter.GetAtPath(tex_path) as TextureImporter;
            TextureImporterSettings setting = new TextureImporterSettings();
            setting.textureType = TextureImporterType.Default;
            setting.filterMode = FilterMode.Point;
            setting.sRGBTexture = false;
            setting.readable = false;
            setting.wrapMode = TextureWrapMode.Clamp;
            setting.alphaSource = TextureImporterAlphaSource.FromInput;
            setting.alphaIsTransparency = true;
            importer.SetTextureSettings(setting);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            // mat
            var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/UIGPUAni/Sample/GPUAni.mat");
            material.SetTexture("_AniTex", animation.animation_texture);
            material.SetFloat("_RangeY", animation.range.y);
            material.SetFloat("_RangeY_D", 1.0f / animation.range.y);
            material.SetInt("_FrameCount", GPUAniHelper.frame_count);
            material.SetInt("_DataHeight", GPUAniHelper.frame_data_height);
            material.SetFloat("_Bound", animation.space_width);
            animation.material = material;
            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssets();
        }
    }
}

