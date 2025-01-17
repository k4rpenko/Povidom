package com.Spacegram.server.utility.Grpc;

import greet.Greet;
import greet.GreeterGrpc;
import io.grpc.ManagedChannel;
import io.grpc.netty.GrpcSslContexts;
import io.grpc.netty.NettyChannelBuilder;
import io.netty.handler.ssl.*;
import io.netty.handler.ssl.util.InsecureTrustManagerFactory;

import java.io.File;

public class Client {
    public void Send(byte[] data, String fileName, int chunkIndex) {
        ManagedChannel channel = null;
        try {
            File privateKeyFile = new File("D:/Sertificate/private.pem");
            File certificateFile = new File("D:/Sertificate/certificate.pem");

            if (!privateKeyFile.exists() || !certificateFile.exists()) {
                throw new IllegalArgumentException("Private key or certificate file does not exist.");
            }

            SslContext sslContext = GrpcSslContexts.forClient()
                    .keyManager(certificateFile, privateKeyFile)
                    .trustManager(InsecureTrustManagerFactory.INSTANCE)
                    .protocols("TLSv1.2")
                    .build();

            channel = NettyChannelBuilder.forTarget("localhost:8083")
                    .sslContext(sslContext)
                    .build();


            GreeterGrpc.GreeterBlockingStub stub = GreeterGrpc.newBlockingStub(channel);


            Greet.TestRequest request = Greet.TestRequest.newBuilder()
                    .setName("test")
                    .build();

            Greet.TestResponse response = stub.test(request);
            /*
            Greet.FileRequest request = Greet.FileRequest.newBuilder()
                    .setData(ByteString.copyFrom(data))
                    .setFileName(fileName)
                    .setChunkIndex(chunkIndex)
                    .build();

            Greet.FileReply response = stub.file(request);

            if (response.getSuccess()) {
                System.out.println("File successfully sent. Message: " + response.getMessage());
            } else {
                System.err.println("Error sending file. Message: " + response.getMessage());
            }
            */

            System.out.println("Message: " + response.getMessage());

        } catch (Exception e) {
            System.err.println("RPC failed: " + e);
            e.printStackTrace();
        } finally {
            if (channel != null && !channel.isShutdown()) {
                channel.shutdown();
            }
        }
    }
}
