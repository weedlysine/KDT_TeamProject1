using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour
{
    float moveSpeed = 20f;
    public float rotateSpeed = 5.0f;
    public float limitAngle = 70.0f;
    public RectTransform targetUI;

    private bool isRotate;
    private float mouseX;
    private float mouseY;
    CinemachineVirtualCamera virtualCamera;

    public GameObject[] flag = new GameObject[4];
    CCTV_Control cctvControl;
    public GameObject tmp;

    private void Start()
    {
        virtualCamera = this.GetComponent<CinemachineVirtualCamera>();
        cctvControl = tmp.GetComponent<CCTV_Control>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ���콺 ��ġ�� ��ũ�� ��ǥ�� ������
            Vector2 mousePosition = Input.mousePosition;

            // ���콺 ��ġ�� UI ��ǥ�� ��ȯ
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetUI, mousePosition, null, out localMousePosition);

            // Ŭ���� ������ UI ����� ���� �ȿ� �ִ��� Ȯ��
            if (targetUI.rect.Contains(localMousePosition))
            {
                isRotate = true;
                Debug.Log("Clicked on the specific UI element!");
            }
            // ���̸� ȭ�鿡�� ���콺 ������ �������� ���ϴ�.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            // ���̿� �浹�� ��ü�� �����մϴ�.
            if (Physics.Raycast(ray, out hitInfo))
            {
                // �浹�� ��ü�� �̺�Ʈ�� ó���մϴ�.
                ObjectClicked(hitInfo.collider.gameObject);
            }
        }
            
        if (Input.GetMouseButtonUp(0))
            isRotate = false;
        if (isRotate)
            Rotation();


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            // �̵� ���� ��� (y���� 0���� ����)
            Vector3 moveDirection = new Vector3(-horizontalInput, 0f, -verticalInput).normalized;

            // ī�޶� �̵�
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            //GameObject.Find("cctv").GetComponent<cctv_rotate>().fix_rotation();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            virtualCamera.m_Lens.FieldOfView -= (20 * Input.GetAxis("Mouse ScrollWheel"));

        if (virtualCamera.m_Lens.FieldOfView < 10)
            virtualCamera.m_Lens.FieldOfView = 10;
        else if (virtualCamera.m_Lens.FieldOfView > 50)
            virtualCamera.m_Lens.FieldOfView = 50;
    }

    public void Rotation()
    {
        mouseX += Input.GetAxis("Mouse X") * rotateSpeed; // AxisX = Mouse Y
        mouseY = Mathf.Clamp(mouseY + Input.GetAxis("Mouse Y") * rotateSpeed, -limitAngle, limitAngle);
        if(mouseX !=0 && mouseY !=0)
            transform.rotation = Quaternion.Euler(35 +transform.rotation.x - mouseY, -173 +transform.rotation.y + mouseX, 0.0f);
    }

    void ObjectClicked(GameObject clickedObject)
    {
        if (clickedObject == flag[0])
        {
            StartCoroutine(cctvControl.cctv_change_tmp(1));
        }
        else if (clickedObject == flag[1])
        {
            StartCoroutine(cctvControl.cctv_change_tmp(2));
        }
        else if (clickedObject == flag[2])
        {
            StartCoroutine(cctvControl.cctv_change_tmp(3));
        }
        else if(clickedObject == flag[3])
        {
            StartCoroutine(cctvControl.cctv_change_tmp(4));
        }
    }
}
