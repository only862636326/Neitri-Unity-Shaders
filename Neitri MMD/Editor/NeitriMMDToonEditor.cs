﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NeitriMMDToonEditor : ShaderGUI
{
	static bool ShowAdvanced = false;

	List<Material> materials = new List<Material>();
	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		materials.Clear();
		foreach (var obj in materialEditor.targets)
		{
			Material material = obj as Material;
			if (!material) continue;
			materials.Add(material);
		}

		{
			GUILayout.BeginHorizontal();

			GUILayout.Label("Presets", GUILayout.ExpandWidth(false));

			if (GUILayout.Button(new GUIContent("Default", "Reverts all changes to default values"), GUILayout.ExpandWidth(false)))
			{
				Undo.RecordObjects(materials.ToArray(), "Preset Default");
				SetTexture("_Ramp", "96ad26bf5aa0f2147b6c1651287c1ae6");
				SetTexture("_Matcap", "d6064d42d7ffecd4cba07c5bd929b6d5");
				SetFloat("_MatcapWeight", 0.1f);
				SetColor("_ShadowColor", new Color(0f, 0f, 0f, 1f));
				SetColor("_ShadowRim", new Color(0f, 0f, 0f, 1f));

				SetFloat("_Shadow", 0.4f);
				SetFloat("_BakedLightingFlatness", 0.9f);
				SetFloat("_ApproximateFakeLight", 0.7f);
				SetFloat("_AlphaCutout", 0.05f);
				SetFloat("_ForceLightDirectionToForward", 0.3f);
				SetFloat("_Cull", 2);
				SetFloat("_ZTest", 4);
			}

			if (GUILayout.Button(new GUIContent("Skin", "Changes shading ramp, shadow color, shadow rim, to skin like values"), GUILayout.ExpandWidth(false)))
			{
				Undo.RecordObjects(materials.ToArray(), "Preset Skin");
				SetFloat("_MatcapWeight", 0.5f);
				SetFloat("_MatcapType", 2);
				SetTexture("_Matcap", "746904ac01669074b9b1539c50574111");
				SetColor("_ShadowRim", new Color(0.3f, 0f, 0f, 1f));
			}

			if (GUILayout.Button(new GUIContent("Rim Light", ""), GUILayout.ExpandWidth(false)))
			{
				Undo.RecordObjects(materials.ToArray(), "Preset Rim Light");
				SetFloat("_MatcapWeight", 1f);
				SetFloat("_MatcapType", 1);
				SetTexture("_Matcap", "0c2d781f9138bb74394b78913767973c");
				SetColor("_ShadowRim", new Color(0f, 0f, 0f, 1f));
			}

			// Hair, Silk, Leather, Metal

			GUILayout.EndHorizontal();
		}



		materialEditor.SetDefaultGUIWidths();

		foreach (MaterialProperty property in properties)
		{
			if ((property.flags & MaterialProperty.PropFlags.PerRendererData) != 0) continue;
			if ((property.flags & MaterialProperty.PropFlags.HideInInspector) != 0) continue;

			string displayName = property.displayName;

			bool isAdvanced = false;
			string advancedString = " -advanced";
			int advancedIndex = displayName.IndexOf(advancedString);
			if (advancedIndex != -1)
			{
				displayName = displayName.Remove(advancedIndex, advancedString.Length);
				isAdvanced = true;
			}

			if (!ShowAdvanced && isAdvanced) continue;

			float propertyHeight = materialEditor.GetPropertyHeight(property, displayName);
			Rect controlRect = EditorGUILayout.GetControlRect(true, propertyHeight, EditorStyles.layerMaskField, new GUILayoutOption[0]);
			materialEditor.ShaderProperty(controlRect, property, displayName);
		}

		if (ShowAdvanced)
		{
			materialEditor.RenderQueueField();
		}

		

		GUILayout.Space(10);
		if (ShowAdvanced)
		{
			if (GUILayout.Button("Hide advanced settings"))
			{
				ShowAdvanced = false;
			}
		}
		else
		{
			if (GUILayout.Button("Show advanced settings"))
			{
				ShowAdvanced = true;
			}
		}


	}



	void SetTexture(string name, string guid)
	{
		foreach (var material in materials)
		{
			SetTexture(material, name, guid);
		}
	}

	void SetColor(string name, Color color)
	{
		foreach (var material in materials)
		{
			material.SetColor(name, color);
		}
	}

	void SetFloat(string name, float value)
	{
		foreach (var material in materials)
		{
			material.SetFloat(name, value);
		}
	}


	static Texture2D FindTextureByGuid(string guid)
	{
		string assetPath = AssetDatabase.GUIDToAssetPath(guid);
		if (string.IsNullOrEmpty(assetPath)) return null;
		Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
		if (!texture) return null;
		return texture;
	}

	static void SetTexture(Material material, string name, string guid)
	{
		if (string.IsNullOrEmpty(name)) return;
		if (!material) return;
		Texture2D texture = FindTextureByGuid(guid);
		if (!texture) return;
		material.SetTexture(name, texture);
	}

	static class Styles
	{
		public static GUIStyle foldoutBold = new GUIStyle(EditorStyles.foldout);

		static Styles()
		{
			foldoutBold.fontStyle = FontStyle.Bold;
		}
	}

}

