using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftUI : MonoBehaviour
{
    [SerializeField] Button CombineButton;
    [SerializeField] GameObject result;
    [SerializeField] TMP_Text ResultText;
    [SerializeField] TMP_InputField InputField1;
    [SerializeField] TMP_InputField InputField2;
    [SerializeField] GameObject LoadImage;
    [SerializeField] Transform point;
    public int ID1 = 0;
    public int ID2 = 0;
    bool flag = true;
    // Start is called before the first frame update
    void Start()
    {
        result.SetActive(false);

        InputField1.onValueChanged.AddListener(IF1Change);
        InputField2.onValueChanged.AddListener(IF2Change);

        CombineButton.onClick.AddListener(Combine);
    }

    private void OnDestroy()
    {
        InputField1.onValueChanged.RemoveAllListeners();
        InputField2.onValueChanged.RemoveAllListeners();
        CombineButton.onClick.RemoveAllListeners();
    }

    public void IF1Change(string val)
    {
        if(val != "")
            ID1 = int.Parse(val);
    }

    public void IF2Change(string val)
    {
        if(val != "")
            ID2 = int.Parse(val);
    }

    public void Combine()
    {
        if (flag && ID1!=0 && ID2!=0) 
        {
            flag = false;
            StartCoroutine(Result());
        }
    }

    IEnumerator Result()
    {
        ResultText.text = "";
        Instantiate(LoadImage, point);
        yield return  new WaitForSeconds(1f);
        flag = true;
        result.SetActive(true);
        ResultText.text = "2 Green + 10 speed";
        InputField1.text = "";
        InputField2.text = "";
    }
}
