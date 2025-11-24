using System.Collections;
using UnityEngine;

public class WorldSpaceHealthbar : MonoBehaviour
{
    [SerializeField] SpriteRenderer fillSprite;
    private float amount = 1.0f;
    
    public void UpdateFill(float fillPercentage) //percentage is a float out of 1.0f.
    {
        amount = fillPercentage;
        fillSprite.gameObject.transform.localScale = new Vector2(0.95f * amount, 0.75f);

        if(amount <= 0f)
        {
            //DestroyBar(); //destroy this bar, as the hp has been depleted
        }
    }

    public void DestroyBar()
    {
        this.gameObject.SetActive(false);
        Destroy(this);
    }
}