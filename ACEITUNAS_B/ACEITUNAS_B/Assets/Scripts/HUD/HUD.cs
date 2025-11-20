using UnityEngine;
using TMPro;


public class HUD : MonoBehaviour
{
    public GameObject[] vidas;

   

    void Update()
    {
       
    }

    

    public void DesactivarVida(int indice)
    {
        vidas[indice].SetActive(false);
    }

    
    public void ActivarVida(int indice)
    {
        vidas[indice].SetActive(true);
    }
    
  

}
