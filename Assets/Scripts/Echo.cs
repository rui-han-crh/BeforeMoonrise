using System.Collections;
using System.Collections.Generic;
using TNRD;
using UnityEngine;

public class Echo : MonoBehaviour
{
    [SerializeField]
    private float delay = 0.1f;

    [SerializeField]
    private float duration = 0.15f;
    
    private SpriteRenderer spriteRenderer;

    private IEnumerator echoEffect;

    [SerializeField]
    private SerializableInterface<IMovementController> movementController;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementController.Value.OnBeginDodge += () => {
            enabled = true;
            if (echoEffect == null)
            {
                echoEffect = EchoEffect();
                StartCoroutine(echoEffect);
            }
        };

        movementController.Value.OnEndDodge += () => {
            if (echoEffect != null)
            {
                StopCoroutine(echoEffect);
                echoEffect = null;
            }
            enabled = false;
        };

        enabled = false;
    }

    private IEnumerator EchoEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            GameObject echo = new GameObject("echo");
            CopyChildTransform(echo.transform);

            SpriteRenderer echoSpriteRenderer = echo.AddComponent<SpriteRenderer>();
            CopyChildSpriteRenderer(echoSpriteRenderer);
            StartCoroutine(FadeSprite(echoSpriteRenderer));

            Destroy(echo, duration);
        }
    }

    private IEnumerator FadeSprite(SpriteRenderer spriteRenderer) {
        float time = 0;
        while (time < duration) {
            if (spriteRenderer == null) {
                // The sprite renderer has been destroyed
                yield break;
            }

            time += Time.deltaTime;
            spriteRenderer.color = new Color(1, 1, 1, 1 - time / duration);
            yield return null;
        }
    }

    private void CopyChildTransform(Transform child) 
    {
        child.SetParent(transform.parent); // the child cannot follow the parent

        child.position = transform.position;
        child.localScale = transform.localScale;
        child.rotation = transform.rotation;
    }

    private void CopyChildSpriteRenderer(SpriteRenderer childSpriteRenderer)
    {
        // Manually copy all properties of the sprite renderer
        childSpriteRenderer.sprite = spriteRenderer.sprite;
        childSpriteRenderer.flipX = spriteRenderer.flipX;
        childSpriteRenderer.flipY = spriteRenderer.flipY;
        childSpriteRenderer.color = spriteRenderer.color;
        childSpriteRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        childSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder;
        childSpriteRenderer.maskInteraction = spriteRenderer.maskInteraction;
        childSpriteRenderer.drawMode = spriteRenderer.drawMode;
        childSpriteRenderer.size = spriteRenderer.size;
        childSpriteRenderer.tileMode = spriteRenderer.tileMode;
        childSpriteRenderer.adaptiveModeThreshold = spriteRenderer.adaptiveModeThreshold;
        childSpriteRenderer.spriteSortPoint = spriteRenderer.spriteSortPoint;
    }
}
