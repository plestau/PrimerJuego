using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuControl : MonoBehaviour
{
    public PauseMenu pausemenu;
    public void BotonJugar()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void BotonOpciones()
    {
        SceneManager.LoadScene("Opciones");
    }
    
    public void BotonReiniciarNivel()
    {
        pausemenu.RestartLevel();
    }
    
    public void BotonMainMenu()
    {
        pausemenu.ResumeGame();
        SceneManager.LoadScene("MainMenu");
    }
    
    public void BotonMenuPrincipal()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void BotonReanudar()
    {
        pausemenu.ResumeGame();
    }
    
    public void BotonSalir()
    {
        Application.Quit();
    }
}
