using UnityEngine;
using UnityEngine.UI;

public class CambioSprite : MonoBehaviour
{
    public Sprite llenoSprite;
    public Sprite vacioSprite;
    public Image Image;

    public bool lleno;
    void Start()
    {
        Image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lleno)
        { 
            Image.sprite = llenoSprite;
        }
        else
        {
            Image.sprite= vacioSprite;
        }
    }
}
