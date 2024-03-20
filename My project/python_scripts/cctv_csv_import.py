import csv
import pymysql
import os
import pandas as pd

DB_HOST=os.getenv('DB_HOST')
DB_USERNAME=os.getenv('DB_USERNAME')
DB_PASSWORD=os.getenv('DB_PASSWORD')
DB_NAME=os.getenv('DB_NAME')
DB_PORT=os.getenv('DB_PORT')

cctv_name = []
rtsp = []
rtmp = []
hls = []

data = {
    'cctv_name': cctv_name,
    'rtsp': rtsp, 
    'rtmp': rtmp, 
    'hls': hls
    }

f = open('충청남도 천안시_교통정보 CCTV_20220922.csv', 'r')
rdr = csv.reader(f)

for line in rdr:
    cctv_name.append(line[3])
    rtsp.append(line[5])
    rtmp.append(line[6])
    hls.append(line[7])
    
conn = pymysql.connect(host=DB_HOST,
                             user=DB_USERNAME,
                             password=DB_PASSWORD,
                             database=DB_NAME,
                             port=int(DB_PORT),
                             cursorclass=pymysql.cursors.DictCursor)
print(data['cctv_name'][0])   

cursor = conn.cursor()
#cursor.execute("CREATE TABLE userTable (id char(4), userName char(15), email char(20), birthYear int)")
sql = '''
        INSERT INTO cctv (cctv_name, cctv_url_rtsp, cctv_url_rtmp, cctv_url_hls) VALUES (%s, %s, %s, %s)
    '''

df = pd.DataFrame(data)

contents_list = [list(x) for x in df.to_numpy()]

print(type(contents_list[1]))
cursor.executemany(sql, tuple(contents_list[1:len(contents_list)]))

conn.commit()
f.close()