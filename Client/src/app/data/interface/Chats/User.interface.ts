export interface Chats{
    avatar: string;
    nickName: string;
    lastName: string;
    createdAt: Date; 
    chatId: string;
    lastMessage: string;
}

export interface chatData {
    chat: Chats; 
}
  