import { Message } from "./Message.interface";

export interface GetMessageModel{
    avatar?: string;
    nickName?: string;
    lastName?: string;
    createdAt?: Date; 
    chatId?: string;
    lastMessage?: string;
    message?: Array<Message>;
}