package com.Spacegram.server.PostgresDB.Models;

import jakarta.persistence.*;

import java.util.ArrayList;
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
    private String UserName;

    @Column(name = "\"Email\"")
    private String email;

    @Column(name = "\"PasswordHash\"")
    private String passwordHash;

    @Column(name = "\"ConcurrencyStamp\"")
    private String ConcurrencyStamp;

    @Column(name = "\"PhoneNumber\"")
    private String PhoneNumber;

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

    @ElementCollection
    @Column(name = "\"StoriesId\"", columnDefinition = "TEXT[]")
    private List<String> storiesId = new ArrayList<>();

    @ElementCollection
    @Column(name = "\"Subscribers\"", columnDefinition = "TEXT[]")
    private List<String> subscribers = new ArrayList<>();

    @ElementCollection
    @Column(name = "\"Followers\"", columnDefinition = "TEXT[]")
    private List<String> followers = new ArrayList<>();

    @ElementCollection
    @Column(name = "\"LikePostID\"", columnDefinition = "TEXT[]")
    private List<String> likePostID = new ArrayList<>();

    @ElementCollection
    @Column(name = "\"CommentPostID\"", columnDefinition = "TEXT[]")
    private List<String> commentPostID = new ArrayList<>();

    @ElementCollection
    @Column(name = "\"RetweetPostID\"", columnDefinition = "TEXT[]")
    private List<String> retweetPostID = new ArrayList<>();

    @ElementCollection
    @Column(name = "\"PostID\"", columnDefinition = "TEXT[]")
    private List<String> postID = new ArrayList<>();

    @ElementCollection
    @Column(name = "\"RecallPostId\"", columnDefinition = "TEXT[]")
    private List<String> recallPostId =  new ArrayList<>();

    @ElementCollection
    @Column(name = "\"ChatsID\"", columnDefinition = "TEXT")
    private List<String> chatsID = new ArrayList<>();

    @Column(name = "\"LastLogin\"")
    private Date lastLogin;

    @Column(name = "\"DateOfBirth\"")
    private Date dateOfBirth;

    @Column(name = "\"IsVerified\"")
    private boolean isVerified = false;

    @Column(name = "\"IsOnline\"")
    private boolean isOnline = false;

    // Getters and Setters

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getUsername() {
        return UserName;
    }

    public void setUsername(String UserName) {
        this.UserName = UserName;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getPasswordHash() {
        return passwordHash;
    }

    public void setPasswordHash(String passwordHash) {
        this.passwordHash = passwordHash;
    }

    public String getConcurrencyStamp() {
        return ConcurrencyStamp;
    }

    public void setConcurrencyStamp(String ConcurrencyStamp) {
        this.ConcurrencyStamp = ConcurrencyStamp;
    }

    public String getPhoneNumber() {
        return PhoneNumber;
    }

    public void setPhoneNumber(String PhoneNumber) {
        this.PhoneNumber = PhoneNumber;
    }

    public String getFirstName() {
        return firstName;
    }

    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }

    public String getLastName() {
        return lastName;
    }

    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    public String getAvatar() {
        return avatar;
    }

    public void setAvatar(String avatar) {
        this.avatar = avatar;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    public String getPublicKey() {
        return publicKey;
    }

    public void setPublicKey(String publicKey) {
        this.publicKey = publicKey;
    }

    public String getPrivateKey() {
        return privateKey;
    }

    public void setPrivateKey(String privateKey) {
        this.privateKey = privateKey;
    }

    public String getConnectionId() {
        return connectionId;
    }

    public void setConnectionId(String connectionId) {
        this.connectionId = connectionId;
    }

    public List<String> getStoriesId() {
        return storiesId;
    }

    public void setStoriesId(List<String> storiesId) {
        this.storiesId = storiesId;
    }

    public List<String> getSubscribers() {
        return subscribers;
    }

    public void setSubscribers(List<String> subscribers) {
        this.subscribers = subscribers;
    }

    public List<String> getFollowers() {
        return followers;
    }

    public void setFollowers(List<String> followers) {
        this.followers = followers;
    }

    public List<String> getLikePostID() {
        return likePostID;
    }

    public void setLikePostID(List<String> likePostID) {
        this.likePostID = likePostID;
    }

    public List<String> getCommentPostID() {
        return commentPostID;
    }

    public void setCommentPostID(List<String> commentPostID) {
        this.commentPostID = commentPostID;
    }

    public List<String> getRetweetPostID() {
        return retweetPostID;
    }

    public void setRetweetPostID(List<String> retweetPostID) {
        this.retweetPostID = retweetPostID;
    }

    public List<String> getPostID() {
        return postID;
    }

    public void setPostID(List<String> postID) {
        this.postID = postID;
    }

    public List<String> getRecallPostId() {
        return recallPostId;
    }

    public void setRecallPostId(List<String> recallPostId) {
        this.recallPostId = recallPostId;
    }

    public List<String> getChatsID() {
        return chatsID;
    }

    public void setChatsID(List<String> chatsID) {
        this.chatsID = chatsID;
    }

    public Date getLastLogin() {
        return lastLogin;
    }

    public void setLastLogin(Date lastLogin) {
        this.lastLogin = lastLogin;
    }

    public Date getDateOfBirth() {
        return dateOfBirth;
    }

    public void setDateOfBirth(Date dateOfBirth) {
        this.dateOfBirth = dateOfBirth;
    }

    public boolean isVerified() {
        return isVerified;
    }

    public void setVerified(boolean verified) {
        isVerified = verified;
    }

    public boolean isOnline() {
        return isOnline;
    }

    public void setOnline(boolean online) {
        isOnline = online;
    }
}
