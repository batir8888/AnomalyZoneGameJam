using System.Collections;
using System.Collections.Generic;
using Batyr.Scripts;
using UnityEngine;
using TMPro;

public class ListUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public void UpdateList(List<ArtifactData> artifacts)
    {
        text.text = "";
        foreach (var artifact in artifacts)
        {
            text.text += artifact + "\n";
        }
    }
}
