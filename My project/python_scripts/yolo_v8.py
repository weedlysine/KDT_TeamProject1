import datetime
import socket
import cv2
import numpy as np
from ultralytics import YOLO
import time
from deep_sort_realtime.deepsort_tracker import DeepSort
import threading  # 스레드 모듈 추가

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

# 두 번째 코드의 소켓 통신을 처리하는 함수
def handle_socket(url):
    from py_socket import func1
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
    for data in detection.boxes.data.tolist(): # data : [xmin, ymin, xmax, ymax, confidence_score, class_id]
        confidence = float(data[4])
        if confidence > CONFIDENCE_THRESHOLD and class_list[int(data[5])] == 'person':
            xmin, ymin, xmax, ymax = int(data[0]), int(data[1]), int(data[2]), int(data[3])
            label = int(data[5])
            socket_thread.start()
            #cv2.rectangle(frame, (xmin, ymin), (xmax, ymax), (0, 255, 0), 2)
            #cv2.putText(frame, class_list[label]+' '+str(round(confidence, 3)*100) + '%', (xmin, ymin), cv2.FONT_ITALIC, 1, (255, 255, 255), 2)
            results.append([[xmin, ymin, xmax-xmin, ymax-ymin], confidence, label])
    tracks = tracker.update_tracks(results, frame=frame)

    end = datetime.datetime.now()

    total = (end - start).total_seconds()
    print(f'Time to process 1 frame: {total * 1000:.0f} milliseconds')

    fps = f'FPS: {1 / total:.2f}'
    #cv2.putText(frame, fps, (10, 20), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 255), 2)

    #cv2.imshow('frame', frame)
    #time.sleep(0.1)
    if cv2.waitKey(1) == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()