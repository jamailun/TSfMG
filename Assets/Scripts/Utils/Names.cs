using UnityEngine;

[System.Serializable]
public struct NamesFile {
	public LanguageNames fr;
	public LanguageNames en;

	public LanguageNames Get(Language lang) {
		return lang switch {
			Language.FR => fr,
			_ => en
		};
	}
	public override string ToString() {
		return "NamesFile{fr=" + fr + ",\n en=" + en + "\n}";
	}
}

[System.Serializable]
public struct LanguageNames {
	public string[] male;
	public string[] female;

	public string Random(bool isMale) {
		string[] names = isMale ? male : female;
		if(names == null) {
			Debug.LogError("Carefull, null array in " + ToString());
			return "error";
		}
		return names[UnityEngine.Random.Range(0, names.Length)];
	}

	public override string ToString() {
		return "LanguageName{male=" + male?.NiceString() + ", female=" + female?.NiceString() + "}";
	}
}

[System.Serializable]
public enum Language {
	FR,
	EN
}

public static class NamesLoader {
	public static LanguageNames Load(TextAsset asset, Language lang) {
		try {
			NamesFile all = JsonUtility.FromJson<NamesFile>(asset.text);
			return all.Get(lang);
		} catch(System.Exception e) {
			Debug.LogError("Could not read names file ("+asset+") : " + e.Message);
			return new LanguageNames();
		}
	}
}



