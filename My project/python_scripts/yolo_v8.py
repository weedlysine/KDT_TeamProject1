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
import asyncio

CONFIDENCE_THRESHOLD = 0.6
coco128 = open('./coco.names', 'r')
data = coco128.read()
class_list = data.split('\n')
coco128.close()

async_working = False

model = YOLO('./yolov8n.pt')
tracker = DeepSort(max_age=50)

url = 'rtsp://210.99.70.120:1935/live/cctv007.stream'

cap = cv2.VideoCapture(url)
#cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
#cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

# 두 번째 코드의 소켓 통신 및 db저장을 처리하는 함수
async def handle_socket(url, frame):
    global async_working
    sync_working = True
    print("스레드 불러짐")
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
    print(1)
    conn = pymysql.connect(host=DB_HOST,
                             user=DB_USERNAME,
                             password=DB_PASSWORD,
                             database=DB_NAME,
                             port=int(DB_PORT),
                             cursorclass=pymysql.cursors.DictCursor)
    
    tmp_cursor =  conn.cursor()
    sql_tmp = '''SELECT cctv_ID, cctv_url_hls FROM cctv WHERE cctv_url_rtsp = (%s)'''
    tmp_cursor.execute(sql_tmp,url)
    result = tmp_cursor.fetchall()
    
    print(2)
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
    print(3)
    cursor = conn.cursor()
    sql = '''
        INSERT INTO cctv_db (cctv_num, time, image_link) VALUES (%s, %s, %s)
    '''
    img_url = 'https://cctv-dectec-db.s3.ap-northeast-2.amazonaws.com/DB_Picture/'
    cursor.execute(sql,(result[0]['cctv_ID'],check_time.strftime("%Y-%m-%d %H:%M:%S"),img_url+filename))
    conn.commit()
    print(4)
    tmp_cursor.close()
    cursor.close()
    func1(result[0]['cctv_url_hls'])
    async_working = False
    print('스레드 끝남')
# 소켓 통신을 멀티스레딩으로 처리

    #if cv2.waitKey(1) == ord('q'):
    #    break

async def test():
    await asyncio.sleep(1)
    print("ㅅㅄㅄㅄㅄㅄㅄㅄㅄㅄㅄㅄ")

async def main():
    global async_working
    url = 'rtsp://210.99.70.120:1935/live/cctv007.stream'
    while True:
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
        tracked_persons = []
        person_dectected = False
        for data in detection.boxes.data.tolist(): # data : [xmin, ymin, xmax, ymax, confidence_score, class_id]
            confidence = float(data[4])
            if confidence > CONFIDENCE_THRESHOLD and class_list[int(data[5])] == 'person':
                person_dectected = True
                xmin, ymin, xmax, ymax = int(data[0]), int(data[1]), int(data[2]), int(data[3])
                label = int(data[5])
                
                cv2.rectangle(frame, (xmin, ymin), (xmax, ymax), (0, 255, 0), 2)
                #cv2.putText(frame, class_list[label]+' '+str(round(confidence, 3)*100) + '%', (xmin, ymin), cv2.FONT_ITALIC, 1, (255, 255, 255), 2)
                results.append([[xmin, ymin, xmax-xmin, ymax-ymin], confidence, label])
        tracks = tracker.update_tracks(results, frame=frame)
        
        if person_dectected == True:#사람이 탐지된경우에만
            temp = []
            for track in tracks:#현재 발견된것들 리스트에 넣기
                temp.append(track.track_id)
            for id in tracked_persons:#이번프레임에 사라진 객체 제거
                if not id in temp:
                    tracked_persons.remove(id)
            for track in tracks:
                person_id = track.track_id
                if not person_id in tracked_persons:#새로운 추적대상 발생시 함수 호출
                    if not async_working:
                        print("작동시작")
                        await test()
                        #async_working = True
                    else:
                        print("비동기식 작동중")
            tracked_persons = temp
    
    cap.release()
    cv2.destroyAllWindows()
    

    
if __name__ == "__main__":
    asyncio.run(main())