using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;
using System.Threading.Tasks;

public static class CameraEffectsUtility
{
    public static void DoCameraShake(float duration, float force, Action action = null)
    {
        Transform cameraTrans = GameSceneManager.Instance.CurrentCamera.transform;

        cameraTrans.DOShakePosition(duration, new Vector3(force, force, 0));
        action?.Invoke();
    }

    public static void DoCameraShake(float duration, float force, float delay, Action action = null)
    {
        DoDelay(delay, () => DoCameraShake(duration, force, action));
    }
    
    private static async void DoDelay(float duration, Action action)
    {
        await Task.Delay((int)(duration * 1000));

        action?.Invoke();
    }
}