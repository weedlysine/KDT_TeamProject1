import socket
import time
from datetime import datetime
import random

HOST = '127.0.0.1' 
PORT = 8000       

def func1(url):
    print("헤위헤위")
    client_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    random_int = random.randint(1, 4)
    msg = str(random_int)
    client_socket.sendto(msg.encode(), (HOST, PORT))
    #server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    #server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    #server_socket.bind((HOST, PORT))
    #server_socket.listen()
    #client_socket, addr = server_socket.accept()
    
    #print('Connected by', addr)
    #msg = url
    #client_socket.sendall(msg.encode())
    
    print('send 완료 ')
    
    client_socket.close()
    #server_socket.close()
    return 0