package com.Spacegram.server.PostgresDB.Models;

import jakarta.persistence.*;

@Entity
@Table(name = "\"AspNetUserRoles\"")
public class AspNetUserRoles
{
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "\"UserId\"")
    private String UserId;

    @Column(name = "\"RoleId\"")
    private String RoleId;

    public String getUserId(){
        return UserId;
    }

    public void setUserId(String userId){
        this.UserId = userId;
    }
}
