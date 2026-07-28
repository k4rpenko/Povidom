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
    yourMessage: boolean
}

export interface MessageModel{
    idChat: string;
    peopleId?: string
    message: Message;
}
