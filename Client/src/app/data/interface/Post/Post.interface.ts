import { User } from "../Users/User.interface";

export interface Comment {
  userId?: string;
  createdAt?: string;
}

export interface Like {
  userId?: string;
  createdAt?: string;
}

export interface Post {
  id?: string;
  user?: User;
  content?: string;
  createdAt?: string;
  updatedAt?: string;
  mediaUrls?: string[];
  like?: Like[];
  youLike?: boolean;
  likeAmount?: number;
  retpost?: string[];
  inRetpost?: string[];
  youRetpost?: boolean;
  retpostAmount?: number;
  hashtags?: string[];
  mentions?: string[];
  comments?: Comment[];
  commentAmount?: number;
  youComment?: boolean;
  views?: string[];
  sPublished?: boolean;
}

export interface PostArray {
  post: Post[];
}
