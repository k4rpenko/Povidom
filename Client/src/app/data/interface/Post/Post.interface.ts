export interface Post {
    id: {
      timestamp: number;
      creationTime: string;
    };
    userId: string;
    userNickname: string;
    userName: string;
    userAvatar: string;
    content: string;
    createdAt: string;
    updatedAt: string;
    mediaUrls: number;
    like: number;
    retpost: number;
    inRetpost: number;
    hashtags: number;
    mentions: number;
    comments: number;
    views: number;
    sPublished: boolean;
}
  
export interface PostArray{
    post: Array<Post>
}