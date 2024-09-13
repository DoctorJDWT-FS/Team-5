using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [Header("----- Menu Color -----")]
    [SerializeField] List<GameObject> MenuList = new List<GameObject>();
    [SerializeField] Color buttonDefaultColor;
    [SerializeField] Color HighlightColor;

    int MenuPosition;
    GameObject currentHighlight;

    void Start()
    {
        // change the color of all  button to this default 
        foreach (GameObject menuItem in MenuList)
        {
            if (menuItem != null)//safe check 
            {
                Image menuColor = menuItem.GetComponent<Image>();
                menuColor.color = buttonDefaultColor;
              
            }
        }

        // Highlight the first item on the list
        if (MenuList.Count > 0)
        {
            MenuPosition = 0;
            HighlightMenu(MenuPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MenuNav();
        // Activate button if Enter is pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            
            Button buttonToActivate = MenuList[MenuPosition].GetComponent<Button>();
            if (buttonToActivate != null)
            { 
                //activate the button
                buttonToActivate.onClick.Invoke(); 
            }
            Toggle toggleToActivate = MenuList[MenuPosition].GetComponentInParent<Toggle>();
            if (toggleToActivate != null)
            {
                toggleToActivate.isOn = !toggleToActivate.isOn;
            }
            
        }
        
    }

    private void MenuNav()
    {
        // Navigate the menu if the player press up moves the menu up or down
        Slider sliderToActivate = MenuList[MenuPosition].GetComponentInParent<Slider>();
         //p
        if (Input.GetKeyDown(KeyCode.UpArrow) && MenuPosition > 0)
        {
            MenuPosition--;
            HighlightMenu(MenuPosition);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && MenuPosition < MenuList.Count - 1)
        {
            MenuPosition++;
            HighlightMenu(MenuPosition);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)&& sliderToActivate != null)
        {
           sliderToActivate.value += 200;
           sliderToActivate.value = Mathf.Clamp(sliderToActivate.value,sliderToActivate.minValue,sliderToActivate.maxValue);
        
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && sliderToActivate != null)
        {
            sliderToActivate.value -= 200;
            sliderToActivate.value = Mathf.Clamp(sliderToActivate.value, sliderToActivate.minValue, sliderToActivate.maxValue);

        }
    }

    private void HighlightMenu(int position)
    {
        //reset the previous highlight color  
        if (currentHighlight != null)
        {
            Image previousImage = currentHighlight.GetComponent<Image>();
            previousImage.color = buttonDefaultColor;
            
        }

        //set the new menu position as the current highliht
        currentHighlight = MenuList[position];
        if (currentHighlight != null)
        {
            Image currentImage = currentHighlight.GetComponent<Image>();
            currentImage.color = HighlightColor;
        }
    }
}
