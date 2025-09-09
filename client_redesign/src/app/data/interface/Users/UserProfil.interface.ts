import { Post } from "../Post/Post.interface";
import { User } from "./User.interface";

export interface UserProfil {
  id?: string;
  avatar?: string;
  userName?: string;
  title?: string;
  phoneNumber?: string;
  firstName?: string;
  lastName?: string;

  post?: Array<Post>;
  recall?: Array<Post>;

  followersAmount?: number;
  followers?: Array<User>;
  amountFollowers?: number;
  youFollower?: boolean;

  subscribers?: Array<User>;
  subscribersAmount?: number;
  youSubscriber?: boolean;
}
