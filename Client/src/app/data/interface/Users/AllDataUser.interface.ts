export interface User{
    id?: string;
    discriminator?: string;
    firstName?: string;
    lastName?: string;
    avatar?: string | null;
    title?: string | null;
    location?: string | null;
    subscribers?: Array<string>;
    followers?: Array<string>;
    subscribersLenght?: number;
    followersLenght?: number;
    likePostID?: Array<string>;
    likePostIDLenght?: number;
    commentPostID?: Array<string>;
    commentPostIDLenght?: number;
    retweetPostID?: Array<string>;
    retweetPostIDLenght?: number;
    postID?: Array<string>;
    postIDLenght?: number;
    appeal?: Array<string>;
    lastLogin?: Date;
    dateOfBirth?: Date;
    isVerified?: boolean;
    userName?: string;
    normalizedUserName?: string;
    email?: string;
    normalizedEmail?: string;
    emailConfirmed?: boolean;
    passwordHash?: string;
    securityStamp?: string;
    concurrencyStamp?: string;
    phoneNumber?: string;
    phoneNumberConfirmed?: boolean;
    twoFactorEnabled?: boolean;
    lockoutEnd?: Date;
    lockoutEnabled?: boolean;
    accessFailedCount?: number;
    privateKey?: string;
}

export interface user {
    user: User; 
}
  