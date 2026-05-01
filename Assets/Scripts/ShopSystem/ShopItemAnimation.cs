using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopItemAnimation : MonoBehaviour
{
    public void DoItemAnim(Sprite sprite, Vector3 position)
    {
        GetComponent<Image>().sprite = sprite;
        transform.DOMove(position, 1f).SetEase(Ease.InOutQuad).OnComplete(() => Destroy(gameObject));
    }
}
