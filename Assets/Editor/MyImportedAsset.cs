using UnityEngine;

class MyImportedAsset {

	public Texture2D texture;
	public Texture2D textureCopy;
	public string target_path;
	private string m_sourcePath;
	public string source_path {
		get { return m_sourcePath; }
		set { 
			m_sourcePath = value;
			target_path = value.Replace("4x", "2x");
		}
	}
	public bool targetAsset_created = false;

	public MyImportedAsset() {}
	
	public string ToString() {
		return "MyImportedAsset \n source_path : " + source_path + "\n target_path : " + target_path + "\n texture : " + texture;
	}
}