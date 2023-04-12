using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public List<TypeSetting> typeSettings = new List<TypeSetting>();
    public GameObject simulationSettings;
    public GameObject settingsContent;
    private Dropdown dimensions;
    public GameObject scrollView;
    public GameObject content;
    private SimulationBehaviour simulationControl;
    public GameObject simulationControlGameObject;
    private Button resetButton;
    private Button typesButton;
    private Button newTypeButton;
    private Slider worldForceSlider;
    public GameObject typeSettingsTemplate;
    public GameObject typeAttractionsTemplate;
    // Start is called before the first frame update
    void Start()
    {
        simulationControl = simulationControlGameObject.GetComponent<SimulationBehaviour>();

        Button[] buttons = this.gameObject.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.name == "resetButton") 
            {
                resetButton = button;
                resetButton.onClick.AddListener(simulationControl.Reset);
            }
            else if (button.name == "typesButton") 
            {
                typesButton = button;
                typesButton.onClick.AddListener(TypesButtonTask);
            }
            else if (button.name == "newTypeButton")
            {
                newTypeButton = button;
                newTypeButton.onClick.AddListener(NewTypeButtonTask);
            }
            else if (button.name == "topPerspectiveButton")
            {
                button.onClick.AddListener(simulationControl.SetCameraPositionTop);
            }
            else if (button.name == "rightPerspectiveButton")
            {
                button.onClick.AddListener(simulationControl.SetCameraPositionRight);
            }
            else if (button.name == "frontPerspectiveButton")
            {
                button.onClick.AddListener(simulationControl.SetCameraPositionFront);
            }
            else if (button.name == "settingsButton")
            {
                button.onClick.AddListener(SettingsButtonTask);
            }
        }
        worldForceSlider = GetComponentInChildren<Slider>();
        worldForceSlider.onValueChanged.AddListener(delegate{WorldForceValueChange();});

        if (settingsContent)
        {
            Dropdown[] dropdowns = settingsContent.GetComponentsInChildren<Dropdown>();
            foreach (Dropdown dropdown in dropdowns)
            {
                if (dropdown.name == "dimensions")
                {
                    dropdown.onValueChanged.AddListener(delegate{DimensionsDropdownTask();});
                }
            }
        }
    }

    private void DimensionsDropdownTask()
    {
        simulationControl.physics.simulationSetting = dimensions.value;
    }

    private void SettingsButtonTask()
    {
        if (simulationSettings.activeSelf) simulationSettings.SetActive(false);
        else simulationSettings.SetActive(true);
    }

    void WorldForceValueChange()
    {
        simulationControl.worldForce = worldForceSlider.value;
    }

    void NewTypeButtonTask()
    {
        GameObject newObject = Instantiate(typeSettingsTemplate, new Vector3(content.transform.position.x, content.transform.position.y, content.transform.position.z), Quaternion.identity, content.transform);
        TypeSetting settings = newObject.GetComponent<TypeSetting>();
        settings.index = typeSettings.Count;
        newObject.GetComponentInChildren<Text>().text = typeSettings.Count.ToString();
        settings.controller = simulationControl;
        settings.typeData = CreateNewTypeData(typeSettings.Count);
        settings.menu = this;
        typeSettings.Add(settings);
    }

    public TypeSettings CreateNewTypeData(int index)
    {
        TypeSettings newTypeData = simulationControlGameObject.AddComponent<TypeSettings>();
        newTypeData.force = 0;
        newTypeData.type = index;
        newTypeData.color = ChooseColor();

        newTypeData.attractions = new List<float>();
        for (int i = 0; i < simulationControl.types.Count + 1; i++)
        {
            newTypeData.attractions.Add(0);
        }
        
        simulationControl.types.Add(newTypeData);
        simulationControl.particleAmounts.Add(0);
        simulationControl.UpdateTypeAttractions();
        return newTypeData;
    }

    private Color ChooseColor()
    {
        switch (simulationControl.types.Count)
        {
            case 0:
                return Color.red;
            case 1:
                return Color.green;
            case 2:
                return Color.blue;
            case 3:
                return Color.yellow;
            case 4: 
                return Color.white;
            default:
                return Color.gray;
        }
    }

    private void RemoveButtonTask()
    {
        foreach (TypeSettings type in simulationControlGameObject.GetComponents<TypeSettings>())
        {
            if (type == typeSettings[typeSettings.Count - 1]) Destroy(type);
        }
        typeSettings.RemoveAt(typeSettings.Count - 1);
    }

    void TypesButtonTask()
    {
        CanvasGroup canvasGroup = scrollView.GetComponent<CanvasGroup>();
        if (canvasGroup.alpha == 0) canvasGroup.alpha = 2;
        else canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
