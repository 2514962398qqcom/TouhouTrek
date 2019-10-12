using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using ZMDFQ;

public class Test : MonoBehaviour
{
    TaskCompletionSource<bool> t1, t2;
    // Start is called before the first frame update
    async void Start()
    {
        var t = wait1();
        var tt = wait2();
        List<Task<bool>> list = new List<Task<bool>>() { t, tt }; 
        while (true)
        {
            var end = await Task.WhenAny(list);
            if (end.Result)
            {
                endall();
                break;
            }
            else
            {
                list.Remove(end);
                if (list.Count == 0)
                {
                    break;
                }
            }
        }
        Debug.Log(123);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            t1.SetResult(true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            t1.SetResult(false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            t1.SetCanceled();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            t2.SetResult(false);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            t2.SetResult(true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            t2.SetCanceled();
        }
    }

    void endall()
    {
        t1.TrySetCanceled();
        t2.TrySetCanceled();
    }

    Task<bool> wait1()
    {
        t1 = new TaskCompletionSource<bool>();
        return t1.Task;
    }
    Task<bool> wait2()
    {
        t2 = new TaskCompletionSource<bool>();
        return t2.Task;
    }
}
