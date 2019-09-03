﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //玩家位置
    private Transform target;
    //偏移量
    private Vector3 offset;
    private Vector2 velocity;
    private void Update()
    {
        if(target == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            offset = target.position - transform.position;
        }
    }
    private void FixedUpdate()
    {
        if(target != null)
        {
          float posX =  Mathf.SmoothDamp(transform.position.x, 
              target.position.x - offset.x, ref velocity.x, 0.05f);
          float posY = Mathf.SmoothDamp(transform.position.y, 
              target.position.y - offset.x, ref velocity.y, 0.05f);
            //防止摄像机上下抖动
            if (posY > transform.position.y)
            transform.position = new Vector3(posX, posY, transform.position.z);
        }
    }
}