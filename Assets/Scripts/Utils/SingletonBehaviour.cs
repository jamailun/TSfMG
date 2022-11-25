using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> {

	public static T Instance { get; protected set; }

	public virtual bool DontDestroyObjectOnLoad => true;

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = (T) this;
		if(DontDestroyObjectOnLoad)
			DontDestroyOnLoad(gameObject);
	}

	private void OnDestroy() {
		if(Instance == this)
			Instance = null;
	}

}