export interface Message{
    id?: number;
    idUser?: string;
    text?: string;
    img?: string;
    idAnswer?: string;
    view?: boolean;
    send?: boolean
    updatedAt?: Date;
    createdAt?: Date;
}

export interface MessageModel{
    idChat: string;
    message: Message;
}
