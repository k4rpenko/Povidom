interface Comment {
    AuthorId: string; 
    Content: string;
    createdAt: Date; 
}

interface Like {
    userId: string; 
    createdAt: Date; 
}

interface Post {
    id: string; 
    userId: string; 
    userNickname: string; 
    userName: string;
    userAvatar: string; 
    content?: string | null; 
    createdAt?: Date | null; 
    updatedAt?: Date | null; 
    mediaUrls?: string[]; 
    likes?: Like[];
    retweets?: string[];
    inRetweet?: string[]; 
    hashtags?: string[]; 
    mentions?: string[];
    comments?: Comment[]; 
    isPublished: boolean;
}