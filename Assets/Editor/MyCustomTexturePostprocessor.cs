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
		if( asset != null && asset.targetAsset_created ) return;
		
		if( asset == null ) {// should never be true because asset was first in preprocess state and created there
			asset = new MyImportedAsset();
			asset.source_path = assetPath;
		}

		asset.texture = new Texture2D( texture.width, texture.height, texture.format, true);
		asset.texture.LoadRawTextureData( texture.GetRawTextureData() );
	}
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
		List<MyImportedAsset> copies = new List<MyImportedAsset>();
		
		foreach (string source in importedAssets) {
			Debug.Log("Reimported Asset: " + source);
			MyImportedAsset asset = GetImportedAsset( source );
			if( asset != null && !asset.targetAsset_created ){
				copies.Add( asset );
			}
		}

		foreach (string str in deletedAssets) {
			MyImportedAsset asset = GetImportedAsset( str );
			Debug.Log("Deleted Asset: " + str + " asset : " + asset);
			RemoveByPath( str );
		}

		for (int i=0; i<movedAssets.Length; i++) {
			Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
		}
		
		if( copies.Count > 0 )
		{
			foreach( MyImportedAsset asset in copies )
			{
				asset.targetAsset_created = true;
				Texture2D textureCopy = new Texture2D( asset.texture.width/2, asset.texture.height/2);//, asset.texture.format, true );//asset.texture.mipmapCount > 1 );
				textureCopy.SetPixels( asset.texture.GetPixels(1) );//LoadRawTextureData( asset.texture.GetRawTextureData() );
				textureCopy.Apply();

				System.IO.File.WriteAllBytes( asset.target_path, textureCopy.EncodeToPNG() );
				AssetDatabase.Refresh();
			}
		}
	}
	
	static MyImportedAsset GetImportedAsset(string sourcePath) {
		foreach ( MyImportedAsset import in importedAssetList ) {
			if( import.source_path == sourcePath || import.target_path == sourcePath ) {
				return import;
			}
		}
		
		return null;
	}
	
	static void RemoveByPath( string path ) {
		foreach ( MyImportedAsset import in importedAssetList ) {
			if( import.source_path == path || import.target_path == path ) {
				importedAssetList.Remove( import );
				
				System.IO.File.Delete( import.target_path );
				AssetDatabase.Refresh();
				return;
			}
		}
	}
}