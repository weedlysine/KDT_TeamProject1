using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    static MainThreadDispatcher instance;

    void Awake()
    {
        instance = this;
    }

    // 메인 스레드에서 실행될 액션을 받아 처리하는 함수
    public static void ExecuteOnMainThread(System.Action action)
    {
        if (instance != null)
        {
            instance.actionQueue.Enqueue(action);
            Debug.Log("여긴가???");
        }
            
    }

    // 큐에 있는 액션을 순차적으로 실행하는 함수
    void Update()
    {
        while (actionQueue.Count > 0)
        {
            actionQueue.Dequeue().Invoke();
            Debug.Log("여긴냐고?????");
        }
    }

    // 액션을 저장할 큐
    private System.Collections.Generic.Queue<System.Action> actionQueue = new System.Collections.Generic.Queue<System.Action>();
}
