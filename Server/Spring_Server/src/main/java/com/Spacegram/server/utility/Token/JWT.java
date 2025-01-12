package com.Spacegram.server.utility.Token;

import io.jsonwebtoken.*;
import io.jsonwebtoken.security.Keys;

import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;
import java.security.Key;
import java.time.Instant;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;

public class JWT
{

    public String generateJwtToken(String userId, String key, int hours, String userRoleId) {
        Instant now = Instant.now();
        Key signingKey = new SecretKeySpec(key.getBytes(), SignatureAlgorithm.HS256.getJcaName());
        Map<String, Object> claims = new HashMap<>();
        claims.put("sub", userId);
        claims.put("jti", java.util.UUID.randomUUID().toString());
        if (userRoleId != null && !userRoleId.isEmpty()) {
            claims.put("role", userRoleId);
        }

        return Jwts.builder()
                .setClaims(claims)
                .setIssuedAt(Date.from(now))
                .setExpiration(Date.from(now.plusSeconds(hours * 3600)))
                .signWith(signingKey, SignatureAlgorithm.HS256)
                .compact();
    }

    public String getUserIdFromToken(String token) {
        try {
            Claims claims = Jwts.parserBuilder()
                    .build()
                    .parseClaimsJws(token)
                    .getBody();
            return claims.getSubject();
        } catch (JwtException ex) {
            return null;
        }
    }

    public boolean validateToken(String token, String key) {
        try {
            Key signingKey = new SecretKeySpec(key.getBytes(), SignatureAlgorithm.HS256.getJcaName());

            Jwts.parserBuilder()
                    .setSigningKey(key)
                    .build()
                    .parseClaimsJws(token);

            Claims claims = Jwts.parserBuilder()
                    .setSigningKey(signingKey)
                    .build()
                    .parseClaimsJws(token)
                    .getBody();

            Date expiration = claims.getExpiration();
            return expiration != null && expiration.after(new Date());
        } catch (JwtException | IllegalArgumentException e) {
            return false;
        }
    }
}
