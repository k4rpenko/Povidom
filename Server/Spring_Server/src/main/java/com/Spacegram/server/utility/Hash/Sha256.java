package com.Spacegram.server.utility.Hash;


import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;
import java.nio.charset.StandardCharsets;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.util.Base64;

public class Sha256
{

    public static String generateKey() {
        SecureRandom secureRandom = new SecureRandom();
        byte[] key = new byte[32]; // 256 біт
        secureRandom.nextBytes(key);
        return Base64.getEncoder().encodeToString(key);
    }


        public static String encrypt(String key, String data) throws NoSuchAlgorithmException, InvalidKeyException {
            byte[] keyBytes = key.getBytes(StandardCharsets.UTF_8);
            byte[] messageBytes = data.getBytes(StandardCharsets.UTF_8);

            Mac mac = Mac.getInstance("HmacSHA256");
            SecretKeySpec secretKeySpec = new SecretKeySpec(keyBytes, "HmacSHA256");
            mac.init(secretKeySpec);


            byte[] hashBytes = mac.doFinal(messageBytes);


            StringBuilder sb = new StringBuilder();
            for (byte b : hashBytes) {
                sb.append(String.format("%02x", b));
            }

            return sb.toString();
        }
}
