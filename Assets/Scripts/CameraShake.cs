using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    private Camera camera;
    void Start() {
        camera = GetComponent<Camera>();
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 orignalPosition = camera.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            camera.transform.position = new Vector3(x, y, -10f);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        camera.transform.position = orignalPosition;
    }
}
