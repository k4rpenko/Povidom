package com.Spacegram.server.controllers;

import com.Spacegram.server.Models.UserModel;
import com.Spacegram.server.PostgresDB.Hibernate.HibernateUtil;
import com.Spacegram.server.PostgresDB.Models.AspNetUsers;
import org.hibernate.Transaction;
import org.hibernate.Session;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api")
public class UserSettings
{

    @DeleteMapping("/user")
    public ResponseEntity<String> DeleteUser(@RequestBody UserModel _user){
        try (Session session = HibernateUtil.getSessionFactory().openSession()) {
            Transaction transaction = session.beginTransaction();
            AspNetUsers user = session.get(AspNetUsers.class, _user.id);
            if (user != null) {
                session.delete(user);
                transaction.commit();
                return new ResponseEntity<>("User deleted successfully", HttpStatus.OK);
            }
            return new ResponseEntity<>("Not found user ", HttpStatus.NOT_FOUND);
        }
        catch (Exception e)
        {
            System.out.println(e);
            return new ResponseEntity<>("Server Error", HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }
}
