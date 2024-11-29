import { Message } from "./Message.interface";
import { lastMessage } from "./User.interface";

export interface SendModel{
    avatar?: string;
    idUser?: string;
    nickName?: string;
    lastName?: string;
    createdAt?: Date; 
    idChat?: string;
    lastMessage?: lastMessage;
    message?: Array<Message>;
    IdChat?: string;
    creatorId?: string; 
    changIdMessageView?: string;
    text?: string;
    img?: string;
    answer?: string;
    userId?: string;
    isOnline?: boolean;
}