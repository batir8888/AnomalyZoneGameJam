using System.Collections;
using System.Collections.Generic;
using Batyr.Scripts;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_InputField InputField1;
        [SerializeField] private TMP_InputField InputField2;
        [SerializeField] private TMP_Text ResultText;
        [SerializeField] private GameObject result;
        [SerializeField] private GameObject LoadImage;
        [SerializeField] private Transform point;
        [SerializeField] private Button craftButton;
        
        [Header("Settings")]
        [SerializeField] private float loadingDelay = 1f;
        
        private CraftCapsule _craftCapsule;
        private bool _isCrafting = false;

        private void Awake()
        {
            _craftCapsule = GetComponent<CraftCapsule>();
            if (_craftCapsule == null)
            {
                _craftCapsule = gameObject.AddComponent<CraftCapsule>();
            }
            
            if (craftButton != null)
            {
                craftButton.onClick.AddListener(OnCraftButtonClick);
            }
        }

        public void OnCraftButtonClick()
        {
            if (_isCrafting) return;
            
            // Парсинг ID из инпутов
            if (!int.TryParse(InputField1.text, out int id1))
            {
                ShowError("Неверный ID артефакта 1");
                return;
            }
            
            if (!int.TryParse(InputField2.text, out int id2))
            {
                ShowError("Неверный ID артефакта 2");
                return;
            }
            
            StartCoroutine(CraftCoroutine(id1, id2));
        }

        private IEnumerator CraftCoroutine(int id1, int id2)
        {
            _isCrafting = true;
            ResultText.text = "";
            result.SetActive(false);
            
            // Проверка наличия артефактов
            if (!Inventory.Instance.HasArtifact(id1))
            {
                ShowError($"Артефакт с ID {id1} не найден в инвентаре");
                _isCrafting = false;
                yield break;
            }
            
            if (!Inventory.Instance.HasArtifact(id2))
            {
                ShowError($"Артефакт с ID {id2} не найден в инвентаре");
                _isCrafting = false;
                yield break;
            }
            
            var artifact1 = Inventory.Instance.GetDataById(id1);
            var artifact2 = Inventory.Instance.GetDataById(id2);
            
            // Показываем загрузку
            GameObject loadingObj = null;
            if (LoadImage != null && point != null)
            {
                loadingObj = Instantiate(LoadImage, point);
            }
            
            yield return new WaitForSeconds(loadingDelay);
            
            // Уничтожаем загрузку
            if (loadingObj != null)
            {
                Destroy(loadingObj);
            }
            
            // Попытка крафта
            if (_craftCapsule.CanCraft((artifact1, artifact2), out var newArtifact))
            {
                // КРИТИЧНО: Сначала удаляем, потом добавляем
                // Иначе можем удалить только что созданный артефакт, если он совпадает с входными
                Inventory.Instance.DeleteArtifact(artifact1);
                Inventory.Instance.DeleteArtifact(artifact2);
                Inventory.Instance.AddArtifact(newArtifact);
                
                ShowSuccess(newArtifact);
            }
            else
            {
                ShowError("Невозможно скрафтить эти артефакты");
            }
            
            // Очистка полей
            InputField1.text = "";
            InputField2.text = "";
            
            _isCrafting = false;
        }

        private void ShowSuccess(ArtifactData artifact)
        {
            string message;
            Color color;
            
            // Особое отображение для квестовых артефактов Tier 5
            if (artifact.Tier == 5 && artifact.IsQuest)
            {
                message = "КВЕСТОВЫЙ АРТЕФАКТ\n" +
                          $"Tier: {artifact.Tier}\n" +
                          $"ID: {artifact.Id}\n" +
                          $"LocalId: {artifact.LocalId}\n";
                color = new Color(1f, 0.84f, 0f); // Золотой
            }
            else
            {
                message = "<color=green>Успех!</color>\n" +
                          $"Tier: {artifact.Tier}\n" +
                          $"ID: {artifact.Id}\n" +
                          $"Бонус: {artifact.Bonus} +{artifact.BonusValue}";
                color = Color.green;
            }
            
            ResultText.text = message;
            ResultText.color = color;
            result.SetActive(true);
        }

        private void ShowError(string message)
        {
            ResultText.text = $"<color=red>Ошибка:</color>\n{message}";
            ResultText.color = Color.red;
            result.SetActive(true);
        }
    }
