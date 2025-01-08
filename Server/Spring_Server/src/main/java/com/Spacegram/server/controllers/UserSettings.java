package com.Spacegram.server.controllers;

import com.Spacegram.server.utility.Hash.Sha256;
import com.Spacegram.server.Models.UserModel;
import com.Spacegram.server.PostgresDB.Hibernate.HibernateUtil;
import com.Spacegram.server.PostgresDB.Models.AspNetUsers;
import com.Spacegram.server.utility.UserServices.userName;
import org.hibernate.Transaction;
import org.hibernate.Session;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.ArrayList;

@RestController
@RequestMapping("/api")
public class UserSettings
{
    Sha256 _sha256 = new Sha256();
    userName _userName = new userName();

    @PutMapping("ChangePassword")
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
}
