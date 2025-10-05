using System.Collections;
using System.Collections.Generic;
using Batyr.Scripts;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EquipUI : MonoBehaviour
{
    [SerializeField] TMP_InputField InputField1;
    [SerializeField] TMP_Text ResultText;
    [SerializeField] Button EquipButton;
    public int ID1 = 0;
    // Start is called before the first frame update
    void Start()
    {
        ResultText.text = "";
        InputField1.onValueChanged.AddListener(IF1Change);
        EquipButton.onClick.AddListener(Equip);
    }

    // Update is called once per frame

    private void OnDestroy()
    {
        InputField1.onValueChanged.RemoveAllListeners();
        EquipButton.onClick.RemoveAllListeners();
    }

    public void IF1Change(string val)
    {
        if (val != "")
            ID1 = int.Parse(val);
    }

    public void Equip()
    {
        var data = Inventory.Instance.GetDataById(ID1);
        ResultText.text = $"Set bonus {data.Bonus} +{data.BonusValue}%!";
        var equipData = new EquipData(data.Bonus, data.BonusValue);
        
    }
}

public struct EquipData
{
    BonusType bonusType;
    int bonusValue;

    public EquipData(BonusType bonusType, int bonusValue)
    {
        this.bonusType = bonusType;
        this.bonusValue = bonusValue;
    }
}
