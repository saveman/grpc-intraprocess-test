import time
import threading
import grpc
from concurrent.futures import ThreadPoolExecutor
from greet_pb2_grpc import GreeterServicer, add_GreeterServicer_to_server, GreeterStub
from greet_pb2 import HelloRequest, HelloReply

class Servicer(GreeterServicer):
    def SayHello(self, request, context):
        print(f'Server - received request: {request}')

        response = HelloReply(message = request.message)
        print(f'Server - prepared response: {request}')

        return response

class TestApp:
    def _start_server(self):
        self.server = grpc.server(ThreadPoolExecutor(max_workers=10))
        servicer = Servicer()
        add_GreeterServicer_to_server(servicer, self.server)
        self.server.add_insecure_port('[::]:50051')
        self.server.start()
        self.server.wait_for_termination()

    def _start_client(self):
        channel = grpc.insecure_channel('localhost:50051')
        stub = GreeterStub(channel)

        request = HelloRequest(message='Something')

        print(f'Client - prepared request: {request}')

        response = stub.SayHello(request)

        print(f'Client - received response: {response}')

    def run(self):
        print('Starting server')
        server_thread = threading.Thread(target=self._start_server)
        server_thread.start()

        time.sleep(2.0)

        print('Starting client')
        client_thread = threading.Thread(target=self._start_client)
        client_thread.start()

        time.sleep(5.0)

        self.server.stop(1.0)
        server_thread.join()
        client_thread.join()

