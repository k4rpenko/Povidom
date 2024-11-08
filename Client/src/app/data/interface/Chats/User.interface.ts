export interface Chats{
    avatar: string;
    nickName: string;
    createdAt: Date; 
    ChatId: string;
    LastMessage: string;
}

export interface chatData {
    chat: Chats; 
}
  