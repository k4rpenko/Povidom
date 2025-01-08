package com.Spacegram.server.utility.UserServices;

import com.Spacegram.server.PostgresDB.Models.AspNetUsers;
import com.Spacegram.server.utility.Sql.Users;
import org.hibernate.Transaction;

import org.hibernate.Session;
import java.util.ArrayList;
import java.util.Random;

public class userName
{
    Random random = new Random();
    public String GenerateRandomNickname(String nickname, Session users, ArrayList LastNames)
    {
        final String chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        StringBuilder sb = new StringBuilder(nickname);
        String newNickname;

        if(random.nextInt(0, 2) == 1){
            sb.append('_');
        }

        do
        {
            sb.delete(0, sb.length());
            sb.append(nickname);
            int randomLength = random.nextInt(0, 6);
            for (int i = 0; i < randomLength; i++)
            {
                sb.append(chars.charAt(random.nextInt(chars.length())));
            }
            newNickname = sb.toString();
        }
        while (LastNames.contains(newNickname) || Users.isNicknameExistInDatabase(newNickname, users));

        return newNickname;
    }




}
