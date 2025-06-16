using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonsterScript : MonoBehaviour
{
    public GameObject player;

    public float speed = 5f;
    public float hp = 100f;
    public float attack = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // 计算朝向主角的方向
            Vector3 direction = (player.transform.position - transform.position).normalized;
            
            // 根据方向设置怪物的朝向
            if (direction.x != 0)
            {
                float scaleX = direction.x > 0 ? 1 : -1;
                transform.localScale = new Vector3(scaleX, 1, 1);
            }

            // 向主角方向移动
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }
}
