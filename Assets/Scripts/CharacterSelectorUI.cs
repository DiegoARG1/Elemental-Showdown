using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectorUI : MonoBehaviour
{
    [Header("Personajes disponibles")]
    [SerializeField] private CharacterData[] characters;

    [Header("UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button exitButton;

    private int currentIndex = 0;

    private void Start()
    {
        leftButton.onClick.AddListener(Previous);
        rightButton.onClick.AddListener(Next);
        selectButton.onClick.AddListener(Confirm);
        exitButton.onClick.AddListener(Salir);

        // Si ya había uno elegido, empezamos en ese
        if (CharacterSelection.selected != null)
        {
            int i = System.Array.IndexOf(characters, CharacterSelection.selected);
            if (i >= 0) currentIndex = i;
        }

        RefreshUI();
    }

    private void Next()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        RefreshUI();
    }

    private void Previous()
    {
        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        RefreshUI();
    }

    private void RefreshUI()
    {
        CharacterData c = characters[currentIndex];
        portraitImage.sprite = c.selectionPortrait != null ? c.selectionPortrait : c.defaultSprite;
    }

    private void Confirm()
    {
        CharacterSelection.selected = characters[currentIndex];
        SceneManager.LoadScene("MainScene");
    }

    public void Salir()
    {
        SceneManager.LoadScene("MainMenu");
    }
}