using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeSetting : MonoBehaviour
{
    public int index = 0;
    public TypeSettings typeData;
    public GameObject content;
    private List<Slider> typeSliders = new List<Slider>();
    public GameObject typeAttractionTemplate;
    private float yPosition = 0;
    private Slider forceSlider;
    private Slider rangeSlider;
    private Slider amountSlider;
    private Button removeSettingButton;
    public SimulationBehaviour controller;
    public MenuScript menu;

    // Start is called before the first frame update
    void Start()
    {
        Button[] buttons = this.gameObject.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.gameObject.name == "removeButton") 
            {
                removeSettingButton = button;
                removeSettingButton.onClick.AddListener(RemoveType);   
            }
        }
             
        Slider[] sliders = this.gameObject.GetComponentsInChildren<Slider>();
        foreach (Slider child in sliders)
        {
            if (child.gameObject.name == "forceSlider") 
            {
                forceSlider = child;
                forceSlider.onValueChanged.AddListener(delegate{ForceChanged();});
            }
            else if (child.gameObject.name == "amountSlider")
            {
                amountSlider = child;
                amountSlider.onValueChanged.AddListener(delegate{AmountChanged();});
            }
        }

        UpdateAllSettingsAttractions();
    }

    public void UpdateAllSettingsAttractions()
    {
        foreach (TypeSetting setting in menu.typeSettings)
        {
            setting.UpdateAttractions();
        }
    }

    public void UpdateAttractions()
    {
        if (typeSliders.Count < controller.types.Count) while(typeSliders.Count < controller.types.Count) AddTypeToContent(typeSliders.Count);
        else if (typeSliders.Count > controller.types.Count) while(typeSliders.Count > controller.types.Count) typeSliders.RemoveAt(typeSliders.Count - 1);
    }

    void RemoveType()
    {
        Destroy(controller.types[index]);
        UpdateAllSettingsAttractions();
        Destroy(this.gameObject);
    }

    void AmountChanged()
    {
        controller.particleAmounts[index] = (int)amountSlider.value;
        controller.UpdateParticleCount();
    }

    void ForceChanged()
    {
        typeData.force = forceSlider.value;
    }
    
    void AttractionValueChange(int i)
    {
        print(i);
        print(typeSliders.Count);
        typeData.attractions[i] = typeSliders[i].value;
    }

    void AddTypeToContent(int i)
    {
        GameObject newObj = Instantiate(typeAttractionTemplate, new Vector3(0, yPosition, 0), new Quaternion(0, 0, 0, 0), content.transform);
        yPosition -= 20;
        Slider slider = newObj.GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate{AttractionValueChange(i);});
        typeSliders.Add(slider);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
