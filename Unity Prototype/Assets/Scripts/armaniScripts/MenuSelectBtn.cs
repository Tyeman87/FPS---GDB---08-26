using CodeMonkey.Utils;
using UnityEngine;
using static damage;

public class MenuSelectBtn : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] GameObject UI;
    [SerializeField] MenuItem MenuType;


    bool canPressMenu;

    public enum MenuItem
    {
        Shop,
        Loadout,
        Defend,
        Stealth,
        InvStash,
        BaseAsslt,
        Hostage,
    }

    private void Update()
    {
        if (MenuType == MenuItem.Shop)
        {
            if (canPressMenu)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    UI.SetActive(false);

                    MenuLdr.load(MenuLdr.Scene.shopScene);

                }
            }

        }
        else if (MenuType == MenuItem.Loadout)
        {
            if (canPressMenu)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    UI.SetActive(false);


                }
            }

        }
        else if (MenuType == MenuItem.BaseAsslt)
        {
            if (canPressMenu)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    UI.SetActive(false);

                    MenuLdr.load(MenuLdr.Scene.testAssault);

                }
            }

        }
        else if (MenuType == MenuItem.Hostage)
        {
            if (canPressMenu)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    UI.SetActive(false);

                    MenuLdr.load(MenuLdr.Scene.testHostage);

                }
            }

        }
        else if (MenuType == MenuItem.InvStash)
        {
            if (canPressMenu)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    UI.SetActive(false);

                    

                }
            }

        }
      
        else if (MenuType == MenuItem.Stealth)
        {
            if (canPressMenu)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    UI.SetActive(false);

                    MenuLdr.load(MenuLdr.Scene.Moni);

                }
            }

        }
        else if (MenuType == MenuItem.Defend)
        {
            if (canPressMenu)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    UI.SetActive(false);

                    MenuLdr.load(MenuLdr.Scene.testProtect);

                }
            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        IMenuBttn open = other.GetComponent<IMenuBttn>();
        if (open != null)
        {
            UI.SetActive(true);
            canPressMenu = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        IMenuBttn open = other.GetComponent<IMenuBttn>();
        if (open != null)
        {
            model.SetActive(true);
            UI.SetActive(false);
            canPressMenu = false;
        }
    }

 
}
