export interface User{
    id?: string;
    userName?: string;
    firstName?: string;
    lastName?: string;
    avatar?: string;
    title?: string;
    publicKey?: string;
    isOnline?: boolean;
}

export interface userG{
    user: Array<User>;
}
