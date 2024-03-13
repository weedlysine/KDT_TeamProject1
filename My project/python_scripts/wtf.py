import asyncio

async def handle_socket_test():
    print("비동기 함수 호출")

async def main_test():
    print("테스트 시작")
    asyncio.create_task(handle_socket_test())
    print("테스트 종료")

if __name__ == "__main__":
    asyncio.run(main_test())