package com.Spacegram.server.PostgresDB.Models;

import jakarta.persistence.*;

import java.util.Date;


@Entity
@Table(name = "Appeals")
public class Appeal {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    private String id;

    @ManyToOne
    @JoinColumn(name = "user_id", nullable = false)
    private AspNetUsers user;

    private String content;

    @Temporal(TemporalType.TIMESTAMP)
    private Date createdAt = new Date();

    // Getters and setters
}
