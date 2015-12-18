using UnityEngine;
using System.Collections;

public class SceneManager : SingletonMonoBehaviour<SceneManager> {

    public void Awake() {
        if(this != Instance) {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

	void LoadScene (string scene) {
        Application.LoadLevel(scene);
	}

    void Retry () {
        Application.LoadLevel(Application.loadedLevelName);
    }
}
