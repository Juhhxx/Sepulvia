using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

public static class CameraEffectsUtility
{
    public static void DoCameraShake(float duration, float force, Camera camera = null, Action action = null)
    {
        Transform cameraTrans = camera == null ? GameSceneManager.Instance.CurrentCamera.transform : camera.transform;

        cameraTrans.DOShakePosition(duration, new Vector3(force, force, 0));
        action?.Invoke();
    }

    public static void DoCameraShake(float duration, float force, float delay, Camera camera = null, Action action = null)
    {
        Transform cameraTrans = camera == null ? GameSceneManager.Instance.CurrentCamera.transform : camera.transform;

        cameraTrans.DOShakePosition(duration, new Vector3(force, force, 0)).SetDelay(delay).OnComplete(() => action?.Invoke());
    }
}