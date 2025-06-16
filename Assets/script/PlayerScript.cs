using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Camera mainCamera; // 暴露给编辑器的摄像头参数
    public GameObject ground; // 暴露给编辑器的ground参数

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D 或 左/右
        float moveY = Input.GetAxis("Vertical");   // W/S 或 上/下
        Vector3 movement = new Vector3(moveX, moveY, 0);

        //  float originalScaleX = transform.localScale.x;
        if (moveX != 0)
        {
            float scaleX = moveX > 0 ? 1 : -1;
            transform.localScale = new Vector3(scaleX, 1, 1);
        }



        float speed = 5f; // 可根据需要调整速度
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        Bounds groundBounds;
        // 限制角色移动在ground范围内
        if (ground != null)
        {
            Renderer[] renderers = ground.GetComponentsInChildren<Renderer>();
            groundBounds = new Bounds(renderers[0].bounds.center, renderers[0].bounds.size);
            for (int i = 1; i < renderers.Length; i++)
            {
                groundBounds.Encapsulate(renderers[i].bounds);
            }
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, groundBounds.min.x, groundBounds.max.x);
            pos.y = Mathf.Clamp(pos.y, groundBounds.min.y, groundBounds.max.y);
            transform.position = pos;

            // 镜头跟随主角移动
            if (mainCamera != null)
            {
                Camera cam = mainCamera.GetComponent<Camera>();
                float height = 2f * cam.orthographicSize;
                float width = height * cam.aspect;

                // 计算相机可移动的范围
                float minX = groundBounds.min.x + width / 2;
                float maxX = groundBounds.max.x - width / 2;
                float minY = groundBounds.min.y + height / 2;
                float maxY = groundBounds.max.y - height / 2;

                Vector3 cameraPos = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
                cameraPos.x = Mathf.Clamp(cameraPos.x, minX, maxX);
                cameraPos.y = Mathf.Clamp(cameraPos.y, minY, maxY);
                mainCamera.transform.position = cameraPos;
            }
        }

    }
}
