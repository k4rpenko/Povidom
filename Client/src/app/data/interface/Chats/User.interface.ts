export interface lastMessage{
    userId: string
    message: string
}

export interface Chats{
    avatar: string;
    nickName: string;
    lastName: string;
    createdAt: Date; 
    chatId: string;
    lastMessage: lastMessage;
    view: boolean;
    creatorId: string;
}

export interface chatData {
    chat: Chats; 
}
  