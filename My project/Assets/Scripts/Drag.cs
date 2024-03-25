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
    Camera Camera;

    private void Start()
    {
        Camera = this.GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 위치를 스크린 좌표로 가져옴
            Vector2 mousePosition = Input.mousePosition;

            // 마우스 위치를 UI 좌표로 변환
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetUI, mousePosition, null, out localMousePosition);

            // 클릭한 지점이 UI 요소의 영역 안에 있는지 확인
            if (targetUI.rect.Contains(localMousePosition))
            {
                isRotate = true;
                Debug.Log("Clicked on the specific UI element!");
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
            // 이동 방향 계산 (y값은 0으로 고정)
            Vector3 moveDirection = new Vector3(-horizontalInput, 0f, -verticalInput).normalized;

            // 카메라 이동
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            //GameObject.Find("cctv").GetComponent<cctv_rotate>().fix_rotation();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            Camera.fieldOfView += (20 * Input.GetAxis("Mouse ScrollWheel"));

        if (Camera.fieldOfView < 10)
            Camera.fieldOfView = 10;
        else if (Camera.fieldOfView > 50)
            Camera.fieldOfView = 50;
    }

    public void Rotation()
    {
        mouseX += Input.GetAxis("Mouse X") * rotateSpeed; // AxisX = Mouse Y
        mouseY = Mathf.Clamp(mouseY + Input.GetAxis("Mouse Y") * rotateSpeed, -limitAngle, limitAngle);
        if(mouseX !=0 && mouseY !=0)
            transform.rotation = Quaternion.Euler(35 +transform.rotation.x - mouseY, -173 +transform.rotation.y + mouseX, 0.0f);
    }
}
