import { User } from "../Users/User.interface";

export interface Comment {
  id?: string;
  userId?: string;
  user?: User;
  repostAmount: number;
  youRetpost?: boolean;
  likeAmount: number;
  youLike?: boolean;
  content?: string;
  createdAt?: string;
}

export interface Like {
  userId?: string;
  createdAt?: string;
}

export interface Post {
  id?: string;
  user?: User;
  userId: string;
  content?: string;
  createdAt?: string;
  updatedAt?: string;
  mediaUrls?: string[];
  like?: Like[];
  youLike?: boolean;
  likeAmount?: number;
  repost?: string[];
  inRepost?: string[];
  youRepost?: boolean;
  repostAmount?: number;
  youSaved?: boolean;
  hashtags?: string[];
  mentions?: string[];
  comments?: Comment[];
  commentAmount?: number;
  youComment?: boolean;
  views?: string[];
  viewsAmount: number;
  sPublished?: boolean;
  shaveAnswer?: boolean;
  ansver?: Post;
}

export interface PostArray {
  post: Post[];
}
