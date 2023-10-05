using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    Animator anim;
    public string id;
    public bool activationStatus;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    [ContextMenu("Generate checkpoint id ")]
    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString(); //tạo 1 ID, nhưng mỗi lần gọi nó thì nó sẽ tạo 1 ID mới
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.GetComponent<Player>() != null)
        {
            ActiveCheckPoint();
        }
    }

    public void ActiveCheckPoint()
    {   
        if(activationStatus == false)
            AudioManager.instance.PlaySFX(5, transform);
            
        activationStatus = true;
        anim.SetBool("active", true);
    }
}
