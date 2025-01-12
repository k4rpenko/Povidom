package com.Spacegram.server.controllers;

import com.Spacegram.server.PostgresDB.Models.AspNetUserRoles;
import com.Spacegram.server.PostgresDB.Models.AspNetUserTokens;
import com.Spacegram.server.utility.Grpc.Client;
import com.Spacegram.server.utility.Hash.Sha256;
import com.Spacegram.server.Models.UserModel;
import com.Spacegram.server.PostgresDB.Hibernate.HibernateUtil;
import com.Spacegram.server.PostgresDB.Models.AspNetUsers;
import com.Spacegram.server.utility.Token.JWT;
import com.Spacegram.server.utility.UserServices.userName;
import org.hibernate.Transaction;
import org.hibernate.Session;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;

@RestController
@RequestMapping("/api")
public class UserSettings
{
    JWT _jwt = new JWT();
    Sha256 _sha256 = new Sha256();
    userName _userName = new userName();


    @PutMapping("/ChangePassword")
    public ResponseEntity<String> UpdateUser(@RequestBody UserModel _user)
    {
        try (Session session = HibernateUtil.getSessionFactory().openSession()) {
            Transaction transaction = session.beginTransaction();
            AspNetUsers user = session.get(AspNetUsers.class, _user.id);
            if (user != null) {
                String HashNewPassword = _sha256.encrypt(user.getConcurrencyStamp(), _user.newPassword);
                String HashPassword = _sha256.encrypt(user.getConcurrencyStamp(), _user.password);

                if(user.getPasswordHash().equals(HashPassword))
                {
                    user.setPasswordHash(HashNewPassword);
                    session.update(user);
                    transaction.commit();
                    return new ResponseEntity<>("User Chang Password", HttpStatus.OK);
                }
            }
            return new ResponseEntity<>("Not found user ", HttpStatus.NOT_FOUND);
        }
        catch (Exception e)
        {
            System.out.println(e);
            return new ResponseEntity<>("Server Error", HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    @PutMapping("/NickName")
    public ResponseEntity<ArrayList<String>> GetUserProfile(@RequestBody UserModel _user)
    {
        try (Session session = HibernateUtil.getSessionFactory().openSession()) {

            if (session != null) {
                ArrayList<String> nicknames = new ArrayList<>();
                for (int i = 0; i < 3; i++){
                    nicknames.add(_userName.GenerateRandomNickname(_user.nickName, session, nicknames));
                }
                return new ResponseEntity<>(nicknames, HttpStatus.OK);
            }
            return new ResponseEntity<>(HttpStatus.NOT_FOUND);
        }
        catch (Exception e)
        {
            System.out.println(e);
            return new ResponseEntity<>(HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    @PutMapping("/Update")
    public ResponseEntity<String> Update(@RequestBody UserModel _user)
    {
        try (Session session = HibernateUtil.getSessionFactory().openSession()) {
            Transaction transaction = session.beginTransaction();
            AspNetUsers user = session.get(AspNetUsers.class, _user.id);

            if (user != null) {

                if (_user.FirstName != null && !_user.FirstName.trim().isEmpty()) { user.setFirstName(_user.FirstName); }
                if (_user.Email != null && !_user.Email.trim().isEmpty()) { user.setEmail(_user.Email); }
                if (_user.PhoneNumber != null && !_user.PhoneNumber.trim().isEmpty()) { user.setPhoneNumber(_user.PhoneNumber); }
                if (_user.LastName != null && !_user.LastName.trim().isEmpty()) { user.setLastName(_user.LastName); }
                if (_user.Avatar != null && !_user.Avatar.trim().isEmpty()) { user.setAvatar(_user.Avatar); }
                if (_user.nickName != null && !_user.nickName.trim().isEmpty()) { user.setUsername(_user.nickName.toLowerCase()); }
                if (_user.Title != null && !_user.Title.trim().isEmpty()) { user.setTitle(_user.Title); }
                session.update(user);
                transaction.commit();
                return new ResponseEntity<>("Update", HttpStatus.OK);
            }
            return new ResponseEntity<>("Not found user ", HttpStatus.NOT_FOUND);
        }
        catch (Exception e)
        {
            System.out.println(e);
            return new ResponseEntity<>("Server Error", HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    @PutMapping("/TokenUpdate")
    public ResponseEntity<String> TokenUpdate(@RequestBody UserModel _user)
    {
        try (Session session = HibernateUtil.getSessionFactory().openSession()) {
            Transaction transaction = session.beginTransaction();
            AspNetUsers user = session.get(AspNetUsers.class, _user.id);
            AspNetUserRoles Roles = session.get(AspNetUserRoles.class, _user.id);
            AspNetUserTokens Token = session.get(AspNetUserTokens.class, _user.id);

            if (user != null) {


                if (_jwt.validateToken(_user.Token, user.getConcurrencyStamp()) == false)
                {
                    Token.setValue(null);
                    session.update(user);
                    transaction.commit();
                    return new ResponseEntity<>("Not Authhorization", HttpStatus.UNAUTHORIZED);
                }
                String accessToken = _jwt.generateJwtToken(_user.id, user.getConcurrencyStamp(), 1, Roles.getUserId());
                return new ResponseEntity<>(accessToken, HttpStatus.OK);
            }
            return new ResponseEntity<>("Not found user ", HttpStatus.NOT_FOUND);
        }
        catch (Exception e)
        {
            System.out.println(e);
            return new ResponseEntity<>("Server Error", HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    @GetMapping("Test")
    public ResponseEntity<String> Test()
    {
        try {
            String filePath = "D:/test.txt";
            Path path = Paths.get(filePath);

            byte[] fileBytes = Files.readAllBytes(path);
            String filename = "test.txt";
            int Index = 1;
            new Client().Send(fileBytes, filename, Index);
            return new ResponseEntity<>("ok", HttpStatus.OK);
        }
        catch (Exception e)
        {
            System.out.println(e);
            return new ResponseEntity<>("Server Error", HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

}
