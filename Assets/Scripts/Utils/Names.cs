using UnityEngine;

public struct NamesFile {
	public LanguageNames fr;
	public LanguageNames en;

	public LanguageNames Get(Language lang) {
		LanguageNames names = lang switch {
			Language.FR => fr,
			_ => en
		};
		return names;
	}
}

public struct LanguageNames {
	public string[] male;
	public string[] female;

	public string Random(bool isMale) {
		string[] names = isMale ? male : female;
		if(names == null)
			return "error";
		return names[UnityEngine.Random.Range(0, names.Length)];
	}
}

public enum Language {
	FR,
	EN
}

public static class NamesLoader {
	public const string namesPath = "Lang/names.json";
	public static LanguageNames Load(Language lang) {
		try {
			TextAsset asset = Resources.Load<TextAsset>(namesPath);
			NamesFile all = JsonUtility.FromJson<NamesFile>(asset.text);
			return all.Get(lang);
		} catch(System.Exception e) {
			Debug.LogError("Could not read names files () : " + e.Message);
			return new LanguageNames();
		}
	}
}



