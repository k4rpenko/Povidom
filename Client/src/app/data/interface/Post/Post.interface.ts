export interface Post {
    Id: string; 
    UserId: string; 
    UserNickname: string; 
    UserName: string;
    UserAvatar: string; 
    Video: string; 
    Content?: string | null; 
    CreatedAt?: Date | null; 
    UpdatedAt?: Date | null; 
    MediaUrls?: string[]; 
    Likes?: number;
    Retpost?: string[];
    InRetpost?: string[]; 
    Hashtags?: string[]; 
    Mentions?: string[];
    Comments?: number; 
    Views?: number;
    IsPublished: boolean;
}

export interface post {
    [x: string]: any;
    post: Array<Post>;
}
  