using UnityEngine;
using UnityEngine.UI;

public class OpenScnScript : MonoBehaviour
{
    public Image bookPic; // Drag your Image object here in the Inspector

    public Button bookButton; 



    void Start()
    {
        
        if (bookButton != null)
        {
            bookButton.onClick.AddListener(Enbl_Dcbl_image);
        }
    }

    void Enbl_Dcbl_image()
    {
        if(bookPic.enabled)
            bookPic.enabled = false;
        else
            bookPic.enabled = true;
    }



}
