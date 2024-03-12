import datetime
import socket
import cv2
import numpy as np
from ultralytics import YOLO
import time
from deep_sort_realtime.deepsort_tracker import DeepSort
import threading  # 스레드 모듈 추가
from py_socket import func1
import pymysql
import os
import boto3

CONFIDENCE_THRESHOLD = 0.6
GREEN = (0, 255, 0)
WHITE = (255, 255, 255)

coco128 = open('./coco.names', 'r')
data = coco128.read()
class_list = data.split('\n')
coco128.close()

model = YOLO('./yolov8n.pt')
tracker = DeepSort(max_age=50)

url = 'rtsp://210.99.70.120:1935/live/cctv007.stream'

cap = cv2.VideoCapture(url)
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

# 두 번째 코드의 소켓 통신 및 db저장을 처리하는 함수
def handle_socket(url, frame):
    check_time = datetime.datetime.now()
    DB_HOST=os.getenv('DB_HOST')
    DB_USERNAME=os.getenv('DB_USERNAME')
    DB_PASSWORD=os.getenv('DB_PASSWORD')
    DB_NAME=os.getenv('DB_NAME')
    DB_PORT=os.getenv('DB_PORT')
    access_key = os.getenv('ACCESS_KEY')
    secret_key = os.getenv('SECRET_KEY')
    bucket_name = 'cctv-dectec-db'
    region = 'ap-northeast-2'
    
    conn = pymysql.connect(host=DB_HOST,
                             user=DB_USERNAME,
                             password=DB_PASSWORD,
                             database=DB_NAME,
                             port=int(DB_PORT),
                             cursorclass=pymysql.cursors.DictCursor)
    
    tmp_cursor =  conn.cursor()
    sql_tmp = '''SELECT cctv_ID FROM cctv WHERE cctv_url_rtsp = (%s)'''
    tmp_cursor.execute(sql_tmp,url)
    result = tmp_cursor.fetchall()
    
    
    s3 = boto3.client(
        service_name="s3",
        region_name=region, # 자신이 설정한 bucket region
        aws_access_key_id=access_key,
        aws_secret_access_key=secret_key,
        )
    filename = check_time.strftime("%Y-%m-%d %H_%M_%S")+' CAM_NUM('+str(result[0]['cctv_ID'])+').jpg'
    cv2.imwrite(filename,frame)
    s3.upload_file(filename,bucket_name,'DB_Picture/'+filename)
    os.remove(filename)
    
    cursor = conn.cursor()
    sql = '''
        INSERT INTO cctv_db (cctv_num, time, image_link) VALUES (%s, %s, %s)
    '''
    img_url = 'https://cctv-dectec-db.s3.ap-northeast-2.amazonaws.com/DB_Picture/'
    cursor.execute(sql,(result[0]['cctv_ID'],check_time.strftime("%Y-%m-%d %H:%M:%S"),img_url+filename))
    conn.commit()
    
    tmp_cursor.close()
    cursor.close()
    func1(url)

# 소켓 통신을 멀티스레딩으로 처리
socket_thread = threading.Thread(target=handle_socket, args=(url,))
socket_thread.daemon = True


while True:
    start = datetime.datetime.now()

    ret, frame = cap.read()
    if not ret:
        print('Cam Error')
        break
    detection = model.predict(source=[frame], save=False)[0]
    
    tmp = 0
    for data in detection.boxes.data.tolist():
        if class_list[int(data[5])] == 'person':
            tmp +=1 
    if tmp == 0:
        #cv2.imshow('frame', frame)
        continue
    
    results = []
    person_dectected = False
    for data in detection.boxes.data.tolist(): # data : [xmin, ymin, xmax, ymax, confidence_score, class_id]
        confidence = float(data[4])
        if confidence > CONFIDENCE_THRESHOLD and class_list[int(data[5])] == 'person':
            person_dectected = True
            xmin, ymin, xmax, ymax = int(data[0]), int(data[1]), int(data[2]), int(data[3])
            label = int(data[5])
            
            cv2.rectangle(frame, (xmin, ymin), (xmax, ymax), (0, 255, 0), 2)
            cv2.putText(frame, class_list[label]+' '+str(round(confidence, 3)*100) + '%', (xmin, ymin), cv2.FONT_ITALIC, 1, (255, 255, 255), 2)
            results.append([[xmin, ymin, xmax-xmin, ymax-ymin], confidence, label])
    tracks = tracker.update_tracks(results, frame=frame)
    
    if person_dectected == True:
        handle_socket(url,frame)

    end = datetime.datetime.now()

    total = (end - start).total_seconds()
    print(f'Time to process 1 frame: {total * 1000:.0f} milliseconds')

    fps = f'FPS: {1 / total:.2f}'
    #cv2.putText(frame, fps, (10, 20), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 255), 2)

    cv2.imshow('frame', frame)
    #time.sleep(0.1)
    if cv2.waitKey(1) == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()