package com.Spacegram.server.utility.Sql;

import org.hibernate.Session;
import org.hibernate.query.Query;

public class Users
{
    public static boolean isNicknameExistInDatabase(String nickname, Session session) {

        String hql = "SELECT 1 FROM AspNetUsers WHERE UserName = :UserName";
        Query<?> query = session.createQuery(hql);
        query.setParameter("UserName", nickname);
        return query.uniqueResult() != null;
    }
}
