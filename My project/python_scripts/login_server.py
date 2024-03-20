import socket
import os
import pymysql
import threading  # 스레드 모듈 추가
import json
import hashlib
import string
import random
from datetime import datetime


HOST = '127.0.0.1' 
PORT = 8001

DB_HOST=os.getenv('DB_HOST')
DB_USERNAME=os.getenv('DB_USERNAME')
DB_PASSWORD=os.getenv('DB_PASSWORD')
DB_NAME=os.getenv('DB_NAME')
DB_PORT=os.getenv('DB_PORT')      

def main():
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server_socket.bind((HOST, PORT))
    server_socket.listen()
    client_socket, addr = server_socket.accept()
    
    print('Connected by', addr)
    
    data_length_bytes = client_socket.recv(4)
    data_length = int.from_bytes(data_length_bytes, byteorder='big')

    # 실제 데이터 수신
    data = client_socket.recv(data_length).decode()
    
    json_data = json.loads(data)
    print(json_data)
    type = json_data["Type"]
    
    if type == 'login':
        identfy = identification(json_data)
        if identfy == True:
            client_socket.sendall(("true").encode())
        else:
            client_socket.sendall(("false").encode())
    elif type == 'signup':
        result = signup(json_data)
        if result == 'success':
            client_socket.sendall(("success").encode())
        elif result == 'fail':
            client_socket.sendall(("fail").encode())    
    
    client_socket.close()
    server_socket.close()
    
def signup(data):
    id = data["ID"]
    password = data["password"]
    name = data["name"]
    conn = pymysql.connect(host=DB_HOST,user=DB_USERNAME, password=DB_PASSWORD,database=DB_NAME,port=int(DB_PORT),cursorclass=pymysql.cursors.DictCursor)
    tmp_cursor =  conn.cursor()
    sql_tmp = '''SELECT * FROM user WHERE ID = (%s)'''
    tmp_cursor.execute(sql_tmp,id)
    result = tmp_cursor.fetchall()
    if len(result) == 0:
        print("해당아이디는 사용가능")
        letters_set = string.ascii_lowercase + string.digits
        salt = ''.join(random.sample(letters_set,10))
        hash = str((hashlib.sha256((salt+password).encode())).hexdigest())
        sql_tmp = '''INSERT INTO user (ID, Password, Name, salt) VALUES (%s, %s, %s, %s);'''
        tmp_cursor.execute(sql_tmp,(id,hash,name,salt))
        conn.commit()
        return 'success'
    else:
        print("해당아이디는 사용불가")
        return 'fail'
    
    

def identification(data):
    id = data["ID"]
    password = data["password"]
    conn = pymysql.connect(host=DB_HOST,user=DB_USERNAME, password=DB_PASSWORD,database=DB_NAME,port=int(DB_PORT),cursorclass=pymysql.cursors.DictCursor)
    tmp_cursor =  conn.cursor()
    sql_tmp = '''SELECT identification_number,salt, Password FROM user WHERE ID = (%s)'''
    tmp_cursor.execute(sql_tmp,id)
    result = tmp_cursor.fetchall()
    if len(result) == 0:
        return False
    salt = result[0]['salt']
    pwd = result[0]['Password']
    hash = hashlib.sha256((salt+password).encode())
    if pwd == str(hash.hexdigest()):
        ip = data["ip"]
        mac = data["MAC"]
        sql_tmp = '''INSERT INTO login_db (LogIn_user, Time, login_ip, login_MAC) VALUES (%s, %s, %s, %s);'''
        tmp_cursor.execute(sql_tmp,(result[0]['identification_number'],datetime.today().strftime("%Y%m%d%H%M%S") ,ip,mac))
        conn.commit()
        return True
    else:
        return False
    


if __name__ == "__main__":
    main()