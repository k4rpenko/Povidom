package com.Spacegram.server.PostgresDB.Models;

import jakarta.persistence.*;

@Entity
@Table(name = "\"AspNetUserTokens\"")
public class AspNetUserTokens
{
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "\"UserId\"")
    private String UserId;

    @Column(name = "\"LoginProvider\"")
    private String LoginProvider;

    @Column(name = "\"Name\"")
    private String Name;

    @Column(name = "\"Value\"")
    private String Value;

    public String getValue()
    {
        return Value;
    }
    public void setValue(String Value)
    {
        this.Value = Value;
    }
}
