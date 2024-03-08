import socket
import os
import pymysql
import threading  # 스레드 모듈 추가
import json
import hashlib


HOST = '127.0.0.1' 
PORT = 8001       


def func1():
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
    id = json_data["ID"]
    password = json_data["password"]
    print("Received data - ID:", id, "Password:", password)

    identification = func2(id,password)
    if identification == True:
        res ="true"
        client_socket.sendall(("true").encode())
    else:
        client_socket.sendall(("false").encode())
        
    client_socket.close()
    server_socket.close()


DB_HOST=os.getenv('DB_HOST')
DB_USERNAME=os.getenv('DB_USERNAME')
DB_PASSWORD=os.getenv('DB_PASSWORD')
DB_NAME=os.getenv('DB_NAME')
DB_PORT=os.getenv('DB_PORT')


def func2(id,password):
    conn = pymysql.connect(host=DB_HOST,user=DB_USERNAME, password=DB_PASSWORD,database=DB_NAME,port=int(DB_PORT),cursorclass=pymysql.cursors.DictCursor)
    tmp_cursor =  conn.cursor()
    sql_tmp = '''SELECT salt, Password FROM user WHERE ID = (%s)'''
    tmp_cursor.execute(sql_tmp,id)
    result = tmp_cursor.fetchall()
    print(result)
    salt = result[0]['salt']
    pwd = result[0]['Password']
    hash = hashlib.sha256((salt+password).encode())
    if pwd == str(hash.hexdigest()):
        return True
    else:
        return False
    
    
    
func1()