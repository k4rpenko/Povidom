export interface User{
    id: string;
    userName: string;
    firstName: string;
    lastName: string;
    avatar: string;
    title: string;
    publicKey: string;
}
  
export interface userG{
    user: Array<User>;
}