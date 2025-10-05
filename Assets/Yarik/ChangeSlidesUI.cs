using UnityEngine;
using UnityEngine.UI;

public class ChangeSlidesUI : MonoBehaviour
{
    [SerializeField] private GameObject[] slideGroups;
    [SerializeField] private Canvas tutorialSlides;
    [SerializeField] private Canvas dynamicItems;
    private int currentPageIndex = 0;
    private GameObject[] icons;

    void Start()
    {
        if (slideGroups.Length == 0)
        {
            Debug.LogError("PageSwitcher: tutorialPages is empty!");
            return;
        }
        for (int i = 0; i < slideGroups.Length; i++)
        {
            slideGroups[i].SetActive(i == 0);
        }
        Transform icon1 = dynamicItems.transform.GetChild(0);
        GameObject childObject = icon1.gameObject;
        Transform icon2 = dynamicItems.transform.GetChild(1);
        GameObject childObject2 = icon2.gameObject;
        Transform icon3 = dynamicItems.transform.GetChild(2);
        GameObject childObject3 = icon3.gameObject;
        icons = new[] { childObject, childObject2, childObject3 };
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
             Input.GetMouseButtonDown(0))
        {
            switchSlide();
        }
    }

    void switchSlide()
    {
        if (!(currentPageIndex >= slideGroups.Length - 1))
        {
            animatingIcons(icons[currentPageIndex+1]);
            slideGroups[currentPageIndex].SetActive(false);
            currentPageIndex = (currentPageIndex + 1) % slideGroups.Length;
            slideGroups[currentPageIndex].SetActive(true);
        }
        else
        {
            slideGroups[currentPageIndex].SetActive(false);
            destroyCanvases();
        }
    }
    void animatingIcons(GameObject icon)
    {
        Image image = icon.GetComponent<Image>();
        if (image != null)
        {
            Color newColor = new Color32(68, 186, 17, 255);
            image.color = newColor;
        }
    }
     void destroyCanvases()
    {
        Destroy(tutorialSlides.gameObject);
        Destroy(dynamicItems.gameObject);
    }
}
