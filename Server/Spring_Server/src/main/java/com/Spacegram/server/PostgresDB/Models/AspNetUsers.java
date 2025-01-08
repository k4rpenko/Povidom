package com.Spacegram.server.PostgresDB.Models;

import jakarta.persistence.*;
import java.util.Date;
import java.util.List;

@Entity
@Table(name = "\"AspNetUsers\"")
public class AspNetUsers {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "\"Id\"")
    private String id;

    @Column(name = "\"UserName\"")
    private String username;

    @Column(name = "\"Email\"")
    private String email;

    @Column(name = "\"PasswordHash\"")
    private String passwordHash;

    @Column(name = "\"FirstName\"")
    private String firstName;

    @Column(name = "\"LastName\"")
    private String lastName;

    @Column(name = "\"Avatar\"")
    private String avatar;

    @Column(name = "\"Title\"")
    private String title;

    @Column(name = "\"Location\"")
    private String location;

    @Column(name = "\"PublicKey\"")
    private String publicKey;

    @Column(name = "\"PrivateKey\"")
    private String privateKey;

    @Column(name = "\"ConnectionId\"")
    private String connectionId;

    @Column(name = "\"StoriesId\"", columnDefinition = "TEXT")
    private String storiesId;

    @Column(name = "\"Subscribers\"", columnDefinition = "TEXT")
    private String subscribers;

    @Column(name = "\"Followers\"", columnDefinition = "TEXT")
    private String followers;

    @Column(name = "\"LikePostID\"", columnDefinition = "TEXT")
    private String likePostID;

    @Column(name = "\"CommentPostID\"", columnDefinition = "TEXT")
    private String commentPostID;

    @Column(name = "\"RetweetPostID\"", columnDefinition = "TEXT")
    private String retweetPostID;

    @Column(name = "\"PostID\"", columnDefinition = "TEXT")
    private String postID;

    @Column(name = "\"RecallPostId\"", columnDefinition = "TEXT")
    private String recallPostId;

    @Column(name = "\"ChatsID\"", columnDefinition = "TEXT")
    private String chatsID;

    @Column(name = "\"LastLogin\"")
    private Date lastLogin;

    @Column(name = "\"DateOfBirth\"")
    private Date dateOfBirth;

    @Column(name = "\"IsVerified\"")
    private boolean isVerified = false;

    @Column(name = "\"IsOnline\"")
    private boolean isOnline = false;

}
