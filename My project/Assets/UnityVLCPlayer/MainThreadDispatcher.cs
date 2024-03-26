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

    // ���� �����忡�� ����� �׼��� �޾� ó���ϴ� �Լ�
    public static void ExecuteOnMainThread(System.Action action)
    {
        if (instance != null)
        {
            instance.actionQueue.Enqueue(action);
            Debug.Log("���䰡???");
        }
            
    }

    // ť�� �ִ� �׼��� ���������� �����ϴ� �Լ�
    void Update()
    {
        while (actionQueue.Count > 0)
        {
            actionQueue.Dequeue().Invoke();
            Debug.Log("����İ�?????");
        }
    }

    // �׼��� ������ ť
    private System.Collections.Generic.Queue<System.Action> actionQueue = new System.Collections.Generic.Queue<System.Action>();
}
