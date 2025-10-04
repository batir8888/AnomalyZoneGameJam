using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleUI : MonoBehaviour
{
    [SerializeField] GameObject Panel;
    [SerializeField] GameObject ListPanel;
    [SerializeField] GameObject CraftPanel;
    [SerializeField] GameObject EquipPanel;

    bool l, c, e;

    // Start is called before the first frame update
    void Start()
    {
        Panel.SetActive(true);
        ListPanel.SetActive(false);
        CraftPanel.SetActive(false);
        EquipPanel.SetActive(false);
        l = false;
        c = false;
        e = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenList()
    {
        if (l)
        {
            l = false;
            ListPanel.SetActive(false);
            Panel.SetActive(true);
        }
        else
        {
            l = true;
            ListPanel.SetActive(true);
            Panel.SetActive(false);
        }
    }

    public void OpenCraft()
    {
        if (c)
        {
            c = false;
            CraftPanel.SetActive(false);
            Panel.SetActive(true);
        }
        else
        {
            c = true;
            CraftPanel.SetActive(true);
            Panel.SetActive(false);

        }
    }

    public void OpenEquip()
    {
        if (e)
        {
            e = false;
            EquipPanel.SetActive(false);
            Panel.SetActive(true);
        }
        else
        {
            e = true;
            EquipPanel.SetActive(true);
            Panel.SetActive(false);

        }
    }


}
