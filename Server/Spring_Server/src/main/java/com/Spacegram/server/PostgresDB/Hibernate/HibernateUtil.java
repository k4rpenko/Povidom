package com.Spacegram.server.PostgresDB.Hibernate;
import com.Spacegram.server.PostgresDB.Models.AspNetUsers;
import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

public class HibernateUtil {
    private static SessionFactory sessionFactory;

    static {
        try {
            sessionFactory = new Configuration().configure("/Config/Postgres/hibernate.cfg.xml")
                    .addAnnotatedClass(AspNetUsers.class)
                    .buildSessionFactory();

        } catch (Throwable ex) {
            throw new ExceptionInInitializerError(ex);
        }
    }

    public static SessionFactory getSessionFactory() {
        return sessionFactory;
    }
}
