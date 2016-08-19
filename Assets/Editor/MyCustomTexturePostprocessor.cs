using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyCustomTexturePostprocessor : AssetPostprocessor {
	
	static List<MyImportedAsset> importedAssetList = new List<MyImportedAsset>();
	
	void OnPreprocessTexture () {
		MyImportedAsset asset = GetImportedAsset( assetPath );
		Debug.Log("MyCustomTexturePostprocessor :: OnPreprocessTexture :: assetPath / asset : " + assetPath + " / " + asset);
		
		if( asset == null ){
			asset = new MyImportedAsset();
			asset.source_path = assetPath;

			importedAssetList.Add( asset );
		}
	}
	
	void OnPostprocessTexture (Texture2D texture) {
		MyImportedAsset asset = GetImportedAsset( assetPath );
		Debug.Log("MyCustomTexturePostprocessor :: OnPostprocessTexture :: assetPath / texture : " + assetPath + " / " + texture);
		if( asset == null ) {// should never be true because asset was first in preprocess state and created there
			asset = new MyImportedAsset();
			asset.source_path = assetPath;
		}
		
		asset.texture = texture;
	}
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
		foreach (string source in importedAssets) {
			Debug.Log("Reimported Asset: " + source);
			MyImportedAsset asset = GetImportedAsset( source );
			if( asset != null && !asset.targetAsset_created ){
				Debug.Log("Reimported Asset :: create low resolution image for '" + asset.source_path + "' at '" + asset.target_path + "'");
			}
		}

		foreach (string str in deletedAssets) {
			MyImportedAsset asset = GetImportedAsset( str );
			Debug.Log("Deleted Asset: " + str + " asset : " + asset);
		}

		for (int i=0; i<movedAssets.Length; i++) {
			Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
		}
	}
	
	static MyImportedAsset GetImportedAsset(string sourcePath) {
		foreach ( MyImportedAsset import in importedAssetList ) {
			if( import.source_path == sourcePath ) {
				return import;
			}
		}
		
		return null;
	}
}