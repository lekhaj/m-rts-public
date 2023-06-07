using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(Shop))]
public class ShopEditor : Editor{
	
	int unitCount;
	Shop shop;
	bool gemCounts = false;
	
	CharacterManager manager;
	
	void OnEnable(){
		shop = target as Shop;
		manager = GameObject.FindObjectOfType<CharacterManager>();
		
		if(manager != null)
			unitCount = manager.troops.Count;
	}
	
	public override void OnInspectorGUI(){	
		DrawDefaultInspector();
		
		if(manager == null)
			return;
	
		while(shop.gemCounts.Count != unitCount){
			if(shop.gemCounts.Count < unitCount){
				shop.gemCounts.Add(0);
			}
			else{
				shop.gemCounts.RemoveAt(shop.gemCounts.Count - 1);
			}
		}
		
		GUILayout.Space(5);
		gemCounts = EditorGUILayout.Foldout(gemCounts, "Gems needed");
		
		if(gemCounts){
			GUI.color = new Color(1, 1, 1, 0.5f);
			GUILayout.BeginVertical("Box");
			GUI.color = Color.white;
			
			for(int i = 0; i < unitCount; i++){
				GUILayout.BeginHorizontal();
				GUILayout.Label("" + manager.troops[i].deployableTroops.name);
				shop.gemCounts[i] = EditorGUILayout.IntField(shop.gemCounts[i], GUILayout.Width(50));
			
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}
		
		serializedObject.ApplyModifiedProperties();
		Undo.RecordObject(shop, "change in shop");
		EditorUtility.SetDirty(shop);
	}
}
