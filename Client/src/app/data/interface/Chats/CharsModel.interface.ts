import { Message } from "./Message.interface";
import { lastMessage } from "./User.interface";

export interface CharsModel{
    avatar?: string;
    idUser?: string;
    nickName?: string;
    lastName?: string;
    createdAt?: Date; 
    chatId?: string;
    lastMessage?: lastMessage;
    message?: Array<Message>;
}